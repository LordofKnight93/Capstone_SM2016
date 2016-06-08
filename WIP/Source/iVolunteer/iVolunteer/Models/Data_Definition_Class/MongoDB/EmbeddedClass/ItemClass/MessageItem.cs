using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass
{
    public class MessageItem
    {
        public ObjectId _id { get; set; }
        public DateTime DateSend { get; set; }
        public UserSD Sender { get; set; }
        public string Content { get; set; }

        public MessageItem()
        {
            this._id = new ObjectId();
            this.DateSend = new DateTime();
            this.Sender = new UserSD();
            this.Content = "";
        }
        public MessageItem(UserSD sender, string content)
        {
            this._id = ObjectId.GenerateNewId();
            this.DateSend = DateTime.Now;
            this.Sender = sender;
            this.Content = content;
        }
    }
}