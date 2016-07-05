using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL;
using iVolunteer.Common;

namespace iVolunteer.DAL.SQL
{
    public class SQL_Album_DAO
    {              
        /// <summary>
        /// Add album to project
        /// </summary>
        /// <param name="album">SQL_Album instance</param>
        /// <returns>true if success</returns>
        public bool Add_Album(SQL_Album album)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    dbEntities.SQL_Album.Add(album);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {

                return false;
            }
        }
        /// <summary>
        /// Set permission status to album
        /// </summary>
        /// <param name="albumID"></param>
        /// <param name="permission">get in Constant</param>
        /// <returns>true if success</returns>
        public bool Set_IsPublic(string albumID, bool permission)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Album.FirstOrDefault(al => al.AlbumID == albumID);
                    result.IsPublic = permission;
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// Delete a album
        /// </summary>
        /// <param name="albumID"></param>
        /// <returns>true if success</returns>
        public bool Delete_Album(string albumID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Album.FirstOrDefault(al => al.AlbumID == albumID);
                    dbEntities.SQL_Album.Remove(result);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// Check if a user can access to a album
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="albumID"></param>
        /// <returns>true if accessalble, false if not</returns>
        public bool IsAccessable(string userID, string albumID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    //is album exist
                    SQL_Album album = dbEntities.SQL_Album.FirstOrDefault(al => al.AlbumID == albumID);
                    if (album == null) return false;

                    //check if it's public or not
                    if (album.IsPublic == Status.IS_PUBLIC) return true;

                    //if not check user has direct relation with project or group album belong to
                    if (album.ProjectID != null)
                    {
                        SQL_AcPr_Relation project_relation = dbEntities.SQL_AcPr_Relation.FirstOrDefault(pr => pr.ProjectID == album.ProjectID && pr.UserID == userID
                                                                                                                && pr.Relation != Relation.FOLLOW_RELATION);
                        if (project_relation != null) return true;
                    }
                    else
                    {
                        SQL_AcGr_Relation group_relation = dbEntities.SQL_AcGr_Relation.FirstOrDefault(gr => gr.GroupID == album.GroupID && gr.UserID == userID
                                                                                                                && gr.Relation != Relation.FOLLOW_RELATION);
                        if (group_relation != null) return true;
                    }

                    // case else is not have permission
                    return false;
                }

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}