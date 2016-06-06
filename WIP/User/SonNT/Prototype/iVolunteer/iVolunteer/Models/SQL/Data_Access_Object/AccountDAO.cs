using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL.DBContext;

namespace iVolunteer.Models.SQL.Data_Access_Object
{
    public static class AccountDAO
    {
        static iVolunteerEntities dbEntities = new iVolunteerEntities();
        // Add an account to DB
        public static void AddAccount(Account account)
        {
            
            dbEntities.Accounts.Add(account);
            dbEntities.SaveChanges();
        }
        //Get all accounts
        public static List<Account> GetAllAccounts()
        {
            List<Account> result = new List<Account>();
            result = dbEntities.Accounts.ToList<Account>();
            return result;
        }
        // Get an account by Email
        public static Account GetAccountByEmail(string email)
        {
            Account result = dbEntities.Accounts.FirstOrDefault(acc => acc.Email == email);
            return result;
        }
        // Get an account by UserID
        public static Account GetAccountByUserID(string userID)
        {
            Account result = dbEntities.Accounts.FirstOrDefault(acc => acc.UserID == userID);
            return result;
        }
        // Set activatie status for an account, true is activate, false is banned
        public static void SetActivate(string id, bool isActivate)
        {
            Account result = dbEntities.Accounts.FirstOrDefault(acc => acc.UserID == id);
            result.IsActivate = isActivate;
            dbEntities.SaveChanges();
        }
        // Set confirmation for an account, true is confirmed, false is not confirmed yet
        public static void SetConfirm(string id, bool isConfirm)
        {
            Account result = dbEntities.Accounts.FirstOrDefault(acc => acc.UserID == id);
            result.IsConfirm = isConfirm;
            dbEntities.SaveChanges();
        }

        public static bool IsIdentifyIDExisted(string identifyID)
        {
            bool result = false;
            if (dbEntities.Accounts.Any(acc => acc.IndentifyID == identifyID)) result = true;
            return result;
        }
        public static bool IsEmailExisted(string email)
        {
            bool result = false;
            if (dbEntities.Accounts.Any(acc => acc.Email == email)) result = true;
            return result;
        }
    }
}