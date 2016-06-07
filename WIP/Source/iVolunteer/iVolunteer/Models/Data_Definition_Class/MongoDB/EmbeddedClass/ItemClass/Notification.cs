using System;
using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass
{
    public class Notification
    {
        public ObjectId _id { get; set; }
        public DateTime DateNotice { get; set; }
        public UserSD[] Actors { get; set; }
        public string Content { get; set; }
        public string PostID { get; set; }
        public GroupLink GroupLink { get; set; }
        public ProjectLink ProjectLink { get; set; }
        public bool IsSeen { get; set; }
    }
}