using MongoDB.Bson;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using System.Collections.Generic;

namespace iVolunteer.Models.MongoDB.CollectionClass
{
    /// <summary>
    /// This class define structure of "Post" collection in MongoDB
    /// </summary>
    public class Mongo_Post
    {
        public ObjectId _id { get; set; }
        public PostInformation PostInfomation { get; set; }
        public List<SDLink> LikerList { get; set; }
        public List<string> FollowerIDs { get; set; }
        public List<Comment> CommentList { get; set; }

        public Mongo_Post()
        {
            this._id = new ObjectId();
            this.PostInfomation = new PostInformation();
            this.LikerList = new List<SDLink>();
            this.CommentList = new List<Comment>();
        }
    }
}