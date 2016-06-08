using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.SQL;

namespace iVolunteer.Models.Data_Access_Object.SQL
{
    public static class SQL_Project_DAO
    {
        static iVolunteerEntities dbEntitied = new iVolunteerEntities();
        public static bool Add_Project(SQL_Project project)
        {
            try
            {
                dbEntitied.SQL_Project.Add(project);
                dbEntitied.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}