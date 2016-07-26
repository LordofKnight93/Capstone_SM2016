using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    [BsonIgnoreExtraElements]
    public class TeamFoundDonator
    {
        public ObjectId TeamFoundDonatorID { get; set; }
        public string Name { get; set; }
        public string Information { get; set; }
        public double Amount { get; set; }
        public DateTime ReceiveDate { get; set; }
        public int Method { get; set; }
        public bool IsReceived { get; set; }
        public TeamFoundDonator()
        {
            this.TeamFoundDonatorID = ObjectId.GenerateNewId();
            this.Name = "";
            this.Information = "";
            this.Amount = 0;
            this.ReceiveDate = new DateTime();
            this.Method = 0;
            this.IsReceived = false;
        }
    }
}