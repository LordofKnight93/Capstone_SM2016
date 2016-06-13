using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.SQL;

namespace iVolunteer.Models.Data_Access_Object.SQL
{
    public static class SQL_Group_DAO
    {
        static iVolunteerEntities dbEntities = new iVolunteerEntities();

        public static List<SQL_Group> Get_All_Group()
        {
            return dbEntities.SQL_Group.ToList();
        }
        /// <summary>
        /// Add group to SQL DB
        /// </summary>
        /// <param name="group">SQL_Group instance</param>
        /// <returns>true if success</returns>
        public static bool Add_Group(SQL_Group group)
        {
            try
            {
                dbEntities.SQL_Group.Add(group);
                dbEntities.SaveChangesAsync();
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
                SQL_Group group = dbEntities.SQL_Group.FirstOrDefault(g => g.GroupID == groupID);
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
                SQL_Group group = dbEntities.SQL_Group.FirstOrDefault(g => g.GroupID == groupID);
                group.IsActivate = status;
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