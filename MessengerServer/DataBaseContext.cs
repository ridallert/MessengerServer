using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MessengerServer.Common;

namespace MessengerServer
{
    public class DataBaseContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<LogEntry> EventList { get; set; }
        public DataBaseContext(string connectionString) : base(connectionString) { }
    }
}
