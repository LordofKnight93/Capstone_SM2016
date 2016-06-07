using MongoDB.Bson;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass
{
    public class AlbumLink
    {
        public ObjectId _id { get; set; }
        public string AlbumName { get; set; }
    }
}