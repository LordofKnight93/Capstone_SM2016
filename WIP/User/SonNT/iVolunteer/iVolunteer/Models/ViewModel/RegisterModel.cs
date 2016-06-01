using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace iVolunteer.Models.ViewModel
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        [DisplayName("Email")]
        public string Email { get; set; }
        [Required]
        [PasswordPropertyText(true)]
        [DisplayName("Mật khẩu")]
        public string Password { get; set; }
        [Required]
        [PasswordPropertyText(true)]
        [DisplayName("Xác nhận mật khẩu")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        [DisplayName("Họ và Tên")]
        public string UserName { get; set; }
        [Required]
        [DisplayName("Số chứng minh thư")]
        public string IndentifyID { get; set; }
        [DisplayName("Địa chỉ")]
        public string Address { get; set; }
        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }
    }
}