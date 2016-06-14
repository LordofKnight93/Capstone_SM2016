using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.StructureClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ListClass;
using System.Collections.Generic;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass
{
    /// <summary>
    /// This class define structure of "Project" collection in MongoDB
    /// </summary>
    public class Mongo_Project
    {
        public ObjectId _id { get; set; }
        public ProjectInformation ProjectInformation { get; set; }
        public ProjectStructure ProjectStructure { get; set; }
        public RequestList RequestList { get; set; }
        public List<Team> Teams { get; set; }
        public Agenda Agenda { get; set; }

        public Mongo_Project()
        {
            this._id = new ObjectId();
            this.ProjectInformation = new ProjectInformation();
            this.ProjectStructure = new ProjectStructure();
            this.RequestList = new RequestList();
            this.Teams = new List<Team>();
        }

        public Mongo_Project(UserSD creator, ProjectInformation projectInfo)
        {
            this._id = ObjectId.GenerateNewId();
            this.ProjectInformation = new ProjectInformation();
            this.ProjectInformation.ProjectID = this._id.ToString();
            this.ProjectInformation = projectInfo;
            this.ProjectStructure = new ProjectStructure(creator);
            this.RequestList = new RequestList();
            this.Teams = new List<Team>();
            this.Agenda = new Agenda();
        }
    }
}