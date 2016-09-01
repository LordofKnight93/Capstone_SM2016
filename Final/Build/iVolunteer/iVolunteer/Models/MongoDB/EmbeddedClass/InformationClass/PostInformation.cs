using System;
using MongoDB.Bson;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Common;
using MongoDB.Bson.Serialization.Attributes;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store post's infomation
    /// </summary>
    [BsonIgnoreExtraElements]
    public class PostInformation
    {
        public string PostID { get; set; }
        public SDLink Creator { get; set; }
        public SDLink Destination { get; set; }
        [BsonIgnoreIfDefault]
        public DateTime DateCreate { get; set; }
        public DateTime DateLastActivity { get; set; }
        public int Type { get; set; }
        [BsonIgnoreIfDefault]
        public string Content { get; set; }
        [BsonIgnoreIfDefault]
        public string ImgLink { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsPinned { get; set; }
        public bool IsPublic { get; set; }

        public PostInformation()
        {
            this.PostID = "";
            this.Creator = new SDLink();
            this.Destination = new SDLink();
            this.DateCreate = new DateTime();
            this.DateLastActivity = new DateTime();
            this.Type = 0;
            this.Content = "";
            this.ImgLink = "";
            this.LikeCount = 0;
            this.CommentCount = 0;
            this.IsPinned = Status.IS_NOT_PINNED;
            this.IsPublic = Status.IS_PRIVATE;
        }
    }
}