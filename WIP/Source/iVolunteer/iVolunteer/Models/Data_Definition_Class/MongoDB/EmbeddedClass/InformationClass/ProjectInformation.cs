using System;
using MongoDB.Bson;
using System.Collections.Generic;
using iVolunteer.Common;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store project's infomation
    /// </summary>
    public class ProjectInformation
    {
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string ProjectDescription { get; set; }
        public List<string> Tags { get; set; }
        public string Location { get; set; }
        public int MemberCount { get; set; }
        public int FollowerCount { get; set; }
        public string AvtImgLink { get; set; }
        public string CoverImgLink { get; set; }
        public bool IsRecruit { get; set; }
        public bool IsActivate { get; set; }

        public ProjectInformation()
        {
            this.ProjectID = "";
            this.ProjectName = "";
            this.DateCreate = new DateTime();
            this.DateStart = new DateTime();
            this.DateEnd = new DateTime();
            this.ProjectDescription = "";
            this.Tags = new List<string>();
            this.Location = "";
            this.MemberCount = 0;
            this.FollowerCount = 0;
            this.AvtImgLink = Constant.DEFAULT_AVATAR;
            this.CoverImgLink = Constant.DEFAULT_COVER;
            this.IsRecruit = Constant.IS_RECRUITING;
            this.IsActivate = Constant.IS_ACTIVATE;
        }
    }
}