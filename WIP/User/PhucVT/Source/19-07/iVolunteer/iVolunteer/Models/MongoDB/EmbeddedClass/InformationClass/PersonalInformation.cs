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
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; set; }
        public string IdentifyID { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool Gender { get; set; }

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
            //missing interest
        }
    }
}