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
                        "~/Scripts/DataTables-1.9.4/media/js/jquery.dataTables.js",
                        "~/Scripts/jquery.jeditable.js",
                        "~/Scripts/jquery.dataTables.editable.js", //downloaded separately from http://code.google.com/p/jquery-datatables-editable/ to allow editing of datatables
                        "~/Scripts/DataTables-1.9.4/extras/TableTools/media/js/ZeroClipboard.js",
                        "~/Scripts/DataTables-1.9.4/extras/TableTools/media/js/TableTools.js"));

            bundles.Add(new ScriptBundle("~/bundles/AutoGrow").Include(
                        "~/Scripts/autogrow-1.0.2/autogrow.js")); //A plugin so that the notes textbox grows as the user types..

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/DataTablesCustomFilter").Include(//adds the ability to do simultaneously search multiple fields on a datatable
                        "~/Scripts/jquery.dataTables.custom-filter.js"));

            bundles.Add(new ScriptBundle("~/bundles/DataTablesColumnFilter").Include(//a column filter addon from http://jquery-datatables-column-filter.googlecode.com/svn/trunk/index.html
                        "~/Scripts/jquery.dataTables.columnFilter.js"));

            bundles.Add(new ScriptBundle("~/bundles/DataTablesColReorderResize").Include(//a column reorder and resize addon from http://jquery-datatables-column-filter.googlecode.com/svn/trunk/index.html
                        "~/Scripts/ColReorderWithResize.js"));
            //bundles.Add(new ScriptBundle("~/bundles/DataTablesColReorderResize").Include(//column reorder from datatables...
            //            "~/Scripts/DataTables-1.9.4/extras/ColReorder/media/js/ColReorder.js"));

            //I created the below bundles so that I could keep the javascript off the Search pages for the respective controllers.
            bundles.Add(new ScriptBundle("~/bundles/SearchCustomerOrders").Include(
                        "~/Scripts/DateTimeFormatter.js",       //This is a custom script that allows for flexible formatting of Javascript Date/Time objects
                        "~/Scripts/SearchCustomerOrders.js"));
            bundles.Add(new ScriptBundle("~/bundles/SearchPurchaseOrders").Include(
                        "~/Scripts/DateTimeFormatter.js",
                        "~/Scripts/SearchPurchaseOrders.js"));
            bundles.Add(new ScriptBundle("~/bundles/SearchTransferOrders").Include(
                        "~/Scripts/DateTimeFormatter.js",
                        "~/Scripts/SearchTransferOrders.js"));
            bundles.Add(new ScriptBundle("~/bundles/SearchInventory").Include(
                        "~/Scripts/SearchInventory.js"));
            bundles.Add(new ScriptBundle("~/bundles/SearchVendorRequests").Include(
                        "~/Scripts/DateTimeFormatter.js",
                        "~/Scripts/SearchVendorRequests.js"));
            //For the Shared Notes Viewer
            bundles.Add(new ScriptBundle("~/bundles/SharedNotesViewer").Include(
                        "~/Scripts/DateTimeFormatter.js",
                        "~/Scripts/SharedNotesViewer.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/reset.css",
                        "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.all.css",
                        "~/Content/themes/flick/jquery-ui-1.10.3.custom.css",
                        "~/Content/DataTables-1.9.4/media/css/jquery.dataTables_themeroller.css"
                        ));

            //bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
            //            "~/Content/themes/base/jquery.ui.core.css",
            //            "~/Content/themes/base/jquery.ui.resizable.css",
            //            "~/Content/themes/base/jquery.ui.selectable.css",
            //            "~/Content/themes/base/jquery.ui.accordion.css",
            //            "~/Content/themes/base/jquery.ui.autocomplete.css",
            //            "~/Content/themes/base/jquery.ui.button.css",
            //            "~/Content/themes/base/jquery.ui.dialog.css",
            //            "~/Content/themes/base/jquery.ui.slider.css",
            //            "~/Content/themes/base/jquery.ui.tabs.css",
            //            "~/Content/themes/base/jquery.ui.datepicker.css",
            //            "~/Content/themes/base/jquery.ui.progressbar.css",
            //            "~/Content/themes/base/jquery.ui.theme.css",
            //    "~/Content/DataTables-1.9.4/media/css/jquery.dataTables.css"));
        }
    }
}