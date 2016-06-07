using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.StructureClass
{
    /// <summary>
    /// This class is used to store group member structure, and as model for view group member
    /// </summary>
    public class GroupStructure
    {
        public UserSD Creator { get; set; }
        public UserSD[] Leaders { get; set; }
        public Member[] JoinedUsers { get; set; }
    }
}