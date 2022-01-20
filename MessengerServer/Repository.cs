using MessengerServer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer
{
    public class Repository
    {
        private ServerStateContext _dataBase;

        public void Connect(string connectionString)
        {
            try
            {
                _dataBase = new ServerStateContext(connectionString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }
        public List<Contact> GetContacts()
        {
            //var list = _dataBase.Contacts;
            var contacts = _dataBase.Contacts;
            List<Contact> contactList = new List<Contact>();

            foreach (Contact contact in contacts)
            {
                contactList.Add(contact);
                //contactList.Add(new Contact(contact.))
            }
            return contactList;
        }
        public List<Message> GetMessages()
        {
            return new List<Message>();
        }
        public List<LogEntry> GetEventLog()
        {
            return new List<LogEntry>();
        }
        public void AddContact(Contact contact)
        {
            _dataBase.Contacts.Add(contact);
            _dataBase.SaveChanges();
        }
        public void AddContacts(List<Contact> contacts)
        {
            _dataBase.Contacts.AddRange(contacts);
            _dataBase.SaveChanges();
        }
        public void AddMessage(Message message)
        {
            _dataBase.Messages.Add(message);
            _dataBase.SaveChanges();
        }
        public void AddMessages(List<Message> messages)
        {
            _dataBase.Messages.AddRange(messages);
            _dataBase.SaveChanges();
        }
        public void AddLogEntry(LogEntry entry)
        {
            _dataBase.EventList.Add(entry);
            _dataBase.SaveChanges();
        }
        public void AddLogEntries(List<LogEntry> entries)
        {
            _dataBase.EventList.AddRange(entries);
            _dataBase.SaveChanges();
        }
    }
}
