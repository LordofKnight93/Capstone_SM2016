using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass
{
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

        public Sponsor(UserInformation userInfo)
        {
            this.SponsorName = userInfo.RealName;
            this.SponsorMail = userInfo.Email;
            this.SponsorPhone = userInfo.Phone;
            this.SponsorAddress = userInfo.Address;
        }
    }
}