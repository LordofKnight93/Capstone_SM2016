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
        //Email
        [Required(ErrorMessage = "Vui lòng nhập email!")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng!")]
        [DisplayName("Email")]
        public string Email { get; set; }

        //Mật khẩu
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
        [PasswordPropertyText(true)]
        [DisplayName("Mật khẩu")]
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
        [Required(ErrorMessage = "Vui lòng nhập nhập ngày sinh!")]
        [DisplayName("Ngày sinh")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; set; }

        //Số CMT
        [Required(ErrorMessage = "Vui lòng nhập số chứng minh thư!")]
        [DisplayName("Số chứng minh thư")]
        [RegularExpression(@"^[0-9]*$",ErrorMessage ="Chỉ chấp nhận số và độ dài 9 hoặc 12 ký tự!")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public string IdentifyID { get; set; }

        //Địa chỉ
        [DisplayName("Địa chỉ")]
        public string Address { get; set; }

        //Số điện thoại
        [DisplayName("Điện thoại")]
        [RegularExpression(@"^[0-9]*$",ErrorMessage ="Chỉ chấp nhận ký tự số!")]
        public string Phone { get; set; }
    }
}