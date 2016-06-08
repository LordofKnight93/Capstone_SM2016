using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.SQL;

namespace iVolunteer.Models.Data_Access_Object.SQL
{
    public static class SQL_User_Friend_DAO
    {
        static iVolunteerEntities dbEntities = new iVolunteerEntities();
        public static bool Add_Relation( SQL_User_Friend relation)
        {
            try
            {
                dbEntities.SQL_User_Friend.Add(relation);
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public static bool Delete_Specific_Relation(string userID, string friendID)
        {
            try
            {
                var result = dbEntities.SQL_User_Friend.Where(rls => rls.UserID == userID && rls.FriendID == friendID 
                                                                  || rls.UserID == friendID && rls.FriendID == userID);
                dbEntities.SQL_User_Friend.RemoveRange(result);
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public static bool Delete_Relation_By_UserID(string userID)
        {
            try
            {
                var result = dbEntities.SQL_User_Friend.Where(rls => rls.UserID == userID || rls.FriendID == userID);
                dbEntities.SQL_User_Friend.RemoveRange(result);
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