using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using iVolunteer.Models.Data_Access_Object.MongoDB;
using iVolunteer.Models.Data_Definition_Class.MongoDB.CollectionClass;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDBTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IMongoClient client = new MongoClient();
            IMongoDatabase db = client.GetDatabase("iVolunteer");
            IMongoCollection<Mongo_Group> collection = db.GetCollection<Mongo_Group>("Group");
            var result = collection.Find(new BsonDocument()).ToList();
            Console.WriteLine(result.Capacity);
            Thread.Sleep(5000);
        }
    }
}
