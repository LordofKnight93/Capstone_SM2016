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
        public bool Update_Relation(string userID, string groupID, int relationType, bool status)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID && rl.GroupID == groupID && rl.Relation == relationType);
                    result.Status = status;
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
                    dbEntities.SQL_AcGr_Relation.Remove(result);
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
        /// get relation between a user and group
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public int Get_Relation(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID && rl.GroupID == groupID && rl.Status == Status.ACCEPTED);
                    return result.Relation;
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
                    var result = dbEntities.SQL_AcGr_Relation.FirstOrDefault(rl => rl.UserID == userID && rl.GroupID == groupID 
                                                                                && rl.Relation == Relation.MEMBER_RELATION && rl.Status == Status.PENDING);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }

    }
}