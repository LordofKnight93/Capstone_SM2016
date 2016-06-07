using System;
using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store post's infomation
    /// </summary>
    public class PostInformation
    {
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
    }
}