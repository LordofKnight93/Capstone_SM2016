using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL;
using iVolunteer.Common;

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
        /// set status of a relation
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <param name="relationType"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool Update_Relation(string userID, string projectID, int relationType, bool status)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                          && rl.ProjectID == projectID 
                                                                          && rl.Relation == relationType);
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
        /// delete a relation between a user and project
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <param name="relationTYpe"></param>
        /// <returns></returns>
        public bool Delete_Specific_Relation(string userID, string projectID, int relationTYpe)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID 
                                                                            && rl.ProjectID == projectID 
                                                                            && rl.Relation == relationTYpe);
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

        public bool De
        /// <summary>
        /// get relation between a user and a project
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public int Get_Relation(string userID, string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPr_Relation.FirstOrDefault(rl => rl.UserID == userID 
                                                                                && rl.ProjectID == projectID 
                                                                                && rl.Status == Status.ACCEPTED);
                    return result.Relation;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}