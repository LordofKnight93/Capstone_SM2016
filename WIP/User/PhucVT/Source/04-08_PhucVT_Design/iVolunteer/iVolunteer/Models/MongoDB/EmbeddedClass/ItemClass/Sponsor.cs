using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    [BsonIgnoreExtraElements]
    public class Sponsor
    {
        public string SponsorID { get; set; }
        public string SponsorName { get; set; }
        public string SponsorAddress { get; set; }
        public string SponsorEmail { get; set; }
        public string SponsorPhone { get; set; }
        public bool Status { get; set; }

        public Sponsor()
        {
            this.SponsorID = "";
            this.SponsorName = "";
            this.SponsorAddress = "";
            this.SponsorEmail = "";
            this.SponsorPhone = "";
            this.Status = false;
        }
    }
}