using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using iVolunteer.DAL.SQL;
using iVolunteer.DAL.MongoDB;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;

namespace iVolunteer.Hubs
{
    public class ChatHub : Hub
    {
        /// <summary>
        /// Announce that User has Onlined.
        /// </summary>
        /// <param name="userID"></param>
        public void ConnectToHub(string userID)
        {
            string connectionID = Context.ConnectionId;
            SQL_HubConnection_DAO connDAO = new SQL_HubConnection_DAO();
            try
            {
                bool isConnected = connDAO.Is_Connected(connectionID);
                if (isConnected == false)
                {
                    connDAO.Add_Connection(userID, connectionID);
                }
                else return;
            }
            catch
            {
                throw;
            }
            Clients.Others.userHasConnected(userID);
        }
        /// <summary>
        /// Announce that User has OffLined 
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled)
            {
                string connectionID = Context.ConnectionId;
                try
                {
                    SQL_HubConnection_DAO connDAO = new SQL_HubConnection_DAO();
                    // Who has disconnected???
                    string userID = connDAO.Get_UserID(connectionID);
                    connDAO.Delete_Connection(connectionID);

                    Clients.Others.userHasDisconnected(userID);
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                Console.WriteLine(String.Format("Client {0} timed out .", Context.ConnectionId));
            }

            return base.OnDisconnected(stopCalled);
        }
        public void SendMessage(string userID, string friendID, string content, string displayTime)
        {
            Clients.Others.getMessage(userID, friendID, content, displayTime);
        }
        public void GetFriendList(string userID)
        {
            List<bool> isOnline = new List<bool>();
            try
            {
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                List<SDLink> friendList = userDAO.Get_FriendList(userID);
                SQL_HubConnection_DAO connDAO = new SQL_HubConnection_DAO();
                List<bool> statusList = connDAO.Get_Online_Status(userID);

                Clients.Caller.receiveFriendList(JsonConvert.SerializeObject(friendList), JsonConvert.SerializeObject(statusList));
            }
            catch
            {
                throw;
            }
        }
    }

}