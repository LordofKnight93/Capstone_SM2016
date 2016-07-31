using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace iVolunteer.Models.ViewModel
{
    public class ChangePasswordModel
    {
        [Required]
        [PasswordPropertyText(true)]
        [DisplayName("Mật khẩu cũ")]
        public string OldPassword { get; set; }
        [Required]
        [PasswordPropertyText(true)]
        [DisplayName("Mật khẩu mới")]
        public string NewPassword { get; set; }
        [Required]
        [PasswordPropertyText(true)]
        [DisplayName("Xác nhận mật khẩu mới")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận và mật khẩu mới không giống nhau!")]
        public string ConfirmNewPassword { get; set; }
    }
}