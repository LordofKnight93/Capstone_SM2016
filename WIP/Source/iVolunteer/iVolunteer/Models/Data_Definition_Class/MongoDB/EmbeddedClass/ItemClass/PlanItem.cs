using System;
using MongoDB.Bson;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass
{
    public class PlanItem
    {
        public ObjectId _id { get; set; }
        public DateTime WorkDate { get; set; }
        public string Content { get; set; }
        public Team[] PIC { get; set; }
    }
}