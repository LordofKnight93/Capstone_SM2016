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
        public int Type { get; set; }
        
        public Mongo_Report()
        {
            this._Id = new ObjectId();
            this.Actor = new SDLink();
            this.Destination = new SDLink();
            this.Type = 0;
        }
        public Mongo_Report(SDLink actor, SDLink destination, int type)
        {
            this._Id = ObjectId.GenerateNewId();
            this.Actor = actor;
            this.Destination = destination;
            this.Type = type;
        }
     }
}