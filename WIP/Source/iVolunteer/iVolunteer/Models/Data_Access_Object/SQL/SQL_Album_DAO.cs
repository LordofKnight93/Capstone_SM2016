using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.SQL;
using iVolunteer.Common;

namespace iVolunteer.Models.Data_Access_Object.SQL
{
    public class SQL_Album_DAO
    {
        iVolunteerEntities dbEntities = new iVolunteerEntities();
        /// <summary>
        /// Add album to project
        /// </summary>
        /// <param name="album">SQL_Album instance</param>
        /// <returns>true if success</returns>
        public bool Add_Album(SQL_Album album)
        {
            try
            {
                dbEntities.SQL_Album.Add(album);
                dbEntities.SaveChanges();
                return true;
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
        public bool Set_Permission(string albumID, bool permission)
        {
            try
            {
                var result = dbEntities.SQL_Album.FirstOrDefault(al => al.AlbumID == albumID);
                result.Permission = permission;
                dbEntities.SaveChanges();
                return true;
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
                var result = dbEntities.SQL_Album.FirstOrDefault(al => al.AlbumID == albumID);
                dbEntities.SQL_Album.Remove(result);
                dbEntities.SaveChanges();
                return true;
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
                //is album exist
                SQL_Album album = dbEntities.SQL_Album.FirstOrDefault(al => al.AlbumID == albumID);
                if (album == null) return false;

                //check if it's public or not
                if(album.Permission == Constant.IS_PUBLIC) return true;

                //if not check user has direct relation with project or group album belong to
                if (album.ProjectID != null)
                {
                    SQL_User_Project project_relation = dbEntities.SQL_User_Project.FirstOrDefault(pr => pr.ProjectID == album.ProjectID && pr.UserID == userID
                                                                                                            && pr.RelationType == Constant.DIRECT_RELATION);
                    if (project_relation != null) return true;
                }
                else
                {
                    SQL_User_Group group_relation = dbEntities.SQL_User_Group.FirstOrDefault(gr => gr.GroupID == album.GroupID && gr.UserID == userID
                                                                                                            && gr.RelationType == Constant.DIRECT_RELATION);
                    if (group_relation != null) return true;
                }

                // case else is not have permission
                return false;

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}