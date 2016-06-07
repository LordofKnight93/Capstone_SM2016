using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.SQL;

namespace iVolunteer.Models.Data_Access_Object.SQL
{
    public static class SQL_Plan_DAO
    {
        static iVolunteerEntities dbEntities = new iVolunteerEntities();
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
                return false;
            }
        }

        public static bool Delete_Plan(string planID)
        {
            try
            {
                var plan = dbEntities.SQL_Plan.Where(p => p.PlanID == planID).FirstOrDefault();
                dbEntities.SQL_Plan.Remove(plan);
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsAccessable(string userID, string planID)
        {
            try
            {
                var result = from pl in dbEntities.SQL_Plan
                             join pr in dbEntities.SQL_User_Project on pl.ProjectID equals pr.ProjectID
                             where pr.UserID == userID && pr.RelationType == 1
                             select pr.ID;
                if (result == null) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}