using System;
using MongoDB.Bson;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass
{
    /// <summary>
    /// This class is used to store simple information of a album, store in project and group.
    /// And used as model for view project/group gallery 
    /// </summary>
    public class AlbumSD
    {
        public string AlbumID  { get; set; }
        public string Name { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateLastActivity { get; set; }
        public string CoverImgLink { get; set; }
        public int ImageCount { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool Permisson { get; set; }

        public AlbumSD()
        {
            this.AlbumID = "";
            this.Name = "";
            this.DateCreate = new DateTime();
            this.DateLastActivity = new DateTime();
            this.CoverImgLink = "";
            this.ImageCount = 0;
            this.CommentCount = 0;
            this.LikeCount = 0;
            this.Permisson = false;
        }
    }
}