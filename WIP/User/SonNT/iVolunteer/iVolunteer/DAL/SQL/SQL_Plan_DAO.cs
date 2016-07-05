using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL;
using iVolunteer.Common;

namespace iVolunteer.DAL.SQL
{
    public class SQL_Plan_DAO
    {
        /// <summary>
        /// Add new plan to SQL DB
        /// </summary>
        /// <param name="plan">SQL_Plan instantce</param>
        /// <returns>true if succeess</returns>
        public bool Add_Plan(SQL_Plan plan)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    dbEntities.SQL_Plan.Add(plan);
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
        /// Delete a plan 
        /// </summary>
        /// <param name="planID"></param>
        /// <returns>true if success</returns>
        public bool Delete_Plan(string planID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var plan = dbEntities.SQL_Plan.FirstOrDefault(p => p.PlanID == planID);
                    dbEntities.SQL_Plan.Remove(plan);
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
        /// Check if a user can view a plan
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="planID"></param>
        /// <returns>true if accessable, false if not</returns>
        public bool IsAccessable(string userID, string planID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    //get plan
                    var result = from plan in dbEntities.SQL_Plan
                                 join project in dbEntities.SQL_Project on plan.ProjectID equals project.ProjectID
                                 join relation in dbEntities.SQL_AcPr_Relation on plan.ProjectID equals relation.ProjectID
                                 where plan.PlanID == planID && project.IsActivate == Status.IS_ACTIVATE
                                        && relation.UserID == userID && relation.Relation != Relation.FOLLOW_RELATION
                                 select plan;
                    if (result == null) return false;
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