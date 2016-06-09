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
        /// <summary>
        /// Add new account to SQL DB
        /// </summary>
        /// <param name="account">SQL_Account instance</param>
        /// <returns>true if add success</returns>
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

                throw;
            }
        }
        /// <summary>
        /// Get all account in system with no condition, used by admin only, 
        /// </summary>
        /// <returns>A list of SQL_Account</returns>
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
        /// <summary>
        /// Get an account by email
        /// </summary>
        /// <param name="email">account email</param>
        /// <returns> SQL_Account instance that have the same email value as nput</returns>
        public static SQL_Account Get_Account_By_Email(string email)
        {
            try
            {
                var result = dbEntities.SQL_Account.Where(acc => acc.Email == email).First();
                return result;
            }
            catch
            {
                throw;
            }

        }
        /// <summary>
        /// Set activation status of an account
        /// </summary>
        /// <param name="userID">userID of account want to set</param>
        /// <param name="status">status, get in Constant</param>
        /// <returns>true if success</returns>
        public static bool Set_Activation_Status(string userID, bool status)
        {
            try
            {
                var account = dbEntities.SQL_Account.First(acc => acc.UserID == userID);
                account.IsActivate = status;
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Set confirmation status of an account
        /// </summary>
        /// <param name="userID"> userID of account want to set</param>
        /// <param name="status">status, get in Constant</param>
        /// <returns>true if success</returns>
        public static bool Set_Confirmation_Status(string userID, bool status)
        {
            try
            {
                var account = dbEntities.SQL_Account.First(acc => acc.UserID == userID);
                account.IsConfirm = status;
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Set password of an account
        /// </summary>
        /// <param name="userID">userID of account want to set</param>
        /// <param name="password">new password, should be encrypted</param>
        /// <returns>true if success</returns>
        public static bool Set_Password(string userID, string password)
        {
            try
            {
                var account = dbEntities.SQL_Account.First(acc => acc.UserID == userID);
                account.Password = password;
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Set avatar link to an account
        /// </summary>
        /// <param name="userID">userID of account want to set</param>
        /// <param name="imgLink">new image link</param>
        /// <returns>true if success</returns>
        public static bool Set_Avatar(string userID, string imgLink)
        {
            try
            {
                var account = dbEntities.SQL_Account.First(acc => acc.UserID == userID);
                account.AvtImgLink = imgLink;
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Set display name of an account
        /// </summary>
        /// <param name="userID">userID of account want to set</param>
        /// <param name="displayname">new display name</param>
        /// <returns> true if success </returns>
        public static bool Set_DisplayName(string userID, string displayname)
        {
            try
            {
                var account = dbEntities.SQL_Account.First(acc => acc.UserID == userID);
                account.DisplayName = displayname;
                dbEntities.SaveChanges();
                return true;
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Check if a userID is activate
        /// </summary>
        /// <param name="userID">userID of account want to check</param>
        /// <returns>activation status of account to compare with Constant</returns>
        public static bool IsActivate(string userID)
        {
            try
            {
                var account = dbEntities.SQL_Account.First(acc => acc.UserID == userID);
                return account.IsActivate;
            }
            catch
            {
                throw;
            }
        }

    }
}