using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;

namespace iVolunteer.Models.Data_Definition_Class.ViewModel
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