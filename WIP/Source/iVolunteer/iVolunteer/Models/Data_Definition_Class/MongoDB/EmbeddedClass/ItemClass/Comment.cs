using System;
using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass
{
    public class Comment
    {
        public ObjectId _id { get; set; }
        public UserSD Creater { get; set; }
        public string Content { get; set; }
        public DateTime DateCreate { get; set; }
    }
}