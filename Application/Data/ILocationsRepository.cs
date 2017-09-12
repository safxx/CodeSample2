using System.Threading.Tasks;
using HighLoad.Application.Entities;

namespace HighLoad.Application.Data
{
    public interface ILocationsRepository
    {
        Location Find(int locationId);
        CreateCommandResult Create(Location location);
        Task<UpdateCommandResult> UpdateAsync(int locationId, Location location);
        void CreateBatchAsync(Location[] locations);
        bool Exists(int locationId);
    }
}