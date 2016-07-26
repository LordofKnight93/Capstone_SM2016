using MongoDB.Bson;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
namespace iVolunteer.Models.MongoDB.CollectionClass
{
    public class Mongo_Image
    {
        public ObjectId _id { get; set; }
        public ImageInformation ImageInformation { get; set; }
        [BsonIgnoreIfDefault]
        public List<SDLink> LikerList { get; set; }
        [BsonIgnoreIfDefault]
        public List<Comment> CommentList { get; set; }

        public Mongo_Image()
        {
            this._id = new ObjectId();
            this.ImageInformation = new ImageInformation();
            this.LikerList = new List<SDLink>();
            this.CommentList = new List<Comment>();
        }
    }
}