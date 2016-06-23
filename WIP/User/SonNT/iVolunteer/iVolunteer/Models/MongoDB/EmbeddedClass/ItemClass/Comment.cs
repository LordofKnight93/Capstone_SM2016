using System;
using MongoDB.Bson;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    public class Comment
    {
        public string CommentID { get; set; }
        public SDLink Creater { get; set; }
        public string Content { get; set; }
        public DateTime DateCreate { get; set; }

        public Comment()
        {
            this.CommentID = "";
            this.Creater = new SDLink();
            this.Content = "";
            this.DateCreate = new DateTime();
        }
        public Comment(SDLink user, string content)
        {
            this.CommentID = ObjectId.GenerateNewId().ToString();
            this.Creater = user;
            this.Content = content;
            this.DateCreate = DateTime.Now;
        }
    }
}