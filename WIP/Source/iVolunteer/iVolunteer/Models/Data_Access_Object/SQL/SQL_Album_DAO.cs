using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.SQL;
using iVolunteer.Common;

namespace iVolunteer.Models.Data_Access_Object.SQL
{
    public static class SQL_Album_DAO
    {
        static iVolunteerEntities dbEntities = new iVolunteerEntities();
        public static bool Add_Album(SQL_Album album)
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
        public static bool Set_Permission(string albumID, bool permission)
        {
            try
            {
                var result = dbEntities.SQL_Album.Where(al => al.AlbumID == albumID).FirstOrDefault();
                result.Permission = permission;
                dbEntities.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool Delete_Album(string albumID)
        {
            try
            {
                var result = dbEntities.SQL_Album.Where(al => al.AlbumID == albumID).FirstOrDefault();
                dbEntities.SQL_Album.Remove(result);
                dbEntities.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsAccessable(string userID, string albumID)
        {
            try
            {
                //get album information
                var album = dbEntities.SQL_Album.Where(al => al.AlbumID == albumID).FirstOrDefault();

                //check if it's public or not
                if(album.Permission == Constant.IS_PUBLIC) return true;

                //in case album not public, check if user have direct relation with project album belong to
                var project_relation = dbEntities.SQL_User_Project.FirstOrDefault(pr => pr.ProjectID == album.ProjectID && pr.UserID == userID);
                if (project_relation.RelationType == Constant.DIRECT_RELATION) return true;

                //in case album not public, check if user have direct relation with group album belong to
                var group_relation = dbEntities.SQL_User_Group.FirstOrDefault(gr => gr.GroupID == album.GroupID && gr.UserID == userID);
                if (group_relation.RelationType == Constant.DIRECT_RELATION) return true;

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