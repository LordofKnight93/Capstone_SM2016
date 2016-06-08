using System.Linq;
using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using System.Collections.Generic;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass
{
    public class Team
    {
        // team ID
        public ObjectId _id { get; set; }
        //team's name
        public string TeamName { get; set; }
        // array members of team
        public List<UserSD> Members { get; set; }

        public Team()
        {
            this._id = new ObjectId();
            this.TeamName = "";
            this.Members = new List<UserSD>();
        }

        public Team(string name)
        {
            this._id = ObjectId.GenerateNewId();
            this.TeamName = name;
            this.Members = new List<UserSD>();
        }
        public void Add_Member(UserSD newMember)
        {
            this.Members.Add(newMember);
        }
        public void Delete_Member(string memberID)
        {
            UserSD member = this.Members.Find(mem => mem.UserID == memberID);
            this.Members.Remove(member);
        }
    }
}