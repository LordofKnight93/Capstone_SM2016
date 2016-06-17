using MongoDB.Bson;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass
{
    public class UserLink
    {
        public string UserID { get; set; }
        public string DisplayName { get; set; }
        public UserLink()
        {
            this.UserID = "";
            this.DisplayName = "";
        }
    }
}