using MongoDB.Bson;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass
{
    public class GroupLink
    {
        public ObjectId _id { get; set; }
        public string GroupName { get; set; }
    }
}