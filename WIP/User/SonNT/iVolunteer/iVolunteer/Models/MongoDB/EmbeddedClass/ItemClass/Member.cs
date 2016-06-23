using System;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    public class Member
    {
        public SDLink SDInfo { get; set; }
        public DateTime JoinDate { get; set; }

        public Member()
        {
            this.SDInfo = new SDLink();
            this.JoinDate = new DateTime();
        }

        public Member(SDLink member)
        {
            this.SDInfo = member;
            this.JoinDate = DateTime.Now.Date;
        }
    }
}