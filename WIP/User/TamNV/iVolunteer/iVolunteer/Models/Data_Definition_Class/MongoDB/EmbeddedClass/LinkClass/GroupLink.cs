using MongoDB.Bson;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass
{
    public class GroupLink
    {
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public GroupLink()
        {
            this.GroupID = "";
            this.GroupName = "";
        }
    }
}