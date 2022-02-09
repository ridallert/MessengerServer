namespace MessengerServer.DataObjects
{
    using System;
    public class LogEntry
    {
        public int LogEntryId { get; set; }
        public EventType Type { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }

        public LogEntry(EventType type, string message, DateTime sendTime) : this()
        {
            Type = type;
            Message = message;
            DateTime = sendTime;
        }
        public LogEntry(EventType type, string message) : this()
        {
            Type = type;
            Message = message;
            DateTime = DateTime.Now;
        }
        public LogEntry() {}
    }
}
