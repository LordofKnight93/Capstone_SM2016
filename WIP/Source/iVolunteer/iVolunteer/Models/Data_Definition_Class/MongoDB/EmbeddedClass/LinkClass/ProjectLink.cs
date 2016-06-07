using MongoDB.Bson;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass
{
    public class ProjectLink
    {
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
        public ProjectLink()
        {
            this.ProjectID = "";
            this.ProjectName = "";
        }
    }
}