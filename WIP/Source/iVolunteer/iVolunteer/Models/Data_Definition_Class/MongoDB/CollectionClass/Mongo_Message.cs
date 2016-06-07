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
        public MessageItem MessageList { get; set; }
        public UserSD[] Senders { get; set; }
    }
}