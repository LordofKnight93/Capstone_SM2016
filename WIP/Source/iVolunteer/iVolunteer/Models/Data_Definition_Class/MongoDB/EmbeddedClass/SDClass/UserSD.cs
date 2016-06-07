using MongoDB.Bson;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass
{
    public class UserSD
    {
        public string UserID { get; set; }
        public string DisplayName { get; set; }
        public string AvtImgLink { get; set; }

        public UserSD()
        {
            this.UserID = "";
            this.DisplayName = "";
            this.AvtImgLink = "";
        }
    }
}