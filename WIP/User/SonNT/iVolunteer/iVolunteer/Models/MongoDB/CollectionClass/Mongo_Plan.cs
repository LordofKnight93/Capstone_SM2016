using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;

namespace iVolunteer.Models.MongoDB.CollectionClass
{
    /// <summary>
    /// This class define structure of "Plan" collection in MongoDB
    /// </summary>
    public class Mongo_Plan
    {
        public ObjectId _id { get; set; }
        public SDLink Project { get; set; }
        public string PlanName { get; set; }
        public List<PlanItem> ItemList { get; set; }
        public Mongo_Plan()
        {
            this._id = new ObjectId();
            this.Project = new SDLink();
            this.PlanName = "";
            this.ItemList = new List<PlanItem>();
        }

        public void Add_Item(PlanItem item)
        {
            this.ItemList.Add(item);
        }

        public void Delete_Item(string itemID)
        {
            PlanItem item = this.ItemList.Find(i => i.ItemID == itemID);
            this.ItemList.Remove(item);
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