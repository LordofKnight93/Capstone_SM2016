using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL;
using iVolunteer.Common;

namespace iVolunteer.DAL.SQL
{
    public class SQL_Account_DAO
    {
        /// <summary>
        /// Add new account to SQL DB
        /// </summary>
        /// <param name="account">SQL_Account instance</param>
        /// <returns>true if add success</returns>
        public bool Add_Account(SQL_Account account)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    dbEntities.SQL_Account.Add(account);
                    dbEntities.SaveChanges();
                    return true;
                }
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
        public List<SQL_Account> Get_All_Account()
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Account.ToList();
                    return result;
                }
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
        public SQL_Account Get_Account_By_Email(string email)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Account.Where(acc => acc.Email == email).First();
                    return result;
                }
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
        public bool Set_Activation_Status(string userID, bool status)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var account = dbEntities.SQL_Account.First(acc => acc.UserID == userID);
                    account.IsActivate = status;
                    dbEntities.SaveChanges();
                    return true;
                }
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
        public bool Set_Confirmation_Status(string userID, bool status)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var account = dbEntities.SQL_Account.First(acc => acc.UserID == userID);
                    account.IsConfirm = status;
                    dbEntities.SaveChanges();
                    return true;
                }
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
        public bool Set_Password(string userID, string password)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var account = dbEntities.SQL_Account.FirstOrDefault(acc => acc.UserID == userID);
                    account.Password = password;
                    dbEntities.SaveChanges();
                    return true;
                }
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
        public bool Set_DisplayName(string userID, string displayname)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var account = dbEntities.SQL_Account.FirstOrDefault(acc => acc.UserID == userID);
                    account.DisplayName = displayname;
                    dbEntities.SaveChanges();
                    return true;
                }
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
        public bool IsActivate(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var account = dbEntities.SQL_Account.FirstOrDefault(acc => acc.UserID == userID);
                    return account.IsActivate;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// check if an email exist in system
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool Is_Email_Exist(string email)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var account = dbEntities.SQL_Account.FirstOrDefault(acc => acc.Email == email);
                    return account != null;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// check if an identifyID exist in system
        /// </summary>
        /// <param name="identifyID"></param>
        /// <returns></returns>
        public bool Is_IdentifyID_Exist(string identifyID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var account = dbEntities.SQL_Account.FirstOrDefault(acc => acc.IndentifyID == identifyID);
                    return account != null;
                }
            }
            catch
            {
                throw;
            }
        }

    }
}