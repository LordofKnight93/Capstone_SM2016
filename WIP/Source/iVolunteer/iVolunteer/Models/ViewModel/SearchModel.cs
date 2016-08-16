using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.MongoDB.EmbeddedClass.ListClass;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace iVolunteer.Models.ViewModel
{
    public class SearchModel
    {
        [MaxLength(100, ErrorMessage ="Độ dài không quá 100 ký tự")]
        public string Name { get; set; }
        [MaxLength(100, ErrorMessage = "Độ dài không quá 100 ký tự")]
        public string Location { get; set; }
        [BsonDateTimeOptions(DateOnly = true, Kind = DateTimeKind.Local)]
        [DataType(DataType.Date, ErrorMessage = "Ngày bạn nhập không hợp lệ!")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateStart { get; set; }
        [BsonDateTimeOptions(DateOnly = true, Kind = DateTimeKind.Local)]
        [DataType(DataType.Date, ErrorMessage = "Ngày bạn nhập không hợp lệ!")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateEnd { get; set; }
        public string[] TagsList { get; set; }
        public bool Progress { get; set; }
        public bool Recruiting { get; set; }

        public SearchModel()
        {
            this.Name = "";
            this.Location = "";
            this.DateStart = DateTime.Now.AddYears(-1).Date;
            this.DateEnd = DateTime.Now.AddYears(1).Date;
            this.Progress = true;
            this.Recruiting = false;
        }
    }
}