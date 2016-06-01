using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Bson;
using iVolunteer.Models.MongoDB.Data_Definition_Class.Embedded_CLass;

namespace iVolunteer.Models.MongoDB.Data_Access_Object
{
    public static class UserInformationDAO
    {
        static IMongoClient _client = new MongoClient();
        static IMongoDatabase _db = _client.GetDatabase("iVolunteer");
        static IMongoCollection<UserInformation> _collection = _db.GetCollection<UserInformation>("UserInformation");
        public static void AddUserInformation( UserInformation userInfo)
        {
            _collection.InsertOneAsync(userInfo);
        }

        public static UserInformation GetUserInformationByID(string id)
        {
            UserInformation result = new UserInformation();
            result = _collection.Find(c => c._id == new ObjectId(id)).FirstOrDefault();
            return result;
        }
    }
}