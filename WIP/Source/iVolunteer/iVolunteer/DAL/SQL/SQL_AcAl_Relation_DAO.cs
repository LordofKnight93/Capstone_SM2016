using iVolunteer.Models.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Common;

namespace iVolunteer.DAL.SQL
{
    public class SQL_AcAl_Relation_DAO
    {
        public bool Add_Creator(string userID, string albumID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcAl_Relation relation = new SQL_AcAl_Relation();
                    relation.AlbumID = albumID;
                    relation.UserID = userID;
                    relation.Relation = AcAlRelation.CREATOR_RELATION;
                    dbEntities.SQL_AcAl_Relation.Add(relation);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
        public bool Delete_relation_Al(string albumID, int type)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAl_Relation.FirstOrDefault(rl =>
                                                                    rl.AlbumID == albumID
                                                                   && rl.Relation == AcAlRelation.CREATOR_RELATION);
                        dbEntities.SQL_AcAl_Relation.Remove(result);
                        dbEntities.SaveChanges();
                        return true;
                }
            }
            catch
            {
                throw;
            }
        }
        public bool Is_Creator(string userID, string albumID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAl_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.AlbumID == albumID
                                                                   && rl.Relation == AcAlRelation.CREATOR_RELATION);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }


    }
}