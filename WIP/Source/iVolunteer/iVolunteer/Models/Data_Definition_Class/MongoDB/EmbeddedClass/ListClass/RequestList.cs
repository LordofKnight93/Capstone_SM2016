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

        public void Add_GroupJoinRequest(RequestItem request)
        {
            this.GroupJoinRequest.Add(request);
        }

        public void Delete_GroupJoinRequest(string requestID)
        {
            RequestItem item = this.GroupJoinRequest.Find(r => r.RequestID == requestID);
            this.GroupJoinRequest.Remove(item);
        }

        public void Add_UserJoinRequest(RequestItem request)
        {
            this.UserJoinRequest.Add(request);
        }

        public void Delete_UserJoinRequest(string requestID)
        {
            RequestItem item = this.UserJoinRequest.Find(r => r.RequestID == requestID);
            this.UserJoinRequest.Remove(item);
        }

        public void Add_UserSponsorRequest(RequestItem request)
        {
            this.UserSponsorRequest.Add(request);
        }

        public void Delete_UserSponsorRequest(string requestID)
        {
            RequestItem item = this.UserSponsorRequest.Find(r => r.RequestID == requestID);
            this.UserSponsorRequest.Remove(item);
        }

        public void Add_GroupSponsorRequest(RequestItem request)
        {
            this.GroupSponsorRequest.Add(request);
        }

        public void Delete_GroupSponsorRequest(string requestID)
        {
            RequestItem item = this.GroupSponsorRequest.Find(r => r.RequestID == requestID);
            this.GroupSponsorRequest.Remove(item);
        }

        public void Add_GuestSponsorRequest(Sponsor sponsor)
        {
            this.GuestSponsorRequest.Add(sponsor);
        }

        public void Delete_GuestSponsorRequest(string sponsorID)
        {
            Sponsor sponsor = this.GuestSponsorRequest.Find(r => r.SponsorID == sponsorID);
            this.GuestSponsorRequest.Remove(sponsor);
        }
    }
}