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
using iVolunteer.Models.MongoDB.EmbeddedClass.StructureClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ListClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.ViewModel;
using iVolunteer.Common;

namespace iVolunteer.DAL.MongoDB
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
                var result = collection.DeleteOne(pr => pr.ProjectInformation.ProjectID == projectID);
                return result.IsAcknowledged;
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
        public List<ProjectInformation> Get_All_ProjectInformation(int skip, int number)
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
                var result = collection.AsQueryable().FirstOrDefault(p => p.ProjectInformation.ProjectID == projectID);
                return result.ProjectInformation;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// search activate project, user use
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public List<ProjectInformation> User_Search_ProjectInformation(SearchModel searchModel)
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
                var result = collection.AsQueryable().FirstOrDefault(p => p.ProjectInformation.ProjectID == projectID);
                return result.ProjectStructure;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get project join requests
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<RequestItem> Get_JoinRequests(string projectID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p.ProjectInformation.ProjectID == projectID);
                return result.RequestList.JoinRequests;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get project sponsor requests
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<RequestItem> Get_SponsorRequests(string projectID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p.ProjectInformation.ProjectID == projectID);
                return result.RequestList.SponsorRequests;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get project suggest user  requests
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<RequestItem> Get_SuggestUsers(string projectID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p.ProjectInformation.ProjectID == projectID);
                return result.RequestList.SuggestUsers;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get project guest sponsor requests
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<Sponsor> Get_GuestSponsorRequests(string projectID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p.ProjectInformation.ProjectID == projectID);
                return result.RequestList.GuestSponsorRequests;
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
                var result = collection.AsQueryable().FirstOrDefault(p => p.ProjectInformation.ProjectID == projectID);
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
                var result = collection.AsQueryable().FirstOrDefault(p => p.ProjectInformation.ProjectID == projectID);
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
        /// add new item to agenda
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Add_AgendaItem(string projectID, AgendaItem item)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.AddToSet<AgendaItem>(pr => pr.Agenda.ItemList, item);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete item of agenda
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Delete_AgendaItem(string projectID, string itemID)
        {
            try
            {
                var project_filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var item_filter = Builders<AgendaItem>.Filter.Eq(it => it.ItemID, itemID);
                var update = Builders<Mongo_Project>.Update.PullFilter<AgendaItem>(pr => pr.Agenda.ItemList, item_filter);
                var result = collection.UpdateOne(project_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// set leader to to project
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="leader"></param>
        /// <returns></returns>
        public bool Add_Leader(string projectID, Member leader)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.AddToSet<Member>(pr => pr.ProjectStructure.Leaders, leader);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// dismiss a leader of a project
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Delete_Leader(string projectID, string userID)
        {
            try
            {
                var project_filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var user_filter = Builders<Member>.Filter.Eq(m => m.SDInfo.ID, userID);
                var update = Builders<Mongo_Project>.Update.PullFilter(pr => pr.ProjectStructure.Leaders, user_filter);
                var result = collection.UpdateOne(project_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Add a user to project
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Add_JoinedUser(string projectID, SDLink user)
        {
            try
            {
                Member member = new Member(user);
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.AddToSet<Member>(pr => pr.ProjectStructure.JoinedUsers, member);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// expell a user from a project's joineduser list
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Delete_JoinedUser(string projectID, string userID)
        {
            try
            {
                var project_filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var user_filter = Builders<Member>.Filter.Eq(m => m.SDInfo.ID, userID);
                var update = Builders<Mongo_Project>.Update.PullFilter(pr => pr.ProjectStructure.JoinedUsers, user_filter);
                var result = collection.UpdateOne(project_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add individual organize user who already in group
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Add_OrganizeUser(string projectID, Member user)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.AddToSet<Member>(pr => pr.ProjectStructure.OrganizeUsers, user);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add individual organize user who not in group
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Add_OrganizeUser(string projectID, SDLink user)
        {
            try
            {
                Member member = new Member(user);
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.AddToSet<Member>(pr => pr.ProjectStructure.OrganizeUsers, member);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete organize user
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Delete_OrganizeUser(string projectID, string userID)
        {
            try
            {
                var project_filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var user_filter = Builders<Member>.Filter.Eq(u => u.SDInfo.ID, userID);
                var update = Builders<Mongo_Project>.Update.PullFilter<Member>(pr => pr.ProjectStructure.OrganizeUsers , user_filter);
                var result = collection.UpdateOne(project_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add sponsor user who is system user
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Add_SponsoredUser(string projectID, Member user)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.AddToSet<Member>(pr => pr.ProjectStructure.SponsoredUsers, user);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete sponsored user who is system user
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Delete_SponsoredUser(string projectID, string userID)
        {
            try
            {
                var project_filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var user_filter = Builders<Member>.Filter.Eq(u => u.SDInfo.ID, userID);
                var update = Builders<Mongo_Project>.Update.PullFilter<Member>(pr => pr.ProjectStructure.SponsoredUsers, user_filter);
                var result = collection.UpdateOne(project_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add none user sponsore
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="guest"></param>
        /// <returns></returns>
        public bool Add_SponsoredGuest(string projectID, Sponsor guest)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.AddToSet<Sponsor>(pr => pr.ProjectStructure.SponsoredGuests, guest);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete none user sponsor
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="sponsorID"></param>
        /// <returns></returns>
        public bool Delete_SponsoredGuest(string projectID, string sponsorID)
        {
            try
            {
                var project_filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var guest_filter = Builders<Sponsor>.Filter.Eq(sp => sp.SponsorID, sponsorID);
                var update = Builders<Mongo_Project>.Update.PullFilter<Sponsor>(pr => pr.ProjectStructure.SponsoredGuests, guest_filter);
                var result = collection.UpdateOne(project_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add organize group, which not in project
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool Add_OrganizeGroup(string projectID, SDLink group)
        {
            try
            {
                Member member = new Member(group);
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.AddToSet<Member>(pr => pr.ProjectStructure.OrganizeGroups, member);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add organize group, which already in project
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool Add_OrganizeGroup(string projectID, Member group)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.AddToSet<Member>(pr => pr.ProjectStructure.OrganizeGroups, group);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete organize group
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Delete_OrganizeGroup(string projectID, string groupID)
        {
            try
            {
                var project_filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var group_filter = Builders<Member>.Filter.Eq(gr => gr.SDInfo.ID, groupID);
                var update = Builders<Mongo_Project>.Update.PullFilter<Member>(pr => pr.ProjectStructure.OrganizeGroups, group_filter);
                var result = collection.UpdateOne(project_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add joined group
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool Add_JoinedGroup(string projectID, SDLink group)
        {
            try
            {
                Member member = new Member(group);
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.AddToSet<Member>(pr => pr.ProjectStructure.JoinedGroups, member);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete joined group
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Delete_JoinedGroup(string projectID, string groupID)
        {
            try
            {
                var project_filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var group_filter = Builders<Member>.Filter.Eq(gr => gr.SDInfo.ID, groupID);
                var update = Builders<Mongo_Project>.Update.PullFilter<Member>(pr => pr.ProjectStructure.JoinedGroups, group_filter);
                var result = collection.UpdateOne(project_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add sponsored group, not in project
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool Add_SponsoredGroup(string projectID, SDLink group)
        {
            try
            {
                Member member = new Member(group);
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.AddToSet<Member>(pr => pr.ProjectStructure.SponsoredGroups, member);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add sponsored group, already in project
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool Add_SponsoredGroup(string projectID, Member group)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.AddToSet<Member>(pr => pr.ProjectStructure.SponsoredGroups, group);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete sponsored group
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Delete_SponsoredGroup(string projectID, string groupID)
        {
            try
            {
                var project_filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var group_filter = Builders<Member>.Filter.Eq(gr => gr.SDInfo.ID, groupID);
                var update = Builders<Mongo_Project>.Update.PullFilter<Member>(pr => pr.ProjectStructure.SponsoredGroups, group_filter);
                var result = collection.UpdateOne(project_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Update project information
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="projectInfo"></param>
        /// <returns></returns>
        public bool Update_ProjectInfomarion(string projectID, ProjectInformation projectInfo)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.Set(pr => pr.ProjectInformation, projectInfo);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get projectSD
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public SDLink Get_SDLink(string projectID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p.ProjectInformation.ProjectID == projectID);
                return new SDLink(result.ProjectInformation);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// check if a join request is exist
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool Is_JoinRequest_Exist(string projectID, RequestItem request)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(gr => gr.ProjectInformation.ProjectID == projectID)
                                                     .RequestList.JoinRequests.FirstOrDefault(rq => rq.Type == request.Type
                                                                                    && rq.Actor.ID == request.Actor.ID
                                                                                    && rq.Actor.Handler == request.Actor.Handler);
                return result != null;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add join request to project request list
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool Add_JoinRequest(string projectID, RequestItem request)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.AddToSet<RequestItem>(pr => pr.RequestList.JoinRequests, request);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete join request
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool Delete_JoinRequest(string projectID, string requestID)
        {
            try
            {
                var project_filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var request_filter = Builders<RequestItem>.Filter.Eq(rq => rq.RequestID, requestID);
                var update = Builders<Mongo_Project>.Update.PullFilter(pr => pr.RequestList.JoinRequests, request_filter);
                var result = collection.UpdateOne(project_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get a join request details
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public RequestItem Get_JoinRequest(string projectID, string requestID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p.ProjectInformation.ProjectID == projectID)
                                                     .RequestList.JoinRequests.FirstOrDefault(rq => rq.RequestID == requestID);
                return result;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// check if a sponsor request is exist
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool Is_SponsorRequest_Exist(string projectID, RequestItem request)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(gr => gr.ProjectInformation.ProjectID == projectID)
                                                     .RequestList.SponsorRequests.FirstOrDefault(rq => rq.Type == request.Type
                                                                                    && rq.Actor.ID == request.Actor.ID
                                                                                    && rq.Actor.Handler == request.Actor.Handler);
                return result != null;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add sponsor request to project request list
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool Add_SponsorRequest(string projectID, RequestItem request)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.AddToSet<RequestItem>(pr => pr.RequestList.SponsorRequests, request);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete sponsor request
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool Delete_SponsorRequest(string projectID, string requestID)
        {
            try
            {
                var project_filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var request_filter = Builders<RequestItem>.Filter.Eq(rq => rq.RequestID, requestID);
                var update = Builders<Mongo_Project>.Update.PullFilter(pr => pr.RequestList.SponsorRequests, request_filter);
                var result = collection.UpdateOne(project_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get a sponsor request details
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public RequestItem Get_SponsorRequest(string projectID, string requestID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p.ProjectInformation.ProjectID == projectID)
                                                     .RequestList.SponsorRequests.FirstOrDefault(rq => rq.RequestID == requestID);
                return result;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// check if a suggest request is exist
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool Is_SuggestUser_Exist(string projectID, RequestItem request)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(gr => gr.ProjectInformation.ProjectID == projectID)
                                                     .RequestList.SuggestUsers.FirstOrDefault(rq => rq.Type == request.Type
                                                                                    && rq.Actor.ID == request.Actor.ID
                                                                                    && rq.Actor.Handler == request.Actor.Handler
                                                                                    && rq.Destination.ID == request.Destination.ID);
                return result != null;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add sponsor suggest user
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool Add_SuggestUser(string projectID, RequestItem request)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.AddToSet(pr => pr.RequestList.SuggestUsers, request);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete suggest user
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public bool Delete_SuggestUser(string projectID, string requestID)
        {
            try
            {
                var project_filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var request_filter = Builders<RequestItem>.Filter.Eq(rq => rq.RequestID, requestID);
                var update = Builders<Mongo_Project>.Update.PullFilter(pr => pr.RequestList.SuggestUsers, request_filter);
                var result = collection.UpdateOne(project_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get a suggest user request
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public RequestItem Get_SuggestUser(string projectID, string requestID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p.ProjectInformation.ProjectID == projectID)
                                                     .RequestList.SuggestUsers.FirstOrDefault(rq => rq.RequestID == requestID);
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// check if a guest sponsor request is exist
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="sponsor"></param>
        /// <returns></returns>
        public bool Is_GuestSponsorRequest_Exist(string projectID, Sponsor sponsor)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(gr => gr.ProjectInformation.ProjectID == projectID)
                                                     .RequestList.GuestSponsorRequests.FirstOrDefault(sp => sp.SponsorName == sponsor.SponsorName
                                                                                                         && sp.SponsorMail == sponsor.SponsorMail
                                                                                                         && sp.SponsorPhone == sponsor.SponsorName);
                return result != null;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add guest sponsor request to project request list
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool Add_GuestSponsorRequest(string projectID, Sponsor sponsor)
        {
            try
            {
                var filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var update = Builders<Mongo_Project>.Update.AddToSet(pr => pr.RequestList.GuestSponsorRequests, sponsor);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete sponsor request
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="sponsorID"></param>
        /// <returns></returns>
        public bool Delete_GuestSponsorRequest(string projectID, string sponsorID)
        {
            try
            {
                var project_filter = Builders<Mongo_Project>.Filter.Eq(pr => pr.ProjectInformation.ProjectID, projectID);
                var sponsor_filter = Builders<Sponsor>.Filter.Eq(rq => rq.SponsorID, sponsorID);
                var update = Builders<Mongo_Project>.Update.PullFilter(pr => pr.RequestList.GuestSponsorRequests, sponsor_filter);
                var result = collection.UpdateOne(project_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get a sponsor request details
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="sponsorID"></param>
        /// <returns></returns>
        public Sponsor Get_GuestSponsorRequest(string projectID, string sponsorID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p.ProjectInformation.ProjectID == projectID)
                                                     .RequestList.GuestSponsorRequests.FirstOrDefault(rq => rq.SponsorID == sponsorID);
                return result;
            }
            catch
            {
                throw;
            }
        }
    }
}