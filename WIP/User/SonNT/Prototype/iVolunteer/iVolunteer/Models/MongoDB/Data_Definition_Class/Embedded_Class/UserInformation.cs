using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using System.ComponentModel;

namespace iVolunteer.Models.MongoDB.Data_Definition_Class.Embedded_CLass
{
    public class UserInformation
    {
        [DisplayName("UserID")]
        public ObjectId _id { get; set; }
        [DisplayName("Họ và Tên")]
        public string UserName { get; set; }
        [DisplayName("Số chứng minh thư")]
        public string IndentifyID { get; set; }
        [DisplayName("Địa chỉ")]
        public string Address { get; set; }
        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }
    }
}