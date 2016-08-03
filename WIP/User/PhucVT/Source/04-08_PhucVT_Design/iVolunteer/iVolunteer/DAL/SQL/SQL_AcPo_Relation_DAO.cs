using iVolunteer.Models.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Common;

namespace iVolunteer.DAL.SQL
{
    public class SQL_AcPo_Relation_DAO
    {
        public bool Add_Relation(string userID, string postID, int type)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcPo_Relation relation = new SQL_AcPo_Relation();
                    relation.UserID = userID;
                    relation.PostID = postID;
                    relation.Relation = type;
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
        public bool Delete_relation(string userID, string postID, int type)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPo_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.PostID == postID
                                                                   && rl.Relation == AcPoRelation.CREATOR_RELATION);
                    if (result != null)
                    {
                        dbEntities.SQL_AcPo_Relation.Remove(result);
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
        public bool Delete_all_relations(string postID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPo_Relation.FirstOrDefault(rl => rl.PostID == postID);
                    if (result != null)
                    {
                        dbEntities.SQL_AcPo_Relation.RemoveRange(dbEntities.SQL_AcPo_Relation.Where(rl => rl.PostID == postID));
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
        public bool Like(string userID, string postID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_AcPo_Relation relation = new SQL_AcPo_Relation();
                    relation.UserID = userID;
                    relation.PostID = postID;
                    relation.Relation = AcPoRelation.LIKE_RELATION;
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
        public bool Is_Owner(string userID, string postID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPo_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.PostID == postID
                                                                   && rl.Relation == AcPoRelation.CREATOR_RELATION);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }
        public string Get_Owner(string postID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPo_Relation.FirstOrDefault(rl => rl.PostID == postID && rl.Relation == AcPoRelation.CREATOR_RELATION);
                    return result.UserID;
                }
            }
            catch
            {
                throw;
            }
        }
        public bool Is_Liked(string userID, string postID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPo_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.PostID == postID
                                                                   && rl.Relation == AcPoRelation.LIKE_RELATION);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }
        public bool Dislike(string userID, string postID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPo_Relation.FirstOrDefault(rl => rl.UserID == userID
                                                                   && rl.PostID == postID
                                                                   && rl.Relation == AcPoRelation.LIKE_RELATION);
                    if (result != null)
                    {
                        dbEntities.SQL_AcPo_Relation.Remove(result);
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
        /// Delete all like of post when it's deleted
        /// </summary>
        /// <returns></returns>
        public bool Delete_All_Likes(string postID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_AcPo_Relation.RemoveRange(dbEntities.SQL_AcPo_Relation.Where(rl => rl.PostID == postID
                                                                                                            && rl.Relation == AcPoRelation.LIKE_RELATION));
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