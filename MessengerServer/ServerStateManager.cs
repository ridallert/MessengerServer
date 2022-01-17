using MessengerServer.Common;
using MessengerServer.Network;
using MessengerServer.Network.EventArgs;
using MessengerServer.Network.Responses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerServer
{
    public class ServerStateManager
    {
        private List<Contact> _contacts;
        private List<Message> _messages;
        private Contact _publicChat;
        private List<LogEntry> _eventList;
        public List<Contact> Contacts
        {
            get { return _contacts; }
            set
            {
                _contacts = value;
                UserListChanged?.Invoke();
            }
        }
        public List<Message> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }
        public Contact PublicChat
        {
            get { return _publicChat; }
            set { _publicChat = value; }
        }
        private List<LogEntry> EventList
        {
            get { return _eventList; }
            set { _eventList = value; }
        }

        public event Action UserListChanged;
        public event EventHandler<Message> PrivateMessageReceived;
        public event EventHandler<Message> PublicMessageReceived;
        public ServerStateManager()
        {
            Contacts = new List<Contact>();
            Messages = new List<Message>();
            EventList = new List<LogEntry>();

            PublicChat = new Contact("Public chat");
            Contacts.Add(new Contact("Евгений", OnlineStatus.Offline));
            Contacts.Add(new Contact("Яков", OnlineStatus.Offline));
            Contacts.Add(new Contact("Виктория", OnlineStatus.Online));
            Contacts.Add(new Contact("Мария", OnlineStatus.Offline));
            Contacts.Add(new Contact("Ридаль", OnlineStatus.Offline));

            foreach (Contact contact in Contacts)
            {
                if (contact.Title != "Public chat" && !contact.IsGroop())
                {
                    PublicChat.Users.Add(contact.Title);
                }
            }

            Messages.Add(new Message(Contacts[0].Title, Contacts[2].Title, Contacts[2].Title + ", Привет от " + Contacts[0].Title + "!", DateTime.Now));
            Messages.Add(new Message(Contacts[1].Title, Contacts[2].Title, Contacts[2].Title + ", Привет от " + Contacts[1].Title + "!", DateTime.Now));
            Messages.Add(new Message(Contacts[3].Title, Contacts[2].Title, Contacts[2].Title + ", Привет от " + Contacts[3].Title + "!", DateTime.Now));
            Messages.Add(new Message(Contacts[4].Title, Contacts[2].Title, Contacts[2].Title + ", Привет от " + Contacts[4].Title + "!", DateTime.Now));
            Messages.Add(new Message(Contacts[2].Title, Contacts[1].Title, Contacts[1].Title + ", Привет от " + Contacts[2].Title + "!", DateTime.Now));
            Messages.Add(new Message(Contacts[3].Title, Contacts[4].Title, Contacts[4].Title + ", Привет от " + Contacts[3].Title + "!", DateTime.Now));
            Messages.Add(new Message(Contacts[0].Title, Contacts[3].Title, Contacts[3].Title + ", Привет от " + Contacts[0].Title + "!", DateTime.Now));

            Messages.Add(new Message(Contacts[0].Title, Contacts[3].Title, Contacts[3].Title + ", Привет от " + Contacts[0].Title + "!", DateTime.Now));
            Messages.Add(new Message(Contacts[1].Title, Contacts[3].Title, Contacts[3].Title + ", Привет от " + Contacts[1].Title + "!", DateTime.Now));
            Messages.Add(new Message(Contacts[3].Title, Contacts[3].Title, Contacts[3].Title + ", Привет от " + Contacts[3].Title + "!", DateTime.Now));
            Messages.Add(new Message(Contacts[4].Title, Contacts[3].Title, Contacts[3].Title + ", Привет от " + Contacts[4].Title + "!", DateTime.Now));
            Messages.Add(new Message(Contacts[2].Title, Contacts[2].Title, Contacts[2].Title + ", Привет от " + Contacts[2].Title + "!", DateTime.Now));
            Messages.Add(new Message(Contacts[3].Title, Contacts[0].Title, Contacts[0].Title + ", Привет от " + Contacts[3].Title + "!", DateTime.Now));
            Messages.Add(new Message(Contacts[0].Title, Contacts[4].Title, Contacts[4].Title + ", Привет от " + Contacts[0].Title + "!", DateTime.Now));

            Messages.Add(new Message(Contacts[0].Title, PublicChat.Title, "Привет всем от " + Contacts[0].Title + "!", DateTime.Now));
            Messages.Add(new Message(Contacts[1].Title, PublicChat.Title, "Привет всем от " + Contacts[1].Title + "!", DateTime.Now));
            Messages.Add(new Message(Contacts[2].Title, PublicChat.Title, "Привет всем от " + Contacts[2].Title + "!", DateTime.Now));

            DateTime startDate = new DateTime(2022, 1, 12, 16, 45, 58);

            foreach (Contact contact in Contacts)
            {
                startDate = startDate.AddDays(1);
                EventList.Add(new LogEntry(EventType.Event, contact.Title + " is joined", startDate));
            }
            foreach (Message message in Messages)
            {
                EventList.Add(new LogEntry(EventType.Message, message.Sender + " sent а private message to " + message.Receiver, message.SendTime));
            }
        }
        public AuthorizationResponse AuthorizeUser(string name)
        {
            bool isUserAlreadyExists = false;
            bool isNameTaken = false;

            foreach (Contact contact in Contacts)
            {
                if (contact.Title == name)
                {
                    isUserAlreadyExists = true;

                    if (contact.IsOnline == OnlineStatus.Offline)
                    {
                        contact.IsOnline = OnlineStatus.Online;
                    }
                    else
                    {
                        isNameTaken = true;
                    }
                }
            }

            if (isUserAlreadyExists)
            {
                if (isNameTaken)
                {
                    return new AuthorizationResponse("NameIsTaken", name);
                }
                else
                {
                    return new AuthorizationResponse("AlreadyExists", name);
                }
            }
            else
            {
                Contacts.Add(new Contact(name, OnlineStatus.Online));
                return new AuthorizationResponse("NewUserAdded", name);
            }
        }


        public void SetUserOffline(string name)
        {
            foreach (Contact contact in Contacts)
            {
                if (contact.Title == name)
                {
                    contact.IsOnline = OnlineStatus.Offline;
                }
            }
        }
        public GetContactsResponse GetContacts(string name)
        {
            List<Contact> contactList = new List<Contact>();
            contactList = Contacts.FindAll(contact => contact.Title != name);

            if (contactList.Count != 0)
            {
                return new GetContactsResponse("Success", contactList);
            }
            else
            {
                return new GetContactsResponse("Empty", new List<Contact>());
            }
        }

        public void AddPrivateMessage(string sender, string receiver, string text, DateTime sendTime)
        {
            if (Contacts.Exists(contact => contact.Title == sender && !contact.IsGroop()))
            {
                if (Contacts.Exists(contact => contact.Title == receiver && !contact.IsGroop()))
                {
                    Message message = new Message(sender, receiver, text, sendTime);
                    Messages.Add(message);
                    PrivateMessageReceived?.Invoke(this, message);
                }
                else
                {
                    throw new Exception("AddPrivateMessage: Sender '" + receiver + "' does not exist");
                }
            }
            else
            {
                throw new Exception("AddPrivateMessage: Sender '" + sender + "' does not exist");
            }
        }
        public void AddPublicMessage(string sender, string text, DateTime sendTime)
        {
            if (Contacts.Exists(contact => contact.Title == sender && !contact.IsGroop()))
            {
                Message message = new Message(sender, PublicChat.Title, text, sendTime);
                Messages.Add(message);
                PublicMessageReceived?.Invoke(this, message);
            }
            else
            {
                throw new Exception("AddPublicMessage: Sender '" + sender + "' does not exist");
            }
        }

        public GetPrivateMessageListResponse GetPrivateMessageList(string name)
        {
            List<Message> messages = new List<Message>();

            if (Contacts.Exists(contact => contact.Title == name && !contact.IsGroop()))
            {
                messages = Messages.FindAll(message => message.Sender == name || message.Receiver == name);
            }

            if (messages.Count != 0)
            {
                return new GetPrivateMessageListResponse("Success", messages);
            }
            else
            {
                return new GetPrivateMessageListResponse("Empty", messages);
            }
        }
        public GetPublicMessageListResponse GetPublicMessageList()
        {
            List<Message> messages = Messages.FindAll(message => message.Receiver == "Public chat");

            if (messages.Count != 0)
            {
                return new GetPublicMessageListResponse("Success", messages);
            }
            else
            {
                return new GetPublicMessageListResponse("Empty", messages);
            }
        }
        public GetEventListResponse GetEventLog(DateTime from, DateTime to)
        {
            List<LogEntry> logResponseList = EventList.FindAll(entry => entry.DateTime >= from && entry.DateTime <= to.AddDays(1));

            return new GetEventListResponse("Success", logResponseList);
        }
    }
}
