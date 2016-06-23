using System;
using iVolunteer.Models.ViewModel;
using MongoDB.Bson;
using System.Collections.Generic;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store user's infomation
    /// </summary>
    public class UserInformation
    {
        public string UserID { get; set; }
        public string RealName { get; set; }
        public DateTime Birthday { get; set; }
        public string IdentifyID { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool Gender { get; set; }

        public UserInformation()
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

        public UserInformation(RegisterModel registerModel)
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