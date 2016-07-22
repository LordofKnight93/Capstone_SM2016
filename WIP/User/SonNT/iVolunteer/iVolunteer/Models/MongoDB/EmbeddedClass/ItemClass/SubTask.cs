using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    [BsonIgnoreExtraElements]
    public class SubTask
    {
        public string Content { get; set; }
        public SDLink AssignPeople { get; set; }
        public DateTime Deadline { get; set; }
        public int Priolity { get; set; }
        public bool IsDone { get; set; }
        public SubTask()
        {
            this.Content = "";
            this.AssignPeople = new SDLink();
            this.Deadline = new DateTime();
            this.Priolity = 1;
            this.IsDone = false;
        }
        public SubTask(SubTask task)
        {
            this.Content = task.Content;
            this.AssignPeople = task.AssignPeople;
            this.Deadline = task.Deadline;
            this.Priolity = task.Priolity;
            this.IsDone = task.IsDone;
        }
    }
}