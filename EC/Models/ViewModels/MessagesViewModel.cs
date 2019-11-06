using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EC.Models;
using EC.Models.ECModel;
using EC.Models.Database;

namespace EC.Controllers.ViewModel
{
    public class MessagesViewModel
    {
        public Int32 CallerUserId { get; set; }
        public Int32 Unread_Number { get; set; }

        public List<MessageExtended> _messages = new List<MessageExtended>();
        private MessageExtended _extended;



        public MessagesViewModel()
        {
            CallerUserId = 0;
            Unread_Number = 0;
            _messages = new List<MessageExtended>();
        }

        public MessagesViewModel(int user_id)
        {
            UserModel um = new UserModel(user_id);
            CallerUserId = user_id;
            Unread_Number = um.Unread_Messages_Quantity(0, 1) + um.Unread_Messages_Quantity(0, 2) + um.Unread_Messages_Quantity(0, 3);
            List<message> _message = um.UserMessages(0, 1).ToList();
            List<message> _message1 = um.UserMessages(0, 2).ToList();
            List<message> _message2 = um.UserMessages(0, 3).ToList();
            _message.AddRange(_message1);
            _message.AddRange(_message2);

            _message = _message.OrderByDescending(item => item.created_dt).ToList();

            foreach (message temp_message in _message)
            {
                _extended = new MessageExtended(temp_message.id, CallerUserId);
                _messages.Add(_extended);
            }

            _messages = _messages.OrderByDescending(item => item._message.created_dt).ToList();
        }
    }

}