using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace KnowledgeConquest.Client.Connection
{
    public static class Validation
    {
        public readonly struct Error
        {
            public string Code { get; }
            public string Description { get; }

            public Error(string code, string description)
            {
                Code = code;
                Description = description;
            }
        }

        public static List<Error> ParseErrors(JToken json)
        {
            var results = new List<Error>();
            foreach ((var errorKey, var errorsGroup) in json["errors"] as JObject)
            {
                foreach (var error in errorsGroup)
                {
                    results.Add(new Error(errorKey, error.Value<string>()));
                }
            }
            return results;
        }
    }
}
