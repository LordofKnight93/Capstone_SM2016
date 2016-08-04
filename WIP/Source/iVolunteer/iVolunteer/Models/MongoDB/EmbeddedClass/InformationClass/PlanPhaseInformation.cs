using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass
{
    public class PlanPhaseInformation
    {
        public string PlanPhaseID { get; set; }
        public SDLink Project { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên giai đoạn.")]
        public string Name { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        public DateTime StartTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
        public PlanPhaseInformation()
        {
            this.PlanPhaseID = "";
            this.Project = new SDLink();
            this.Name = "";
            this.StartTime = new DateTime();
            this.EndTime = new DateTime();
            this.Description = "";
        }
    }
}