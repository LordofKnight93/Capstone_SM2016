using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using iVolunteer.Models.SQL;
using iVolunteer.Common;

namespace iVolunteer.DAL.SQL
{
    public class SQL_AcPr_Relation_DAO
    {   
        /// <summary>
        /// add organize relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Add_Organizer(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcPr_Relation relation = new SQL_AcPr_Relation();
                    relation.UserID = userID;
                    relation.ProjectID = projectID;
                    relation.Relation = AcPrRelation.ORGANIZE_RELATION;
                    relation.Status = Status.ACCEPTED;

                    dbEntities.SQL_AcPr_Relation.Add(relation);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add leaer relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Add_Leader(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcPr_Relation relation = new SQL_AcPr_Relation();
                    relation.UserID = userID;
                    relation.ProjectID = projectID;
                    relation.Relation = AcPrRelation.LEADER_RELATION;
                    relation.Status = Status.ACCEPTED;

                    dbEntities.SQL_AcPr_Relation.Add(relation);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add memebr relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Add_Member(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcPr_Relation relation = new SQL_AcPr_Relation();
                    relation.UserID = userID;
                    relation.ProjectID = projectID;
                    relation.Relation = AcPrRelation.MEMBER_RELATION;
                    relation.Status = Status.ACCEPTED;

                    dbEntities.SQL_AcPr_Relation.Add(relation);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add sponsor relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Add_Sponsor(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcPr_Relation relation = new SQL_AcPr_Relation();
                    relation.UserID = userID;
                    relation.ProjectID = projectID;
                    relation.Relation = AcPrRelation.SPONSOR_RELATION;
                    relation.Status = Status.ACCEPTED;

                    dbEntities.SQL_AcPr_Relation.Add(relation);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add join request relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Add_Join_Request(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcPr_Relation relation = new SQL_AcPr_Relation();
                    relation.UserID = userID;
                    relation.ProjectID = projectID;
                    relation.Relation = AcPrRelation.MEMBER_RELATION;
                    relation.Status = Status.PENDING;

                    dbEntities.SQL_AcPr_Relation.Add(relation);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add sponsor request relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Add_Sponsor_Request(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcPr_Relation relation = new SQL_AcPr_Relation();
                    relation.UserID = userID;
                    relation.ProjectID = projectID;
                    relation.Relation = AcPrRelation.SPONSOR_RELATION;
                    relation.Status = Status.PENDING;

                    dbEntities.SQL_AcPr_Relation.Add(relation);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add follow relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Add_Follower(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcPr_Relation relation = new SQL_AcPr_Relation();
                    relation.UserID = userID;
                    relation.ProjectID = projectID;
                    relation.Relation = AcPrRelation.FOLLOW_RELATION;
                    relation.Status = Status.ACCEPTED;

                    dbEntities.SQL_AcPr_Relation.Add(relation);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get all participant
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_AddPaticipants(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.UserID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add report relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Add_Report(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcPr_Relation relation = new SQL_AcPr_Relation();
                    relation.UserID = userID;
                    relation.ProjectID = projectID;
                    relation.Relation = AcPrRelation.REPORT_RELATION;
                    relation.Status = Status.ACCEPTED;

                    dbEntities.SQL_AcPr_Relation.Add(relation);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete member from project
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_Member(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.MEMBER_RELATION
                                                                   && rl.Status == Status.ACCEPTED);
                    if (result != null)
                    {
                        dbEntities.SQL_AcPr_Relation.Remove(result);
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// delete follower
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_Follower(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.FOLLOW_RELATION
                                                                   && rl.Status == Status.ACCEPTED);
                    if (result != null)
                    {
                        dbEntities.SQL_AcPr_Relation.Remove(result);
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete organizer
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_Organizer(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.ORGANIZE_RELATION
                                                                   && rl.Status == Status.ACCEPTED);
                    if (result != null)
                    {
                        dbEntities.SQL_AcPr_Relation.Remove(result);
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// delete leader
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_Leader(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.LEADER_RELATION
                                                                   && rl.Status == Status.ACCEPTED);
                    if (result != null)
                    {
                        dbEntities.SQL_AcPr_Relation.Remove(result);
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// delete sponsor
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_Sponsor(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.SPONSOR_RELATION
                                                                   && rl.Status == Status.ACCEPTED);
                    if (result != null)
                    {
                        dbEntities.SQL_AcPr_Relation.Remove(result);
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// delete report
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_Report(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.REPORT_RELATION
                                                                   && rl.Status == Status.ACCEPTED);
                    if (result != null)
                    {
                        dbEntities.SQL_AcPr_Relation.Remove(result);
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// set a leader to member
        /// </summary>
        /// <param name="leaderID"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public bool Set_Member(string leaderID, string projectID)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    Delete_Leader(leaderID, projectID);
                    Add_Member(leaderID, projectID);

                    transaction.Complete();
                    return true;
                }
                catch
                {
                    transaction.Dispose();
                    throw;
                }
            }
        }

        /// <summary>
        /// set a member to leader
        /// </summary>
        /// <param name="leaderID"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public bool Set_Leader(string memberID, string projectID)
        {
            using(var transaction = new TransactionScope())
            {
                try
                {
                    Delete_Member(memberID, projectID);
                    Add_Leader(memberID, projectID);

                    transaction.Complete();
                    return true;
                }
                catch
                {
                    transaction.Dispose();
                    throw;
                }
            }
        }
        /// <summary>
        /// check if a user follow a project
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        public bool Is_Follower(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.FOLLOW_RELATION
                                                                   && rl.Status == Status.ACCEPTED);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// check if a user is leader a project
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        public bool Is_Leader(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.LEADER_RELATION
                                                                   && rl.Status == Status.ACCEPTED);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// check if a user is sponsor a project
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        public bool Is_Sponsor(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.SPONSOR_RELATION
                                                                   && rl.Status == Status.ACCEPTED);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// check if a user is organizer a project
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        public bool Is_Organizer(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.ORGANIZE_RELATION
                                                                   && rl.Status == Status.ACCEPTED);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// check if a user is member a project
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        public bool Is_Member(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.MEMBER_RELATION
                                                                   && rl.Status == Status.ACCEPTED);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// check if a user joined a project
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Is_Joined(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && (rl.Relation == AcPrRelation.LEADER_RELATION || rl.Relation == AcPrRelation.MEMBER_RELATION)
                                                                   && rl.Status == Status.ACCEPTED);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// check if a user reported a project
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Is_Reported(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.REPORT_RELATION
                                                                   && rl.Status == Status.ACCEPTED);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// check if a user requested to join a project
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Is_Join_Requested(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.MEMBER_RELATION
                                                                   && rl.Status == Status.PENDING);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// check if a user requested to sponsor a project
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Is_Sponsor_Requested(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.SPONSOR_RELATION 
                                                                   && rl.Status == Status.PENDING);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// accept user to join project
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Accept_Join_Request(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.MEMBER_RELATION
                                                                   && rl.Status == Status.PENDING);
                    if (result != null)
                    {
                        result.Status = Status.ACCEPTED;
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// accept user to sponsor project
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Accept_Sponsor_Request(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.SPONSOR_RELATION
                                                                   && rl.Status == Status.PENDING);
                    if (result != null)
                    {
                        result.Status = Status.ACCEPTED;
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// decline a request
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_Join_Request(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.MEMBER_RELATION
                                                                   && rl.Status == Status.PENDING);
                    if (result != null)
                    {
                        dbEntities.SQL_AcPr_Relation.Remove(result);
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// decline a request
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_Sponsor_Request(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.SPONSOR_RELATION
                                                                   && rl.Status == Status.PENDING);
                    if (result != null)
                    {
                        dbEntities.SQL_AcPr_Relation.Remove(result);
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get project active members
        /// </summary>
        /// <param name="projectID"></param>
        public List<string> Get_Members(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.MEMBER_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.UserID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get project active sponsor
        /// </summary>
        /// <param name="projectID"></param>
        public List<string> Get_Sponsors(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.SPONSOR_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.UserID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get project active organizer
        /// </summary>
        /// <param name="projectID"></param>
        public List<string> Get_Organizers(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.ORGANIZE_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.UserID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get project active join request
        /// </summary>
        /// <param name="projectID"></param>
        public List<string> Get_Join_Requests(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.MEMBER_RELATION
                                                                   && rl.Status == Status.PENDING
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.UserID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get project active sponsor request
        /// </summary>
        /// <param name="projectID"></param>
        public List<string> Get_Sponsor_Requests(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.SPONSOR_RELATION
                                                                   && rl.Status == Status.PENDING
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.UserID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get project  leaders
        /// </summary>
        /// <param name="projectID"></param>
        public List<string> Get_Leaders(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.LEADER_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.UserID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get user joined active sponsored, organized, joined project
        /// </summary>
        /// <param name="userID"></param>
        public List<string> Get_Joined_Projects(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.UserID == userID
                                                                   && (rl.Relation == AcPrRelation.ORGANIZE_RELATION
                                                                        || rl.Relation == AcPrRelation.SPONSOR_RELATION
                                                                        || rl.Relation == AcPrRelation.LEADER_RELATION
                                                                        || rl.Relation == AcPrRelation.MEMBER_RELATION)
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Project.IsActivate == Status.IS_ACTIVATE
                                                                   && rl.SQL_Project.InProgress == Status.ENDED)
                                                             .Select(rl => rl.ProjectID).Distinct().ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get user participate active project
        /// </summary>
        /// <param name="userID"></param>
        public List<string> Get_Participated_Projects(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.UserID == userID
                                                                   && ( rl.Relation == AcPrRelation.LEADER_RELATION || rl.Relation == AcPrRelation.MEMBER_RELATION )
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Project.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.ProjectID).Distinct().ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get user sponsored active project
        /// </summary>
        /// <param name="userID"></param>
        public List<string> Get_Sponsored_Projects(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.UserID == userID
                                                                   && rl.Relation == AcPrRelation.SPONSOR_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Project.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.ProjectID).Distinct().ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get user organized active project
        /// </summary>
        /// <param name="userID"></param>
        public List<string> Get_Organized_Projects(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.UserID == userID
                                                                   && rl.Relation == AcPrRelation.ORGANIZE_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Project.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.ProjectID).Distinct().ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get user current active project
        /// </summary>
        /// <param name="userID"></param>
        public List<string> Get_Current_Projects(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.UserID == userID
                                                                   && (rl.Relation == AcPrRelation.ORGANIZE_RELATION
                                                                        || rl.Relation == AcPrRelation.SPONSOR_RELATION
                                                                        || rl.Relation == AcPrRelation.LEADER_RELATION
                                                                        || rl.Relation == AcPrRelation.MEMBER_RELATION)
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Project.IsActivate == Status.IS_ACTIVATE
                                                                   && rl.SQL_Project.InProgress == Status.ONGOING)
                                                             .Select(rl => rl.ProjectID).Distinct().ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }
        public bool Delete_Reports(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.RemoveRange(dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                                                            && rl.Relation == AcPrRelation.REPORT_RELATION));
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        public bool Is_More_Than_One_Leader(string projectID)
        {

            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                   && rl.Relation == AcGrRelation.LEADER_RELATION
                                                                   && rl.Status == Status.ACCEPTED).Count();
                    return result > 1;
                }
            }
            catch
            {
                throw;
            }
        }

        public int Get_UserRole(string projectID, string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID);
                    return result.Relation;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get all joined group members
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_JoinedGroup_Members(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                       && (rl.Relation == AcPrRelation.LEADER_RELATION || rl.Relation == AcPrRelation.MEMBER_RELATION)
                                                                       && rl.Status == Status.ACCEPTED
                                                                       && rl.SQL_Account.SQL_AcGr_Relation.FirstOrDefault(rl2 => rl2.GroupID == groupID
                                                                                                                     && (rl2.Relation == AcGrRelation.LEADER_RELATION || rl2.Relation == AcGrRelation.MEMBER_RELATION)
                                                                                                                     && rl2.Status == Status.ACCEPTED) != null)
                                                            .Select(rl => rl.UserID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get all sponsored group members
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_SponsoredGroup_Members(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                       && rl.Relation == AcPrRelation.SPONSOR_RELATION
                                                                       && rl.Status == Status.ACCEPTED
                                                                       && rl.SQL_Account.SQL_AcGr_Relation.FirstOrDefault(rl2 => rl2.GroupID == groupID
                                                                                                                     && (rl2.Relation == AcGrRelation.LEADER_RELATION || rl2.Relation == AcGrRelation.MEMBER_RELATION)
                                                                                                                     && rl2.Status == Status.ACCEPTED) != null)
                                                            .Select(rl => rl.UserID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get all organized group members
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_OrganizedGroup_Members(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                       && rl.Relation == AcPrRelation.ORGANIZE_RELATION
                                                                       && rl.Status == Status.ACCEPTED
                                                                       && rl.SQL_Account.SQL_AcGr_Relation.FirstOrDefault(rl2 => rl2.GroupID == groupID
                                                                                                                     && (rl2.Relation == AcGrRelation.LEADER_RELATION || rl2.Relation == AcGrRelation.MEMBER_RELATION)
                                                                                                                     && rl2.Status == Status.ACCEPTED) != null)
                                                            .Select(rl => rl.UserID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get all members of group that request to join project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_Join_Requested_Group_Members(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                       && rl.Relation == AcPrRelation.MEMBER_RELATION
                                                                       && rl.Status == Status.PENDING
                                                                       && rl.SQL_Account.SQL_AcGr_Relation.FirstOrDefault(rl2 => rl2.GroupID == groupID
                                                                                                                     && (rl2.Relation == AcGrRelation.LEADER_RELATION || rl2.Relation == AcGrRelation.MEMBER_RELATION)
                                                                                                                     && rl2.Status == Status.ACCEPTED) != null)
                                                            .Select(rl => rl.UserID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// get all members of group that request to sponser project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_Sponsor_Requested_Group_Members(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                       && rl.Relation == AcPrRelation.SPONSOR_RELATION
                                                                       && rl.Status == Status.PENDING
                                                                       && rl.SQL_Account.SQL_AcGr_Relation.FirstOrDefault(rl2 => rl2.GroupID == groupID
                                                                                                                     && (rl2.Relation == AcGrRelation.LEADER_RELATION || rl2.Relation == AcGrRelation.MEMBER_RELATION)
                                                                                                                     && rl2.Status == Status.ACCEPTED) != null)
                                                            .Select(rl => rl.UserID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add invite relations
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Invite_Users(string[] userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    foreach (var item in userID)
                    {
                        SQL_AcPr_Relation relation = new SQL_AcPr_Relation();
                        relation.UserID = item;
                        relation.ProjectID = projectID;
                        relation.Relation = AcPrRelation.INVITED_RELATION;
                        relation.Status = Status.PENDING;

                        dbEntities.SQL_AcPr_Relation.Add(relation);
                    }
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add invite relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Invite_User(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcPr_Relation relation = new SQL_AcPr_Relation();
                    relation.UserID = userID;
                    relation.ProjectID = projectID;
                    relation.Relation = AcPrRelation.INVITED_RELATION;
                    relation.Status = Status.PENDING;

                    dbEntities.SQL_AcPr_Relation.Add(relation);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add suggest relations
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Suggest_Users(string[] userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    foreach (var item in userID)
                    {
                        SQL_AcPr_Relation relation = new SQL_AcPr_Relation();
                        relation.UserID = item;
                        relation.ProjectID = projectID;
                        relation.Relation = AcPrRelation.SUGGESTED_RELATION;
                        relation.Status = Status.PENDING;

                        dbEntities.SQL_AcPr_Relation.Add(relation);
                    }
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete an invite relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_Invite_User(string userID, string projectID)
        {

            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.INVITED_RELATION);
                    if (result != null)
                    {
                        dbEntities.SQL_AcPr_Relation.Remove(result);
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete a suggest relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_Suggest_User(string userID, string projectID)
        {

            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.SUGGESTED_RELATION);
                    if (result != null)
                    {
                        dbEntities.SQL_AcPr_Relation.Remove(result);
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get active user that is suggested to project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_Suggest_Users(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.SUGGESTED_RELATION
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.UserID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get a invited projects
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_Invited_Projects(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.UserID == userID
                                                                   && rl.Relation == AcPrRelation.INVITED_RELATION
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.ProjectID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get number of project user had organized, join, sponsor,
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public int Get_ProjectCount(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.UserID == userID
                                                                   && (rl.Relation == AcPrRelation.ORGANIZE_RELATION 
                                                                        || rl.Relation == AcPrRelation.SPONSOR_RELATION
                                                                        || rl.Relation == AcPrRelation.LEADER_RELATION
                                                                        || rl.Relation == AcPrRelation.MEMBER_RELATION)
                                                                   && rl.Status == Status.ACCEPTED)
                                                             .Select(rl => rl.ProjectID).Distinct().ToList().Count;
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add join request relations
        /// </summary>
        /// <param name="listID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Add_Join_Requests(string[] listID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    foreach (var item in listID)
                    {
                        SQL_AcPr_Relation relation = new SQL_AcPr_Relation();
                        relation.UserID = item;
                        relation.ProjectID = projectID;
                        relation.Relation = AcPrRelation.MEMBER_RELATION;
                        relation.Status = Status.PENDING;

                        dbEntities.SQL_AcPr_Relation.Add(relation);
                    } 
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// add sponsor request relations
        /// </summary>
        /// <param name="listID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Add_Sponsor_Requests(string[] listID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    foreach (var item in listID)
                    {
                        SQL_AcPr_Relation relation = new SQL_AcPr_Relation();
                        relation.UserID = item;
                        relation.ProjectID = projectID;
                        relation.Relation = AcPrRelation.SPONSOR_RELATION;
                        relation.Status = Status.PENDING;

                        dbEntities.SQL_AcPr_Relation.Add(relation);
                    }
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// accept all members of group that request to join project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Accept_Join_Requested_Group_Members(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                       && rl.Relation == AcPrRelation.MEMBER_RELATION
                                                                       && rl.Status == Status.PENDING
                                                                       && rl.SQL_Account.SQL_AcGr_Relation.FirstOrDefault(rl2 => rl2.GroupID == groupID
                                                                                                                     && (rl2.Relation == AcGrRelation.LEADER_RELATION || rl2.Relation == AcGrRelation.MEMBER_RELATION)
                                                                                                                     && rl2.Status == Status.ACCEPTED) != null);
                    foreach(var item in result)
                    {
                        item.Status = Status.ACCEPTED;
                    }
                    var finalResult = result.Select(rl => rl.UserID).ToList();
                    dbEntities.SaveChanges();
                    return finalResult;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// decline all members of group that request to join project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_Join_Requested_Group_Members(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                       && rl.Relation == AcPrRelation.MEMBER_RELATION
                                                                       && rl.Status == Status.PENDING
                                                                       && rl.SQL_Account.SQL_AcGr_Relation.FirstOrDefault(rl2 => rl2.GroupID == groupID
                                                                                                                     && (rl2.Relation == AcGrRelation.LEADER_RELATION || rl2.Relation == AcGrRelation.MEMBER_RELATION)
                                                                                                                     && rl2.Status == Status.ACCEPTED) != null);
                    if (result != null)
                    {
                        dbEntities.SQL_AcPr_Relation.RemoveRange(result);
                        dbEntities.SaveChanges();
                        return true;
                    }
                    else return false;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// accept all members of group that request to sponsor project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Accept_Sponsor_Requested_Group_Members(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                       && rl.Relation == AcPrRelation.SPONSOR_RELATION
                                                                       && rl.Status == Status.PENDING
                                                                       && rl.SQL_Account.SQL_AcGr_Relation.FirstOrDefault(rl2 => rl2.GroupID == groupID
                                                                                                                     && (rl2.Relation == AcGrRelation.LEADER_RELATION || rl2.Relation == AcGrRelation.MEMBER_RELATION)
                                                                                                                     && rl2.Status == Status.ACCEPTED) != null);
                    foreach (var item in result)
                    {
                        item.Status = Status.ACCEPTED;
                    }
                    var finalResult = result.Select(rl => rl.UserID).ToList();
                    dbEntities.SaveChanges();
                    return finalResult;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// decline all members of group that request to sponsor project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Delete_Sponsor_Requested_Group_Members(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                       && rl.Relation == AcPrRelation.SPONSOR_RELATION
                                                                       && rl.Status == Status.PENDING
                                                                       && rl.SQL_Account.SQL_AcGr_Relation.FirstOrDefault(rl2 => rl2.GroupID == groupID
                                                                                                                     && (rl2.Relation == AcGrRelation.LEADER_RELATION || rl2.Relation == AcGrRelation.MEMBER_RELATION)
                                                                                                                     && rl2.Status == Status.ACCEPTED) != null);
                    if (result != null)
                    {
                        dbEntities.SQL_AcPr_Relation.RemoveRange(result);
                        dbEntities.SaveChanges();
                        return true;
                    }
                    else return false;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get member of project that not organize that project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_Member_Not_Organizer(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.ProjectID == projectID
                                                                   && (rl.Relation != AcPrRelation.FOLLOW_RELATION || rl.Relation != AcPrRelation.REPORT_RELATION)
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.UserID).Distinct()
                                                             .Except(Get_Organizers(projectID)).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// add multi organizers
        /// </summary>
        /// <param name="listID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Add_Organizers(string[] listID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    foreach (var item in listID)
                    {
                        SQL_AcPr_Relation relation = new SQL_AcPr_Relation();
                        relation.UserID = item;
                        relation.ProjectID = projectID;
                        relation.Relation = AcPrRelation.ORGANIZE_RELATION;
                        relation.Status = Status.ACCEPTED;

                        dbEntities.SQL_AcPr_Relation.Add(relation);
                    }
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}