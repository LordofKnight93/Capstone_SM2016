using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store activity history of group or user fomr day join system
    /// </summary>
    [BsonIgnoreExtraElements]
    public class ActivityInformation
    {
        [BsonIgnoreIfDefault]
        public List<SDLink> OrganizedProjects { get; set; }
        [BsonIgnoreIfDefault]
        public List<SDLink> JoinedProjects { get; set; }
        [BsonIgnoreIfDefault]
        public List<SDLink> SponsoredProjects { get; set; }

        public ActivityInformation()
        {
            this.OrganizedProjects = new List<SDLink>();
            this.JoinedProjects = new List<SDLink>();
            this.SponsoredProjects = new List<SDLink>();
        }
    }
}