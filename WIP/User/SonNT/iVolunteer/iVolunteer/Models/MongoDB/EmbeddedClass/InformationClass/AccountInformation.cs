using iVolunteer.Common;
using iVolunteer.Models.ViewModel;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using System;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store user's account infomation
    /// </summary>
    public class AccountInformation
    {
        public string UserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string IdentifyID { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActivate { get; set; }
        public bool IsConfirmed { get; set; }

        public AccountInformation()
        {
            this.UserID = "";
            this.Email = "";
            this.Password = "";
            this.DisplayName = "";
            this.IdentifyID = "";
            this.IsAdmin = Role.IS_USER;
            this.IsActivate = Status.IS_ACTIVATE;
            this.IsConfirmed = Status.IS_NOT_CONFIRMED;
        }

        public AccountInformation(RegisterModel registerModel)
        {
            this.Email = registerModel.Email;
            this.Password = registerModel.Password;
            this.DisplayName = registerModel.RealName;
            this.IdentifyID = registerModel.IdentifyID;
            this.IsAdmin = Role.IS_USER;
            this.IsActivate = Status.IS_ACTIVATE;
            this.IsConfirmed = Status.IS_NOT_CONFIRMED;
        }
    }
}