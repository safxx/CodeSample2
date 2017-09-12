using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace HighLoad.Framework.Json
{
    public class VisitContractResolver : DefaultContractResolver
    {
        public static IReadOnlyDictionary<string, string> PropertyMappings { get; }

        static VisitContractResolver()
        {
            PropertyMappings = new Dictionary<string, string>
            {
                {"Id", "id"},
                {"UserId", "user"},
                {"LocationId", "location"},
                {"VisitedAt", "visited_at"},
                {"Mark", "mark"}
            };
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            string resolvedName;
            var resolved = PropertyMappings.TryGetValue(propertyName, out resolvedName);
            return resolved ? resolvedName : base.ResolvePropertyName(propertyName);
        }
    }
}
