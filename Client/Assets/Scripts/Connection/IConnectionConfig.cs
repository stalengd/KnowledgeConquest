namespace KnowledgeConquest.Client.Connection
{
    public interface IConnectionConfig
    {
        string BaseUrl { get; }
        string Password { get; }
        string Username { get; }
    }
}