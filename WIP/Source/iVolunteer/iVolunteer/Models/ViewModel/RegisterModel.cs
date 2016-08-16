using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Common;

namespace iVolunteer.Models.ViewModel
{
    public class BirthdayAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime d = Convert.ToDateTime(value);
            return d <= DateTime.Now;
        }
    }
    public class RegisterModel
    {
        //Email
        [Required(ErrorMessage = "Vui lòng nhập email!")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng!")]
        [DisplayName("Email")]
        public string Email { get; set; }

        //Mật khẩu
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
        [PasswordPropertyText(true)]
        [DisplayName("Mật khẩu")]
        [RegularExpression(@"(^[A-Z](?=.*[0-9])(?=.*[a-z])|^[a-z](?=.*[A-Z])(?=.*[0-9])|^[0-9](?=.*[a-z])(?=.*[A-Z]))[a-zA-Z0-9]{7,14}$", ErrorMessage = Error.PASSWORD_INVALID)]
        public string Password { get; set; }

        //Xác nhận mật khẩu
        [Required(ErrorMessage = "Vui lòng nhập xác nhận mật khẩu!")]
        [PasswordPropertyText(true)]
        [DisplayName("Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận và mật khẩu không giống nhau!")]
        public string ConfirmPassword { get; set; }

        //Họ và Tên
        [Required(ErrorMessage = "Vui lòng nhập họ và tên!")]
        [DisplayName("Họ và Tên")]
        public string RealName { get; set; }

        //Giới tính
        [Required(ErrorMessage = "Vui lòng nhập chọn giới tính!")]
        [DisplayName("Giới tính")]
        public bool Gender { get; set; }

        //Ngày sinh
        [Required(ErrorMessage = "Vui lòng nhập ngày sinh!")]
        [DisplayName("Ngày sinh")]
        [DataType(DataType.Date, ErrorMessage = "Ngày sinh bạn nhập không hợp lệ!")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Birthday(ErrorMessage = "Ngày sinh bạn nhập không hợp lệ!")]
        public DateTime Birthday { get; set; }

        //Số CMT
        [Required(ErrorMessage = "Vui lòng nhập số chứng minh thư!")]
        [DisplayName("Số chứng minh thư")]
        [RegularExpression(@"^(([0-9]{12})|([0-9]{9}))$", ErrorMessage ="Chỉ chấp nhận số và độ dài 9 hoặc 12 ký tự!")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public string IdentifyID { get; set; }

        //Địa chỉ
        [DisplayName("Địa chỉ")]
        public string Address { get; set; }

        //Số điện thoại
        [DisplayName("Điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string Phone { get; set; }
    }
}