using System.Linq;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using System.Collections.Generic;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.StructureClass
{
    /// <summary>
    /// This class is used to store project member structure, and as model for view project member
    /// </summary>
    public class ProjectStructure
    {
        public string CreatorID { get; set; }
        public List<Member> Leaders { get; set; }
        // users individually organize event
        public List<Member> OrganizeUsers { get; set; }
        // groups organize event
        public List<Member> OrganizeGroups { get; set; }
        public List<Member> JoinedGroups { get; set; }
        public List<Member> SponsoredGroups { get; set; }
        public List<Member> JoinedUsers { get; set; }
        public List<Member> SponsoredUsers { get; set; }
        // for individual or guest sponsor
        public List<Sponsor> SponsoredGuests { get; set; }

        public ProjectStructure()
        {
            this.CreatorID = "";
            this.Leaders = new List<Member>();
            this.OrganizeUsers = new List<Member>();
            this.OrganizeGroups = new List<Member>();
            this.JoinedGroups = new List<Member>();
            this.SponsoredGroups = new List<Member>();
            this.SponsoredGuests = new List<Sponsor>();
            this.JoinedUsers = new List<Member>();
            this.SponsoredUsers = new List<Member>();
        }
        public ProjectStructure(SDLink creator)
        {
            Member member = new Member(creator);
            this.CreatorID = creator.ID;
            this.Leaders = new List<Member>();
            this.Leaders.Add(member);
            this.OrganizeUsers = new List<Member>();
            this.OrganizeUsers.Add(member);
            this.OrganizeGroups = new List<Member>();
            this.JoinedGroups = new List<Member>();
            this.SponsoredGroups = new List<Member>();
            this.SponsoredGuests = new List<Sponsor>();
            this.JoinedUsers = new List<Member>();
            this.SponsoredUsers = new List<Member>();
        }
    }
}