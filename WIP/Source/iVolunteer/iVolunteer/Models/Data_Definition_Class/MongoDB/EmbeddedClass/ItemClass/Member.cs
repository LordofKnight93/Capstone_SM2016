using System;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass
{
    public class Member
    {
        public UserSD User { get; set; }
        public DateTime JoinDate { get; set; }

        public Member()
        {
            this.User = new UserSD();
            this.JoinDate = new DateTime();
        }
    }
}