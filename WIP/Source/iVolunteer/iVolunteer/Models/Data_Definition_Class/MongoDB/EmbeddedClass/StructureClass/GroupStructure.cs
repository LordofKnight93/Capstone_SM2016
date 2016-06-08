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
        public string CreatorID { get; set; }
        public List<UserSD> Leaders { get; set; }
        public List<Member> JoinedUsers { get; set; }

        public GroupStructure()
        {
            this.CreatorID = "";
            this.Leaders = new List<UserSD>();
            this.JoinedUsers = new List<Member>();
        }

        public GroupStructure(UserSD creator)
        {
            this.CreatorID = creator.UserID;
            this.Leaders = new List<UserSD>();
            this.JoinedUsers = new List<Member>();
            this.Add_Leader(creator);
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
    }
}