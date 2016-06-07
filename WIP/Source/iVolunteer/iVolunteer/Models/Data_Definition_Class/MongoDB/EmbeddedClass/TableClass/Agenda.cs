using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.ItemClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.TableClass
{
    public class Agenda
    {
        public ObjectId _id { get; set; }
        public string AgendaName { get; set; }
        public AgendaItem[] ItemList { get; set; }
    }
}