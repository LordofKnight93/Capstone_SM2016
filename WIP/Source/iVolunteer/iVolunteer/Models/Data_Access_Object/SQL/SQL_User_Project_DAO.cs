using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.SQL;

namespace iVolunteer.Models.Data_Access_Object.SQL
{
    public static class SQL_User_Project_DAO
    {
        static iVolunteerEntities dbEntities = new iVolunteerEntities();
        /// <summary>
        /// Add a relaton between a user and project to SQLDB
        /// </summary>
        /// <param name="relation">a SQL_User_Project instance</param>
        /// <returns>true if add success</returns>
        public static bool Add_Relation(SQL_User_Project relation)
        {
            try
            {
                dbEntities.SQL_User_Project.Add(relation);
                dbEntities.SaveChanges();
                return true;
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

        public static int Get_Specific_Relation(string userID, string projectID)
        {
            try
            {
                var result = dbEntities.SQL_User_Project.FirstOrDefault(rls => rls.UserID == userID && rls.ProjectID == projectID);
                return result.RelationType;
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
        public static bool Delete_Specific_Relation(string userID, string projectID)
        {
            try
            {
                var result = dbEntities.SQL_User_Project.FirstOrDefault(rls => rls.UserID == userID && rls.ProjectID == projectID);
                dbEntities.SQL_User_Project.Remove(result);
                dbEntities.SaveChanges();
                return true;
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
        public static bool Delete_Relation_By_UserID(string userID)
        {
            try
            {
                var result = dbEntities.SQL_User_Project.Where(rls => rls.UserID == userID);
                dbEntities.SQL_User_Project.RemoveRange(result);
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}