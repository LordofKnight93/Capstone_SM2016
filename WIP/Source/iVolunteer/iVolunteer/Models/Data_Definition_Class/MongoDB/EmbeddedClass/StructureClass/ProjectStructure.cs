using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.StructureClass
{
    /// <summary>
    /// This class is used to store project member structure, and as model for view project member
    /// </summary>
    public class ProjectStructure
    {
        public string CreatorID { get; set; }
        public UserSD[] Leaders { get; set; }
        // users individually organize
        public UserSD[] OrganizeUser { get; set; }
        // groups organize
        public GroupSD[] OrganizeGroup { get; set; }
        public GroupSD[] JoinedGroups { get; set; }
        public GroupSD[] SponsorGroups { get; set; }
        public Member[] JoinedUsers { get; set; }
        public Sponsor[] Sponsors { get; set; }
    }
}