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
    public class TeamFoundDonator
    {
        public ObjectId TeamFoundDonatorID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên cá nhân hoặc tổ chức ửng hộ")]
        [DisplayName("Tên tổ chức / cá nhân")]
        public string Name { get; set; }
        [DisplayName("Thông tin")]
        public string Information { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào số tiền được nhận.")]
        [DisplayName("Số tiền")]
        public double Amount { get; set; }
        [DisplayName("Ngày nhận")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Unspecified)]
        public DateTime ReceiveDate { get; set; }
        [DisplayName("Phương thức")]
        public int Method { get; set; }
        [DisplayName("Đã nhận")]
        public bool IsReceived { get; set; }
        public TeamFoundDonator()
        {
            this.TeamFoundDonatorID = ObjectId.GenerateNewId();
            this.Name = "";
            this.Information = "";
            this.Amount = 0;
            this.ReceiveDate = new DateTime();
            this.Method = 0;
            this.IsReceived = false;
        }
    }
}