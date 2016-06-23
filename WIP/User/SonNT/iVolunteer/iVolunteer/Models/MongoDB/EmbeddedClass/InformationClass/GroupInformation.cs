using System;
using MongoDB.Bson;
using iVolunteer.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store group's infomation
    /// </summary>
    public class GroupInformation 
    {
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public DateTime DateCreate { get; set; }
        public string GroupDescription { get; set; }
        public int MemberCount { get; set; }
        public bool IsActivate { get; set; }

        public GroupInformation()
        {
            this.GroupID = "";
            this.GroupName = "";
            this.DateCreate = new DateTime();
            this.GroupDescription = "";
            this.MemberCount = 0;
            this.IsActivate = Status.IS_ACTIVATE;
        }
    }
}