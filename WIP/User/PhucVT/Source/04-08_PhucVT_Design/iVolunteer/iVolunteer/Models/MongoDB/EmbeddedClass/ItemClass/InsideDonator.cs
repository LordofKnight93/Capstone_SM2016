using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    [BsonIgnoreExtraElements]
    public class InsideDonator
    {
        public SDLink Donator { get; set; }
        public double Amount { get; set; }
        public DateTime ReceivedDate { get; set; }
        public int Method { get; set; }
        public bool IsReceived { get; set; }
        public InsideDonator()
        {
            this.Donator = new SDLink();
            this.Amount = 0;
            this.ReceivedDate = new DateTime();
            this.Method = 0;
            this.IsReceived = false;
        }
    }
}