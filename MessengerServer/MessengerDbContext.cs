namespace MessengerServer.Data
{
    using System.Data.Entity;

    using MessengerServer.DataObjects;

    public class MessengerDbContext : DbContext
    {
        #region Properties

        public DbSet<User> Users { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<LogEntry> EventList { get; set; }

        #endregion //Properties

        #region Constructors

        public MessengerDbContext(string connectionString) : base(connectionString) { }

        #endregion //Constructors
    }
}
