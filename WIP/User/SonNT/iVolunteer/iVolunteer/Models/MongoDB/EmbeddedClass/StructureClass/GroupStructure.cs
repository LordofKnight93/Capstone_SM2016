using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.StructureClass
{
    /// <summary>
    /// This class is used to store group member structure, and as model for view group member
    /// </summary>
    [BsonIgnoreExtraElements]
    public class GroupStructure
    {
        public string CreatorID { get; set; }
        public List<Member> Leaders { get; set; }
        [BsonIgnoreIfDefault]
        public List<Member> JoinedUsers { get; set; }

        public GroupStructure()
        {
            this.CreatorID = "";
            this.Leaders = new List<Member>();
            this.JoinedUsers = new List<Member>();
        }

        public GroupStructure(SDLink creator)
        {
            Member member = new Member(creator);
            this.CreatorID = creator.ID;
            this.Leaders = new List<Member>();
            this.JoinedUsers = new List<Member>();
            this.Leaders.Add(member);
        }
    }
}