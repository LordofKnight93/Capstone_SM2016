using System;
using MongoDB.Bson;
using System.Collections.Generic;
using iVolunteer.Common;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ListClass;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;
using Foolproof;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass
{
    public class DateEndAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime d = Convert.ToDateTime(value);
            return d > DateTime.Now;
        }
    }
    /// <summary>
    /// This class store project's infomation
    /// </summary>
    /// 
    [BsonIgnoreExtraElements]
    public class ProjectInformation
    {
        public string ProjectID { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập tên dự án!")]
        public string ProjectName { get; set; }
        public DateTime DateCreate { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập ngày bắt đầu!")]
        [BsonDateTimeOptions(DateOnly = true, Kind = DateTimeKind.Local)]
        [DataType(DataType.Date, ErrorMessage = "Ngày bạn nhập không hợp lệ!")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateStart { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập ngày kết thúc!")]
        [BsonDateTimeOptions(DateOnly = true, Kind = DateTimeKind.Local)]
        [DataType(DataType.Date, ErrorMessage = "Ngày bạn nhập không hợp lệ!")]
        [GreaterThanOrEqualTo("DateStart",ErrorMessage ="Ngày kết thúc không thể sớm hơn ngày bắt đầu!")]
        [DateEnd(ErrorMessage = "Ngày kết thúc không thể sớm hơn hôm nay!")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateEnd { get; set; }
        [MaxLength(100, ErrorMessage = "Độ dài không quá 100 ký tự!")]
        [Required(ErrorMessage = "Vui lòng nhập mô tả!")]
        public string ProjectShortDescription { get; set; }
        public string ProjectFullDescription { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập địa điểm!")]
        public string Location { get; set; }
        public int MemberCount { get; set; }
        public int FollowerCount { get; set; }
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng!")]
        public string Email { get; set;}
        [Phone(ErrorMessage ="Số điện thoại không đúng định dạng!")]
        public string Phone { get; set; }
        public bool InProgress { get; set; }
        public bool IsRecruit { get; set; }
        public bool IsActivate { get; set; }
        public string TagsString { get; set; }

        public ProjectInformation()
        {
            this.ProjectID = "";
            this.ProjectName = "";
            this.DateCreate = new DateTime();
            this.DateStart = new DateTime();
            this.DateEnd = new DateTime();
            this.ProjectShortDescription = "";
            this.Location = "";
            this.MemberCount = 0;
            this.FollowerCount = 0;
            this.IsRecruit = Status.IS_RECRUITING;
            this.IsActivate = Status.IS_ACTIVATE;
            this.TagsString = "";
        }

        public string Get_AvatarLink()
        {
            return "/Images/Project/Avatar/" + this.ProjectID + ".jpg";
        }

        public string Get_CoverLink()
        {
            return "/Images/Project/Cover/" + this.ProjectID + ".jpg";
        }
    }
}