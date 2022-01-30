namespace MessengerServer.Common
{
    using System;
    public class LogEntry
    {
        private static int _idCounter;
        public int LogEntryId { get; set; }
        public EventType Type { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }

        public LogEntry(EventType type, string message, DateTime dateTime) : this()
        {
            Type = type;
            Message = message;
            DateTime = dateTime;
        }
        public LogEntry()
        {
            LogEntryId = ++_idCounter;
        }
    }
}
