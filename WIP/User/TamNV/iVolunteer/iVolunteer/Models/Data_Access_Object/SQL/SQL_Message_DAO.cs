using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.SQL;

namespace iVolunteer.Models.Data_Access_Object.SQL
{
    public class SQL_Message_DAO
    {
        /// <summary>
        /// Add message to SQL DB
        /// </summary>
        /// <param name="message">SQL_Message instance</param>
        /// <returns>true if success</returns>
        public bool Add_Message(SQL_Message message)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    dbEntities.SQL_Message.Add(message);
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
        /// Delete a message
        /// </summary>
        /// <param name="messageID"></param>
        /// <returns>true if success</returns>
        public bool Delete_Message(string messageID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var msg = dbEntities.SQL_Message.Where(m => m.MessageID == messageID);
                    dbEntities.SQL_Message.RemoveRange(msg);
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
        /// check if a user can access to a messsage
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="messageID"></param>
        /// <returns>true if accessable, false if not</returns>
        public bool IsAccessable(string userID, string messageID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Message.FirstOrDefault(msg => msg.UserID == userID && msg.MessageID == messageID);
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