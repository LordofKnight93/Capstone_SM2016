using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL;
using iVolunteer.Common;

namespace iVolunteer.DAL.SQL
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
        /// deactivte an group
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Deactive(string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Group.FirstOrDefault(acc => acc.GroupID == groupID
                                                                            && acc.IsActivate == Status.IS_ACTIVATE);
                    if (result != null)
                    {
                        result.IsActivate = Status.IS_BANNED;
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// activate an group
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool Activate(string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Group.FirstOrDefault(acc => acc.GroupID == groupID
                                                                            && acc.IsActivate == Status.IS_BANNED);
                    if (result != null)
                    {
                        result.IsActivate = Status.IS_ACTIVATE;
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}