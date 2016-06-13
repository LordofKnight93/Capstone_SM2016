using System;
using MongoDB.Bson.Serialization.Attributes;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass
{
    /// <summary>
    /// This class is used to store simple information of a album, store in project and group.
    /// And used as model for view project/group gallery 
    /// </summary>
    [BsonIgnoreExtraElements]
    public class AlbumSD
    {
        public string AlbumID  { get; set; }
        public string AlbumName { get; set; }
        public string CoverImgLink { get; set; }
        public int ImageCount { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public bool Permisson { get; set; }

        public AlbumSD()
        {
            this.AlbumID = "";
            this.AlbumName = "";
            this.CoverImgLink = "";
            this.ImageCount = 0;
            this.CommentCount = 0;
            this.LikeCount = 0;
            this.Permisson = false;
        }
    }
}