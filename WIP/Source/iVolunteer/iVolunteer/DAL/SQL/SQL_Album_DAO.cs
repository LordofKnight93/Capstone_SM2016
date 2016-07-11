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
    }
}