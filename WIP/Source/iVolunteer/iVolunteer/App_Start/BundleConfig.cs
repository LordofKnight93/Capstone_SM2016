using System.Web;
using System.Web.Optimization;


namespace iVolunteer
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/scripts/jquery-2.1.4.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
         "~/scripts/jquery-ui-1.11.4.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/scripts/bootstrap.js",
                      "~/scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/jQuery-File-Upload").Include(
                    "~/Content/jQuery.FileUpload/css/jquery.fileupload.css",
                   "~/Content/jQuery.FileUpload/css/jquery.fileupload-ui.css",
                   "~/Content/blueimp-gallery2/css/blueimp-gallery.css",
                     "~/Content/blueimp-gallery2/css/blueimp-gallery-video.css",
                       "~/Content/blueimp-gallery2/css/blueimp-gallery-indicator.css"
                   ));

            bundles.Add(new ScriptBundle("~/bundles/jQuery-File-Upload").Include(
                     //<!-- The Templates plugin is included to render the upload/download listings -->
                     "~/scripts/jQuery.FileUpload/vendor/jquery.ui.widget.js",
                       "~/scripts/jQuery.FileUpload/tmpl.min.js",
//<!-- The Load Image plugin is included for the preview images and image resizing functionality -->
"~/scripts/jQuery.FileUpload/load-image.all.min.js",
//<!-- The Canvas to Blob plugin is included for image resizing functionality -->
"~/scripts/jQuery.FileUpload/canvas-to-blob.min.js",
//"~/Scripts/file-upload/jquery.blueimp-gallery.min.js",
//<!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
"~/scripts/jQuery.FileUpload/jquery.iframe-transport.js",
//<!-- The basic File Upload plugin -->
"~/scripts/jQuery.FileUpload/jquery.fileupload.js",
//<!-- The File Upload processing plugin -->
"~/scripts/jQuery.FileUpload/jquery.fileupload-process.js",
//<!-- The File Upload image preview & resize plugin -->
"~/scripts/jQuery.FileUpload/jquery.fileupload-image.js",
//<!-- The File Upload audio preview plugin -->
"~/scripts/jQuery.FileUpload/jquery.fileupload-audio.js",
//<!-- The File Upload video preview plugin -->
"~/scripts/jQuery.FileUpload/jquery.fileupload-video.js",
//<!-- The File Upload validation plugin -->
"~/scripts/jQuery.FileUpload/jquery.fileupload-validate.js",
//!-- The File Upload user interface plugin -->
"~/scripts/jQuery.FileUpload/jquery.fileupload-ui.js",
//Blueimp Gallery 2 
"~/scripts/blueimp-gallery2/js/blueimp-gallery.js",
"~/scripts/blueimp-gallery2/js/blueimp-gallery-video.js",
"~/scripts/blueimp-gallery2/js/blueimp-gallery-indicator.js",
"~/scripts/blueimp-gallery2/js/jquery.blueimp-gallery.js"

));
            bundles.Add(new ScriptBundle("~/bundles/Blueimp-Gallerry2").Include(//Blueimp Gallery 2 
"~/scripts/blueimp-gallery2/js/blueimp-gallery.js",
"~/scripts/blueimp-gallery2/js/blueimp-gallery-video.js",
"~/scripts/blueimp-gallery2/js/blueimp-gallery-indicator.js",
"~/scripts/blueimp-gallery2/js/jquery.blueimp-gallery.js"));
        }
    }
}
