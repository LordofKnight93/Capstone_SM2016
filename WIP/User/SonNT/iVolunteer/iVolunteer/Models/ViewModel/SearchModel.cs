using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.MongoDB.EmbeddedClass.ListClass;

namespace iVolunteer.Models.ViewModel
{
    public class SearchModel
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public TagsList TagsList { get; set; }
    }
}