using System;
using System.Data;
using System.Threading.Tasks;
using HighLoad.Application;
using HighLoad.Application.Data.ReadModel.VisitView;
using ServiceStack.OrmLite;

namespace HighLoad.Framework.Data.ReadModel
{
    public interface IVisitViewUpdater
    {
        Task<CreateCommandResult> AddNewAsync(Entities.Visit visit);
        Task<UpdateCommandResult> UpdateExistingVisitAsync(int visitId, int? newLocationId, int? newUserId, DateTime? newVisitedAt, byte? newMark);
        Task UpdateExistingLocationsAsync(int locationId, string newCountry, int? newDistance, string newPlace);
        Task AddNewBatch(Entities.Visit[] visits);
    }

    public class VisitViewUpdater : IVisitViewUpdater
    {
        private readonly IExistingEntitiesLookup<Entities.Location> _existingLocationsLookup;
        private readonly IExistingEntitiesLookup<Entities.User> _existingUsersLookup;
        private readonly IVisitViewDbConnectionFactory _visitViewDbConnectionFactory;

        public VisitViewUpdater(IExistingEntitiesLookup<Entities.Location> existingLocationsLookup, IExistingEntitiesLookup<Entities.User> existingUsersLookup,IVisitViewDbConnectionFactory visitViewDbConnectionFactory )
        {
            _existingLocationsLookup = existingLocationsLookup;
            _existingUsersLookup = existingUsersLookup;
            _visitViewDbConnectionFactory = visitViewDbConnectionFactory;
        }

        public async Task AddNewBatch(Entities.Visit[] visits)
        {
            var visitViews = new VisitView[visits.Length];

            for (int i = 0; i < visits.Length; i++)
            {
                var visit = visits[i];
                var location = _existingLocationsLookup.Get(visit.LocationId.Value);

                var visitView = new VisitView
                {
                    VisitId = visit.Id,
                    Country = location.Country,
                    Distance = location.Distance.Value,
                    Mark = visit.Mark.Value,
                    Place = location.Place,
                    UserId = visit.UserId.Value,
                    VisitedAt = visit.VisitedAt.Value,
                    LocationId = location.Id
                };
                visitViews[i] = visitView;
            }

            using (IDbConnection db = _visitViewDbConnectionFactory.OpenDbConnection())
                 await db.InsertAllAsync(visitViews);
        }

        public async Task<CreateCommandResult> AddNewAsync(Entities.Visit visit)
        {
            var location = _existingLocationsLookup.Get(visit.LocationId.Value);
            if (location == null) return CreateCommandResult.Failure;

            var visitView = new VisitView
            {
                VisitId = visit.Id,
                Country = location.Country,
                Distance = location.Distance.Value,
                Mark = visit.Mark.Value,
                Place = location.Place,
                UserId = visit.UserId.Value,
                VisitedAt = visit.VisitedAt.Value,
                LocationId = location.Id
            };

            using (IDbConnection db = _visitViewDbConnectionFactory.OpenDbConnection())
                await db.InsertAsync(visitView);

            return CreateCommandResult.Success;
        }

        public async Task<UpdateCommandResult> UpdateExistingVisitAsync(int visitId, int? newLocationId, int? newUserId, DateTime? newVisitedAt, byte? newMark)
        {
            using (var db = _visitViewDbConnectionFactory.OpenDbConnection())
            {
                var existingVisitView = await db.SingleAsync<VisitView>(vv => vv.VisitId == visitId);

                if (existingVisitView == null) return UpdateCommandResult.InvalidData;

                if (newLocationId.HasValue)
                {
                    var newLocation = _existingLocationsLookup.Get(newLocationId.Value);
                    if (newLocation == null) return UpdateCommandResult.InvalidData;

                    existingVisitView.Country = newLocation.Country;
                    existingVisitView.Distance = newLocation.Distance.Value;
                    existingVisitView.Place = newLocation.Place;
                    existingVisitView.LocationId = newLocationId.Value;
                }

                if (newUserId.HasValue)
                {
                    if (!_existingUsersLookup.Exists(newUserId.Value))
                        return UpdateCommandResult.InvalidData;
                    existingVisitView.UserId = newUserId.Value;
                }

                if (newVisitedAt.HasValue) existingVisitView.VisitedAt = newVisitedAt.Value;
                if (newMark.HasValue) existingVisitView.Mark = newMark.Value;

                await db.UpdateAsync(existingVisitView);
            }

            return UpdateCommandResult.Success;
        }

        public async Task UpdateExistingLocationsAsync(int locationId, string newCountry, int? newDistance, string newPlace)
        {
            using (var db = _visitViewDbConnectionFactory.OpenDbConnection())
            {
                var existingVisitViews = await db.SelectAsync<VisitView>(vv => vv.LocationId == locationId);

                foreach (var existingVisitView in existingVisitViews)
                {
                    if (!string.IsNullOrEmpty(newCountry)) existingVisitView.Country = newCountry;
                    if (newDistance.HasValue) existingVisitView.Distance = newDistance.Value;
                    if (!string.IsNullOrEmpty(newPlace)) existingVisitView.Place = newPlace;
                }

                await db.UpdateAllAsync(existingVisitViews);
            }
        }
    }
}

