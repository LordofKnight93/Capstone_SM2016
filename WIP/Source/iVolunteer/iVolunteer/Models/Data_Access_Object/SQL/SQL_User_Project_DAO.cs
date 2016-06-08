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

        public static int Get_Specific_Relation(string userID, string projectID)
        {
            try
            {
                var result = dbEntities.SQL_User_Project.Where(rls => rls.UserID == userID && rls.ProjectID == projectID).FirstOrDefault();
                return result.RelationType;
            }
            catch
            {
                throw;
            }
        }

        public static bool Delete_Specific_Relation(string userID1, string projectID)
        {
            try
            {
                var result = dbEntities.SQL_User_Project.Where(rls => rls.UserID == userID1 && rls.ProjectID == projectID).FirstOrDefault();
                dbEntities.SQL_User_Project.Remove(result);
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public static bool Delete_Relation_By_OneID(string userID)
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