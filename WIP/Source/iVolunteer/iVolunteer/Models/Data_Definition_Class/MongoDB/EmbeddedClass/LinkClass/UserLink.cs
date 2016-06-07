using MongoDB.Bson;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass
{
    public class UserLink
    {
        public ObjectId _id { get; set; }
        public string DisplayName { get; set; }
    }
}