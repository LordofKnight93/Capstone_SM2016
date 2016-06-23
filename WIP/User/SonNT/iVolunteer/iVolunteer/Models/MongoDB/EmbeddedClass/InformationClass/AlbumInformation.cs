﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Common;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass
{
    public class AlbumInformation
    {
        public string AlbumID { get; set; }
        public string AlbumName { get; set; }
        public SDLink Creator { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateLastActivity { get; set; }
        public int ImageCount { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool Permisson { get; set; }

        public AlbumInformation()
        {
            this.AlbumID = "";
            this.AlbumName = "";
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