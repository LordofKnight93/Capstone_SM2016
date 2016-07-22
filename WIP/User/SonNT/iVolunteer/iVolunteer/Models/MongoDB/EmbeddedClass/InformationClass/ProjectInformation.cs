using System;
using MongoDB.Bson;
using System.Collections.Generic;
using iVolunteer.Common;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ListClass;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store project's infomation
    /// </summary>
    [BsonIgnoreExtraElements]
    public class ProjectInformation
    {
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
        public DateTime DateCreate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateStart { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateEnd { get; set; }
        [MaxLength(150, ErrorMessage = "Độ dài không quá 150 ký tự")]
        public string ProjectShortDescription { get; set; }
        public string ProjectFullDescription { get; set; }
        public string Location { get; set; }
        public int MemberCount { get; set; }
        public int FollowerCount { get; set; }
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng!")]
        public string Email { get; set;}
        [RegularExpression(@"^[0-9]*$", ErrorMessage ="Chỉ chấp nhận ký tự số")]
        public string Phone { get; set; }
        public bool InProgress { get; set; }
        public bool IsRecruit { get; set; }
        public bool IsActivate { get; set; }


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
        }

        public string Get_AvatarLink()
        {
            return "/Images/Project/Avatar/" + this.ProjectID + ".jpg";
        }
    }
}