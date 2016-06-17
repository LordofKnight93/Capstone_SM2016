using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;
using MongoDB.Bson;
using System.Collections.Generic;
using iVolunteer.Models.Data_Definition_Class.ViewModel;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass
{
    /// <summary>
    /// This class define structure of "User" collection in MongoDB
    /// </summary>
    public class Mongo_User
    {
        public ObjectId _id { get; set; }
        public UserInformation UserInformation { get; set; }
        public AccountInformation AccountInformation { get; set; }
        public List<GroupSD> JoinedGroup { get; set; }
        public List<Member> FriendList { get; set; }
        public HistoryInformation ActivityHistory { get; set; }
        public List<ProjectSD> CurrentProjects { get; set; }
        public List<Notification> NotificationList { get; set; }
        public Mongo_User()
        {
            this._id = new ObjectId();
            this.UserInformation = new UserInformation();
            this.AccountInformation = new AccountInformation();
            this.JoinedGroup = new List<GroupSD>();
            this.FriendList = new List<Member>();
            this.ActivityHistory = new HistoryInformation();
            this.CurrentProjects = new List<ProjectSD>();
            this.NotificationList = new List<Notification>();
        }

        public Mongo_User(RegisterModel registerModel)
        {
            this._id = ObjectId.GenerateNewId();
            this.UserInformation = new UserInformation(registerModel);
            this.AccountInformation = new AccountInformation(registerModel);
            this.AccountInformation.UserID = this._id.ToString();
            this.JoinedGroup = new List<GroupSD>();
            this.FriendList = new List<Member>();
            this.ActivityHistory = new HistoryInformation();
            this.CurrentProjects = new List<ProjectSD>();
            this.NotificationList = new List<Notification>();
        }
    }
}