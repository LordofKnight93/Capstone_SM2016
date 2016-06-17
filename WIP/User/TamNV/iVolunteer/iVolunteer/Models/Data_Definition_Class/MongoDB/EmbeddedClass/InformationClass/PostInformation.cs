using System;
using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using iVolunteer.Common;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store post's infomation
    /// </summary>
    public class PostInformation
    {
        public string PostID { get; set; }
        public UserSD Creator { get; set; }
        public GroupLink GroupLink { get; set; }
        public ProjectLink ProjectLink { get; set; }
        public AlbumLink AlbumLink { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateLastActivity { get; set; }
        public int PostType { get; set; }
        public string Content { get; set; }
        public string ImgLink { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsPinned { get; set; }
        public bool IsPublic { get; set; }

        public PostInformation()
        {
            this.PostID = "";
            this.Creator = new UserSD();
            this.GroupLink = new GroupLink();
            this.ProjectLink = new ProjectLink();
            this.AlbumLink = new AlbumLink();
            this.DateCreate = new DateTime();
            this.DateLastActivity = new DateTime();
            this.PostType = 0;
            this.Content = "";
            this.ImgLink = "";
            this.LikeCount = 0;
            this.CommentCount = 0;
            this.IsPinned = Constant.IS_PINNED;
            this.IsPublic = Constant.IS_PRIVATE;
        }
    }
}