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

        public string Get_ProjectID(string planPhaseID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_Plan planPhase = dbEntities.SQL_Plan.FirstOrDefault(pl => pl.PlanID == planPhaseID);
                    return planPhase.ProjectID;
                }
            }
            catch
            {

                throw;
            }
        }

        //Delete Plan Phase
        public bool Delete_PlanPhase(string planPhaseID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_Plan planPhase = dbEntities.SQL_Plan.FirstOrDefault(pl => pl.PlanID == planPhaseID);
                    dbEntities.SQL_Plan.Remove(planPhase);
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