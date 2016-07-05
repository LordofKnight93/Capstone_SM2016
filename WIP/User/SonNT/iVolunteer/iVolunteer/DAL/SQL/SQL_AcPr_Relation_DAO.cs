using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL;

namespace iVolunteer.DAL.SQL
{
    public class SQL_AcPr_Relation_DAO
    {
        /// <summary>
        /// Add a relaton between a user and project to SQLDB
        /// </summary>
        /// <param name="relation">a SQL_AcPr_Relation instance</param>
        /// <returns>true if add success</returns>
        public bool Add_Relation(SQL_AcPr_Relation relation)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
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
        /// Get relation between a user and a project
        /// </summary>
        /// <param name="userID">ID of User</param>
        /// <param name="projectID">ID of Project</param>
        /// <returns>return relation's type to compare with Constant</returns>

        public int Get_Specific_Relation(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rls => rls.UserID == userID && rls.ProjectID == projectID);
                    return result == null ? 0 : result.Relation;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Delete a relation between a user and project
        /// </summary>
        /// <param name="userID">user ID in relation</param>
        /// <param name="projectID">project id in relation</param>
        /// <returns>true if delete success</returns>
        public bool Delete_Specific_Relation(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rls => rls.UserID == userID && rls.ProjectID == projectID);
                    dbEntities.SQL_AcPr_Relation.Remove(result);
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
        /// Delete all relation with project of a user, use when delete user, maybe not be used
        /// </summary>
        /// <param name="userID">deleted userID</param>
        /// <returns>true if delete success</returns>
        public bool Delete_Relation_By_UserID(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.Where(rls => rls.UserID == userID);
                    dbEntities.SQL_AcPr_Relation.RemoveRange(result);
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