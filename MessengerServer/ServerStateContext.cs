using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MessengerServer.Common;

namespace MessengerServer
{
    public class ServerStateContext: DbContext
    {
        public DbSet<Message> Messages { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<LogEntry> EventList { get; set; }
        public ServerStateContext() : base("DBConnection") { }
    }
}
