using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;


namespace iVolunteer.Models.MongoDB.CollectionClass
{
    /// <summary>
    /// This class define structure of "Message" collection in MongoDB
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Mongo_Message
    {
        public ObjectId _id { get; set; }
        public DateTime DateLastActivity { get; set; }
        public List<MessageItem> ItemList { get; set; }
        public List<SDLink> Senders { get; set; }
        public Mongo_Message()
        {
            this._id = new ObjectId();
            this.DateLastActivity = new DateTime();
            this.ItemList = new List<MessageItem>();
            this.Senders = new List<SDLink>();
        }
    }

    public class MessageItem
    {
        public ObjectId _id { get; set; }
        public DateTime DateSend { get; set; }
        public SDLink Sender { get; set; }
        public string Content { get; set; }
        public List<SDLink> SeenBy { get; set; }

        public MessageItem()
        {
            this._id = new ObjectId();
            this.DateSend = new DateTime();
            this.Sender = new SDLink();
            this.Content = "";
            this.SeenBy = new List<SDLink>();
        }
        public MessageItem(SDLink sender, string content)
        {
            this._id = ObjectId.GenerateNewId();
            this.DateSend = DateTime.Now;
            this.Sender = sender;
            this.Content = content;
            this.SeenBy = new List<SDLink>();
            this.SeenBy.Add(sender);
        }
    }
}