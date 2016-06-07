using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using System.Collections.Generic;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store activity history of group or user
    /// </summary>
    public class HistoryInformation
    {
        public List<ProjectSD> OrganizedProject { get; set; }
        public List<ProjectSD> JoinedProject { get; set; }
        public List<ProjectSD> SponsoredProject { get; set; }

        public HistoryInformation()
        {
            this.OrganizedProject = new List<ProjectSD>();
            this.JoinedProject = new List<ProjectSD>();
            this.SponsoredProject = new List<ProjectSD>();
        }
    }
}