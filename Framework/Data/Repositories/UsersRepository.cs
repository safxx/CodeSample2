using System;
using System.Threading.Tasks;
using AutoMapper;
using HighLoad.Application;
using HighLoad.Application.Data;
using HighLoad.Application.Entities;
using HighLoad.Framework.Data.ReadModel;

namespace HighLoad.Framework.Data.Repositories
{
    class UsersRepository : IUsersRepository
    {
        private readonly IMarkViewUpdater _markViewUpdater;
        private readonly IExistingEntitiesLookup<Entities.User> _existingUsersLookup;

        public UsersRepository(IMarkViewUpdater markViewUpdater, IExistingEntitiesLookup<Entities.User> existingUsersLookup)
        {
            _markViewUpdater = markViewUpdater;
            _existingUsersLookup = existingUsersLookup;
        }

        public User Find(int userId)
        {
            var user = _existingUsersLookup.Get(userId);
            return Mapper.Map<Entities.User, User>(user);
        }

        public CreateCommandResult Create(User user)
        {
            _existingUsersLookup.Add(Mapper.Map<User, Entities.User>(user));
            return CreateCommandResult.Success;
        }

        public async Task<UpdateCommandResult> UpdateAsync(int userId, User user)
        {
            var entity = Mapper.Map<User, Entities.User>(user);
            var exitsingUser = _existingUsersLookup.Get(userId);
            if (exitsingUser == null) return UpdateCommandResult.NotFound;

            DateTime? newBirthDate = null;
            char? newGender = null;
            if (entity.BirthDate.HasValue && exitsingUser.BirthDate != entity.BirthDate)
            {
                exitsingUser.BirthDate = entity.BirthDate;
                newBirthDate = entity.BirthDate;
            }
            if (entity.Gender.HasValue && exitsingUser.Gender != entity.Gender)
            {
                exitsingUser.Gender = entity.Gender;
                newGender = entity.Gender;
            }
            if (!string.IsNullOrEmpty(entity.FirstName) && exitsingUser.FirstName != entity.FirstName)
            {
                exitsingUser.FirstName = entity.FirstName;
            }
            if (!string.IsNullOrEmpty(entity.LastName) && exitsingUser.LastName != entity.LastName)
            {
                exitsingUser.LastName = entity.LastName;
            }
            if (!string.IsNullOrEmpty(entity.Email) && exitsingUser.Email != entity.Email)
            {
                exitsingUser.Email = entity.Email;
            }

            if (newBirthDate != null || newGender != null)
                await _markViewUpdater.UpdateExistingUserAsync(userId, newBirthDate, newGender);

            return UpdateCommandResult.Success;
        }

        public void CreateBatchAsync(User[] users)
        {
            _existingUsersLookup.AddRange(Mapper.Map<User[], Entities.User[]>(users));
        }

        public bool Exists(int locationId)
        {
            return _existingUsersLookup.Exists(locationId);
        }
    }
}