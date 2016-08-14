using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.DAL.MongoDB;
using iVolunteer.Models.MongoDB.CollectionClass;

namespace iVolunteer.DAL.SQL
{
    public class SQL_HubConnection_DAO
    {
        public bool Add_Connection(string userID, string connectionID)
        {
            SQL_HubConnection relation = new SQL_HubConnection();
            relation.UserID = userID;
            relation.ConnectionID = connectionID;
            try
            {
                using (iVolunteerEntities dbEntitties = new iVolunteerEntities())
                {
                    dbEntitties.SQL_HubConnection.Add(relation);
                    dbEntitties.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
        public bool Is_Connected(string connectionID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_HubConnection.FirstOrDefault(rl => rl.ConnectionID == connectionID);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }
        public bool Delete_Connection(string connectionID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_HubConnection.FirstOrDefault(rl => rl.ConnectionID == connectionID);
                    dbEntities.SQL_HubConnection.Remove(result);
                    dbEntities.SaveChanges();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
        public string Get_UserID(string connectionID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_HubConnection.FirstOrDefault(rl => rl.ConnectionID == connectionID);
                    return result.UserID;
                }
            }
            catch
            {
                throw;
            }
        }
        public bool Is_Friend_Online(string friendID)
        {
            try
            {
                using (iVolunteerEntities dbEntities = new iVolunteerEntities())
                {
                    var result = dbEntities.SQL_HubConnection.FirstOrDefault(rl => rl.UserID == friendID);
                    return result != null;
                }
            }
            catch
            {
                throw;
            }
        }
        public List<ChatFriend> Get_Online_Status(List<SDLink> friends, string userID)
        {
            //List<bool> statusList = new List<bool>();
            List<ChatFriend> chatFriend = new List<ChatFriend>();
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            Mongo_Message_DAO messDAO = new Mongo_Message_DAO();
            //int no = 0;
            try
            {
                int index = 0;
                foreach (var item in friends)
                {
                    UnreadItem unread = messDAO.Get_UnreadMss(userID, item.ID);
                    //The two have talked before
                    if (unread != null)
                    {
                        if (Is_Friend_Online(item.ID) == true)
                        {
                            chatFriend.Add(new ChatFriend(item, true, unread));
                        }
                        else
                            chatFriend.Add(new ChatFriend(item, false, unread));
                    }
                    //The two haven't had any conversation before
                    else
                    {
                        if (Is_Friend_Online(item.ID) == true)
                            chatFriend.Add(new ChatFriend(item, true, new UnreadItem()));
                        else
                            chatFriend.Add(new ChatFriend(item, false, new UnreadItem()));
                    }
                    index++;
                }
                return chatFriend;
            }
            catch
            {
                throw;
            }
        }
    }
}