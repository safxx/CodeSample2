using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace HighLoad.Framework.Json
{
    public class UserContractResolver: DefaultContractResolver
    {
        public static IReadOnlyDictionary<string, string> PropertyMappings { get; }

        static UserContractResolver()
        {
            PropertyMappings = new Dictionary<string, string>
        {
            {"Id", "id"},
            {"Email", "email"},
            {"FirstName", "first_name"},
            {"LastName", "last_name"},
            {"Gender", "gender"},
            {"BirthDate", "birth_date"}
        };
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            var resolved = PropertyMappings.TryGetValue(propertyName, out string resolvedName);
            return resolved ? resolvedName : base.ResolvePropertyName(propertyName);
        }
    }
}
