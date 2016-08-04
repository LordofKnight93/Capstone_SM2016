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
        public string DisplayName { get; set; }
        public string Address { get; set; }
        public int FriendCount { get; set; }
        public int GroupCount { get; set; }
        public int OrganizedProjectCount { get; set; }
        public int SponsoredProjectCount { get; set; }
        public int JoinedProjectCount { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActivate { get; set; }
        public bool IsConfirmed { get; set; }

        public AccountInformation()
        {
            this.UserID = "";
            this.DisplayName = "";
            this.Address = "";
            this.FriendCount = 0;
            this.GroupCount = 0;
            this.OrganizedProjectCount = 0;
            this.SponsoredProjectCount = 0;
            this.JoinedProjectCount = 0;
            this.IsAdmin = Role.IS_USER;
            this.IsActivate = Status.IS_ACTIVATE;
            this.IsConfirmed = Status.IS_NOT_CONFIRMED;
        }

        public AccountInformation(RegisterModel registerModel)
        {
            this.DisplayName = registerModel.RealName;
            this.Address = registerModel.Address;
            this.FriendCount = 0;
            this.GroupCount = 0;
            this.OrganizedProjectCount = 0;
            this.SponsoredProjectCount = 0;
            this.JoinedProjectCount = 0;
            this.IsAdmin = Role.IS_USER;
            this.IsActivate = Status.IS_ACTIVATE;
            this.IsConfirmed = Status.IS_NOT_CONFIRMED;
        }
        public string Get_AvatarLink()
        {
            return "/Images/User/Avatar/" + this.UserID + ".jpg";
        }
    }
}