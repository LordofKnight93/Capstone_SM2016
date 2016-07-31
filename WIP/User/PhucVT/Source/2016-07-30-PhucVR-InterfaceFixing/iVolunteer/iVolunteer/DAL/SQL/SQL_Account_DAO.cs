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
                    var result = dbEntities.SQL_Account.FirstOrDefault(acc => acc.Email == email);
                    return result;
                }
            }
            catch
            {
                throw;
            }

        }
        
        /// <summary>
        /// check ifa string is correct password
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Is_Password_Match(string userID, string password)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Account.FirstOrDefault(acc => acc.UserID == userID);
                    return result.Password == password;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// deactivte an account
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Deactive(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Account.FirstOrDefault(acc => acc.UserID == userID
                                                                            && acc.IsActivate == Status.IS_ACTIVATE);
                    if (result != null)
                    {
                        result.IsActivate = Status.IS_BANNED;
                        dbEntities.SaveChanges();
                        return true;
                    } 
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// activate an account
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool Activate(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Account.FirstOrDefault(acc => acc.UserID == userID
                                                                            && acc.IsActivate == Status.IS_BANNED);
                    if (result != null)
                    {
                        result.IsActivate = Status.IS_ACTIVATE;
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// confirm an account
        /// </summary>
        /// <param name="userID"> userID of account want to set</param>
        /// <param name="status">status, get in Constant</param>
        /// <returns>true if success</returns>
        public bool Confirmed(string userID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_Account.FirstOrDefault(acc => acc.UserID == userID
                                                                            && acc.IsConfirm == Status.IS_NOT_CONFIRMED);
                    if (result != null)
                    {
                        result.IsConfirm = Status.IS_CONFIRMED;
                        dbEntities.SaveChanges();
                        return true;
                    }
                    return false;
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
        /// <param name="newPassword">new password, should be encrypted</param>
        /// <returns>true if success</returns>
        public bool Set_Password(string userID, string newPassword)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var account = dbEntities.SQL_Account.FirstOrDefault(acc => acc.UserID == userID);
                    account.Password = newPassword;
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
        /// check if an identifyID exist in system, only activate account
        /// </summary>
        /// <param name="identifyID"></param>
        /// <returns></returns>
        public bool Is_IdentifyID_Exist(string identifyID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var account = dbEntities.SQL_Account.FirstOrDefault(acc => acc.IndentifyID == identifyID
                                                                            && acc.IsActivate == Status.IS_ACTIVATE);
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