using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.IO;
using MongoDB.Driver.Builders;
using iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.StructureClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ListClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.Data_Definition_Class.ViewModel;
using iVolunteer.Common;

namespace iVolunteer.Models.Data_Access_Object.MongoDB
{
    public class Mongo_Project_DAO : Mongo_DAO
    {
        IMongoCollection<Mongo_Project> collection = db.GetCollection<Mongo_Project>("Project");
        /// <summary>
        /// add Project to mongoDB
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public bool Add_Project(Mongo_Project project)
        {
            try
            {
                collection.InsertOne(project);
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delelte a project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_Project(string projectID)
        {
            try
            {
                collection.DeleteOne(pr => pr._id == new ObjectId(projectID));
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get a number of Project infomation,
        /// </summary>
        /// <param name="number">number of information get</param>
        /// <param name="skip">number of Project skip</param>
        /// <returns></returns>
        public List<ProjectInformation> Get_All_ProjectInformation(int number, int skip)
        {
            try
            {
                var result = collection.AsQueryable().Select(g => g.ProjectInformation).Skip(skip).Take(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get a project information
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ProjectInformation Get_ProjectInformation(string projectID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p._id == new ObjectId(projectID));
                return result.ProjectInformation;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// search project, activate project only
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public List<ProjectInformation> Search_ProjectInformation(SearchModel searchModel)
        {
            var result = new List<ProjectInformation>();
            return result;
        }
        /// <summary>
        /// get a project structure
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ProjectStructure Get_ProjectStructure(string projectID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p._id == new ObjectId(projectID));
                return result.ProjectStructure;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get project requests
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public RequestList Get_RequestList(string projectID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p._id == new ObjectId(projectID));
                return result.RequestList;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get project agenda
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public Agenda Get_Agenda(string projectID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p._id == new ObjectId(projectID));
                return result.Agenda;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get project team list
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<Team> Get_Teams(string projectID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p._id == new ObjectId(projectID));
                return result.Teams;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// set activatiom status for a project
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool Set_Activation_Status(string projectID, bool status)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr._id, new ObjectId(projectID));
                var update = Builders<Mongo_Project>.Update.Set(pr => pr.ProjectInformation.IsActivate, status);
                collection.UpdateOne(filter, update);
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}