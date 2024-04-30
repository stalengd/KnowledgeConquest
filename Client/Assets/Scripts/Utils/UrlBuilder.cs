using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace KnowledgeConquest.Client.Utils
{
    public struct UrlBuilder
    {
        public string BasePath { get; set; }
        public string RelativePath { get; set; }
        public NameValueCollection Query { get; set; }

        public static UrlBuilder FromRelative(string relativePath)
        {
            return new UrlBuilder()
            {
                RelativePath = relativePath,
            };
        }

        public UrlBuilder Param(string key, string value)
        {
            Query ??= new();
            Query[key] = value;
            return this;
        }

        public override readonly string ToString()
        {
            if (BasePath == null)
            {
                throw new NullReferenceException("Base path should not be null");
            }
            var builder = new StringBuilder(BasePath);
            if (RelativePath != null)
            {
                if (builder[^1] != '/')
                {
                    builder.Append('/');
                }
                builder.Append(RelativePath);
            }
            if (Query != null)
            {
                builder.Append('?');
                for (int i = 0; i < Query.Count; i++)
                {
                    var key = Query.GetKey(i);
                    var value = Query[i];
                    builder.Append(HttpUtility.UrlEncode(key));
                    builder.Append("=");
                    builder.Append(HttpUtility.UrlEncode(value));
                    if (i + 1 < Query.Count)
                    {
                        builder.Append('&');
                    }
                }
            }
            return builder.ToString();
        }
    }
}
