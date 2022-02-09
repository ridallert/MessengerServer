namespace MessengerServer.Data
{
    using System.Data.Entity;
    using MessengerServer.Common;

    public class MessengerDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<LogEntry> EventList { get; set; }
        public MessengerDbContext(string connectionString) : base(connectionString) { }
    }
}
