using System.Linq;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;
using System.Collections.Generic;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.StructureClass
{
    /// <summary>
    /// This class is used to store project member structure, and as model for view project member
    /// </summary>
    public class ProjectStructure
    {
        public string CreatorID { get; set; }
        public List<UserSD> Leaders { get; set; }
        // users individually organize event
        public List<UserSD> OrganizeUsers { get; set; }
        // groups organize event
        public List<GroupSD> OrganizeGroups { get; set; }
        public List<GroupSD> JoinedGroups { get; set; }
        public List<GroupSD> SponsoredGroups { get; set; }
        public List<Member> JoinedUsers { get; set; }
        public List<UserSD> SponsoredUsers { get; set; }
        // for individual or guest sponsor
        public List<Sponsor> SponsoredGuests { get; set; }

        public ProjectStructure()
        {
            this.CreatorID = "";
            this.Leaders = new List<UserSD>();
            this.OrganizeUsers = new List<UserSD>();
            this.OrganizeGroups = new List<GroupSD>();
            this.JoinedGroups = new List<GroupSD>();
            this.SponsoredGroups = new List<GroupSD>();
            this.SponsoredGuests = new List<Sponsor>();
            this.JoinedUsers = new List<Member>();
            this.SponsoredUsers = new List<UserSD>();
        }
        public ProjectStructure(UserSD creator)
        {
            this.CreatorID = creator.UserID;
            this.Leaders = new List<UserSD>();
            this.OrganizeUsers = new List<UserSD>();
            this.OrganizeGroups = new List<GroupSD>();
            this.JoinedGroups = new List<GroupSD>();
            this.SponsoredGroups = new List<GroupSD>();
            this.SponsoredGuests = new List<Sponsor>();
            this.JoinedUsers = new List<Member>();
            this.SponsoredUsers = new List<UserSD>();
            this.Add_Leader(creator);
            this.Add_OrganizeUser(creator);
            this.Add_Member(creator);
        }

        public void Add_Leader(UserSD leader)
        {
            this.Leaders.Add(leader);
        }

        public void Delete_Leader(string leaderID)
        {
            UserSD leader = this.Leaders.Find(l => l.UserID == leaderID);
            this.Leaders.Remove(leader);
        }

        public void Add_Member(UserSD user)
        {
            Member member = new Member(user);
            this.JoinedUsers.Add(member);
        }

        public void Delete_Member(string memberID)
        {
            Member member = this.JoinedUsers.Find(m => m.User.UserID == memberID);
            this.JoinedUsers.Remove(member);
        }

        public void Add_JoinedGroup(GroupSD group)
        {
            this.JoinedGroups.Add(group);
        }

        public void Delete_JoinedGroup(string groupID)
        {
            GroupSD group = this.JoinedGroups.Find(g => g.GroupID == groupID);
            this.JoinedGroups.Remove(group);
        }

        public void Add_OrganizeUser(UserSD user)
        {
            this.OrganizeUsers.Add(user);
        }

        public void Delete_OrganizeUser(string userID)
        {
            UserSD user = this.OrganizeUsers.Find(m => m.UserID == userID);
            this.OrganizeUsers.Remove(user);
        }

        public void Add_OrganizeGroup(GroupSD group)
        {
            this.OrganizeGroups.Add(group);
        }

        public void Delete_OrganizeGroup(string groupID)
        {
            GroupSD group = this.OrganizeGroups.Find(g => g.GroupID == groupID);
            this.OrganizeGroups.Remove(group);
        }

        public void Add_SponsorGroup(GroupSD group)
        {
            this.SponsoredGroups.Add(group);
        }

        public void Delete_SponsorGroup(string groupID)
        {
            GroupSD group = this.SponsoredGroups.Find(g => g.GroupID == groupID);
            this.SponsoredGroups.Remove(group);
        }

        public void Add_SponsoredUser(UserSD user)
        {
            this.SponsoredUsers.Add(user);
        }

        public void Delete_SponsoredUser(string userID)
        {
            UserSD user = this.SponsoredUsers.Find(s => s.UserID == userID);
            this.SponsoredUsers.Remove(user);
        }

        public void Add_SponsoredGuest(Sponsor sponsor)
        {
            this.SponsoredGuests.Add(sponsor);
        }

        public void Delete_SponsoredGuest(string sponsorID)
        {
            Sponsor sponsor = this.SponsoredGuests.Find(s => s.SponsorID == sponsorID);
            this.SponsoredGuests.Remove(sponsor);
        }
    }
}