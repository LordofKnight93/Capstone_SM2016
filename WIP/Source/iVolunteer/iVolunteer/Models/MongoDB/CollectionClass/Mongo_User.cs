using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ListClass;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using iVolunteer.Models.ViewModel;

namespace iVolunteer.Models.MongoDB.CollectionClass
{
    /// <summary>
    /// This class define structure of "User" collection in MongoDB
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Mongo_User
    {
        public ObjectId _id { get; set; }
        public UserInformation UserInformation { get; set; }
        public AccountInformation AccountInformation { get; set; }
        [BsonIgnoreIfDefault]
        public List<SDLink> JoinedGroups { get; set; }
        [BsonIgnoreIfDefault]
        public List<Member> FriendList { get; set; }
        [BsonIgnoreIfDefault]
        public ActivityInformation ActivityHistory { get; set; }
        [BsonIgnoreIfDefault]
        public List<SDLink> CurrentProjects { get; set; }
        [BsonIgnoreIfDefault]
        public List<Notification> NotificationList { get; set; }
        [BsonIgnoreIfDefault]
        public List<RequestItem> RequestList { get; set; }
        public Mongo_User()
        {
            this._id = new ObjectId();
            this.UserInformation = new UserInformation();
            this.AccountInformation = new AccountInformation();
            this.JoinedGroups = new List<SDLink>();
            this.FriendList = new List<Member>();
            this.ActivityHistory = new ActivityInformation();
            this.CurrentProjects = new List<SDLink>();
            this.NotificationList = new List<Notification>();
            this.RequestList = new List<RequestItem>();
        }

        public Mongo_User(RegisterModel registerModel)
        {
            this._id = ObjectId.GenerateNewId();
            this.UserInformation = new UserInformation(registerModel);
            this.UserInformation.UserID = this._id.ToString();
            this.AccountInformation = new AccountInformation(registerModel);
            this.AccountInformation.UserID = this._id.ToString();
            this.JoinedGroups = new List<SDLink>();
            this.FriendList = new List<Member>();
            this.ActivityHistory = new ActivityInformation();
            this.CurrentProjects = new List<SDLink>();
            this.NotificationList = new List<Notification>();
            this.RequestList = new List<RequestItem>();
        }
    }
}