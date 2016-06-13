using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.StructureClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using System.Collections.Generic;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass
{
    /// <summary>
    /// This class define structure of "Group" collection in MongoDB
    /// </summary>
    public class Mongo_Group
    {
        public ObjectId _id { get; set; }
        public GroupInformation GroupInformation { get; set; }
        public GroupStructure GroupStructure { get; set; }
        public HistoryInformation HistoryInformation { get; set; }
        public List<ProjectSD> CurrentProjects { get; set; }

        public Mongo_Group()
        {
            this._id = new ObjectId();
            this.GroupInformation = new GroupInformation();
            this.GroupStructure = new GroupStructure();
            this.HistoryInformation = new HistoryInformation();
            this.CurrentProjects = new List<ProjectSD>();
        }

        public Mongo_Group(UserSD creator, GroupInformation groupInfo)
        {
            this._id = ObjectId.GenerateNewId();
            this.GroupInformation = groupInfo;
            this.GroupStructure = new GroupStructure(creator);
            this.HistoryInformation = new HistoryInformation();
            this.CurrentProjects = new List<ProjectSD>();
        }
    }
}