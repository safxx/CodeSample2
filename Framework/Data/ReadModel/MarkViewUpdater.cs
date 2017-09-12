using System;
using System.Threading.Tasks;
using HighLoad.Application;
using HighLoad.Application.Data.ReadModel.MarkView;
using HighLoad.Core;
using ServiceStack.OrmLite;

namespace HighLoad.Framework.Data.ReadModel
{
    public interface IMarkViewUpdater
    {
        Task<CreateCommandResult> AddNewAsync(Entities.Visit visit);
        Task<UpdateCommandResult> UpdateExistingVisitAsync(int visitId, int? newLocationId, int? newUserId, DateTime? newVisitedAt, byte? newMark);
        Task UpdateExistingUserAsync(int newUserId, DateTime? newBirthDate, char? newGender);
        Task AddNewBatch(Entities.Visit[] visits);
    }

    public class MarkViewUpdater : IMarkViewUpdater
    {
        private readonly IExistingEntitiesLookup<Entities.Location> _existingLocationsLookup;
        private readonly IExistingEntitiesLookup<Entities.User> _existingUsersLookup;
        private readonly IMarkViewDbConnectionFactory _markViewDbConnectionFactory;

        public MarkViewUpdater(IExistingEntitiesLookup<Entities.Location> existingLocationsLookup, IExistingEntitiesLookup<Entities.User> existingUsersLookup, IMarkViewDbConnectionFactory markViewDbConnectionFactory)
        {
            _existingLocationsLookup = existingLocationsLookup;
            _existingUsersLookup = existingUsersLookup;
            _markViewDbConnectionFactory = markViewDbConnectionFactory;
        }

        public async Task<CreateCommandResult> AddNewAsync(Entities.Visit visit)
        {
            var user = _existingUsersLookup.Get(visit.UserId.Value);
            if (user == null) return CreateCommandResult.Failure;

            var markView = new MarkView
            {
                LocationId = visit.LocationId.Value,
                UserId = visit.UserId.Value,
                VisitedAt = visit.VisitedAt.Value,
                Age = (byte)user.BirthDate.Value.CalculateAge(),
                Gender = user.Gender == 'm',
                Mark = visit.Mark.Value,
                VisitId = visit.Id
            };

            using (var db = _markViewDbConnectionFactory.OpenDbConnection())
                await db.InsertAsync(markView);

            return CreateCommandResult.Success;
        }

        public async Task<UpdateCommandResult> UpdateExistingVisitAsync(int visitId, int? newLocationId, int? newUserId, DateTime? newVisitedAt, byte? newMark)
        {
            using (var db = _markViewDbConnectionFactory.OpenDbConnection())
            {
                var existingMarkView = await db.SingleAsync<MarkView>(mv=>mv.VisitId==visitId);
                if (existingMarkView == null) return UpdateCommandResult.InvalidData;

                if (newLocationId.HasValue)
                {
                    if (!_existingLocationsLookup.Exists(newLocationId.Value))
                        return UpdateCommandResult.InvalidData;

                    existingMarkView.LocationId = newLocationId.Value;
                }

                if (newUserId.HasValue)
                {
                    var newUser = _existingUsersLookup.Get(newUserId.Value);
                    if (newUser == null) return UpdateCommandResult.InvalidData;

                    existingMarkView.Age = (byte) newUser.BirthDate.Value.CalculateAge();
                    existingMarkView.Gender = newUser.Gender == 'm';
                    existingMarkView.UserId = newUserId.Value;
                }

                if (newVisitedAt.HasValue) existingMarkView.VisitedAt = newVisitedAt.Value;
                if (newMark.HasValue) existingMarkView.Mark = newMark.Value;

                await db.UpdateAsync(existingMarkView);
            }

            return UpdateCommandResult.Success;
        }

        public async Task UpdateExistingUserAsync(int newUserId, DateTime? newBirthDate, char? newGender)
        {
            using (var db = _markViewDbConnectionFactory.OpenDbConnection())
            {
                var existingMarkViews = await db.SelectAsync<MarkView>(mv => mv.UserId == newUserId);

                foreach (var existingMarkView in existingMarkViews)
                {
                    if (newBirthDate.HasValue) existingMarkView.Age = (byte) newBirthDate.Value.CalculateAge();
                    if (newGender.HasValue) existingMarkView.Gender = newGender.Value == 'm';
                }

                await db.UpdateAllAsync(existingMarkViews);
            }
        }

        public async Task AddNewBatch(Entities.Visit[] visits)
        {
            var markViews = new MarkView[visits.Length];
            for (int i = 0; i < visits.Length; i++)
            {
                var visit = visits[i];
                var user = _existingUsersLookup.Get(visit.UserId.Value);

                var markView = new MarkView
                {
                    LocationId = visit.LocationId.Value,
                    UserId = visit.UserId.Value,
                    VisitedAt = visit.VisitedAt.Value,
                    Age = (byte) user.BirthDate.Value.CalculateAge(),
                    Gender = user.Gender == 'm',
                    Mark = visit.Mark.Value,
                    VisitId = visit.Id
                };

                markViews[i] = markView;
            }

            using (var db = _markViewDbConnectionFactory.OpenDbConnection())
                await db.InsertAllAsync(markViews);
        }
    }
}