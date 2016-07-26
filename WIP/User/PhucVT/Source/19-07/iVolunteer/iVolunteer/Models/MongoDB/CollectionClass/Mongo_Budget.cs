using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;

namespace iVolunteer.Models.MongoDB.CollectionClass
{
    /// <summary>
    /// This class define structure of "Post" collection in MongoDB
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Mongo_Budget
    {
        public ObjectId _id { get; set; }
        public BudgetRecordInformation BudgetRecordInformation { get; set; }
        public List<BudgetItem> Item { get; set; }
        public List<Comment> Comment { get; set; }

        public Mongo_Budget()
        {
            this._id = new ObjectId();
            this.BudgetRecordInformation = new BudgetRecordInformation();
            this.Item = new List<BudgetItem>();
            this.Comment = new List<Comment>();
        }
        public Mongo_Budget(BudgetRecordInformation info)
        {
            this._id = ObjectId.GenerateNewId();
            this.BudgetRecordInformation = info;
            this.Item = new List<BudgetItem>();
            this.Comment = new List<Comment>();
        }
    }
}