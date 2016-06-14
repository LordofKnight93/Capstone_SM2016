using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.SQL;
using iVolunteer.Common;

namespace iVolunteer.Models.Data_Access_Object.SQL
{
    public class SQL_Project_DAO
    {
        iVolunteerEntities dbEntities = new iVolunteerEntities();
        /// <summary>
        /// Add new project to SQL DB
        /// </summary>
        /// <param name="project">SQL_Project instance</param>
        /// <returns>true if success</returns>
        public bool Add_Project(SQL_Project project)
        {
            try
            {
                dbEntities.SQL_Project.Add(project);
                dbEntities.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Check if a project is activate
        /// </summary>
        /// <param name="projectID">projectIDt want to check</param>
        /// <returns>activation status of account to compare with Constant</returns>
        public bool IsActivate(string projectID)
        {
            try
            {
                SQL_Project project = dbEntities.SQL_Project.FirstOrDefault(p =>p.ProjectID == projectID);
                return project.IsActivate;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Set activation status of project
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="status">get in Constant</param>
        /// <returns></returns>
        public bool Set_Avtivation_Status(string projectID, bool status)
        {
            try
            {
                SQL_Project project = dbEntities.SQL_Project.FirstOrDefault(p => p.ProjectID == projectID);
                project.IsActivate = status;
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Delete a project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns>true if success</returns>
        public bool Delete_Project(string projectID)
        {
            try
            {
                SQL_Project project = dbEntities.SQL_Project.FirstOrDefault(p => p.ProjectID == projectID);
                dbEntities.SQL_Project.Remove(project);
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