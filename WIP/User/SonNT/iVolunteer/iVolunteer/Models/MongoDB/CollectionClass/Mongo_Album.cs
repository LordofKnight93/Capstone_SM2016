using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;

namespace iVolunteer.Models.MongoDB.CollectionClass
{
    public class Mongo_Album
    {
        public ObjectId _id { get; set; }
        public AlbumInformation AlbumInformation { get; set; } 
        public List<Comment> CommemtList { get; set; }
    }
}