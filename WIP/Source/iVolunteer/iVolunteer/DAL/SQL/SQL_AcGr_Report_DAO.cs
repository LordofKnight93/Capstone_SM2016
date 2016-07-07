using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL;
using iVolunteer.Common;

namespace iVolunteer.DAL.SQL
{
    public class SQL_AcGr_Report_DAO
    {
        public bool Add_Report(SQL_AcGr_Report report)
        {
            try
            {
                using(iVolunteerEntities dbEntitiies = new iVolunteerEntities())
                {
                    dbEntitiies.SQL_AcGr_Report.Add(report);
                    dbEntitiies.SaveChanges();
                    return true;

                }
            }
            catch
            {
                throw;
            }
        }
        public bool IsSentReport(string userID, string groupID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Report.FirstOrDefault(rl => rl.UserID == userID && rl.GroupID == groupID && rl.Status == Status.PENDING_REPORT);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }
        public bool DeleteSentReport(string userID, string groupID)
        {
            try
            {
                using(iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcGr_Report.FirstOrDefault(rl => rl.UserID == userID && rl.GroupID == groupID);
                    dbEntities.SQL_AcGr_Report.Remove(result);
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