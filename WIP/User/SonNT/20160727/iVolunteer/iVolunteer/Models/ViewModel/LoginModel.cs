using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace iVolunteer.Models.ViewModel
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        [DisplayName("Email")]
        public string Email { get; set; }
        [Required]
        [PasswordPropertyText(true)]
        [DisplayName("Mật khẩu")]
        public string Password { get; set; }
        [DisplayName("Tự động đăng nhập")]
        public bool IsRemember { get; set; }
    }
}