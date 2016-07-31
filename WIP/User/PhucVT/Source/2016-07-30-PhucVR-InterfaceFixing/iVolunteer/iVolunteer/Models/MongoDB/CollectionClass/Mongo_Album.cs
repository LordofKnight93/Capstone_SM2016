using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;

namespace iVolunteer.Models.MongoDB.CollectionClass
{
    [BsonIgnoreExtraElements]
    public class Mongo_Album
    {
        public ObjectId _id { get; set; }
        public AlbumInformation AlbumInformation { get; set; }
        [BsonIgnoreIfDefault]
        public List<Comment> CommentList { get; set; }

        public Mongo_Album()
        {
            this._id = new ObjectId();
            this.AlbumInformation = new AlbumInformation();
            this.CommentList = new List<Comment>();
        }
    }
}