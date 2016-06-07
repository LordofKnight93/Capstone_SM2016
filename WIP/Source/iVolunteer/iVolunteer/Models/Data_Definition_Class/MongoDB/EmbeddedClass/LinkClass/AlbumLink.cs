using MongoDB.Bson;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass
{
    public class AlbumLink
    {
        public string AlbumID { get; set; }
        public string AlbumName { get; set; }

        public AlbumLink()
        {
            this.AlbumID = "";
            this.AlbumName = "";
        }
    }
}