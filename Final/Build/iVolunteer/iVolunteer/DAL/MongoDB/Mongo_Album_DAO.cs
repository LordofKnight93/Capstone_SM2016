using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.IO;
using MongoDB.Driver.Builders;
using iVolunteer.Models.MongoDB.CollectionClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Helpers;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.ViewModel;
using iVolunteer.Common;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;

namespace iVolunteer.DAL.MongoDB
{
    public class Mongo_Album_DAO : Mongo_DAO
    {
        IMongoCollection<Mongo_Album> collection = db.GetCollection<Mongo_Album>("Album");

        IMongoCollection<ImageInformation> collect = db.GetCollection<ImageInformation>("Album");

        public bool Add_Album(Mongo_Album mongo_album)
        {
            try
            {
                collection.InsertOne(mongo_album);
                return true;
            }
            catch
            {
                throw;
            }
        }
        public bool Delete_Album(string albumID)
        {
            try
            {
                var filter = Builders<Mongo_Album>.Filter.Eq(p => p.AlbumInformation.AlbumID, albumID);
                collection.DeleteOne(filter);
                return true;
            }
            catch
            {
                throw;
            }
        }
        public bool Add_Image(string albumID, ImageInformation mongo_Image)
        {
            try
            {
                var filter = Builders<Mongo_Album>.Filter.Eq(img => img._id, new ObjectId(albumID));

                var mongoImage = new Mongo_Image();
                mongoImage.ImageInformation = mongo_Image;
                var update = Builders<Mongo_Album>.Update.AddToSet(p => p.ImageList, mongoImage)
                                                 .Inc(p => p.AlbumInformation.ImageCount, 1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        public AlbumInformation Get_Album_By_ID(string albumID)
        {
            try
            {
                var filter = Builders<Mongo_Album>.Filter.Eq(p => p.AlbumInformation.AlbumID, albumID);
                var result = collection.Find(filter).FirstOrDefault();
                return result.AlbumInformation;
            }
            catch
            {
                throw;
            }
        }
        public bool Delete_Image(string imageID, string albumID)
        {
            try
            {
                var album_filter = Builders<Mongo_Album>.Filter.Eq(p => p._id, new ObjectId(albumID));
                var img_filter = Builders<Mongo_Image>.Filter.Eq(img => img.ImageInformation.ImageID, imageID);
                var update = Builders<Mongo_Album>.Update.PullFilter(p => p.ImageList, img_filter)
                                                        .Inc(p => p.AlbumInformation.ImageCount, -1); ;
                var result = collection.UpdateOne(album_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        public SDLink Get_SDLink(string albumID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(al => al.AlbumInformation.AlbumID == albumID);
                return new SDLink(result.AlbumInformation);
            }
            catch
            {
                throw;
            }
        }

        public string Get_TargetID(string albumID)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(al => al.AlbumInformation.AlbumID == albumID);
                return result.AlbumInformation.TargetID;
            }
            catch
            {
                throw;
            }
        }
        public List<Mongo_Album> Get_Private_Album_By_TargetID(string targetID, int skip, int number)
        {
            try
            {
                var filter = Builders<Mongo_Album>.Filter.Eq(p => p.AlbumInformation.TargetID, targetID);
                var sort = Builders<Mongo_Album>.Sort.Descending(p => p.AlbumInformation.DateLastActivity);
                var result = collection.Find(filter).Sort(sort).Skip(skip).Limit(number).ToList();
                return result;
            }
            catch
            {
                throw;
            }
        }
        public Mongo_Album Get_Image_By_AlbumID(string albumID)
        {
            try
            {
                var filter = Builders<Mongo_Album>.Filter.Eq(p => p.AlbumInformation.AlbumID, albumID);
                var result = collection.Find(filter).ToList().FirstOrDefault();
                return result;
            }
            catch
            {
                throw;
            }
        }
        public string Get_Image_By_AlbumID_Name(string imgName, string albumID, string userID)
        {
            try
            {
                var result = collection.AsQueryable().Where(p => p.AlbumInformation.AlbumID == albumID).SelectMany(img => img.ImageList).Where(img => img.ImageInformation.ImageName == imgName).ToList();

                return result.ElementAt(0).ImageInformation.ImageID;
            }
            catch
            {
                throw;
            }
        }
        public bool Check_Image_By_Name(string imgName, string albumID)
        {
            try
            {
                var result = collection.AsQueryable().Where(p => p.AlbumInformation.AlbumID == albumID).SelectMany(img => img.ImageList).Where(img => img.ImageInformation.ImageName == imgName).ToList();
                if (result.Count != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }
        public bool Set_DateLastActivity(string albumID, DateTime date)
        {
            try
            {
                var filter = Builders<Mongo_Album>.Filter.Eq(p => p._id, new ObjectId(albumID));
                var update = Builders<Mongo_Album>.Update.Set(p => p.AlbumInformation.DateLastActivity, date);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        public bool Add_Comment(string albumID, Comment cmt)
        {
            try
            {
                var filter = Builders<Mongo_Album>.Filter.Eq(p => p._id, new ObjectId(albumID));
                var update = Builders<Mongo_Album>.Update.AddToSet<Comment>(p => p.CommentList, cmt)
                                                 .Inc(p => p.AlbumInformation.CommentCount, 1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        public bool Delete_Comment(string albumID, string cmtID)
        {
            try
            {
                var album_filter = Builders<Mongo_Album>.Filter.Eq(p => p._id, new ObjectId(albumID));
                var cmt_filter = Builders<Comment>.Filter.Eq(cmt => cmt.CommentID, cmtID);
                var update = Builders<Mongo_Album>.Update.PullFilter(p => p.CommentList, cmt_filter)
                                                        .Inc(p => p.AlbumInformation.CommentCount, -1); ;
                var result = collection.UpdateOne(album_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        public List<Comment> Get_Comments(string albumID, int skip, int number)
        {
            try
            {
                var result = collection.AsQueryable().FirstOrDefault(p => p.AlbumInformation.AlbumID == albumID);
                return result.CommentList.OrderByDescending(cm => cm.DateCreate).Skip(skip).Take(number).ToList();
            }
            catch
            {
                throw;
            }
        }
        public int Get_Cmt_Count(string albumID)
        {
            try
            {
                var filter = Builders<Mongo_Album>.Filter.Eq(p => p.AlbumInformation.AlbumID, (albumID));
                var result = collection.Find(filter).FirstOrDefault();
                return result.AlbumInformation.CommentCount;
            }
            catch
            {
                throw;
            }
        }
        public bool Add_LikerList(string albumID, SDLink user)
        {
            try
            {
                var filter = Builders<Mongo_Album>.Filter.Eq(p => p._id, new ObjectId(albumID));
                var update = Builders<Mongo_Album>.Update.AddToSet<SDLink>(p => p.LikerList, user)
                                                        .Inc(p => p.AlbumInformation.LikeCount, 1);
                var result = collection.UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }
        public bool Is_User_Liked(string userID, string albumID)
        {
            try
            {
                var filter = Builders<Mongo_Album>.Filter.Where(p => p.AlbumInformation.AlbumID == albumID && p.LikerList.Any(u => u.ID == userID));
                var result = collection.Find(filter).ToList().Count;
                return result != 0;
            }
            catch
            {
                throw;
            }
        }
        public bool Delete_LikerList(string albumID, string userID)
        {
            try
            {
                var post_filter = Builders<Mongo_Album>.Filter.Eq(p => p._id, new ObjectId(albumID));
                var user_filter = Builders<SDLink>.Filter.Eq(u => u.ID, userID);
                var update = Builders<Mongo_Album>.Update.PullFilter(p => p.LikerList, user_filter)
                                                        .Inc(p => p.AlbumInformation.LikeCount, -1); ;
                var result = collection.UpdateOne(post_filter, update);
                return result.IsAcknowledged;
            }
            catch
            {
                throw;
            }
        }

    }
}