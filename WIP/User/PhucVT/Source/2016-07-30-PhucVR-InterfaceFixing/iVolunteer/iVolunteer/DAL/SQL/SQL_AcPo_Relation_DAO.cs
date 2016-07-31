using iVolunteer.Models.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iVolunteer.DAL.SQL
{
    public class SQL_AcPo_Relation_DAO
    {
        public bool Add(string userID, string postID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcPo_Relation relation = new SQL_AcPo_Relation();
                    relation.UserID = userID;
                    relation.PostID = postID;
                    relation.Relation = 1;
                    dbEntities.SQL_AcPo_Relation.Add(relation);
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