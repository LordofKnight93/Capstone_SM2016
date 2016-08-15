using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    [BsonIgnoreExtraElements]
    public class MainTask
    {
        public ObjectId MainTaskID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập nội dung công việc chính.")]
        [DisplayName("Công việc:")]
        public string Name { get; set; }
        [DisplayName("Mô tả:")]
        public string Description { get; set; }
        public SDLink Assign { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        [DisplayName("Thời hạn:")]
		[Required(ErrorMessage = "Vui lòng nhập thời hạn.")]
        public DateTime Duedate { get; set; }
        public int TaskDoneCount { get; set; }
        public int SubTaskCount { get; set; }
        public int CommentCount { get; set; }
        public List<SubTask> Subtask { get; set; }
        public List<Comment> Comment { get; set; }
        public MainTask()
        {
            this.MainTaskID = ObjectId.GenerateNewId();
            this.Name = "";
            this.Description = "";
            this.Assign = new SDLink();
            this.Duedate = new DateTime();
            this.TaskDoneCount = 0;
            this.SubTaskCount = 0;
            this.CommentCount = 0;
            this.Subtask = new List<SubTask>();
            this.Comment = new List<Comment>();
        }
        public MainTask(MainTask task)
        {
            this.MainTaskID = ObjectId.GenerateNewId();
            this.Name = task.Name;
            this.Description = task.Description;
            this.Assign = task.Assign;
            this.Duedate = task.Duedate;
            this.Subtask = task.Subtask;
            this.Comment = task.Comment;
        }
    }
}