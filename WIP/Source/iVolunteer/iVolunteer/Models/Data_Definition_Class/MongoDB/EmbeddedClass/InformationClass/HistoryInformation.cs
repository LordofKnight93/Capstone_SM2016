using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;
using System.Collections.Generic;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store activity history of group or user
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

        public void Add_OrganizedProject(ProjectSD project)
        {
            this.OrganizedProjects.Add(project);
        }

        public void Delete_OrganizedProject(string projectID)
        {
            ProjectSD project = this.OrganizedProjects.Find(p => p.ProjectID == projectID);
            this.OrganizedProjects.Remove(project);
        }

        public void Add_JoinedProject(ProjectSD project)
        {
            this.JoinedProjects.Add(project);
        }

        public void Delete_JoinedProject(string projectID)
        {
            ProjectSD project = this.JoinedProjects.Find(p => p.ProjectID == projectID);
            this.JoinedProjects.Remove(project);
        }

        public void Add_SponsoredProject(ProjectSD project)
        {
            this.OrganizedProjects.Add(project);
        }

        public void Delete_SponsoredProject(string projectID)
        {
            ProjectSD project = this.OrganizedProjects.Find(p => p.ProjectID == projectID);
            this.OrganizedProjects.Remove(project);
        }
    }
}