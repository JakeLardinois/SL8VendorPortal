using System.Web;
using System.Web.Optimization;

namespace SL8VendorPortal
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/JSON").Include(
                        "~/Scripts/json2.js")); //This needed to be added, else it would error: 'JSON' is undefined

            bundles.Add(new ScriptBundle("~/bundles/DataTables").Include(
                        "~/Scripts/DataTables-1.9.4/media/js/jquery.dataTables.js"));

            //I created the below bundles so that I could keep the javascript off the Search pages for the respective controllers.
            bundles.Add(new ScriptBundle("~/bundles/SearchCustomerOrders").Include(
                        "~/Scripts/SearchCustomerOrders.js"));
            bundles.Add(new ScriptBundle("~/bundles/SearchPurchaseOrders").Include(
                        "~/Scripts/SearchPurchaseOrders.js"));
            bundles.Add(new ScriptBundle("~/bundles/SearchTransferOrders").Include(
                        "~/Scripts/SearchTransferOrders.js"));
            //For the Shared Notes Viewer
            bundles.Add(new ScriptBundle("~/bundles/SharedNotesViewer").Include(
                        "~/Scripts/SharedNotesViewer.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css",
                        "~/Content/DataTables-1.9.4/media/css/jquery.dataTables.css"));
        }
    }
}