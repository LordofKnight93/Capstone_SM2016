using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ListClass
{
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