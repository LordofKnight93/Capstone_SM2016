using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace iVolunteer.Common
{
    public static class ValidationHelper
    {
        private static Regex PASSWORD_PATTERN = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$");
        private static Regex NUMBERONLY_PATTERN = new Regex(@"^[0-9]*$");
        /// <summary>
        /// check valid email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// is valid identify ID, must bu only numer and 9 characters
        /// </summary>
        /// <param name="identifyID"></param>
        /// <returns>true if valid</returns>
        public static bool IsValidIdentifyID(string identifyID)
        {
            if (String.IsNullOrEmpty(identifyID)) return false;
            else
                return NUMBERONLY_PATTERN.IsMatch(identifyID) && identifyID.Length == 9;
        }
        /// <summary>
        /// check valid phone, only contain number
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static bool IsValidPhone(string phone)
        {
            if (String.IsNullOrEmpty(phone)) return true;
            else
                return NUMBERONLY_PATTERN.IsMatch(phone);
        }
        /// <summary>
        /// check valid password, con tain atleast 1 upper, 1 lower, 1 number, 8-15 characters
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsValidPassword(string password)
        {
            if (String.IsNullOrEmpty(password)) return false;
            else
                return PASSWORD_PATTERN.IsMatch(password);
        }
        
        /// <summary>
        /// check valid deline, the dateline must be in furture
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsValidDeadline(DateTime date)
        {
            if (date < DateTime.Today) return false;
            else return true;
        }
        
    }
}