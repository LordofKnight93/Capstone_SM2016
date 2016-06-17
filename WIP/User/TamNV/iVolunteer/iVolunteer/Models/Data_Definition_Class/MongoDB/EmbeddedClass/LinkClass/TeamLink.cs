using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.LinkClass
{
    public class TeamLink
    {
        public string TeamID { get; set; }
        public string TeamName { get; set; }
        public TeamLink()
        {
            this.TeamID = "";
            this.TeamName = "";
        }
    }
}