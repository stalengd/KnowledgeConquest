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
            if (json is JObject obj)
            {
                return new List<Error>() { ParseError(obj) };
            }
            if (json is JArray array)
            {
                var results = new List<Error>();
                foreach (var item in array)
                {
                    results.Add(ParseError(item as JObject));
                }
                return results;
            }
            return null;
        }

        public static Error ParseError(JObject obj)
        {
            return new Error((string)obj["code"], (string)obj["description"]);
        }
    }
}
