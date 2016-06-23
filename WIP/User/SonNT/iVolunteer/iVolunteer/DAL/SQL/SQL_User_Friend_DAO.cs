using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL;

namespace iVolunteer.DAL.SQL
{
    public class SQL_User_Friend_DAO
    {
        /// <summary>
        /// Add a relaton between a user and another usser to SQLDB
        /// </summary>
        /// <param name="relation">a SQL_User_Friend instance</param>
        /// <returns>true if add success</returns>
        public bool Add_Relation( SQL_User_Friend relation)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    dbEntities.SQL_User_Friend.Add(relation);
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
        /// Check if 2 user is friend or not
        /// </summary>
        /// <param name="userID1"></param>
        /// <param name="userID2"></param>
        /// <returns>true if is friend, false if not</returns>
        public bool Is_Friend(string userID1, string userID2)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_User_Friend.Where(rls => rls.UserID == userID1 && rls.FriendID == userID2
                                                                  || rls.UserID == userID2 && rls.FriendID == userID1);
                    if (result == null) return false;
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Delete a relation between a user and another user
        /// </summary>
        /// <param name="userID">user ID in relation</param>
        /// <param name="friendID">other user id in relation</param>
        /// <returns>true if delete success</returns>
        public bool Delete_Specific_Relation(string userID, string friendID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_User_Friend.Where(rls => rls.UserID == userID && rls.FriendID == friendID
                                                                  || rls.UserID == friendID && rls.FriendID == userID);
                    dbEntities.SQL_User_Friend.RemoveRange(result);
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
        /// Delete all friend relation of user, use when delete user, maybe not be used
        /// </summary>
        /// <param name="userID">deleted userID</param>
        /// <returns>true if delete success</returns>
        public bool Delete_Relation_By_UserID(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_User_Friend.Where(rls => rls.UserID == userID || rls.FriendID == userID);
                    dbEntities.SQL_User_Friend.RemoveRange(result);
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