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
using iVolunteer.Models.MongoDB.CollectionClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ListClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Common;

namespace iVolunteer.DAL.MongoDB
{
    public class Mongo_Project_DAO : Mongo_DAO
    {
        IMongoCollection<Mongo_Project> collection = db.GetCollection<Mongo_Project>("Project");
        /// <summary>
        /// Add new project to mongoDB
        /// </summary>
        /// <param name="project"></param>
        /// <returns>true if success</returns>
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
        /// get project infofmation of an project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public ProjectInformation Get_ProjectInformation(string projectID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(pr => pr.ProjectInformation.ProjectID == projectID);
                return result.ProjectInformation;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get SDlink of a project
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public SDLink Get_SDLink(string userID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(pr => pr.ProjectInformation.ProjectID == userID);
                return new SDLink(result.ProjectInformation);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get list project information from list ID
        /// </summary>
        /// <param name="listID"></param>
        /// <returns></returns>
        public List<ProjectInformation> Get_ProjectsInformation(List<string> listID)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.In(pr => pr.ProjectInformation.ProjectID, listID);
                var result = collection.Find(filter).ToList().Select(pr => pr.ProjectInformation).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// increase member count by 1
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Member_Join(string projectID)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.Inc(pr => pr.ProjectInformation.MemberCount, 1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// decrease member count by 1
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Member_Out(string projectID)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.Inc(pr => pr.ProjectInformation.MemberCount, -1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// udpate project information, only update description, email, phone,d ate start, date end
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="newInfo"></param>
        /// <returns></returns>
        public bool Update_proupInformation(string projectID, ProjectInformation newInfo)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.Set(pr => pr.ProjectInformation.ProjectShortDescription, newInfo.ProjectShortDescription)
                                                         .Set(pr => pr.ProjectInformation.Email, newInfo.Email)
                                                         .Set(pr => pr.ProjectInformation.Phone, newInfo.Phone)
                                                         .Set(pr => pr.ProjectInformation.DateStart, newInfo.DateStart)
                                                         .Set(pr => pr.ProjectInformation.DateEnd, newInfo.DateEnd);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// set a proejct is recruiting
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Start_Recruit(string projectID)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.Set(pr => pr.ProjectInformation.IsRecruit, Status.IS_RECRUITING);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// set a proejct is not recruiting
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Stop_Recruit(string projectID)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.Set(pr => pr.ProjectInformation.IsRecruit, Status.IS_NOT_RECRUITING);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// search active and confirmed project
        /// </summary>
        /// <param name="name"></param>
        /// <param name="option">true to include deactive project, false to find active project only</param>
        /// <returns></returns>
        public List<ProjectInformation> Project_Search(string name, bool allStatus)
        {
            try
            {
                var preResult = collection.AsQueryable().Where(pr => pr.ProjectInformation.ProjectName.ToLower().Contains(name.ToLower()));
                if (allStatus == false)
                {
                    preResult = preResult.Where(pr => pr.ProjectInformation.IsActivate == Status.IS_ACTIVATE);
                }
                var result = preResult.Select(pr => pr.ProjectInformation).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// load project for frontpage
        /// </summary>
        /// <returns></returns>
        public List<ProjectInformation> FrontPage_Project()
        {
            try
            {
                var result = collection.AsQueryable().Select(pr => pr.ProjectInformation)
                                                     .Where(pr => pr.InProgress == Status.ONGOING
                                                               && pr.IsActivate == Status.IS_ACTIVATE)
                                                     .OrderByDescending(pr => pr.FollowerCount)
                                                     .Take(8).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// increase follow count by 1
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool User_Follow(string projectID)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.Inc(pr => pr.ProjectInformation.FollowerCount, 1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// decrease follow count by 1
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool User_Unfollow(string projectID)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.Inc(pr => pr.ProjectInformation.FollowerCount, -1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        public List<SDLink> Get_Banned_Projects()
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(gr => gr.ProjectInformation.IsActivate, Status.IS_BANNED);
                var result = collection.Find(filter).ToList();
                List<SDLink> BannedProjects = new List<SDLink>();
                foreach (var item in result)
                {
                    BannedProjects.Add(Get_SDLink(item.ProjectInformation.ProjectID));
                }
                return BannedProjects;
            }
            catch
            {
                throw;
            }

        }
        public bool Set_Activation_Status(string projectID, bool status)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.Set(pr => pr.ProjectInformation.IsActivate, status);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add guest sponsor request
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="guest"></param>
        /// <returns></returns>
        public bool Add_GuestSponsor(string projectID, Sponsor guest)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.AddToSet(pr => pr.GuestSponsor, guest);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get guest sponsor request
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<Sponsor> Get_GuestSponsor_Requests(string projectID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(pr => pr.ProjectInformation.ProjectID == projectID)
                                                     .GuestSponsor.Where(g => g.Status == Status.PENDING).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get guest sponsors
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<Sponsor> Get_GuestSponsors(string projectID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(pr => pr.ProjectInformation.ProjectID == projectID)
                                                     .GuestSponsor.Where(g => g.Status == Status.ACCEPTED).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add guest sponsor
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="guest"></param>
        /// <returns></returns>
        public bool Delete_GuestSponsor(string projectID, string guestID)
        {
            try
            {
                var project_filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var guest_filter = Builders<Sponsor>.Filter.Eq(g => g.SponsorID, guestID);
                var update = Builders<Mongo_Project>.Update.PullFilter(pr => pr.GuestSponsor, guest_filter);
                var result = collection.UpdateOne(project_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add guest sponsor request
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="guest"></param>
        /// <returns></returns>
        public bool Accept_GuestSponsor(string projectID, string guestID)
        {
            try
            {
                var project_filter = Builders<Mongo_Project>.Filter.Where(pr => pr.ProjectInformation.ProjectID == projectID && pr.GuestSponsor.Any(gs => gs.SponsorID == guestID));
                var update = Builders<Mongo_Project>.Update.Set(pr => pr.GuestSponsor.ElementAt(-1).Status, Status.ACCEPTED);
                var result = collection.UpdateOne(project_filter, update);

                return false;
            }
            catch
            {
                throw;
            }
        }
    }
}