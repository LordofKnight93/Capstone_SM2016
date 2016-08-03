using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iVolunteer.Models.MongoDB.CollectionClass
{
    public class Mongo_Fund
    {
        public ObjectId _id { get; set; }
        public SDLink Project { get; set; }
        public double TotalMoney { get; set; }
        public double ReceivedMoney { get; set; }
        public double PendingMoney { get; set; }
        public List<InsideDonator> InsideDonator { get; set; }
        public List<OutsideDonator> OutsideDonator { get; set; }
        public List<TeamFoundDonator> TeamFoundDonator { get; set; }
        public Mongo_Fund()
        {
            this._id = ObjectId.GenerateNewId();
            this.Project = new SDLink();
            this.TotalMoney = 0;
            this.ReceivedMoney = 0;
            this.PendingMoney = 0;
            this.InsideDonator = new List<InsideDonator>();
            this.OutsideDonator = new List<OutsideDonator>();
            this.TeamFoundDonator = new List<TeamFoundDonator>();
        }
    }
}