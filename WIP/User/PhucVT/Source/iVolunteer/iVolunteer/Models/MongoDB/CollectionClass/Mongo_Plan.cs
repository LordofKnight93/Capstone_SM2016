using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;

namespace iVolunteer.Models.MongoDB.CollectionClass
{
    /// <summary>
    /// This class define structure of "Plan" collection in MongoDB
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Mongo_Plan
    {
        public ObjectId _id { get; set; }
        public SDLink Project { get; set; }
        public string PlanName { get; set; }
        [BsonIgnoreIfDefault]
        public List<PlanItem> ItemList { get; set; }
        public Mongo_Plan()
        {
            this._id = new ObjectId();
            this.Project = new SDLink();
            this.PlanName = "";
            this.ItemList = new List<PlanItem>();
        }
    }

    public class PlanItem
    {
        public string ItemID { get; set; }
        public DateTime WorkDate { get; set; }
        public string Content { get; set; }
        public List<SDLink> PIC { get; set; }
        public PlanItem()
        {
            this.ItemID = "";
            this.WorkDate = new DateTime();
            this.Content = "";
            this.PIC = new List<SDLink>();
        }
    }
}