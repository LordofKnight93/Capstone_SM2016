using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using System.Collections.Generic;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store activity history of group or user fomr day join system
    /// </summary>
    public class ActivityInformation
    {
        public List<SDLink> OrganizedProjects { get; set; }
        public List<SDLink> JoinedProjects { get; set; }
        public List<SDLink> SponsoredProjects { get; set; }

        public ActivityInformation()
        {
            this.OrganizedProjects = new List<SDLink>();
            this.JoinedProjects = new List<SDLink>();
            this.SponsoredProjects = new List<SDLink>();
        }
    }
}