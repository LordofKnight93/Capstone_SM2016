using System;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass
{
    public class Member
    {
        public UserSD User { get; set; }
        public DateTime JoinDate { get; set; }
    }
}