using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.SQL;
using iVolunteer.Common;

namespace iVolunteer.Models.Data_Access_Object.SQL
{
    public static class SQL_Plan_DAO
    {
        static iVolunteerEntities dbEntities = new iVolunteerEntities();
        /// <summary>
        /// Add new plan to SQL DB
        /// </summary>
        /// <param name="plan">SQL_Plan instantce</param>
        /// <returns>true if succeess</returns>
        public static bool Add_Plan(SQL_Plan plan)
        {
            try
            {
                dbEntities.SQL_Plan.Add(plan);
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Delete a plan 
        /// </summary>
        /// <param name="planID"></param>
        /// <returns>true if success</returns>
        public static bool Delete_Plan(string planID)
        {
            try
            {
                var plan = dbEntities.SQL_Plan.FirstOrDefault(p => p.PlanID == planID);
                dbEntities.SQL_Plan.Remove(plan);
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Check if a user can view a plan
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="planID"></param>
        /// <returns>true if accessable, false if not</returns>
        public static bool IsAccessable(string userID, string planID)
        {
            try
            {
                //get plan
                SQL_Plan plan = dbEntities.SQL_Plan.FirstOrDefault(p => p.PlanID == planID);
                //get direct relation between user and project plan belong to
                SQL_User_Project result = dbEntities.SQL_User_Project.FirstOrDefault(p => p.UserID == userID && p.ProjectID == plan.ProjectID
                                                                                            && p.RelationType == Constant.DIRECT_RELATION); 
                if (result == null) return false;
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}