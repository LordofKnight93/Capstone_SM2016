using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.SQL;

namespace iVolunteer.Models.Data_Access_Object.SQL
{
    public static class SQL_Group_DAO
    {
        static iVolunteerEntities dbEntitied = new iVolunteerEntities();
        public static bool Add_Group(SQL_Group group)
        {
            try
            {
                dbEntitied.SQL_Group.Add(group);
                dbEntitied.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}