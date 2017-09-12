using System.Threading.Tasks;
using HighLoad.Application.Entities;

namespace HighLoad.Application.Data
{
    public interface IVisitsRepository
    {
        Visit Find(int visitId);
        Task<CreateCommandResult> CreateAsync(Visit entity);
        Task<UpdateCommandResult> UpdateAsync(int visitId, Visit entity);
        void CreateBatch(Visit[] visits);
        bool Exists(int visitId);
    }
}