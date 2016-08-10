using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using iVolunteer.Models.MongoDB.CollectionClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.InformationClass;
using iVolunteer.Models.MongoDB.EmbeddedClass.LinkClass;
using iVolunteer.DAL.MongoDB;
using iVolunteer.Models.SQL;
using System.Transactions;
using iVolunteer.DAL.SQL;

namespace iVolunteer.Helpers
{
    public class FilesHelper
    {

        String DeleteURL = null;
        String DeleteType = null;
        String StorageRoot = null;
        String UrlBase = null;
        String tempPath = null;
        //ex:"~/Files/something/";
        String serverMapPath = null;
        public FilesHelper(String DeleteURL, String DeleteType, String StorageRoot, String UrlBase, String tempPath, String serverMapPath)
        {
            this.DeleteURL = DeleteURL;
            this.DeleteType = DeleteType;
            this.StorageRoot = StorageRoot;
            this.UrlBase = UrlBase;
            this.tempPath = tempPath;
            this.serverMapPath = serverMapPath;
        }

        public void DeleteFiles(String pathToDelete)
        {
         
            string path = HostingEnvironment.MapPath(pathToDelete);

            System.Diagnostics.Debug.WriteLine(path);
            if (Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo fi in di.GetFiles())
                {
                    System.IO.File.Delete(fi.FullName);
                    System.Diagnostics.Debug.WriteLine(fi.Name);
                }

                di.Delete(true);
            }
        }

        public String DeleteFile(String file)
        {
            System.Diagnostics.Debug.WriteLine("DeleteFile");
            //    var req = HttpContext.Current;
            System.Diagnostics.Debug.WriteLine(file);
 
            String fullPath = Path.Combine(StorageRoot, file);
            System.Diagnostics.Debug.WriteLine(fullPath);
            System.Diagnostics.Debug.WriteLine(System.IO.File.Exists(fullPath));
            String thumbPath = "/" + file;
            String partThumb1 = Path.Combine(StorageRoot, "thumbs");
            String partThumb2 = Path.Combine(partThumb1, file);

            System.Diagnostics.Debug.WriteLine(partThumb2);
            System.Diagnostics.Debug.WriteLine(System.IO.File.Exists(partThumb2));
            if (System.IO.File.Exists(fullPath))
            {
                //delete thumb 
                if (System.IO.File.Exists(partThumb2))
                {
                    System.IO.File.Delete(partThumb2);
                }
                System.IO.File.Delete(fullPath);
                String succesMessage = "Ok";
                return succesMessage;
            }
            String failMessage = "Error Delete";
            return failMessage;
        }
        public JsonFiles GetFileList(string albumID)
        {

            var r = new List<ImageInformation>();
            String fullPath = Path.Combine(StorageRoot);
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo dir = new DirectoryInfo(fullPath);
                foreach (FileInfo file in dir.GetFiles())
                {
                    Mongo_Album_DAO albumDAO = new Mongo_Album_DAO();
                    if (albumDAO.Check_Image_By_Name(file.Name,albumID) == true)
                    {
                        int SizeInt = unchecked((int)file.Length);
                        r.Add(UploadResult(file.Name, SizeInt, file.FullName));
                    }
   
                }

            }
            JsonFiles files = new JsonFiles(r);

            return files;
        }

        public void UploadAndShowResults(HttpContextBase ContentBase, List<ImageInformation> resultList)
        {
            var httpRequest = ContentBase.Request;
          
            System.Diagnostics.Debug.WriteLine(Directory.Exists(tempPath));

            String fullPath = Path.Combine(StorageRoot);
            Directory.CreateDirectory(fullPath);
            // Create new folder for thumbs
            Directory.CreateDirectory(fullPath + "/thumbs/");

            foreach (String inputTagName in httpRequest.Files)
            {

                var headers = httpRequest.Headers;

                var file = httpRequest.Files[inputTagName];
                System.Diagnostics.Debug.WriteLine(file.FileName);

                if (string.IsNullOrEmpty(headers["X-File-Name"]))
                {

                    UploadWholeFile(ContentBase, resultList);
                }
                else
                {

                    UploadPartialFile(headers["X-File-Name"], ContentBase, resultList);
                }
            }
        }


        private void UploadWholeFile(HttpContextBase requestContext, List<ImageInformation> statuses)
        {

            var request = requestContext.Request;
            

            for (int i = 0; i < request.Files.Count; i++)
            {
                var file = request.Files[i];
                var extension = Path.GetExtension(file.FileName);

                String pathOnServer = Path.Combine(StorageRoot);
                // Path.GetExtension()
               
                var guidImageName = string.Format("{0}{1}", Guid.NewGuid().ToString("N"), extension);
                string[] arrayImageID = guidImageName.Split('.');
                var fullPath = Path.Combine(pathOnServer, guidImageName);
                //
               /* int fileSize = file.ContentLength;
                if (fileSize > (maxFileSize*1024))
                {
                    PanelError.Visible = true;
                    lblError.Text = "Filesize of image is too large. Maximum file size permitted is " + maxFileSize + "KB";
                    return;
                }*/
                file.SaveAs(fullPath);            
                
                //Create thumb
                string[] imageArray = file.FileName.Split('.');
                if (imageArray.Length != 0)
                {
                    String extansion = imageArray[imageArray.Length - 1];
                    if (extansion != "jpg" && extansion != "png") //Do not create thumb if file is not an image
                    {
                        
                    }
                    else
                    {
                        var ThumbfullPath = Path.Combine(pathOnServer, "thumbs");
                        String fileThumb = guidImageName;
                        var ThumbfullPath2 = Path.Combine(ThumbfullPath, fileThumb);
                        using (MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(fullPath)))
                        {
                            var thumbnail = new WebImage(stream).Resize(80, 80);
                            thumbnail.Save(ThumbfullPath2, "jpg");
                        }
                    }
                }

                var result = UploadResult(guidImageName, file.ContentLength, fullPath);
                statuses.Add(result);

                //Add image in DB
                //Session user and album
                string userID = request.RequestContext.HttpContext.Session["UserID"].ToString();
                string albumID = request.RequestContext.HttpContext.Session["Album"].ToString();
                Mongo_User_DAO userDAO = new Mongo_User_DAO();
                SDLink creator = userDAO.Get_SDLink(userID);
                Mongo_Album_DAO albumDAO = new Mongo_Album_DAO();
                SDLink album = albumDAO.Get_SDLink(albumID);

                //Mongo Image
                // ImageInAlbum ImageAlbum = new ImageInAlbum(album);
                Mongo_Image Image = new Mongo_Image();

                //Create mongo Image
                ImageInformation mongo_Image = new ImageInformation();
                mongo_Image.Album = album;
                mongo_Image.Creator = creator;
                mongo_Image.DateCreate = DateTime.Now.ToLocalTime();
                mongo_Image.ImageID = arrayImageID[0];
                mongo_Image.ImageName = result.ImageName;
                mongo_Image.ImageSize = result.ImageSize;
                mongo_Image.ImageType = result.ImageType;
                mongo_Image.ImageUrl = result.ImageUrl;
                mongo_Image.ImageDelete_url = result.ImageDelete_url;
                mongo_Image.ImageThumbnail_url = result.ImageThumbnail_url;
                mongo_Image.ImageDelete_type = result.ImageDelete_type;
                //create sql Image
                SQL_Image sql_Image = new SQL_Image();
                sql_Image.ImageID = arrayImageID[0];
                sql_Image.AlbumID = albumID;
                //start transaction
                try
                {
                    using (var transaction = new TransactionScope())
                    {
                        try
                        {
                            // create DAO instance
                            Mongo_Album_DAO mongo_Album_DAO = new Mongo_Album_DAO();
                            SQL_Album_DAO sql_Album_DAO = new SQL_Album_DAO();
                            SQL_AcIm_Relation_DAO sql_User_Image_DAO = new SQL_AcIm_Relation_DAO();
                            //write data to db
                            sql_Album_DAO.Add_Image(sql_Image);
                            sql_User_Image_DAO.Add_Creator(userID, sql_Image.ImageID);
                            mongo_Album_DAO.Add_Image(albumID,mongo_Image);
                            transaction.Complete();
                        }
                        catch
                        {
                            transaction.Dispose();
                            throw;
                        }
                    }

                }
                catch
                {
                    throw;
                }
            }
        }



        private void UploadPartialFile(string fileName, HttpContextBase requestContext, List<ImageInformation> statuses)
        {
            var request = requestContext.Request;
            if (request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            var file = request.Files[0];
            var inputStream = file.InputStream;
            String patchOnServer = Path.Combine(StorageRoot);
            var fullName = Path.Combine(patchOnServer, Path.GetFileName(file.FileName));
            var ThumbfullPath = Path.Combine(fullName, Path.GetFileName(file.FileName));
            ImageHandler handler = new ImageHandler();

            var ImageBit = ImageHandler.LoadImage(fullName);
            handler.Save(ImageBit, 80, 80, 10, ThumbfullPath);
            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }
            statuses.Add(UploadResult(file.FileName, file.ContentLength, file.FileName));
        }
        public ImageInformation UploadResult(String FileName,int fileSize,String FileFullPath)
        {

            String getType = System.Web.MimeMapping.GetMimeMapping(FileFullPath);
            var result = new ImageInformation()
            {
                ImageName= FileName,
                ImageSize = fileSize,
                ImageType = getType,
                ImageUrl = UrlBase + FileName,
                ImageDelete_url = DeleteURL + FileName,
                ImageThumbnail_url = CheckThumb(getType, FileName),
                ImageDelete_type = DeleteType,
            };
            return result;
        }

        public String CheckThumb(String type,String FileName)
        {
            var splited = type.Split('/');
            if (splited.Length == 2)
            {
                string extansion = splited[1];
                if(extansion.Equals("jpeg") || extansion.Equals("jpg") || extansion.Equals("png") || extansion.Equals("gif"))
                {
                    String thumbnailUrl = UrlBase + "/thumbs/" + FileName;
                    return thumbnailUrl;
                }
                else
                {
                    if (extansion.Equals("octet-stream")) //Fix for exe files
                    {
                        return "/Content/Free-file-icons/48px/exe.png";

                    }
                    if (extansion.Contains("zip")) //Fix for exe files
                    {
                        return "/Content/Free-file-icons/48px/zip.png";
                    }
                    String thumbnailUrl = "/Content/Free-file-icons/48px/"+ extansion +".png";
                    return thumbnailUrl;
                }
            }
            else
            {
                return UrlBase + "/thumbs/" + FileName + ".100x100.jpg";
            }
           
        }
        public List<String> FilesList()
        {

            List<String> Filess = new List<String>();
            string path = HostingEnvironment.MapPath(serverMapPath);
            System.Diagnostics.Debug.WriteLine(path);
            if (Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo fi in di.GetFiles())
                {
                    Filess.Add(fi.Name);
                    System.Diagnostics.Debug.WriteLine(fi.Name);
                }

            }
            return Filess;
        }
    }
    public class ImageInformation
    {
        public string ImageID { get; set;}
        public string ImageName { get; set; }
        public SDLink Creator { get; set; }
        public SDLink Album { get; set; }
        public DateTime DateCreate { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public string Content { get; set; }
        public int ImageSize { get; set; }
        public string ImageType { get; set; }
        public string ImageUrl { get; set; }
        public string ImageDelete_url { get; set; }
        public string ImageThumbnail_url { get; set; }
        public string ImageDelete_type { get; set; }

    }
    public class JsonFiles
    {
        public ImageInformation[] files;
        private ImageInformation list;

        public string TempFolder { get; set; }
        public JsonFiles(List<ImageInformation> filesList)
        {
            files = new ImageInformation[filesList.Count];
            for (int i = 0; i < filesList.Count; i++)
            {
                files[i] = filesList.ElementAt(i);
            }

        }

        public JsonFiles(ImageInformation list)
        {
            this.list = list;
        }
    }
}

    