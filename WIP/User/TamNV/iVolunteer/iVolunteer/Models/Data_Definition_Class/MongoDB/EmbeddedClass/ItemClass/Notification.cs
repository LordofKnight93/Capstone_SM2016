using System;
using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using System.Collections.Generic;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass
{
    public class Notification
    {
        public ObjectId _id { get; set; }
        public DateTime DateNotice { get; set; }
        public List<UserSD> Actors { get; set; }
        public string Content { get; set; }
        public string PostID { get; set; }
        public GroupLink GroupLink { get; set; }
        public ProjectLink ProjectLink { get; set; }
        public bool IsSeen { get; set; }
        
        public Notification()
        {
            this._id = new ObjectId();
            this.DateNotice = new DateTime();
            this.Actors = new List<UserSD>();
            this.Content = "";
            this.PostID = "";
            this.GroupLink = new GroupLink();
            this.ProjectLink = new ProjectLink();
            this.IsSeen = false;
        }
    }
}