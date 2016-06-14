using System;
using MongoDB.Bson;
using iVolunteer.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store group's infomation
    /// </summary>
    public class GroupInformation
    {
        public string GroupID { get; set; }
        [DisplayName("Tên nhóm")]
        public string GroupName { get; set; }
        [DisplayName("Ngày tạo")]
        public DateTime DateCreate { get; set; }
        [DisplayName("Mô tả")]
        public string GroupDescription { get; set; }
        [DisplayName("Số thành viên")]
        public int MemberCount { get; set; }
        public string AvtImgLink { get; set; }
        public string CoverImgLink { get; set; }
        public bool IsActivate { get; set; }

        public GroupInformation()
        {
            this.GroupID = "";
            this.GroupName = "";
            this.DateCreate = new DateTime();
            this.GroupDescription = "";
            this.MemberCount = 0;
            this.AvtImgLink = Constant.DEFAULT_AVATAR;
            this.CoverImgLink = Constant.DEFAULT_COVER;
            this.IsActivate = Constant.IS_ACTIVATE;
        }
    }
}