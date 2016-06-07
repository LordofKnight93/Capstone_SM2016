using System;
using MongoDB.Bson;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass
{
    /// <summary>
    /// This class store user's infomation
    /// </summary>
    public class UserInformation
    {
        public ObjectId _id { get; set; }
        public string RealName { get; set; }
        public DateTime Birthday { get; set; }
        public string IdentifyID { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool Gender { get; set; }
        public string[] Interest { get; set; }
    }
}