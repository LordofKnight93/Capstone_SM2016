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
    }
}