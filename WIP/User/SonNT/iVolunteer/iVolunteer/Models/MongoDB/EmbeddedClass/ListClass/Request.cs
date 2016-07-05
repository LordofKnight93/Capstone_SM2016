using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ListClass
{
    [BsonIgnoreExtraElements]
    public class RequestItem
    {
        public string RequestID { get; set; }
        public SDLink Actor { get; set; }
        public int Type { get; set; }
        public SDLink Destination { get; set; }

        public RequestItem()
        {
            this.RequestID = "";
            this.Actor = new SDLink();
            this.Type = 0;
            this.Destination = new SDLink();
        }

        public RequestItem(SDLink actor, int type, SDLink destination)
        {
            this.RequestID = ObjectId.GenerateNewId().ToString();
            this.Actor = actor;
            this.Type = type;
            this.Destination = destination;
        }
    }
    //use for store quest in project
    [BsonIgnoreExtraElements]
    public class RequestList
    {
        [BsonIgnoreIfDefault]
        public List<RequestItem> JoinRequests { get; set; }
        [BsonIgnoreIfDefault]
        public List<RequestItem> SponsorRequests { get; set; }
        [BsonIgnoreIfDefault]
        public List<RequestItem> SuggestUsers { get; set; }
        [BsonIgnoreIfDefault]
        // guest sonponsor request
        public List<Sponsor> GuestSponsorRequests { get; set; }

        public RequestList()
        {
            this.JoinRequests = new List<RequestItem>();
            this.SponsorRequests = new List<RequestItem>();
            this.SuggestUsers = new List<RequestItem>();
            this.GuestSponsorRequests = new List<Sponsor>();
        }
    }
}