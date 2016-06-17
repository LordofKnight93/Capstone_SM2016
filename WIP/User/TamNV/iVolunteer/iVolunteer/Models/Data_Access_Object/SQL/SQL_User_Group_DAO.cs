using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.SQL;
using iVolunteer.Common;

namespace iVolunteer.Models.Data_Access_Object.SQL
{
    public class SQL_User_Group_DAO
    {
        /// <summary>
        /// Add a relaton between a user and group to SQLDB
        /// </summary>
        /// <param name="relation">a SQL_User_Group instance</param>
        /// <returns>true if add success</returns>
        public bool Add_Relation(SQL_User_Group relation)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    dbEntities.SQL_User_Group.Add(relation);
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
        /// Get relation between a user and a group
        /// </summary>
        /// <param name="userID">ID of User</param>
        /// <param name="groupID">ID of Group</param>
        /// <returns>return relation's type to compare with Constant</returns>
        public int Get_Specific_Relation(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_User_Group.FirstOrDefault(rls => rls.UserID == userID && rls.GroupID == groupID);
                    return result.RelationType;
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
        public bool Delete_Specific_Relation(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_User_Group.FirstOrDefault(b => b.UserID == userID && b.GroupID == groupID);
                    dbEntities.SQL_User_Group.Remove(result);
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
        /// Delete all relation with group of a user, use when delete user, maybe not be used
        /// </summary>
        /// <param name="userID">deleted userID</param>
        /// <returns>true if delete success</returns>
        public bool Delete_Relation_By_OneID(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_User_Group.Where(b => b.UserID == userID);
                    dbEntities.SQL_User_Group.RemoveRange(result);
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