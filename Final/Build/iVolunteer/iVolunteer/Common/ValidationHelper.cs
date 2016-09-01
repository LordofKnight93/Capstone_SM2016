using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace iVolunteer.Common
{
    public class DateEndAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime d = Convert.ToDateTime(value);
            return d > DateTime.Now;
        }
    }
    public static class ValidationHelper
    {
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
        /// check valid deadline, the deadline must be in furture
        /// 締め切りを検証、締め切りは必ず将来にある価値
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsValidDeadlineWithToday(DateTime date)
        {
            if (date < DateTime.Today) return false;
            else return true;
        }

        /// <summary>
        /// check validate between two date
        /// 期間内を検証
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static bool IsValidDeadlineTwoDate(DateTime date1, DateTime date2)
        {
            if (date1 > date2) return false;
            else return true;
        }
    }
}