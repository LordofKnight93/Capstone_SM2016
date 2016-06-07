using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store activity history of group or user
    /// </summary>
    public class HistoryInformation
    {
        public ProjectSD[] OrganizedProject { get; set; }
        public ProjectSD[] JoinedProject { get; set; }
        public ProjectSD[] SponsoredProject { get; set; }
    }
}