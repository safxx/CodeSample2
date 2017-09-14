using System.Threading.Tasks;
using AutoMapper;
using HighLoad.Application;
using HighLoad.Application.Data;
using HighLoad.Application.Entities;
using HighLoad.Framework.Data.ReadModel;

namespace HighLoad.Framework.Data.Repositories
{
    class LocationsRepository : ILocationsRepository
    {
        private readonly IVisitViewUpdater _visitViewUpdater;
        private readonly IExistingEntitiesLookup<Entities.Location> _existingLocationsLookup;

        public LocationsRepository(IVisitViewUpdater visitViewUpdater, IExistingEntitiesLookup<Entities.Location> existingLocationsLookup)
        {
            _visitViewUpdater = visitViewUpdater;
            _existingLocationsLookup = existingLocationsLookup;
        }

        public Location Find(int locationId)
        {
            var location = _existingLocationsLookup.Get(locationId);
            return Mapper.Map<Entities.Location, Location>(location);
        }

        public CreateCommandResult Create(Location location)
        {
            _existingLocationsLookup.Add(Mapper.Map<Location, Entities.Location>(location));
            return CreateCommandResult.Success;
        }

        public async Task<UpdateCommandResult> UpdateAsync(int locationId, Location location)
        {
            var entity = Mapper.Map<Location, Entities.Location>(location);

            var existingLocation = _existingLocationsLookup.Get(locationId);
            if (existingLocation == null) return UpdateCommandResult.NotFound;

            string country = null;
            int? distance = null;
            string place = null;

            if (!string.IsNullOrEmpty(entity.Country) && existingLocation.Country != entity.Country)
            {
                existingLocation.Country = entity.Country;
                country = entity.Country;
            }
            if (entity.Distance.HasValue && existingLocation.Distance != entity.Distance)
            {
                existingLocation.Distance = entity.Distance;
                distance = entity.Distance;
            }
            if (!string.IsNullOrEmpty(entity.Place) && existingLocation.Place != entity.Place)
            {
                existingLocation.Place = entity.Place;
                place = entity.Place;
            }
            if (!string.IsNullOrEmpty(entity.City) && existingLocation.City != entity.City)
                existingLocation.City = entity.City;

            if (country != null || distance != null || place != null)
                await _visitViewUpdater.UpdateExistingLocationsAsync(locationId, country, distance, place);

            return UpdateCommandResult.Success;
        }

        public void CreateBatch(Location[] locations)
        {
            _existingLocationsLookup.AddRange(Mapper.Map<Location[], Entities.Location[]>(locations));
        }

        public bool Exists(int locationId)
        {
            return _existingLocationsLookup.Exists(locationId);
        }
    }
}