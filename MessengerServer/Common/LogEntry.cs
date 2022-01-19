using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer.Common
{
    public class LogEntry
    {
        private static int _idCounter;
        public int LogEntryId { get; set; }
        public EventType Type { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }

        public LogEntry(EventType type, string message, DateTime dateTime)
        {
            LogEntryId = _idCounter++;
            Type = type;
            Message = message;
            DateTime = dateTime;
        }
    }
}
