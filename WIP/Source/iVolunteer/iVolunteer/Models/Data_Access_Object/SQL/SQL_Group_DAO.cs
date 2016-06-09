using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.SQL;

namespace iVolunteer.Models.Data_Access_Object.SQL
{
    public static class SQL_Group_DAO
    {
        static iVolunteerEntities dbEntitied = new iVolunteerEntities();
        /// <summary>
        /// Add group to SQL DB
        /// </summary>
        /// <param name="group">SQL_Group instance</param>
        /// <returns>true if success</returns>
        public static bool Add_Group(SQL_Group group)
        {
            try
            {
                dbEntitied.SQL_Group.Add(group);
                dbEntitied.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Check if a group is activate or not
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns>return value compare with Constant</returns>
        public static bool IsActivate(string groupID)
        {
            try
            {
                SQL_Group group = dbEntitied.SQL_Group.FirstOrDefault(g => g.GroupID == groupID);
                return group.IsActivate;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// set activation status to a group
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="status">get in Constant</param>
        /// <returns>true if success</returns>
        public static bool Set_Activation_Status(string groupID, bool status)
        {
            try
            {
                SQL_Group group = dbEntitied.SQL_Group.FirstOrDefault(g => g.GroupID == groupID);
                group.IsActivate = status;
                dbEntitied.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}