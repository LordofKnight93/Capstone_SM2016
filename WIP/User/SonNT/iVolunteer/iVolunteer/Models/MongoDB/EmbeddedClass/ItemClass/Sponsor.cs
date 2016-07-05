using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    [BsonIgnoreExtraElements]
    public class Sponsor
    {
        // guest create new or get from logined user
        public string SponsorID { get; set; }
        public string SponsorName { get; set; }
        public string SponsorMail { get; set; }
        public string SponsorPhone { get; set; }
        public string SponsorAddress { get; set; }

        public Sponsor()
        {
            this.SponsorID = "";
            this.SponsorName = "";
            this.SponsorMail = "";
            this.SponsorPhone = "";
            this.SponsorAddress = "";
        }
        //create sponsor with system user information
        public Sponsor(UserInformation userInfo)
        {
            this.SponsorID = userInfo.UserID;
            this.SponsorName = userInfo.RealName;
            this.SponsorMail = userInfo.Email;
            this.SponsorPhone = userInfo.Phone;
            this.SponsorAddress = userInfo.Address;
        }
    }
}