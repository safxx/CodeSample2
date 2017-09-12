using System.Collections.Concurrent;
using HighLoad.Application.Entities;
using HighLoad.Framework.Data.Entities;

namespace HighLoad.Framework.Data
{
    public interface IExistingEntitiesLookup<T> where T : IEntity
    {
        bool Exists(int entityId);
        T Get(int entityId);
        void Add(T entiry);
        void AddRange(T[] entities);
    }

    public class ExistingEntitiesLookup<T> : IExistingEntitiesLookup<T> where T : IEntity
    {
        private readonly ConcurrentDictionary<int, T> _existingEntities = new ConcurrentDictionary<int, T>();

        public bool Exists(int entityId)
        {
            return _existingEntities.ContainsKey(entityId);
        }

        public T Get(int entityId)
        {
            T visit;
            _existingEntities.TryGetValue(entityId, out visit);
            return visit;
        }

        public void Add(T entiry)
        {
            _existingEntities.TryAdd(entiry.Id, entiry);
        }

        public void AddRange(T[] entities)
        {
            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                _existingEntities.TryAdd(entity.Id, entity);
            }
        }
    }
}