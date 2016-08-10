using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Common;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass
{
    [BsonIgnoreExtraElements]
    public class AlbumInformation
    {
        public string AlbumID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên Album")]
        public string AlbumName { get; set; }
        public string TargetID { get; set; }
        public string AlbumDescription { get; set; }
        public SDLink Creator { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateCreate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateLastActivity { get; set; }
        public int ImageCount { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool Permisson { get; set; }
        public string ImgLink { get; set; }

        public AlbumInformation()
        {
            this.AlbumID = "";
            this.AlbumName = "";
            this.TargetID = "";
            this.ImgLink = "";
            this.Creator = new SDLink();
            this.DateCreate = new DateTime();
            this.DateLastActivity = new DateTime();
            this.ImageCount = 0;
            this.LikeCount = 0;
            this.CommentCount = 0;
            this.Permisson = Status.IS_PRIVATE;
        }
    }
}