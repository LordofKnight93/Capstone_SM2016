using System;
using MongoDB.Bson;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Common;
using MongoDB.Bson.Serialization.Attributes;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass
{
    public class ImageInformation
    {
        public string ImageID { get; set; }
        public SDLink Creator { get; set; }
        public SDLink Album { get; set;}
        [BsonIgnoreIfDefault]
        public DateTime DateCreate { get; set; }
        public DateTime DateLastActivity { get; set; }
        [BsonIgnoreIfDefault]
        public string Content { get; set; }
        [BsonIgnoreIfDefault]
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }

        public ImageInformation()
        {
            this.ImageID = "";
            this.Creator = new SDLink();
            this.Album = new SDLink();
            this.DateCreate = new DateTime();
            this.DateLastActivity = new DateTime();
            this.Content = "";
            this.LikeCount = 0;
            this.CommentCount = 0;
        }
    }
}