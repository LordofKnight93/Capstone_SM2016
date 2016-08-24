using System;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace iVolunteer.Common
{
    public static class HelperExtensions
    {
        /// <summary>
        /// Customize AjaxActionLink
        /// AjaxActionLInkをカスタマイズ
        /// </summary>
        /// <param name="ajaxHelper"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <param name="ajaxOptions"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString RawActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            var repID = Guid.NewGuid().ToString();
            var lnk = ajaxHelper.ActionLink(repID, actionName, controllerName, routeValues, ajaxOptions, htmlAttributes);
            return MvcHtmlString.Create(lnk.ToString().Replace(repID, linkText));
        }

        public static string DaySpan(DateTime date1, DateTime date2)
        {
            TimeSpan span = date1 - date2;
            int monthGap = (int) (span.TotalDays / 30);
            int dateGap = (int) span.TotalDays;
            int hourGap = (int) span.TotalHours;
            int minuteGap = (int) span.TotalMinutes;
            int seconsGap = (int) span.TotalSeconds;

            if (dateGap > 30 && monthGap < 12)
            {
                return monthGap + " tháng trước";
            }
            else if (hourGap > 24 && dateGap < 30)
            {
                return dateGap + " ngày trước";
            }
            else if (minuteGap > 60 && hourGap < 24)
            {
                return hourGap + " giờ trước";
            }
            else if (seconsGap > 60 && minuteGap < 60)
            {
                return minuteGap + " phút trước";
            }
            else if (seconsGap < 60 && seconsGap > 0)
            {
                return seconsGap + " giây trước";
            }
            else return date1.ToString("HH:mm dd/MM/yyyy");
            
        }
    }
}