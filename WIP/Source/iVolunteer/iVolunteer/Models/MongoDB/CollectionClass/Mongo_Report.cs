using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ListClass;
using System.Collections.Generic;

namespace iVolunteer.Models.MongoDB.CollectionClass
{
    [BsonIgnoreExtraElements]
    public class Mongo_Report
    {
        public ObjectId _Id { get; set; }
        public SDLink Actor { get; set; }
        public SDLink Destination { get; set; }
        public string Reason { get; set; }
        public string Detail { get; set; }
        public Mongo_Report()
        {
            this._Id = new ObjectId();
            this.Actor = new SDLink();
            this.Destination = new SDLink();
            this.Reason = "";
            this.Detail = "";
        }
        public Mongo_Report(SDLink actor, SDLink destination, string reason, string detail)
        {
            this._Id = ObjectId.GenerateNewId();
            this.Actor = actor;
            this.Destination = destination;
            this.Reason = reason;
            this.Detail = detail;
        }
        public Mongo_Report(SDLink actor, SDLink destination, string reason)
        {
            this._Id = ObjectId.GenerateNewId();
            this.Actor = actor;
            this.Destination = destination;
            this.Reason = reason;
        }
    }
}