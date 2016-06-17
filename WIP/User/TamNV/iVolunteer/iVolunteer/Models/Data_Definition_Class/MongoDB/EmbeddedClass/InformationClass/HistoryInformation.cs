using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using System.Collections.Generic;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store activity history of group or user fomr day join system
    /// </summary>
    public class HistoryInformation
    {
        public List<ProjectSD> OrganizedProjects { get; set; }
        public List<ProjectSD> JoinedProjects { get; set; }
        public List<ProjectSD> SponsoredProjects { get; set; }

        public HistoryInformation()
        {
            this.OrganizedProjects = new List<ProjectSD>();
            this.JoinedProjects = new List<ProjectSD>();
            this.SponsoredProjects = new List<ProjectSD>();
        }
    }
}