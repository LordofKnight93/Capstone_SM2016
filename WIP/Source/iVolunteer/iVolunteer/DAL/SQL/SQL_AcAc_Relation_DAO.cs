using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL;
using iVolunteer.Common;

namespace iVolunteer.DAL.SQL
{
    public class SQL_AcAc_Relation_DAO
    {
        /// <summary>
        /// add friend relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="otherID"></param>
        /// <returns></returns>
        public bool Add_Friend(string userID, string otherID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcAc_Relation relation = new SQL_AcAc_Relation();
                    relation.UserID = userID;
                    relation.FriendID = otherID;
                    relation.Relation = AcAcRelation.FRIEND_RELATION;
                    relation.Status = Status.ACCEPTED;

                    dbEntities.SQL_AcAc_Relation.Add(relation);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <report>
        /// add friend relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="otherID"></param>
        /// <returns></returns>
        public bool Add_Report(string userID, string otherID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcAc_Relation relation = new SQL_AcAc_Relation();
                    relation.UserID = userID;
                    relation.FriendID = otherID;
                    relation.Relation = AcAcRelation.REPORT_RELATION;
                    relation.Status = Status.ACCEPTED;

                    dbEntities.SQL_AcAc_Relation.Add(relation);
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
        /// User delete sent Report
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Delete_Report(string userID, string otherUserID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.FirstOrDefault(rl => rl.UserID == userID && rl.FriendID == otherUserID && rl.Relation == AcAcRelation.REPORT_RELATION);
                    dbEntities.SQL_AcAc_Relation.Remove(result);
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
        /// Delete all Report Relations of a User
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Delete_Reports(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.RemoveRange(dbEntities.SQL_AcAc_Relation.Where(rl => rl.FriendID == userID
                                                                                                            && rl.Relation == AcAcRelation.REPORT_RELATION));
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
        /// add friend request
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Add_Request(string userID, string otherID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcAc_Relation relation = new SQL_AcAc_Relation();
                    relation.UserID = userID;
                    relation.FriendID = otherID;
                    relation.Relation = AcAcRelation.FRIEND_RELATION;
                    relation.Status = Status.PENDING;

                    dbEntities.SQL_AcAc_Relation.Add(relation);
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
        /// accepnt a request
        /// </summary>
        /// <param name="userID">recceiver </param>
        /// <param name="otherID">sender</param>
        /// <returns></returns>
        public bool Accept_Request(string userID, string otherID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.FirstOrDefault(rl => rl.UserID == otherID
                                                                   && rl.FriendID == userID
                                                                   && rl.Relation == AcAcRelation.FRIEND_RELATION
                                                                   && rl.Status == Status.PENDING);
                    if (result != null)
                    {
                        result.Status = Status.ACCEPTED;
                        dbEntities.SaveChanges();
                        Add_Friend(userID, otherID);
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
        /// delete a request
        /// </summary>
        /// <param name="userID">sender</param>
        /// <param name="otherID">receiver</param>
        /// <returns></returns>
        public bool Delete_Request(string userID, string otherID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.FriendID == otherID
                                                                   && rl.Relation == AcAcRelation.FRIEND_RELATION
                                                                   && rl.Status == Status.PENDING);
                    if (result != null)
                    {
                        dbEntities.SQL_AcAc_Relation.Remove(result);
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
        /// delete a friend relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="otherID"></param>
        /// <returns></returns>
        public bool Delete_Friend(string userID, string otherID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.FriendID == otherID
                                                                   && rl.Relation == AcAcRelation.FRIEND_RELATION
                                                                   && rl.Status == Status.ACCEPTED);
                    if (result != null)
                    {
                        dbEntities.SQL_AcAc_Relation.Remove(result);
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
        /// check if request is send or not 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="otherID"></param>
        /// <returns></returns>
        public bool Is_Requested(string userID, string otherID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.FriendID == otherID
                                                                   && rl.Relation == AcAcRelation.FRIEND_RELATION
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
        /// check if report is send or not 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="otherID"></param>
        /// <returns></returns>
        public bool Is_Reported(string userID, string otherID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.FriendID == otherID
                                                                   && rl.Relation == AcAcRelation.REPORT_RELATION
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
        /// check if report is friend or not 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="otherID"></param>
        /// <returns></returns>
        public bool Is_Friend(string userID, string otherID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.FriendID == otherID
                                                                   && rl.Relation == AcAcRelation.FRIEND_RELATION
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
        /// get active friend of user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<string> Get_Friends(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.Where(rl => rl.UserID == userID
                                                                   && rl.Relation == AcAcRelation.FRIEND_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account1.IsActivate == Status.IS_ACTIVATE)
                                                             .Select(rl => rl.FriendID).ToList();
                    return result;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// get active request
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<string> Get_Requests(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.Where(rl => rl.FriendID == userID
                                                                   && rl.Relation == AcAcRelation.FRIEND_RELATION
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
        /// get active mutal friend
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="otherID"></param>
        /// <returns></returns>
        public List<string> Get_MutalFriend(string userID, string otherID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.Where(rl => rl.UserID == userID
                                                                   && rl.Relation == AcAcRelation.FRIEND_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account1.IsActivate == Status.IS_ACTIVATE
                                                                   && rl.SQL_Account1.SQL_AcAc_Relation1.FirstOrDefault(rl2 => rl2.FriendID == otherID 
                                                                                                                            && rl2.Status == Status.ACCEPTED) != null)
                                                             .Select(rl => rl.FriendID).ToList();
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