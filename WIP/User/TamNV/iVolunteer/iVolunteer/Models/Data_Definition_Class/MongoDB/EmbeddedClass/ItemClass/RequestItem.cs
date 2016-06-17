using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass
{
    public class RequestItem
    {
        public string RequestID { get; set; }
        public UserLink UserActor { get; set; }
        public GroupLink GroupActor { get; set; }
        public string Content { get; set; }
        public GroupLink GroupLink { get; set; }
        public ProjectLink ProjectLink { get; set; }

        public RequestItem()
        {
            this.UserActor = new UserLink();
            this.GroupActor = new GroupLink();
            this.Content = "";
            this.GroupLink = new GroupLink();
            this.ProjectLink = new ProjectLink();
        }
        public RequestItem(UserLink actor, string content)
        {
            this.UserActor = actor;
            this.Content = content;
        }

        public RequestItem(UserLink actor, string content, GroupLink groupLink)
        {
            this.UserActor = actor;
            this.Content = content;
            this.GroupLink = groupLink;
        }

        public RequestItem(UserLink actor, string content, ProjectLink projectLink)
        {
            this.UserActor = actor;
            this.Content = content;
            this.ProjectLink = projectLink;
        }
        public RequestItem(GroupLink actor, string content, ProjectLink projectLink)
        {
            this.GroupActor = actor;
            this.Content = content;
            this.ProjectLink = projectLink;
        }
    }
}