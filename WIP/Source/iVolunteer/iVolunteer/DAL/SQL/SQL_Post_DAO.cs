using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL;
using iVolunteer.Common;

namespace iVolunteer.DAL.SQL
{
    public class SQL_Post_DAO
    {
        /// <summary>
        /// add new post to SQL DB
        /// </summary>
        /// <param name="post"></param>
        /// <returns>true if success</returns>
        public bool Add_Post(SQL_Post post)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    dbEntities.SQL_Post.Add(post);
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