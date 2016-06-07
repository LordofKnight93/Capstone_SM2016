using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.SQL;
using iVolunteer.Common;

namespace iVolunteer.Models.Data_Access_Object.SQL
{
    public static class SQL_Account_DAO
    {
        static iVolunteerEntities dbEntities = new iVolunteerEntities();
        // Add an account to DB
        public static bool Add_Account(SQL_Account account)
        {
            try
            {
                dbEntities.SQL_Account.Add(account);
                dbEntities.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public static List<SQL_Account> Get_All_Account()
        {
            try
            {
                var result = dbEntities.SQL_Account.ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }

        public static SQL_Account Get_Account_By_Email(string email)
        {
            try
            {
                var result = dbEntities.SQL_Account.Where(acc => acc.Email == email && acc.IsActivate == Constant.IS_ACTIVATE).FirstOrDefault();
                return result;
            }
            catch
            {
                throw;
            }

        }

        public static bool Set_Activation(string userID, bool status)
        {
            try
            {
                var account = dbEntities.SQL_Account.FirstOrDefault(acc => acc.UserID == userID);
                account.IsActivate = status;
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Set_Confirmation(string userID, bool status)
        {
            try
            {
                var account = dbEntities.SQL_Account.FirstOrDefault(acc => acc.UserID == userID);
                account.IsConfirm = status;
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Set_Password(string userID, string password)
        {
            try
            {
                var account = dbEntities.SQL_Account.FirstOrDefault(acc => acc.UserID == userID);
                account.Password = password;
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Set_Avatar(string userID, string imgLink)
        {
            try
            {
                var account = dbEntities.SQL_Account.FirstOrDefault(acc => acc.UserID == userID);
                account.AvtImgLink = imgLink;
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Set_DisplayName(string userID, string displayname)
        {
            try
            {
                var account = dbEntities.SQL_Account.FirstOrDefault(acc => acc.UserID == userID);
                account.DisplayName = displayname;
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsActivate(string userID)
        {
            try
            {
                var account = dbEntities.SQL_Account.FirstOrDefault(acc => acc.UserID == userID);
                return account.IsActivate;
            }
            catch
            {
                return false;
            }
        }

    }
}