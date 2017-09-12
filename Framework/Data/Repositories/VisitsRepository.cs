using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HighLoad.Application;
using HighLoad.Application.Data;
using HighLoad.Application.Entities;
using HighLoad.Framework.Data.ReadModel;

namespace HighLoad.Framework.Data.Repositories
{
    class VisitsRepository : IVisitsRepository
    {
        private readonly IVisitViewUpdater _visitViewUpdater;
        private readonly IMarkViewUpdater _markViewUpdater;
        private readonly IExistingEntitiesLookup<Entities.Visit> _existingVisitsLookup;
        private readonly IVisitsProcessingQueue _visitsProcessingQueue;

        public VisitsRepository(IVisitViewUpdater visitViewUpdater, IMarkViewUpdater markViewUpdater, IExistingEntitiesLookup<Entities.Visit> existingVisitsLookup, IVisitsProcessingQueue visitsProcessingQueue)
        {
            _visitViewUpdater = visitViewUpdater;
            _markViewUpdater = markViewUpdater;
            _existingVisitsLookup = existingVisitsLookup;
            _visitsProcessingQueue = visitsProcessingQueue;
        }

        public Visit Find(int visitId)
        {
            var visit = _existingVisitsLookup.Get(visitId);
            return Mapper.Map<Entities.Visit, Visit>(visit);
        }

        public async Task<CreateCommandResult> CreateAsync(Visit visit)
        {
            var entity = Mapper.Map<Visit, Entities.Visit>(visit);

            _existingVisitsLookup.Add(entity);

            var results = await Task.WhenAll(_visitViewUpdater.AddNewAsync(entity), _markViewUpdater.AddNewAsync(entity));

            return results.All(r => r.IsSuccess)
                ? CreateCommandResult.Success
                : CreateCommandResult.Failure;
        }

        public async Task<UpdateCommandResult> UpdateAsync(int visitId, Visit visit)
        {
            var entity = Mapper.Map<Visit, Entities.Visit>(visit);

            var existingVisit = _existingVisitsLookup.Get(visitId);
            if (existingVisit == null)
                return UpdateCommandResult.NotFound;

            int? newLocationId = null;
            int? newUserId = null;
            DateTime? newVisitedAt = null;
            byte? newMark = null;

            if (entity.Mark.HasValue && existingVisit.Mark != entity.Mark)
            {
                existingVisit.Mark = entity.Mark;
                newMark = entity.Mark;
            }
            if (entity.LocationId.HasValue && existingVisit.LocationId != entity.LocationId)
            {
                existingVisit.LocationId = entity.LocationId;
                newLocationId = entity.LocationId;
            }
            if (entity.UserId.HasValue && existingVisit.UserId != entity.UserId)
            {
                existingVisit.UserId = entity.UserId;
                newUserId = entity.UserId;
            }
            if (entity.VisitedAt.HasValue && existingVisit.VisitedAt != entity.VisitedAt)
            {
                existingVisit.VisitedAt = entity.VisitedAt;
                newVisitedAt = entity.VisitedAt;
            }

            if (newLocationId.HasValue || newUserId.HasValue || newVisitedAt.HasValue || newMark.HasValue)
            {
                var results = await Task.WhenAll(
                    _visitViewUpdater.UpdateExistingVisitAsync(visitId, newLocationId, newUserId, newVisitedAt, newMark),
                    _markViewUpdater.UpdateExistingVisitAsync(visitId, newLocationId, newUserId, newVisitedAt, newMark)
                    );
                if (!results.All(r => r.IsSuccess)) return UpdateCommandResult.InvalidData;
            }

            return UpdateCommandResult.Success;
        }

        public void CreateBatch(Visit[] visits)
        {
            _visitsProcessingQueue.Push(Mapper.Map<Visit[], Entities.Visit[]>(visits));
        }

        public bool Exists(int visitId)
        {
            return _existingVisitsLookup.Exists(visitId);
        }
    }
}