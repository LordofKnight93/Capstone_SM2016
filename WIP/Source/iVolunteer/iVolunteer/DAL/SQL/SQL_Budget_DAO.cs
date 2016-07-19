using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL;
using iVolunteer.Common;

namespace iVolunteer.DAL.SQL
{
    public class SQL_Budget_DAO
    {
        /// <summary>
        /// Add new Budget to SQL DB
        /// </summary>
        /// <param name="Budget">SQL_Budget instantce</param>
        /// <returns>true if succeess</returns>
        public bool Add_Budget(SQL_Budget budget)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    dbEntities.SQL_Budget.Add(budget);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        public bool Delete_BudgetRecord(string budgetID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_Budget budgetRecord = dbEntities.SQL_Budget.FirstOrDefault(bg => bg.BudgetID == budgetID);
                    dbEntities.SQL_Budget.Remove(budgetRecord);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {

                throw;
            }
        }
        public string Get_ProjectID(string budgetID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_Budget budgetRecord = dbEntities.SQL_Budget.FirstOrDefault(bg => bg.BudgetID == budgetID);
                    return budgetRecord.ProjectID;
                }
            }
            catch
            {

                throw;
            }
        }
    }
}