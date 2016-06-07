using System;
using MongoDB.Bson;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store project's infomation
    /// </summary>
    public class ProjectInformation
    {
        public string ProjectName { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string ProjectDescription { get; set; }
        public string[] Tags { get; set; }
        public string Location { get; set; }
        public int MemberCount { get; set; }
        public int FollowerCount { get; set; }
        public string AvtImgLink { get; set; }
        public string CoverImgLink { get; set; }
        public bool IsRecruit { get; set; }
        public bool IsActivate { get; set; }
    }
}