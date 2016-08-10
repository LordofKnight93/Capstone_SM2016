using iVolunteer.Models.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Common;

namespace iVolunteer.DAL.SQL
{
    public class SQL_AcIm_Relation_DAO
    {
        public bool Add_Creator(string userID, string imageID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcIm_Relation relation = new SQL_AcIm_Relation();
                    relation.ImageID = imageID;
                    relation.UserID = userID;
                    relation.Relation = AcAlRelation.CREATOR_RELATION;
                    dbEntities.SQL_AcIm_Relation.Add(relation);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
        public bool Delete_all_relations_Im(string albumID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcAl_Relation.FirstOrDefault(rl => rl.AlbumID == albumID);
                    List<string> img = dbEntities.SQL_Image.Where(al => al.AlbumID == albumID).Select(im =>im.ImageID).ToList();
                    if (result != null)
                    {
                        foreach (var item in img)
                        {
                            dbEntities.SQL_AcIm_Relation.RemoveRange(dbEntities.SQL_AcIm_Relation.Where(rl => rl.ImageID == item));
                        }
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
        public bool Delete_relation_Im(string userID, string imageID, int type)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcIm_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.ImageID == imageID
                                                                   && rl.Relation == AcImRelation.CREATOR_RELATION);
                    if (result != null)
                    {
                        dbEntities.SQL_AcIm_Relation.Remove(result);
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