using System;
using MongoDB.Bson;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass
{
    /// <summary>
    /// This class is used to store simple information of a project.
    /// Stored in history information
    /// </summary>
    public class ProjectSD
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string AvtImgLink { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string[] Tags { get; set; }
    }
}