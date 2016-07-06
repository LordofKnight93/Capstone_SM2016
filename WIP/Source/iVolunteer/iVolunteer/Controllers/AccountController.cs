using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;
using System.Web.Mvc;
using iVolunteer.Models.SQL;
using iVolunteer.Models.ViewModel;
using iVolunteer.Models.MongoDB.CollectionClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ItemClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.ListClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.StructureClass;
using iVolunteer.DAL.SQL;
using iVolunteer.DAL.MongoDB;
using iVolunteer.Common;
using System.IO;

namespace iVolunteer.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// confirmation for user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public ActionResult Confirm(string userID)
        {
            try
            {
                SQL_Account_DAO sqlDAO = new SQL_Account_DAO();
                sqlDAO.Set_Confirmation_Status(userID, Status.IS_CONFIRMED);
                Mongo_User_DAO mongoDAO = new Mongo_User_DAO();
                mongoDAO.Set_Comfirmation_Status(userID, Status.IS_CONFIRMED);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
            return RedirectToAction("Login", "Home");
        }
        /// <summary>
        /// child action, craete left navigation panel
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult NavigationPanel()
        {
            try
            {
                string userID = Session["UserID"].ToString();
                Mongo_User_DAO mongoDAO = new Mongo_User_DAO();
                var result = mongoDAO.Get_AccountInformation(userID);
                return PartialView("_NavigationPanel", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// child action, get current projects for navigation panel
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult CurrentProjects()
        {
            try
            {
                string userID = Session["UserID"].ToString();
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_CurrentProjects(userID);
                return PartialView("_CurrentProjects", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// child action, get joined groups for navigation panel
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult JoinedGroups()
        {
            try
            {
                string userID = Session["UserID"].ToString();
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_JoinedGroups(userID);
                return PartialView("_JoinedGroups", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }
        /// <summary>
        /// child action, get joined projects for navigation panel
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult JoinedProjects()
        {
            try
            {
                string userID = Session["UserID"].ToString();
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_JoinedProjects(userID);
                return PartialView("_JoinedProjects", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult RequestList()
        {
            try
            {
                string userID = Session["UserID"].ToString();
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                var result = userDAO.Get_JoinedProjects(userID);
                return PartialView("_JoinedProjects", result);
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return PartialView("ErrorMessage");
            }
        }

        public ActionResult DeniedRequest(string requestID)
        {
            try
            {
                string userID = Session["UserID"].ToString();
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                //get request
                var request = userDAO.Get_Request(userID, requestID);
                //transaction
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        if (request.Type == RequestType.FRIEND_REQUEST)
                        {
                            SQL_Friendship_DAO relationDAO = new SQL_Friendship_DAO();
                            //delete relation
                            relationDAO.Delete_Specific_Relation(userID, request.Actor.ID);
                            relationDAO.Delete_Specific_Relation(request.Actor.ID, userID);
                        }

                        if (request.Type == RequestType.INVITE_TO)
                        {
                            SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();
                            //delete relation
                            relationDAO.Delete_Specific_Relation(userID, request.Destination.ID, Relation.MEMBER_RELATION);
                        }
                        //delete request
                        userDAO.Delete_Request(userID, requestID);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return View("ErrorMessage");
                    }
                }

                return RedirectToAction("RequestList", "Account");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }

        public ActionResult AcceptRequest(string requestID)
        {
            try
            {
                string userID = Session["UserID"].ToString();
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                //get request
                var request = userDAO.Get_Request(userID, requestID);
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        if (request.Type == RequestType.FRIEND_REQUEST)
                        {
                            SQL_Friendship_DAO relationDAO = new SQL_Friendship_DAO();
                             //update relation
                             relationDAO.Update_Relation(userID, request.Actor.ID, Status.ACCEPTED);
                             relationDAO.Update_Relation(request.Actor.ID, userID, Status.ACCEPTED);
                             //add friend to list
                            userDAO.Add_Friend(request.Actor.ID, request.Destination);
                            userDAO.Add_Friend(request.Destination.ID, request.Actor);
                        }

                        if (request.Type == RequestType.INVITE_TO)
                        {
                             SQL_AcPr_Relation_DAO relationDAO = new SQL_AcPr_Relation_DAO();

                             //update relation
                             relationDAO.Update_Relation(userID, request.Destination.ID, Relation.MEMBER_RELATION, Status.ACCEPTED);
                             //add to mongBD
                             userDAO.Add_CurrentProject(userID, request.Destination);
                             userDAO.Add_JoinedProject_ActivityHistory(userID, request.Destination);
                        }

                        //delete request
                        userDAO.Delete_Request(userID, requestID);
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return View("ErrorMessage");
                    }
                }

                return RedirectToAction("RequestList", "Account");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }

        public ActionResult FriendRequest(string otherID)
        {
            try
            {
                //create actor
                SDLink actor = new SDLink();
                actor.ID = Session["UserID"].ToString();
                actor.DisplayName = Session["DisplayName"].ToString();
                actor.Handler = Handler.USER;
                // create sql relation
                SQL_Friendship relation1 = new SQL_Friendship();
                relation1.UserID = actor.ID;
                relation1.FriendID = otherID;
                relation1.Status = Status.PENDING;

                SQL_Friendship relation2 = new SQL_Friendship();
                relation2.UserID = otherID;
                relation2.FriendID = actor.ID;
                relation2.Status = Status.PENDING;


                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                //create destination
                SDLink destination = userDAO.Get_SDLink(otherID);
                //create request
                RequestItem request = new RequestItem(actor, RequestType.FRIEND_REQUEST, destination);
                //write to DB
                using (var transaction = new TransactionScope())
                {
                    try
                    {
                        SQL_Friendship_DAO relationDAO = new SQL_Friendship_DAO();
                        relationDAO.Add_Relation(relation1);
                        relationDAO.Add_Relation(relation2);

                        userDAO.Add_Request(otherID, request);

                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                        ViewBag.Message = Error.UNEXPECT_ERROR;
                        return View("ErrorMessage");
                    }
                }

                return PartialView("_RequestList");
            }
            catch
            {
                ViewBag.Message = Error.UNEXPECT_ERROR;
                return View("ErrorMessage");
            }
        }

    }
}