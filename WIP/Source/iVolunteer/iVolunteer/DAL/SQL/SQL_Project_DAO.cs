using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL;
using iVolunteer.Common;

namespace iVolunteer.DAL.SQL
{
    public class SQL_Project_DAO
    {
        /// <summary>
        /// Add project to SQL DB
        /// </summary>
        /// <param name="project">SQL_Project instance</param>
        /// <returns>true if success</returns>
        public bool Add_Project(SQL_Project project)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    dbEntities.SQL_Project.Add(project);
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
        /// Check if a project is activate or not
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns>return value compare with Constant</returns>
        public bool IsActivate(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_Project project = dbEntities.SQL_Project.FirstOrDefault(g => g.ProjectID == projectID);
                    return project.IsActivate;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// deactivte an project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Deactive(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Project.FirstOrDefault(acc => acc.ProjectID == projectID
                                                                            && acc.IsActivate == Status.IS_ACTIVATE);
                    if (result != null)
                    {
                        result.IsActivate = Status.IS_BANNED;
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
        /// activate an project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Activate(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Project.FirstOrDefault(acc => acc.ProjectID == projectID
                                                                            && acc.IsActivate == Status.IS_BANNED);
                    if (result != null)
                    {
                        result.IsActivate = Status.IS_ACTIVATE;
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
        /// check if project is ongoing or not
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Is_Ongoing(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_Project project = dbEntities.SQL_Project.FirstOrDefault(g => g.ProjectID == projectID);
                    return project.InProgress;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// close an active project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Close(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Project.FirstOrDefault(acc => acc.ProjectID == projectID
                                                                            && acc.IsActivate == Status.IS_ACTIVATE);
                    if (result != null)
                    {
                        result.InProgress = Status.ENDED;
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
        /// reopen an active project
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public bool Reopen(string projectID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Project.FirstOrDefault(acc => acc.ProjectID == projectID
                                                                            && acc.IsActivate == Status.IS_ACTIVATE);
                    if (result != null)
                    {
                        result.InProgress = Status.ONGOING;
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
    }
}