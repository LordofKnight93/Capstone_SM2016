using iVolunteer.Helpers;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.DAL.MongoDB;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using iVolunteer.Models.MongoDB.CollectionClass;
using iVolunteer.Models.SQL;
using System.Transactions;
using iVolunteer.DAL.SQL;
using iVolunteer.Common;

namespace iVolunteer.Controllers
{
    public class AlbumController : Controller
    {
        FilesHelper filesHelper;
        String tempPath = "~/Album/somefiles/";
        String serverMapPath = "~/Images/Album/somefiles/";
        private string StorageRoot
        {
            get { return Path.Combine(HostingEnvironment.MapPath(serverMapPath)); }
        }
        private string UrlBase = "/Images/Album/somefiles/";
        String DeleteURL = "/Album/DeleteFile/?file=";
        String DeleteType = "GET";
        public AlbumController()
        {
                filesHelper = new FilesHelper(DeleteURL, DeleteType, StorageRoot, UrlBase, tempPath, serverMapPath);           
        }

        //public ActionResult Image(string albumID)
        //{
        //    ViewBag.AlbumID = albumID;
        //    Session["Album"] = albumID;
        //    return View("Image");
        //}
        //public ActionResult EditAlbum(string albumID,string targetID)
        //{
        //    ViewBag.targetID = targetID;
        //    ViewBag.AlbumID = albumID;
        //    Session["Album"] = albumID;
        //    return View("EditAlbum");
        //}
        //public ActionResult ShowAlbum(string albumID)
        //{
        //    string userID = Session["UserID"].ToString();
        //    ImageInformation mongo_Image = new ImageInformation();
        //    Mongo_Album_DAO mongo_Album_DAO = new Mongo_Album_DAO();
        //    var  model = mongo_Album_DAO.Get_Image_By_AlbumID(albumID);
        //    ViewBag.AlbumID = albumID;
        //    Session["Album"] = albumID;
        //    return View("ShowAlbum", model);
        //}

        //[HttpGet]
        //public ActionResult CreateAlbum(string targetID)
        //{
        //    ViewBag.TargetID = targetID;
        //    return PartialView("_AlbumCreate");
        //}
        //[HttpPost]
        //public ActionResult CreateAlbum(AlbumInformation albumInfo, string targetID, int targetType)
        //{
        //    if (!ModelState.IsValid) {
        //        ViewBag.TargetID = targetID;
        //        return PartialView("_AlbumCreate", albumInfo);
        //    };
        //    //create album creator
        //    string userID = Session["UserID"].ToString();
        //    Mongo_User_DAO userDAO = new Mongo_User_DAO();
        //    SDLink creator = userDAO.Get_SDLink(userID);

        //    albumInfo.DateCreate = DateTime.Now;
        //    albumInfo.DateLastActivity = DateTime.Now;


        //    //Create mongo Album
        //    Mongo_Album mongo_Album = new Mongo_Album(albumInfo);
        //    mongo_Album.AlbumInformation = albumInfo;
        //    mongo_Album.AlbumInformation.AlbumID = mongo_Album._id.ToString();
        //    mongo_Album.AlbumInformation.Creator = creator;
        //    //Create sql Album
        //    SQL_Album sql_Album = new SQL_Album();
        //    sql_Album.AlbumID = mongo_Album._id.ToString();
        //    if (targetType == 1)
        //    {
        //        sql_Album.GroupID = targetID;
        //    }
        //    else if (targetType == 2)
        //    {
        //        sql_Album.ProjectID = targetID;
        //    }
        //    //start transaction
        //    try
        //    {
        //        using (var transaction = new TransactionScope())
        //        {
        //            try
        //            {
        //                // create DAO instance
        //                Mongo_Album_DAO mongo_Album_DAO = new Mongo_Album_DAO();
        //                SQL_Album_DAO sql_Album_DAO = new SQL_Album_DAO();
        //                SQL_AcAl_Relation_DAO sql_User_Album_DAO = new SQL_AcAl_Relation_DAO();
        //                //write data to db
        //                sql_Album_DAO.Add_Album(sql_Album);
        //                sql_User_Album_DAO.Add_Creator(userID, sql_Album.AlbumID);
        //                mongo_Album_DAO.Add_Album(mongo_Album);
        //                transaction.Complete();
        //            }
        //            catch
        //            {
        //                transaction.Dispose();
        //                return PartialView("ErrorMessage");
        //            }
        //        }
        //        ViewBag.AlbumID = sql_Album.AlbumID;
        //        Session["Album"] = sql_Album.AlbumID;
        //       // return RedirectToAction("GroupHome", "Group", new { groupID = targetID });
        //        return JavaScript("window.location = '" + Url.Action("Image", "Album", new { albumID = sql_Album.AlbumID }) + "'");
        //    }
        //    catch (Exception e)
        //    {
        //        ViewBag.Message = e.ToString();
        //        return PartialView("ErrorMessage");
        //        throw;
        //    }
        //}


        public ActionResult DeleteImage(string albumID, string imageID, string targetID)
        {

            if (Session["UserID"] == null)
            {
                ViewBag.Message = Error.ACCESS_DENIED;
                return PartialView("ErrorMessage");
            }
            string userID = Session["UserID"].ToString();
            Mongo_Album_DAO albumDAO = new Mongo_Album_DAO();
            SQL_AcIm_Relation_DAO relationIm = new SQL_AcIm_Relation_DAO();
            SQL_Album_DAO sqlAlbum = new SQL_Album_DAO();

            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    relationIm.Delete_relation_Im(userID, imageID, AcImRelation.CREATOR_RELATION);
                    sqlAlbum.Delete_Single_Image(imageID);
                    albumDAO.Delete_Image(imageID,albumID);
                    trans.Complete();
                }
                catch
                {
                    trans.Dispose();
                    throw;
                }
            }
            return JavaScript("window.location = '" + Url.Action("EditAlbum", "Album", new { albumID = albumID ,targetID = targetID}) + "'");
        }
        [HttpPost]
        public JsonResult Upload()
        {
            var resultList = new List<ImageInformation>();
            
            var CurrentContext = HttpContext;

             filesHelper.UploadAndShowResults(CurrentContext, resultList);

            JsonFiles files = new JsonFiles(resultList);
            
            bool isEmpty = !resultList.Any();
            if (isEmpty)
            {
                return Json("Error ");
            }
            else
            {            
                return Json(files);
            }
        }
        public JsonResult GetFileList()
        {
            string albumID = Session["Album"].ToString();
            var list = filesHelper.GetFileList(albumID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult DeleteFile(string file,string albumID)
        {
            albumID = Session["Album"].ToString();
            string userID = Session["UserID"].ToString();
            Mongo_Album_DAO albumDAO = new Mongo_Album_DAO();
            SQL_AcIm_Relation_DAO relationIm = new SQL_AcIm_Relation_DAO();
            SQL_Album_DAO sqlAlbum = new SQL_Album_DAO();
            string imageID = albumDAO.Get_Image_By_AlbumID_Name(file, albumID, userID);
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    relationIm.Delete_relation_Im(userID,imageID, AcImRelation.CREATOR_RELATION);
                    sqlAlbum.Delete_Single_Image(imageID);
                    albumDAO.Delete_Image(imageID, albumID);
                    trans.Complete();
                }
                catch
                {
                    trans.Dispose();
                    throw;
                }
            }
            filesHelper.DeleteFile(file);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

    }
}