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

        public void Add_Item(AgendaItem item)
        {
            this.ItemList.Add(item);
        }

        public void Delete_Item(string itemID)
        {
            AgendaItem item = this.ItemList.Find(i => i.ItemID == itemID);
            this.ItemList.Remove(item);
        }
    }
}