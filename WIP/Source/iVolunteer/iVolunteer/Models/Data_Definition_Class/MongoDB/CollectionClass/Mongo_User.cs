using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass
{
    /// <summary>
    /// This class define structure of "User" collection in MongoDB
    /// </summary>
    public class Mongo_User
    {
        public UserInformation UserInformation { get; set; }
        public AccountInformation AccountInformation { get; set; }
        public GroupSD[] JoinedGroup { get; set; }
        public Member[] FriendList { get; set; }
        public HistoryInformation ActivityHistory { get; set; }
        public ProjectSD[] CurrentProjects { get; set; }
        public Notification[] NotificationList { get; set; }
    }
}