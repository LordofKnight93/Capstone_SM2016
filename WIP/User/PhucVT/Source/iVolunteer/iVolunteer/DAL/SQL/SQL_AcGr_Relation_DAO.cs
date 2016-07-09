using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL;
using iVolunteer.Common;

namespace iVolunteer.DAL.SQL
{
    public class SQL_AcGr_Relation_DAO
    {
        /// <summary>
        /// Add a relaton between a user and group to SQLDB
        /// </summary>
        /// <param name="relation">a SQL_AcGr_Relation instance</param>
        /// <returns>true if add success</returns>
        public bool Add_Relation(SQL_AcGr_Relation relation)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
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
        /// set status of a relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <param name="relationType"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool Accept_Member(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID 
                                                                                && rl.GroupID == groupID 
                                                                                && rl.Relation == Relation.MEMBER_RELATION);
                    result.Status = Status.ACCEPTED;
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
        /// set a member to leader
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Set_Leader(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID 
                                                                                && rl.GroupID == groupID 
                                                                                && rl.Relation == Relation.MEMBER_RELATION
                                                                                && rl.Status == Status.ACCEPTED);
                    result.Relation = Relation.LEADER_RELATION;
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
        /// set a leader to member
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Set_Member(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                                && rl.GroupID == groupID
                                                                                && rl.Relation == Relation.LEADER_RELATION
                                                                                && rl.Status == Status.ACCEPTED);
                    result.Relation = Relation.MEMBER_RELATION;
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
        /// Delete a relation between a user and group
        /// </summary>
        /// <param name="userID">user ID in relation</param>
        /// <param name="groupID">group id in relation</param>
        /// <returns>true if delete success</returns>
        public bool Delete_Specific_Relation(string userID, string groupID, int relationType)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID && rl.GroupID == groupID && rl.Relation == relationType);
                    if (result != null)
                    {
                        dbEntities.SQL_AcGr_Relation.Remove(result);
                        dbEntities.SaveChanges();
                    }
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// check if a user is leader of a group
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Is_Leader(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                                && rl.GroupID == groupID
                                                                                && rl.Relation == Relation.LEADER_RELATION);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// check if a user is member of a group
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Is_Member(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                                && rl.GroupID == groupID
                                                                                && rl.Relation == Relation.MEMBER_RELATION
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
        /// check if a user is following a group
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Is_Following(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                                && rl.GroupID == groupID
                                                                                && rl.Relation == Relation.FOLLOW_RELATION);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// check if a user had send join request to a group
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
                                                                                && rl.Relation == Relation.MEMBER_RELATION 
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
        /// get all userID follow a group
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<string> Get_All_Followers(string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.Where(rl => rl.GroupID == groupID
                                                                       && rl.Relation == Relation.FOLLOW_RELATION
                                                                       && rl.Status == Status.ACCEPTED)
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
        /// get all userID is member, leader with group
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<string> Get_All_Members(string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.Where(rl => rl.GroupID == groupID 
                                                                       && rl.Relation != Relation.FOLLOW_RELATION
                                                                       && rl.Status == Status.ACCEPTED)
                                                             .Select(rl => rl.UserID).ToList();
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