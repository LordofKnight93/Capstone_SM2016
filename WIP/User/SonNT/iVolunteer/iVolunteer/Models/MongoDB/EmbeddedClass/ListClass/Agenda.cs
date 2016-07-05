using System.Collections.Generic;
using System;
using MongoDB.Bson.Serialization.Attributes;

namespace iVolunteer.Models.MongoDB.EmbeddedClass.ListClass
{
    [BsonIgnoreExtraElements]
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
    [BsonIgnoreExtraElements]
    public class Agenda
    {
        public string AgendaName { get; set; }
        [BsonIgnoreIfDefault]
        public List<AgendaItem> ItemList { get; set; }

        public Agenda()
        {
            this.AgendaName = "";
            this.ItemList = new List<AgendaItem>();
        }
    }
}