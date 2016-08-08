using iVolunteer.Common;
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
    public class SubTask
    {
        public ObjectId SubTaskID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập nội dung công việc.")]
        [DisplayName("Nội dung:")]
        public string Content { get; set; }
        [DisplayName("Người thực hiện:")]
        public SDLink AssignPeople { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        [DisplayName("Hạn cuối:")]
        public DateTime Deadline { get; set; }
        [DisplayName("Ưu tiên:")]
        public int Priolity { get; set; }
        [DisplayName("Trạng thái")]
        public int IsDone { get; set; }
        public SubTask()
        {
            this.SubTaskID = ObjectId.GenerateNewId();
            this.Content = "";
            this.AssignPeople = new SDLink();
            this.Deadline = new DateTime();
            this.Priolity = SubTaskPriolity.MEDIUM;
            this.IsDone = SubTaskIsDone.PENDDING;
        }
        public SubTask(SubTask task)
        {
            this.SubTaskID = ObjectId.GenerateNewId();
            this.Content = task.Content;
            this.AssignPeople = task.AssignPeople;
            this.Deadline = task.Deadline;
            this.Priolity = task.Priolity;
            this.IsDone = task.IsDone;
        }
    }
}