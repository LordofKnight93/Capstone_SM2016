using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.SQL;

namespace iVolunteer.Models.Data_Access_Object.SQL
{
    public static class SQL_Message_DAO
    {
        static iVolunteerEntities dbEntities = new iVolunteerEntities();
        public static bool Add_Message(SQL_Message message)
        {
            try
            {
                dbEntities.SQL_Message.Add(message);
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Delete_Message(string messageID)
        {
            try
            {
                var msg = dbEntities.SQL_Message.Where(m => m.MessageID == messageID);
                dbEntities.SQL_Message.RemoveRange(msg);
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsAccessable(string userID, string messageID)
        {
            try
            {
                var result = dbEntities.SQL_Message.Where(msg => msg.UserID == userID && msg.MessageID == messageID);
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