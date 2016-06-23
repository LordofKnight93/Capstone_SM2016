using System.Linq;
using MongoDB.Bson;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using System.Collections.Generic;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    public class Team
    {
        // team ID
        public string  TeamID { get; set; }
        //team's name
        public string TeamName { get; set; }
        public int MemberCount { get; set; }
        // array members of team
        public List<SDLink> Members { get; set; }

        public Team()
        {
            this.TeamID = "";
            this.TeamName = "";
            this.MemberCount = 0;
            this.Members = new List<SDLink>();
        }

        public Team(string name)
        {
            this.TeamID = ObjectId.GenerateNewId().ToString();
            this.TeamName = name;
            this.MemberCount = 0;
            this.Members = new List<SDLink>();
        }
    }
}