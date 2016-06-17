using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.SQL;

namespace iVolunteer.Models.Data_Access_Object.SQL
{
    public class SQL_Group_DAO
    {
        /// <summary>
        /// Add group to SQL DB
        /// </summary>
        /// <param name="group">SQL_Group instance</param>
        /// <returns>true if success</returns>
        public bool Add_Group(SQL_Group group)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    dbEntities.SQL_Group.Add(group);
                    dbEntities.SaveChangesAsync();
                    return true;
                }
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
        public bool IsActivate(string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_Group group = dbEntities.SQL_Group.FirstOrDefault(g => g.GroupID == groupID);
                    return group.IsActivate;
                }
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
        public bool Set_Activation_Status(string groupID, bool status)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_Group group = dbEntities.SQL_Group.FirstOrDefault(g => g.GroupID == groupID);
                    group.IsActivate = status;
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