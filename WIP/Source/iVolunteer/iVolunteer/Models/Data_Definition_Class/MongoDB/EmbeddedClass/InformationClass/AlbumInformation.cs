using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass
{
    public class AlbumInformation
    {
        public string AlbumID { get; set; }
        public string AlbumName { get; set; }
        public UserSD Uploader { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateLastActivity { get; set; }
        public string CoverImgLink { get; set; }
        public int ImageCount { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool Permisson { get; set; }
    }
}