using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.IO;
using iVolunteer.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store group's infomation
    /// </summary>
    [BsonIgnoreExtraElements]
    public class GroupInformation 
    {
        public string GroupID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên nhóm!")]
        public string GroupName { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateCreate { get; set; }
        public string GroupDescription { get; set; }
        public int MemberCount { get; set; }
        [EmailAddress(ErrorMessage ="Địa chỉ email không đúng định dạng!")]
        public string Email { get; set; }
        [Phone(ErrorMessage ="Số điện thoại không đúng định dạng!")]
        public string Phone { get; set; }
        public bool IsActivate { get; set; }

        public GroupInformation()
        {
            this.GroupID = "";
            this.GroupName = "";
            this.DateCreate = new DateTime();
            this.GroupDescription = "";
            this.MemberCount = 0;
            this.IsActivate = Status.IS_ACTIVATE;
        }
        public string Get_AvatarLink()
        {
            return "/Images/Group/Avatar/" + this.GroupID + ".jpg";
        }
    }
}