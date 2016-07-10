using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL;
using iVolunteer.Common;

namespace iVolunteer.DAL.SQL
{
    public class SQL_AcAc_Relation_DAO
    {
        /// <summary>
        /// Add Report that made bt User to other User
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool Add_Report(SQL_AcAc_Relation report)
        {
            try
            {
                using (iVolunteerEntities dbEntitiies = new iVolunteerEntities())
                {
                    dbEntitiies.SQL_AcAc_Relation.Add(report);
                    dbEntitiies.SaveChanges();
                    return true;

                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// User check if Report has been sent
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool IsSentReport(string userID, string targetUserID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.FirstOrDefault(rl => rl.UserID == userID && rl.TargetUserID == targetUserID && rl.Relation == Relation.REPORT_RELATION);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// User delete sent Report
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public bool DeleteSentReport(string userID, string targetUserID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.FirstOrDefault(rl => rl.UserID == userID && rl.TargetUserID == targetUserID && rl.Relation == Relation.REPORT_RELATION);
                    dbEntities.SQL_AcAc_Relation.Remove(result);
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
        /// Delete all Report Relations of a User
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool DeleteReportRelation(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAc_Relation.RemoveRange(dbEntities.SQL_AcAc_Relation.Where(rl => rl.TargetUserID == userID
                                                                                                            && rl.Relation == Relation.REPORT_RELATION));
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