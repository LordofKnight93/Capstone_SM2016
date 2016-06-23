using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using MongoDB.Bson;
using System.Collections.Generic;
using iVolunteer.Models.ViewModel;

namespace iVolunteer.Models.MongoDB.CollectionClass
{
    /// <summary>
    /// This class define structure of "User" collection in MongoDB
    /// </summary>
    public class Mongo_User
    {
        public ObjectId _id { get; set; }
        public UserInformation UserInformation { get; set; }
        public AccountInformation AccountInformation { get; set; }
        public List<SDLink> JoinedGroup { get; set; }
        public List<Member> FriendList { get; set; }
        public ActivityInformation ActivityHistory { get; set; }
        public List<SDLink> CurrentProjects { get; set; }
        public List<Notification> NotificationList { get; set; }
        public Mongo_User()
        {
            this._id = new ObjectId();
            this.UserInformation = new UserInformation();
            this.AccountInformation = new AccountInformation();
            this.JoinedGroup = new List<SDLink>();
            this.FriendList = new List<Member>();
            this.ActivityHistory = new ActivityInformation();
            this.CurrentProjects = new List<SDLink>();
            this.NotificationList = new List<Notification>();
        }

        public Mongo_User(RegisterModel registerModel)
        {
            this._id = ObjectId.GenerateNewId();
            this.UserInformation = new UserInformation(registerModel);
            this.AccountInformation = new AccountInformation(registerModel);
            this.AccountInformation.UserID = this._id.ToString();
            this.JoinedGroup = new List<SDLink>();
            this.FriendList = new List<Member>();
            this.ActivityHistory = new ActivityInformation();
            this.CurrentProjects = new List<SDLink>();
            this.NotificationList = new List<Notification>();
        }
    }
}