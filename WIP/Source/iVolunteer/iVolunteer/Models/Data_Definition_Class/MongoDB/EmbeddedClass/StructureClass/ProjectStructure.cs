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
        // users individually organize
        public List<UserSD> OrganizeUser { get; set; }
        // groups organize
        public List<GroupSD> OrganizeGroup { get; set; }
        public List<GroupSD> JoinedGroups { get; set; }
        public List<GroupSD> SponsorGroups { get; set; }
        public List<Member> JoinedUsers { get; set; }
        public List<Sponsor> Sponsors { get; set; }

        public ProjectStructure()
        {
            this.CreatorID = "";
            this.Leaders = new List<UserSD>();
            this.OrganizeUser = new List<UserSD>();
            this.OrganizeGroup = new List<GroupSD>();
            this.JoinedGroups = new List<GroupSD>();
            this.SponsorGroups = new List<GroupSD>();
            this.Sponsors = new List<Sponsor>();
            this.JoinedUsers = new List<Member>();
        }
    }
}