using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;

namespace iVolunteer.Models.MongoDB.CollectionClass
{
    [BsonIgnoreExtraElements]
    public class Mongo_Album
    {
        public ObjectId _id { get; set; }
        public AlbumInformation AlbumInformation { get; set; }
        public List<Mongo_Image> ImageList { get; set; }
        [BsonIgnoreIfDefault]
        public List<SDLink> LikerList { get; set; }
        [BsonIgnoreIfDefault]
        public List<Comment> CommentList { get; set; }

        public Mongo_Album()
        {
            this._id = new ObjectId();
            this.AlbumInformation = new AlbumInformation();
            this.LikerList = new List<SDLink>();
            this.CommentList = new List<Comment>();
            this.ImageList = new List<Mongo_Image>();
        }
        public Mongo_Album(AlbumInformation albumInfor)
        {
            this._id = ObjectId.GenerateNewId();
            this.AlbumInformation = albumInfor;
            this.ImageList = new List<Mongo_Image>();
            this.AlbumInformation.AlbumID = this._id.ToString();
            this.LikerList = new List<SDLink>();
            this.CommentList = new List<Comment>();
        }
    }
}