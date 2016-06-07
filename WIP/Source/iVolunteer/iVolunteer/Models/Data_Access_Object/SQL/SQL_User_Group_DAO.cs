using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.SQL;
using iVolunteer.Common;

namespace iVolunteer.Models.Data_Access_Object.SQL
{
    public static class SQL_User_Group_DAO
    {
        static iVolunteerEntities dbEntities = new iVolunteerEntities();
        public static bool Add_Relation(SQL_User_Group relation)
        {
            try
            {
                dbEntities.SQL_User_Group.Add(relation);
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static int Get_RelationType(string userID, string groupID)
        {
            try
            {
                var result = dbEntities.SQL_User_Group.Where(rls => rls.UserID == userID && rls.GroupID == groupID).FirstOrDefault();
                return result.RelationType;
            }
            catch
            {
                return 0;
            }
        }

        public static bool Delete_Specific_Relation(string userID, string groupID)
        {
            try
            {
                var result = dbEntities.SQL_User_Group.Where(b => b.UserID == userID && b.GroupID == groupID).FirstOrDefault();
                dbEntities.SQL_User_Group.Remove(result);
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Delete_Relation_By_OneID(string userID)
        {
            try
            {
                var result = dbEntities.SQL_User_Group.Where(b => b.UserID == userID);
                dbEntities.SQL_User_Group.RemoveRange(result);
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}