using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace HighLoad.Framework.Json
{
    public class LocationContractResolver : DefaultContractResolver
    {
        public static IReadOnlyDictionary<string, string> PropertyMappings { get; }

        static LocationContractResolver()
        {
            PropertyMappings = new Dictionary<string, string>
            {
                {"Id", "id"},
                {"Place", "place"},
                {"Country", "country"},
                {"City", "city"},
                {"Distance", "distance"}
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
