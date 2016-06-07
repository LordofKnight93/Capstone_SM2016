using System;
using MongoDB.Bson;
using System.Collections.Generic;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass
{
    public class PlanItem
    {
        public ObjectId _id { get; set; }
        public DateTime WorkDate { get; set; }
        public string Content { get; set; }
        public List<TeamLink> PIC { get; set; }
        public PlanItem()
        {
            this._id = new ObjectId();
            this.WorkDate = new DateTime();
            this.Content = "";
            this.PIC = new List<TeamLink>();
        }
    }
}