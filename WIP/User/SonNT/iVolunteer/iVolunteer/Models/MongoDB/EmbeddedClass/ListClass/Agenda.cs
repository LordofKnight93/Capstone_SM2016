using System.Collections.Generic;
using System;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ListClass
{
    public class AgendaItem
    {
        public string ItemID { get; set; }
        public DateTime WorkDate { get; set; }
        public string Content { get; set; }

        public AgendaItem()
        {
            this.ItemID = "";
            this.WorkDate = new DateTime();
            this.Content = "";
        }

        public AgendaItem(DateTime date, string content)
        {
            this.ItemID = "";
            this.WorkDate = date;
            this.Content = content;
        }
    }

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