﻿using MongoDB.Bson;
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
        public List<AlbumSD> AlbumList { get; set; }

        public Mongo_Group()
        {
            this._id = new ObjectId();
            this.GroupInformation = new GroupInformation();
            this.GroupStructure = new GroupStructure();
            this.HistoryInformation = new HistoryInformation();
            this.CurrentProjects = new List<ProjectSD>();
            this.AlbumList = new List<AlbumSD>();
        }
    }
}