using System.Threading.Tasks;
using HighLoad.Application.Entities;

namespace HighLoad.Application.Data
{
    public interface IUsersRepository
    {
        User Find(int userId);
        CreateCommandResult Create(User user);
        Task<UpdateCommandResult> UpdateAsync(int userId, User user);
        void CreateBatchAsync(User[] users);
        bool Exists(int userId);
    }
}