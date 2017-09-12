using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace HighLoad.Framework.Json
{
    public class TravelingContractResolver : DefaultContractResolver
    {
        public static IReadOnlyDictionary<string, string> PropertyMappings { get; }

        static TravelingContractResolver()
        {
            var globalMappings = new Dictionary<string, string>
            {
                {"Avg", "avg"},
                {"Visits", "visits"}
            };

            PropertyMappings = globalMappings
                .Concat(VisitContractResolver.PropertyMappings)
                .Concat(UserContractResolver.PropertyMappings)
                .Concat(LocationContractResolver.PropertyMappings)
                .Distinct()
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            var resolved = PropertyMappings.TryGetValue(propertyName, out string resolvedName);
            return resolved ? resolvedName : base.ResolvePropertyName(propertyName);

        }
    }
}