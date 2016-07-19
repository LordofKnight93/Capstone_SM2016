using System;
using MongoDB.Bson;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using iVolunteer.Common;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    [BsonIgnoreExtraElements]
    public class Notification
    {
        public string NotifyID { get; set; }
        public DateTime DateNotice { get; set; }
        public List<SDLink> Actors { get; set; }
        public int Type { get; set; }
        public SDLink Target { get; set; }
        public SDLink Destination { get; set; }
        public bool IsSeen { get; set; }
        
        public Notification()
        {
            this.NotifyID = "";
            this.DateNotice = new DateTime();
            this.Actors = new List<SDLink>();
            this.Type = 0;
            this.Target = new SDLink();
            this.Destination = new SDLink();
            this.IsSeen = false;
        }

        public Notification(SDLink actor, int type, SDLink target, SDLink destination)
        {
            this.NotifyID = ObjectId.GenerateNewId().ToString();
            this.DateNotice = DateTime.Now;
            this.Actors = new List<SDLink>();
            this.Actors.Add(actor);
            this.Type = type;
            this.Target = target;
            this.Destination = destination;
            this.IsSeen = Status.IS_NOT_SEEN;
        }
        //For New Message
        public Notification(SDLink actor, int type)
        {
            this.NotifyID = ObjectId.GenerateNewId().ToString();
            this.DateNotice = DateTime.Now;
            this.Actors = new List<SDLink>();
            this.Actors.Add(actor);
            this.Type = type;
            this.IsSeen = Status.IS_NOT_SEEN;
        }
    }
}