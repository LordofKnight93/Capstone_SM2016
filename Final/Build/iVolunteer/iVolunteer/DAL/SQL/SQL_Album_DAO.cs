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
        public bool Add_Image(SQL_Image image)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    dbEntities.SQL_Image.Add(image);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {

                return false;
            }
        }
        public bool Delete_Album(string albumID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    SQL_Album album = dbEntities.SQL_Album.FirstOrDefault(p => p.AlbumID == albumID);
                    dbEntities.SQL_Album.Remove(album);
                    dbEntities.SaveChanges();
                    //dbEntities.SaveChangesAsync();
                    return true;
                }
            }
            catch
            {

                throw;
            }
        }
        public bool Delete_Image(string albumID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    List<SQL_Image> img = dbEntities.SQL_Image.Where(p => p.AlbumID == albumID).ToList();
                    foreach (var item in img)
                    {
                        dbEntities.SQL_Image.Remove(item);
                    }
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {

                throw;
            }
        }
        public bool Delete_Single_Image(string imageID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    List<SQL_Image> img = dbEntities.SQL_Image.Where(p => p.ImageID == imageID).ToList();
                    foreach (var item in img)
                    {
                        dbEntities.SQL_Image.Remove(item);
                    }
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