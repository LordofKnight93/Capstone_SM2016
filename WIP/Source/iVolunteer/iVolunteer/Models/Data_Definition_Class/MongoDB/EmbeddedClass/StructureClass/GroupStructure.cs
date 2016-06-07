using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;
using System.Collections.Generic;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.StructureClass
{
    /// <summary>
    /// This class is used to store group member structure, and as model for view group member
    /// </summary>
    public class GroupStructure
    {
        public UserSD Creator { get; set; }
        public List<UserSD> Leaders { get; set; }
        public List<Member> JoinedUsers { get; set; }

        public GroupStructure()
        {
            this.Creator = new UserSD();
            this.Leaders = new List<UserSD>();
            this.JoinedUsers = new List<Member>();
        }
    }
}