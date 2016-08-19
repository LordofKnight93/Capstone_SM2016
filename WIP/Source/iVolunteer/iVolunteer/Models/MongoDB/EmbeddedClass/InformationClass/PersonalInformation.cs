using System;
using iVolunteer.Models.ViewModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store user's infomation
    /// </summary>
    [BsonIgnoreExtraElements]
    public class PersonalInformation
    {
        public string UserID { get; set; }
        public string RealName { get; set; }
        [BsonDateTimeOptions(DateOnly = true, Kind = DateTimeKind.Local)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime Birthday { get; set; }
        public string IdentifyID { get; set; }
        [EmailAddress(ErrorMessage = "Địa chỉ email không đúng định dạng!")]
        public string Email { get; set; }
        public string Address { get; set; }
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng!")]
        public string Phone { get; set; }
        public bool Gender { get; set; }
        [MaxLength(100, ErrorMessage =" Độ dài tối đa 100 ký tự!")]
        public string Skills { get; set; }
        [MaxLength(200, ErrorMessage = " Độ dài tối đa 200 ký tự!")]
        public string Experience { get; set; }
        [MaxLength(100, ErrorMessage = " Độ dài tối đa 100 ký tự!")]
        public string Interest { get; set; }

        public PersonalInformation()
        {
            this.UserID = "";
            this.RealName = "";
            this.Birthday = new DateTime();
            this.IdentifyID = "";
            this.Email = "";
            this.Address = "";
            this.Phone = "";
            this.Gender = false;
            this.Skills = "";
            this.Experience = "";
            this.Interest = "";
        }

        public PersonalInformation(RegisterModel registerModel)
        {
            this.RealName = registerModel.RealName;
            this.Birthday = registerModel.Birthday;
            this.IdentifyID = registerModel.IdentifyID;
            this.Email = registerModel.Email;
            this.Address = registerModel.Address;
            this.Phone = registerModel.Phone;
            this.Gender = registerModel.Gender;
            this.Skills = "";
            this.Experience = "";
            this.Interest = "";
        }
    }
}