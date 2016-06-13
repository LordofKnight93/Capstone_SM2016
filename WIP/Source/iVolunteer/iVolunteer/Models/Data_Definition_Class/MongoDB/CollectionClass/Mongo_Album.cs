using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass
{
    public class Mongo_Album
    {
        public ObjectId _id { get; set; }
        public AlbumInformation AlbumInformation { get; set; } 
        public List<Comment> CommemtList { get; set; }
    }
}