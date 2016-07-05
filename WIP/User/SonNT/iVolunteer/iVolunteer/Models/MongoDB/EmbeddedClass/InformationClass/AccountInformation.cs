using iVolunteer.Common;
using iVolunteer.Models.ViewModel;
using MongoDB.Bson.Serialization.Attributes;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using System;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store user's account infomation
    /// </summary>
    [BsonIgnoreExtraElements]
    public class AccountInformation
    {
        public string UserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string IdentifyID { get; set; }
        public int FriendCount { get; set; }
        public int GroupCount { get; set; }
        public int ProjectCount { get; set; }
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
            this.FriendCount = 0;
            this.GroupCount = 0;
            this.ProjectCount = 0;
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
            this.FriendCount = 0;
            this.GroupCount = 0;
            this.ProjectCount = 0;
            this.IsAdmin = Role.IS_USER;
            this.IsActivate = Status.IS_ACTIVATE;
            this.IsConfirmed = Status.IS_NOT_CONFIRMED;
        }
    }
}