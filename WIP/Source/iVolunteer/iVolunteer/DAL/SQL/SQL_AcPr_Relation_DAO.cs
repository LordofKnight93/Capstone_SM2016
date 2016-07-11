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
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE);
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
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE);
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
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE);
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
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE);
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
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE);
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
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE);
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
        /// accept user to project
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Accept_Request(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ProjectID == projectID
                                                                   && rl.Relation == AcPrRelation.MEMBER_RELATION
                                                                   && rl.Status == Status.PENDING
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE);
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
                                                                   && rl.Status == Status.PENDING
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE);
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
                                                                   && rl.Status == Status.PENDING
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE);
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
        public List<string> Get_Join_Request(string projectID)
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
        public List<string> Get_Sponsor_Request(string projectID)
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
        /// get project active leaders
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
        /// get user joined active project
        /// </summary>
        /// <param name="projectID"></param>
        public List<string> Get_Joined_Projects(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.UserID == userID
                                                                   && (rl.Relation != AcPrRelation.FOLLOW_RELATION && rl.Relation != AcPrRelation.REPORT_RELATION)
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
        /// <param name="projectID"></param>
        public List<string> Get_Participated_Projects(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.UserID == userID
                                                                   && ( rl.Relation == AcPrRelation.LEADER_RELATION || rl.Relation == AcPrRelation.MEMBER_RELATION )
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
        /// get user sponsored active project
        /// </summary>
        /// <param name="projectID"></param>
        public List<string> Get_Sponsored_Projects(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.UserID == userID
                                                                   && rl.Relation == AcPrRelation.SPONSOR_RELATION
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
        /// get user organized active project
        /// </summary>
        /// <param name="projectID"></param>
        public List<string> Get_Organized_Projects(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.UserID == userID
                                                                   && rl.Relation == AcPrRelation.ORGANIZE_RELATION
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
        /// get user current active project
        /// </summary>
        /// <param name="projectID"></param>
        public List<string> Get_Current_Projects(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rl => rl.UserID == userID
                                                                   && rl.Relation != AcPrRelation.FOLLOW_RELATION
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

    }
}