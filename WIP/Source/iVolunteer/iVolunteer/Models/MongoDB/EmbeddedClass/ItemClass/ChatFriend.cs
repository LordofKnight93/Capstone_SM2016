using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using MongoDB.Bson.Serialization.Attributes;
using iVolunteer.Models.MongoDB.CollectionClass;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    [BsonIgnoreExtraElements]
    public class ChatFriend
    {
        public string ID { get; set; }
        public string DisplayName { get; set; }
        // name of controller handle this link
        public bool isOnline { get; set; }
        public int UnreadMss { get; set; }
        public string UnreadUser { get; set; }

        public ChatFriend(SDLink user, bool status, UnreadItem item)
        {
            this.ID = user.ID;
            this.DisplayName = user.DisplayName;
            this.isOnline = status;
            this.UnreadMss = item.UnreadMessage;
            this.UnreadUser = item.UnreadUser;
        }
    }
}