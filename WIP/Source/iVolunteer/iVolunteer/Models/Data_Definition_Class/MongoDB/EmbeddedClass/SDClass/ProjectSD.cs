using System;
using MongoDB.Bson;
using System.Collections.Generic;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass
{
    /// <summary>
    /// This class is used to store simple information of a project.
    /// Stored in history information
    /// </summary>
    public class ProjectSD
    {
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string AvtImgLink { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public List<string> Tags { get; set; }

        public ProjectSD()
        {
            this.ProjectID = "";
            this.ProjectName = "";
            this.AvtImgLink = "";
            this.DateStart = new DateTime();
            this.DateEnd = new DateTime();
            this.Tags = new List<string>();
        }
    }
}