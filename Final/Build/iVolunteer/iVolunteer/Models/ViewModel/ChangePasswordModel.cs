using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using iVolunteer.Common;

namespace iVolunteer.Models.ViewModel
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu cũ!")]
        [PasswordPropertyText(true)]
        [DisplayName("Mật khẩu cũ")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới!")]
        [PasswordPropertyText(true)]
        [RegularExpression(@"(^[A-Z](?=.*[0-9])(?=.*[a-z])|^[a-z](?=.*[A-Z])(?=.*[0-9])|^[0-9](?=.*[a-z])(?=.*[A-Z]))[a-zA-Z0-9]{7,14}$", ErrorMessage = Error.PASSWORD_INVALID)]
        [DisplayName("Mật khẩu mới")]
        public string NewPassword { get; set; }
        [PasswordPropertyText(true)]
        [DisplayName("Xác nhận mật khẩu mới")]
        [Required(ErrorMessage = "Vui lòng nhập xác nhận mật khẩu mới!")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận và mật khẩu mới không giống nhau!")]
        public string ConfirmNewPassword { get; set; }
    }
}