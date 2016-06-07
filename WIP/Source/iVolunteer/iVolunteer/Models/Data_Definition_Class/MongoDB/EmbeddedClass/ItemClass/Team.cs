using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass
{
    public class Team
    {
        // team ID
        public ObjectId _id { get; set; }
        //team's name
        public string TeamName { get; set; }
        // array members of team
        public UserSD[] Members { get; set; }
    }
}