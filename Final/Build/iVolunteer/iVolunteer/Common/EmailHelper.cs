using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Net;
using System.Web.Helpers;

namespace iVolunteer.Common
{
    public static class EmailHelper
    {
        /// <summary>
        /// アカウントの確認メールを放送
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="email"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static void SendActivationEmail(string displayName, string email, string userID, string actionLink)
        {
            MailMessage message = new MailMessage();
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            message.From = new MailAddress("ivolunteer.noreply@gmail.com");
            message.To.Add(email);
            message.Subject = "Xác nhận Email đăng kí tại cộng đồng ivolunteervn.com";
            message.Body = string.Format("Xin chào {0}, <br /> Cảm ơn bạn đã đăng kí thành viên cộng đồng iVolunteer, bạn hãy nhấn <a href = \"{1}\" title = \"Activate your account\">vào đây</a> để hoàn tất đăng kí!", displayName, actionLink);

            message.IsBodyHtml = true;
            client.EnableSsl = true;
            client.UseDefaultCredentials = true;
            client.Credentials = new System.Net.NetworkCredential("ivolunteer.noreply@gmail.com", "iv0lunt##r");
            client.Send(message);
        }

        /// <summary>
        /// パスワードの忘れメールを放送
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        public static void SendForgotPasswordEmail(string userID, string userName, string email, string newPassword)
        {

            MailMessage message = new MailMessage();
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            message.From = new MailAddress("account-security-noreply@ivolunteervn.com");
            message.To.Add(email);
            message.Subject = "Reset your password on iVolunteerVN.com";
            message.Body = string.Format("Hi {0}, <br /> We haved reset password for your account. <br /> Your new password is {1}", userName, newPassword);

            message.IsBodyHtml = true;
            client.EnableSsl = true;
            client.UseDefaultCredentials = true;
            client.Credentials = new System.Net.NetworkCredential("ivolunteer.noreply@gmail.com", "iv0lunt##r");
            client.Send(message);

        }
    }
}