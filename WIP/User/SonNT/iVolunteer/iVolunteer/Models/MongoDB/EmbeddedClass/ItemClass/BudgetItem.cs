using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    [BsonIgnoreExtraElements]
    public class BudgetItem
    {
        public string Content { get; set; }
        public double UnitPrice { get; set; }
        public int Quatity { get; set; }
        public string Unit { get; set; }
        public double Cost { get; set; }

        public BudgetItem()
        {
            this.Content = "";
            this.UnitPrice = 0.0;
            this.Quatity = 0;
            this.Unit = "";
        }
        public BudgetItem(BudgetItem item)
        {
            this.Content = item.Content;
            this.UnitPrice = item.UnitPrice;
            this.Quatity = item.Quatity;
            this.Unit = item.Unit;
            this.Cost = item.UnitPrice * item.Quatity;
        }
    }
}