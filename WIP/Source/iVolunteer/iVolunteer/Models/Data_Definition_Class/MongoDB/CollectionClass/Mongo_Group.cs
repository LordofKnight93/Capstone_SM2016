using MongoDB.Bson;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.StructureClass;
using iVolunteer.Models.Data_Definition_Class.MongoDB.EmbeddedClass.SDClass;

namespace iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass
{
    /// <summary>
    /// This class define structure of "Group" collection in MongoDB
    /// </summary>
    public class Mongo_Group
    {
        public ObjectId _id { get; set; }
        public GroupInformation GroupInformation { get; set; }
        public GroupStructure GroupStructure { get; set; }
        public HistoryInformation HistoryInformation { get; set; }
        public ProjectSD[] CurrentProjects { get; set; }
        public AlbumSD[] AlbumList { get; set; }

        public Mongo_Group( GroupInformation groupInfo)
        {
            this.GroupInformation = groupInfo;
        }
    }
}