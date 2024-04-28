using System;

namespace KnowledgeConquest.Client.Connection
{
    public interface IConnectionConfig
    {
        Uri BaseUrl { get; }
        string Password { get; }
        string Username { get; }
    }
}