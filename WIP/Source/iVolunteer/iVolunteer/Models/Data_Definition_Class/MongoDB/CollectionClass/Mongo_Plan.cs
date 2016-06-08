﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass
{
    /// <summary>
    /// This class define structure of "Plan" collection in MongoDB
    /// </summary>
    public class Mongo_Plan
    {
        public ObjectId _id { get; set; }
        public ProjectLink Project { get; set; }
        public string PlanName { get; set; }
        public List<PlanItem> ItemList { get; set; }
        public Mongo_Plan()
        {
            this._id = new ObjectId();
            this.Project = new ProjectLink();
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
}