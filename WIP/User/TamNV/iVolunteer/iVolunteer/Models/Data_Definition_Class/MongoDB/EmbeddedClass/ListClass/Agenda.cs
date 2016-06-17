using System.Linq;
using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;
using System.Collections.Generic;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ListClass
{
    public class Agenda
    {
        public string AgendaName { get; set; }
        public List<AgendaItem> ItemList { get; set; }

        public Agenda()
        {
            this.AgendaName = "";
            this.ItemList = new List<AgendaItem>();
        }
    }
}