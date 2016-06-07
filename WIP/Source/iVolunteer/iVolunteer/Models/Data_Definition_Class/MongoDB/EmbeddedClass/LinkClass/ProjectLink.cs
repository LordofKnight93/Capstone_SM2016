using MongoDB.Bson;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass
{
    public class ProjectLink
    {
        public ObjectId _id { get; set; }
        public string ProjectName { get; set; }
    }
}