using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iVolunteer.Models.MongoDB.CollectionClass
{
    public class Mongo_Finance
    {
        public ObjectId _id { get; set; }
        public SDLink Project { get; set; }
        public double Total { get; set; }
        public List<FinanceItem> FinanceItem { get; set; }
        public Mongo_Finance()
        {
            this._id = ObjectId.GenerateNewId();
            this.Project = new SDLink();
            this.Total = 0;
            this.FinanceItem = new List<FinanceItem>();
        }
    }
}