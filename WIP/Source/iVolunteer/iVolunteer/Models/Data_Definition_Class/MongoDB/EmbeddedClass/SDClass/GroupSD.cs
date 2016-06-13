using MongoDB.Bson.Serialization.Attributes;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass
{
    /// <summary>
    /// This class is used to store simple information of a group.
    /// Stored in project structure, user's joined group, and used as model in user joined group view, project structure view
    /// </summary>
    [BsonIgnoreExtraElements]
    public class GroupSD
    {
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public string AvtImgLink { get; set; }

        public GroupSD()
        {
            this.GroupID = "";
            this.GroupName = "";
            this.AvtImgLink = "";
        }
    }
}