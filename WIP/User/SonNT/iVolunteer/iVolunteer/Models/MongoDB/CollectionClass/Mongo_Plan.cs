using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;

namespace iVolunteer.Models.MongoDB.CollectionClass
{
    /// <summary>
    /// This class define structure of "Plan" collection in MongoDB
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Mongo_Plan
    {
        public ObjectId _id { get; set; }
        public PlanPhaseInformation PlanPhaseInformation { get; set; }
        public List<MainTask> MainTask { get; set; }

        public Mongo_Plan()
        {
            this._id = new ObjectId();
            this.PlanPhaseInformation = new PlanPhaseInformation();
            this.MainTask = new List<MainTask>();
        }
        public Mongo_Plan(PlanPhaseInformation info)
        {
            this._id = ObjectId.GenerateNewId();
            this.PlanPhaseInformation = info;
            this.MainTask = new List<MainTask>();
        }
    }
}