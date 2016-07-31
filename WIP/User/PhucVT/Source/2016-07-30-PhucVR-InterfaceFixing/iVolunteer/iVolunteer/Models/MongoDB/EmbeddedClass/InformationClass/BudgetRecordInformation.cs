using System;
using MongoDB.Bson;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Common;
using MongoDB.Bson.Serialization.Attributes;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store post's infomation
    /// </summary>
    [BsonIgnoreExtraElements]
    public class BudgetRecordInformation
    {
        public string BudgetRecordID { get; set; }
        public SDLink Project { get; set; }
        public string Name { get; set; }
        public double Total { get; set; }
        public BudgetRecordInformation()
        {
            this.BudgetRecordID = "";
            this.Project = new SDLink();
            this.Name = "";
            this.Total = 0;
        }
    }
}