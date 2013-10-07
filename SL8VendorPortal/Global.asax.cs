using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using System.Data.Entity;
using SL8VendorPortal.Models;
using jQuery.DataTables.Mvc;


namespace SL8VendorPortal
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //These classes are located in App_Start
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            Database.SetInitializer<SL8VendorPortalDb>(new SL8VendorPortalDbInitializer());

            //THIS IS REQUIRED TO USE THE DATATABLES DATAGRID!
            // Lets MVC know that anytime there is a JQueryDataTablesModel as a parameter in an action to use the
            // JQueryDataTablesModelBinder when binding the model.
            ModelBinders.Binders.Add(typeof(JQueryDataTablesModel), new JQueryDataTablesModelBinder());
        }
    }

    //public class SL8VendorPortalDbInitializer : DropCreateDatabaseIfModelChanges<SL8VendorPortalDb>
    //public class SL8VendorPortalDbInitializer : DropCreateDatabaseAlways<SL8VendorPortalDb>
    public class SL8VendorPortalDbInitializer : DropCreateDatabaseIfModelChanges<SL8VendorPortalDb>
    {
        /*This method passes in the context of the database that was referenced in the initialization strategy above.
         */
        protected override void Seed(SL8VendorPortalDb context)
        {
            base.Seed(context);

            //context.PendingTransferOrderTransactions.Add(new PendingTransferOrderTransaction { Description = "My Description" });

            //context.SaveChanges();
        }

    }
}