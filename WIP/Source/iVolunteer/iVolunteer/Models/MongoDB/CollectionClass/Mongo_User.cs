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
        public PersonalInformation PersonalInformation { get; set; }
        public AccountInformation AccountInformation { get; set; }
        [BsonIgnoreIfDefault]
        public List<Notification> NotificationList { get; set; }
        public Mongo_User()
        {
            this._id = new ObjectId();
            this.PersonalInformation = new PersonalInformation();
            this.AccountInformation = new AccountInformation();
            this.NotificationList = new List<Notification>();
        }

        public Mongo_User(RegisterModel registerModel)
        {
            this._id = ObjectId.GenerateNewId();
            this.PersonalInformation = new PersonalInformation(registerModel);
            this.PersonalInformation.UserID = this._id.ToString();
            this.AccountInformation = new AccountInformation(registerModel);
            this.AccountInformation.UserID = this._id.ToString();
            this.NotificationList = new List<Notification>();
        }
    }
}