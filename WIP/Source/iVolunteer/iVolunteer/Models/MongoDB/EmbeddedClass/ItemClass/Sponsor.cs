using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Foolproof;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    [BsonIgnoreExtraElements]
    public class Sponsor
    {
        public string SponsorID { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập họ và tên!")]
        public string SponsorName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ!")]
        public string SponsorAddress { get; set; }
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng!")]
        public string SponsorEmail { get; set; }
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng!")]
        public string SponsorPhone { get; set; }
        public bool Status { get; set; }

        public Sponsor()
        {
            this.SponsorID = "";
            this.SponsorName = "";
            this.SponsorAddress = "";
            this.SponsorEmail = "";
            this.SponsorPhone = "";
            this.Status = false;
        }
    }
}