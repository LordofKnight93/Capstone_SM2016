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
        /// <param name="senderID"></param>
        /// <param name="otherID"></param>
        /// <returns></returns>
        public bool Add_Report(string senderID, string otherID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcAc_Relation relation = new SQL_AcAc_Relation();
                    relation.UserID = senderID;
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
        /// <param name="senderID"></param>
        /// <param name="otherID"></param>
        /// <returns></returns>
        public bool Delete_Report(string senderID, string otherID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.FirstOrDefault(rl => rl.UserID == senderID && rl.FriendID == otherID && rl.Relation == AcAcRelation.REPORT_RELATION);
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
        /// <param name="otherID"></param>
        /// <returns></returns>
        public bool Add_Request(string senderID, string otherID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcAc_Relation relation = new SQL_AcAc_Relation();
                    relation.UserID = senderID;
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
        /// <param name="senderID">sender</param>
        /// <returns></returns>
        public bool Accept_Request(string receiverID, string senderID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.FirstOrDefault(rl => rl.UserID == senderID
                                                                   && rl.FriendID == receiverID
                                                                   && rl.Relation == AcAcRelation.FRIEND_RELATION
                                                                   && rl.Status == Status.PENDING);
                    if (result != null)
                    {
                        result.Status = Status.ACCEPTED;
                        dbEntities.SaveChanges();
                        Add_Friend(receiverID, senderID);
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
        /// <param name="senderID">sender</param>
        /// <param name="receiverID">receiver</param>
        /// <returns></returns>
        public bool Delete_Request(string senderID, string receiverID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.FirstOrDefault(rl => rl.UserID == senderID
                                                                   && rl.FriendID == receiverID
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
        /// <param name="senderID"></param>
        /// <param name="receiverID"></param>
        /// <returns></returns>
        public bool Is_Requested(string senderID, string receiverID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.FirstOrDefault(rl => rl.UserID == senderID
                                                                   && rl.FriendID == receiverID
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
        /// get friend of user
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
                                                                   && rl.Status == Status.ACCEPTED)
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
        /// <summary>
        /// get active friend that not in or send join request to a group 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public List<string> Get_Friend_Not_In_Group(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.Where(rl => rl.UserID == userID
                                                                   && rl.Relation == AcAcRelation.FRIEND_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account1.IsActivate == Status.IS_ACTIVATE
                                                                   && rl.SQL_Account1.SQL_AcGr_Relation.FirstOrDefault(rl2 => rl2.GroupID == groupID 
                                                                                                                           && (rl2.Relation == AcGrRelation.LEADER_RELATION || rl2.Relation == AcGrRelation.MEMBER_RELATION)) == null)
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
        /// get active friend that not in or send join request a project
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public List<string> Get_Friend_Not_In_Project(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.Where(rl => rl.UserID == userID
                                                                   && rl.Relation == AcAcRelation.FRIEND_RELATION
                                                                   && rl.Status == Status.ACCEPTED
                                                                   && rl.SQL_Account1.IsActivate == Status.IS_ACTIVATE
                                                                   && rl.SQL_Account1.SQL_AcPr_Relation.FirstOrDefault(rl2 => rl2.ProjectID == projectID
                                                                                                                           && (rl2.Relation == AcPrRelation.LEADER_RELATION 
                                                                                                                              || rl2.Relation == AcPrRelation.MEMBER_RELATION
                                                                                                                              || rl2.Relation == AcPrRelation.SUGGESTED_RELATION
                                                                                                                              || rl2.Relation == AcPrRelation.INVITED_RELATION)) == null)
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
        /// Count number of friend Request to display in Notification
        /// </summary>
        /// <param name="senderID"></param>
        /// <param name="receiverID"></param>
        /// <returns></returns>
        public int Count_Request(string receiverID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    int count = dbEntities.SQL_AcAc_Relation.Count(rl => rl.FriendID == receiverID && rl.Status == Status.PENDING);
                    return count;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}