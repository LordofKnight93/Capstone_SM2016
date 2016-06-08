using System;
using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass
{
    public class Comment
    {
        public string CommentID { get; set; }
        public UserSD Creater { get; set; }
        public string Content { get; set; }
        public DateTime DateCreate { get; set; }

        public Comment()
        {
            this.CommentID = "";
            this.Creater = new UserSD();
            this.Content = "";
            this.DateCreate = new DateTime();
        }
        public Comment(UserSD user, string content)
        {
            this.CommentID = ObjectId.GenerateNewId().ToString();
            this.Creater = user;
            this.Content = content;
            this.DateCreate = DateTime.Now;
        }
    }
}