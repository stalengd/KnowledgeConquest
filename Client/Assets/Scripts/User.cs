namespace KnowledgeConquest.Client
{
    public sealed class User
    {
        public string Id { get; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }

        public User(string id)
        {
            Id = id;
        }
    }
}
