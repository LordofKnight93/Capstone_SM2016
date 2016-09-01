using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using MongoDB.Bson.Serialization.Attributes;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass
{
    [BsonIgnoreExtraElements]
    public class SDLink
    {
        public string ID { get; set; }
        public string DisplayName { get; set; }
        // name of controller handle this link
        public string Handler { get; set; }

        public SDLink()
        {
            this.ID = "";
            this.DisplayName = "";
            this.Handler = "";
        }
        /// <summary>
        /// create project SDLink from project information
        /// </summary>
        /// <param name="projectInfo"></param>
        public SDLink(ProjectInformation projectInfo)
        {
            this.ID = projectInfo.ProjectID;
            this.DisplayName = projectInfo.ProjectName;
            this.Handler = "Project";
        }
        /// <summary>
        /// create group SDLink from group information
        /// </summary>
        /// <param name="groupInfo"></param>
        public SDLink(GroupInformation groupInfo)
        {
            this.ID = groupInfo.GroupID;
            this.DisplayName = groupInfo.GroupName;
            this.Handler = "Group";
        }
        /// <summary>
        /// create account SDLink from account information
        /// </summary>
        /// <param name="accountInfo"></param>
        public SDLink(AccountInformation accountInfo)
        {
            this.ID = accountInfo.UserID;
            this.DisplayName = accountInfo.DisplayName;
            this.Handler = "User";
        }
        /// <summary>
        /// create album SDLink from album information
        /// </summary>
        /// <param name="albumInfo"></param>
        public SDLink(AlbumInformation albumInfo)
        {
            this.ID = albumInfo.AlbumID;
            this.DisplayName = albumInfo.AlbumName;
            this.Handler = "Album";
        }
        /// <summary>
        /// create post SDLink from post information
        /// </summary>
        /// <param name="postInfo"></param>
        public SDLink(PostInformation postInfo)
        {
            this.ID = postInfo.PostID;
            this.DisplayName = "bài đăng";
            this.Handler = "Post";
        }
        public SDLink(string postID)
        {
            this.ID = postID;
            this.DisplayName = "bài đăng";
            this.Handler = "Post";
        }

        public string Get_AvatarLink()
        {
            return "/Images/" + this.Handler + "/Avatar/" + this.ID + ".jpg";
        }
        public string Get_CoverLink()
        {
            return "/Images/" + this.Handler + "/Cover/" + this.ID + ".jpg";
        }
    }
}