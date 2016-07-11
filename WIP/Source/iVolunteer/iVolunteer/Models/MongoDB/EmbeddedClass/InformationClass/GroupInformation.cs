using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using iVolunteer.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store group's infomation
    /// </summary>
    [BsonIgnoreExtraElements]
    public class GroupInformation 
    {
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateCreate { get; set; }
        public string GroupDescription { get; set; }
        public int MemberCount { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
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
        public string Get_AvatarLink()
        {
            return "/Images/Group/Avatar/" + this.GroupID + ".jpg";
        }
    }
}