using MongoDB.Bson.Serialization.Attributes;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass
{
    [BsonIgnoreExtraElements]
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
         public UserSD(AccountInformation accountInfo)
        {
            this.UserID = accountInfo.UserID;
            this.DisplayName = accountInfo.DisplayName;
            this.AvtImgLink = accountInfo.AvtImgLink;
        }
    }
}