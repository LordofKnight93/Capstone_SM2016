using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iVolunteer.Models.SQL;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.DAL.MongoDB;

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
                using(iVolunteerEntities dbEntitties = new iVolunteerEntities())
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
                    return result != null ;
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
        public List<bool> Get_Online_Status(string userID)
        {
            List<bool> statusList = new List<bool>();
            Mongo_User_DAO userDAO = new Mongo_User_DAO();
            try
            {
                List<SDLink> friends = new List<SDLink>();
                friends = userDAO.Get_FriendList(userID);
                int index = 0;
                foreach(var item in friends)
                {
                    if (Is_Friend_Online(item.ID) == true)
                    {
                        statusList.Add(true);
                    }
                    else statusList.Add(false);
                    index++;
                }
                return statusList;
            }
            catch
            {
                throw;
            }
        } 
    }
}