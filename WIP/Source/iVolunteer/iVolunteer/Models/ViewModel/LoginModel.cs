﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace iVolunteer.Models.ViewModel
{
    public class LoginModel
    {
        [Required(ErrorMessage ="Vui lòng nhập email!")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng!")]
        [DisplayName("Email")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập mật khẩu!")]
        [PasswordPropertyText(true)]
        [DisplayName("Mật khẩu")]
        public string Password { get; set; }
        [DisplayName("Tự động đăng nhập")]
        public bool IsRemember { get; set; }
    }
}