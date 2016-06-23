using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Core.Bindings;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass
{
    [BsonIgnoreExtraElements]
    public class TagsList
    {
        // Topic tag
        [DisplayName("Giáo dục")]
        public bool IsEducation { get; set; }
        [DisplayName("Xã hội")]
        public bool IsSocial { get; set; }
        [DisplayName("Y tế")]
        public bool IsMedical { get; set; }

        // obbject tag
        [DisplayName("Trẻ em")]
        public bool IsChildren { get; set; }
        [DisplayName("Người nghèo, khó khăn")]
        public bool IsPoorPerson { get; set; }
        [DisplayName("Vùng thiên tai")]
        public bool IsDisaster { get; set; }

        //method tag
        [DisplayName("Quyên góp")]
        public bool IsDonation { get; set; }
        [DisplayName("Hoạt động")]
        public bool IsActivity { get; set; }
    }
}