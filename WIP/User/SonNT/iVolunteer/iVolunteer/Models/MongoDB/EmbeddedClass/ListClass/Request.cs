using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ListClass
{
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
    }
    //use for store quest in project
    public class RequestList
    {
        public List<RequestItem> GroupJoinRequest { get; set; }
        public List<RequestItem> UserJoinRequest { get; set; }
        public List<RequestItem> GroupSponsorRequest { get; set; }
        public List<RequestItem> UserSponsorRequest { get; set; }
        // guest sonponsor request
        public List<Sponsor> GuestSponsorRequest { get; set; }

        public RequestList()
        {
            this.GroupJoinRequest = new List<RequestItem>();
            this.UserJoinRequest = new List<RequestItem>();
            this.UserSponsorRequest = new List<RequestItem>();
            this.GroupSponsorRequest = new List<RequestItem>();
            this.GuestSponsorRequest = new List<Sponsor>();
        }
    }
}