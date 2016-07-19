using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    [BsonIgnoreExtraElements]
    public class MainTask
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public SDLink Assign { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Duedate { get; set; }
        public int SubTaskCount { get; set; }
        public List<SubTask> Subtask { get; set; }
        public List<Comment> Comment { get; set; }
        public MainTask()
        {
            this.Name = "";
            this.Description = "";
            this.Assign = new SDLink();
            this.Duedate = new DateTime();
            this.SubTaskCount = 0;
            this.Subtask = new List<SubTask>();
            this.Comment = new List<Comment>();
        }
        public MainTask(MainTask task)
        {
            this.Name = task.Name;
            this.Description = task.Description;
            this.Assign = task.Assign;
            this.Duedate = task.Duedate;
            this.Subtask = task.Subtask;
            this.Comment = task.Comment;
        }
    }
}