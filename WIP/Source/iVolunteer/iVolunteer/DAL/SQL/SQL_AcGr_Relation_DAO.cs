using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;
using iVolunteer.Models.SQL;
using iVolunteer.Common;

namespace iVolunteer.DAL.SQL
{
    public class SQL_AcGr_Relation_DAO
    {
        /// <summary>
        /// add leaer relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Add_Leader(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcGr_Relation relation = new SQL_AcGr_Relation();
                    relation.UserID = userID;
                    relation.GroupID = groupID;
                    relation.Relation = AcGrRelation.LEADER_RELATION;
                    relation.Status = Status.ACCEPTED;

                    dbEntities.SQL_AcGr_Relation.Add(relation);
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
        /// add membes relation
        /// </summary>
        /// <param name="listID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Add_Members(IEnumerable<string> listID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    foreach(var item in listID)
                    {
                        SQL_AcGr_Relation relation = new SQL_AcGr_Relation();
                        relation.UserID = item;
                        relation.GroupID = groupID;
                        relation.Relation = AcGrRelation.MEMBER_RELATION;
                        relation.Status = Status.ACCEPTED;
                        dbEntities.SQL_AcGr_Relation.Add(relation);
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
        /// add membes relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Add_Member(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcGr_Relation relation = new SQL_AcGr_Relation();
                    relation.UserID = userID;
                    relation.GroupID = groupID;
                    relation.Relation = AcGrRelation.MEMBER_RELATION;
                    relation.Status = Status.ACCEPTED;

                    dbEntities.SQL_AcGr_Relation.Add(relation);
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
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Add_Follower(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcGr_Relation relation = new SQL_AcGr_Relation();
                    relation.UserID = userID;
                    relation.GroupID = groupID;
                    relation.Relation = AcGrRelation.FOLLOW_RELATION;
                    relation.Status = Status.ACCEPTED;

                    dbEntities.SQL_AcGr_Relation.Add(relation);
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
        /// add ajoin request
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Add_Request(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcGr_Relation relation = new SQL_AcGr_Relation();
                    relation.UserID = userID;
                    relation.GroupID = groupID;
                    relation.Relation = AcGrRelation.MEMBER_RELATION;
                    relation.Status = Status.PENDING;

                    dbEntities.SQL_AcGr_Relation.Add(relation);
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
        /// add a report request
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Add_Report(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcGr_Relation relation = new SQL_AcGr_Relation();
                    relation.UserID = userID;
                    relation.GroupID = groupID;
                    relation.Relation = AcGrRelation.REPORT_RELATION;
                    relation.Status = Status.ACCEPTED;

                    dbEntities.SQL_AcGr_Relation.Add(relation);
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
        /// delete member from group
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Delete_Member(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.GroupID == groupID
                                                                   && rl.Relation == AcGrRelation.MEMBER_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE);
                    if (result != null)
                    {
                        dbEntities.SQL_AcGr_Relation.Remove(result);
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
        /// delete leader from group
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Delete_Leader(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.GroupID == groupID
                                                                   && rl.Relation == AcGrRelation.LEADER_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE);
                    if (result != null)
                    {
                        dbEntities.SQL_AcGr_Relation.Remove(result);
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
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Delete_Follower(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.GroupID == groupID
                                                                   && rl.Relation == AcGrRelation.FOLLOW_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE);
                    if (result != null)
                    {
                        dbEntities.SQL_AcGr_Relation.Remove(result);
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
        public bool Set_Member(string leaderID, string groupID)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    Delete_Leader(leaderID, groupID);
                    Add_Member(leaderID, groupID);

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
        /// <param name="memberID"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public bool Set_Leader(string memberID, string groupID)
        {
            using(var transaction = new TransactionScope())
            {
                try
                {
                    Delete_Member(memberID, groupID);
                    Add_Leader(memberID, groupID);

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
        /// check if a user follow a group
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        public bool Is_Follower(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.GroupID == groupID
                                                                   && rl.Relation == AcGrRelation.FOLLOW_RELATION
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
        /// check if a user is leader a group
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        public bool Is_Leader(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.GroupID == groupID
                                                                   && rl.Relation == AcGrRelation.LEADER_RELATION
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
        /// check if a user joined a group
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Is_Joined(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.GroupID == groupID
                                                                   && (rl.Relation == AcGrRelation.LEADER_RELATION || rl.Relation == AcGrRelation.MEMBER_RELATION)
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
        /// check if a user requested to join a group
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Is_Requested(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.GroupID == groupID
                                                                   && rl.Relation == AcGrRelation.MEMBER_RELATION
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
        /// check if a user has reporterd a group
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Is_Reported(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.GroupID == groupID
                                                                   && rl.Relation == AcGrRelation.REPORT_RELATION
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
        /// accept user to group
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Accept_Request(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.GroupID == groupID
                                                                   && rl.Relation == AcGrRelation.MEMBER_RELATION
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
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Delelte_Request(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.GroupID == groupID
                                                                   && rl.Relation == AcGrRelation.MEMBER_RELATION
                                                                   && rl.Status == Status.PENDING
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE);
                    if (result != null)
                    {
                        dbEntities.SQL_AcGr_Relation.Remove(result);
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
        /// get group active members
        /// </summary>
        /// <param name="groupID"></param>
        public List<string> Get_Members(string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.Where(rl => rl.GroupID == groupID
                                                                   && rl.Relation == AcGrRelation.MEMBER_RELATION
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
        /// get group active leaders
        /// </summary>
        /// <param name="groupID"></param>
        public List<string> Get_Leaders(string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.Where(rl => rl.GroupID == groupID
                                                                   && rl.Relation == AcGrRelation.LEADER_RELATION
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
        /// get activate user requested to join group
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<string> Get_Requesters(string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.Where(rl => rl.GroupID == groupID
                                                                   && rl.Relation == AcGrRelation.MEMBER_RELATION
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
        /// get user joined active group
        /// </summary>
        /// <param name="groupID"></param>
        public List<string> Get_Joined_Groups(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.Where(rl => rl.UserID == userID
                                                                   && (rl.Relation == AcGrRelation.LEADER_RELATION || rl.Relation == AcGrRelation.MEMBER_RELATION)
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Group.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.GroupID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// delete report request
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Delete_Report(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.GroupID == groupID
                                                                   && rl.Relation == AcGrRelation.REPORT_RELATION);
                    if (result != null)
                    {
                        dbEntities.SQL_AcGr_Relation.Remove(result);
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

        public bool Is_More_Than_One_Leader(string groupID)
        {

            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.Where(rl => rl.GroupID == groupID
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

        public bool Delete_Reports(string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.RemoveRange(dbEntities.SQL_AcGr_Relation.Where(rl => rl.GroupID == groupID
                                                                                                            && rl.Relation == AcGrRelation.REPORT_RELATION));
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
        /// get member that not in or send join request to a project 
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_Members_Not_Join_Project(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.Where(rl => rl.GroupID == groupID
                                                                   && (rl.Relation == AcGrRelation.MEMBER_RELATION || rl.Relation == AcGrRelation.LEADER_RELATION)
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE
                                                                   && rl.SQL_Account.SQL_AcPr_Relation.FirstOrDefault(rl2 => rl2.ProjectID == projectID
                                                                                                                           && (rl2.Relation == AcPrRelation.LEADER_RELATION || rl2.Relation == AcPrRelation.MEMBER_RELATION)) == null)
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
        /// get active member not sponsored or send sponsor request to a project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_Members_Not_Sponsor_Project(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.Where(rl => rl.GroupID == groupID
                                                                   && (rl.Relation == AcGrRelation.MEMBER_RELATION || rl.Relation == AcGrRelation.LEADER_RELATION)
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE
                                                                   && rl.SQL_Account.SQL_AcPr_Relation.FirstOrDefault(rl2 => rl2.ProjectID == projectID
                                                                                                                           && rl2.Relation == AcPrRelation.SPONSOR_RELATION) == null)
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
        /// get active member of organized group not organized a project
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_Members_Not_Organize_Project(string groupID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.Where(rl => rl.GroupID == groupID
                                                                   && (rl.Relation == AcGrRelation.MEMBER_RELATION || rl.Relation == AcGrRelation.LEADER_RELATION)
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account.IsActivate == Status.IS_ACTIVATE
                                                                   && rl.SQL_Account.SQL_AcPr_Relation.FirstOrDefault(rl2 => rl2.ProjectID == projectID
                                                                                                                           && rl2.Relation == AcPrRelation.ORGANIZE_RELATION) == null)
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
        /// get active group that a user lead
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_Lead_Groups(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.Where(rl => rl.UserID == userID
                                                                   && rl.Relation == AcGrRelation.LEADER_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Group.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.GroupID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Get all followed and Joined Groups (for Newfeed usage)
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<string> Get_Followed_Groups(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.Where(rl => rl.UserID == userID
                                                                    && rl.Relation == AcGrRelation.FOLLOW_RELATION
                                                                    && rl.Status == Status.ACCEPTED
                                                                    && rl.SQL_Group.IsActivate == Status.IS_ACTIVATE)
                                                            .Select(rl => rl.GroupID).Distinct().ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}