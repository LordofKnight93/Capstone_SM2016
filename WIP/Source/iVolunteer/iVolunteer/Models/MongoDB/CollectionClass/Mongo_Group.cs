using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.StructureClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ListClass;
using System.Collections.Generic;

namespace iVolunteer.Models.MongoDB.CollectionClass
{
    /// <summary>
    /// This class define structure of "Group" collection in MongoDB
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Mongo_Group
    {
        public ObjectId _id { get; set; }
        public GroupInformation GroupInformation { get; set; }
        public GroupStructure GroupStructure { get; set; }
        [BsonIgnoreIfDefault]
        public ActivityInformation HistoryInformation { get; set; }
        [BsonIgnoreIfDefault]
        public List<SDLink> CurrentProjects { get; set; }
        [BsonIgnoreIfDefault]
        public List<RequestItem> RequestList { get; set; }

        public Mongo_Group()
        {
            this._id = new ObjectId();
            this.GroupInformation = new GroupInformation();
            this.GroupStructure = new GroupStructure();
            this.HistoryInformation = new ActivityInformation();
            this.CurrentProjects = new List<SDLink>();
            this.RequestList = new List<RequestItem>();
        }

        public Mongo_Group(SDLink creator, GroupInformation groupInfo)
        {
            this._id = ObjectId.GenerateNewId();
            this.GroupInformation = groupInfo;
            this.GroupInformation.GroupID = this._id.ToString();
            this.GroupStructure = new GroupStructure(creator);
            this.HistoryInformation = new ActivityInformation();
            this.CurrentProjects = new List<SDLink>();
            this.RequestList = new List<RequestItem>();
        }
    }
}