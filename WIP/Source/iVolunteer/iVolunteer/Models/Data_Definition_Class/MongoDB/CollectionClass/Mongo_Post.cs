using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;
using System.Collections.Generic;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass
{
    /// <summary>
    /// This class define structure of "Post" collection in MongoDB
    /// </summary>
    public class Mongo_Post
    {
        public ObjectId _id { get; set; }
        public PostInformation PostInfomation { get; set; }
        public List<UserSD> LikerList { get; set; }
        public string[] FollowerIDs { get; set; }
        public List<Comment> CommentList { get; set; }

        public Mongo_Post()
        {
            this._id = new ObjectId();
            this.PostInfomation = new PostInformation();
            this.LikerList = new List<UserSD>();
            this.CommentList = new List<Comment>();
        }
    }
}