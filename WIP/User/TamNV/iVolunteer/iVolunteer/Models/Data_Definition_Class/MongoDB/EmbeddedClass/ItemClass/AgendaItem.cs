using System;
using MongoDB.Bson;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass
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
}