using MongoDB.Bson;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass
{
    public class Sponsor
    {
        public ObjectId _id { get; set; }
        public string SponsorName { get; set; }
        public string SponsorMail { get; set; }
        public string SponsorPhone { get; set; }
        public string SponsorAddress { get; set; }

        public Sponsor()
        {
            this._id = new ObjectId();
            this.SponsorName = "";
            this.SponsorMail = "";
            this.SponsorPhone = "";
            this.SponsorAddress = "";
        }
    }
}