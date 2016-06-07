using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;


namespace iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass
{
    /// <summary>
    /// This class define structure of "Message" collection in MongoDB
    /// </summary>
    public class Mongo_Message
    {
        public ObjectId _id { get; set; }
        public DateTime DateLastActivity { get; set; }
        public List<MessageItem> MessageList { get; set; }
        public List<UserSD> Senders { get; set; }
        public Mongo_Message()
        {
            this._id = new ObjectId();
            this.DateLastActivity = new DateTime();
            this.MessageList = new List<MessageItem>();
            this.Senders = new List<UserSD>();
        }
    }
}