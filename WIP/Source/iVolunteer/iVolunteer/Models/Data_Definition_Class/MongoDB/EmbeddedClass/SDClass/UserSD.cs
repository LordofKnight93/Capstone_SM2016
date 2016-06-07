using MongoDB.Bson;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass
{
    public class UserSD
    {
        public ObjectId _id { get; set; }
        public string DisplayName { get; set; }
        public string AvtImgLink { get; set; }
    }
}