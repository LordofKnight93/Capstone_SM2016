using System;
using MongoDB.Bson;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using MongoDB.Bson.Serialization.Attributes;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    [BsonIgnoreExtraElements]
    public class Comment
    {
        public string CommentID { get; set; }
        public SDLink Creator { get; set; }
        public string Content { get; set; }
        public DateTime DateCreate { get; set; }

        public Comment()
        {
            this.CommentID = "";
            this.Creator = new SDLink();
            this.Content = "";
            this.DateCreate = new DateTime();
        }
        public Comment(SDLink user, string content)
        {
            this.CommentID = ObjectId.GenerateNewId().ToString();
            this.Creator = user;
            this.Content = content;
            this.DateCreate = DateTime.Now;
        }
    }
}