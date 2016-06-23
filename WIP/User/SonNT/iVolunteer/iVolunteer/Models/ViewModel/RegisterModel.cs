using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;

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
        public string RealName { get; set; }
        [Required]
        [DisplayName("Giới tính")]
        public bool Gender { get; set; }
        [Required]
        [DisplayName("Ngày sinh")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; set; }
        [Required]
        [DisplayName("Số chứng minh thư")]
        [RegularExpression(@"^[0-9]*$")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public string IdentifyID { get; set; }
        [DisplayName("Địa chỉ")]
        public string Address { get; set; }
        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }
    }
}