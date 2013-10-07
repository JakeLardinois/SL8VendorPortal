using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SL8VendorPortal.Models;

using WebMatrix.Data;
using WebMatrix.WebData;
using System.Web.Security;
using SL8VendorPortal.Filters;



namespace SL8VendorPortal.Controllers
{
    /*The [InitializeSimpleMembership] data annotation is what is required in order to call anything from the 'WebSecurity' class. Of also importance when searching for data about the currently logged in user are
     * the 'Roles', 'User', and 'Request' classes (ex Request.IsAuthenticated, User.Identity.Name, Roles.IsUserInRole, etc.).
     *      //dynamic users;
            //dynamic user;
            //var Username = WebSecurity.CurrentUserName;
            //using (var db = Database.Open("SecurityConnection"))
            //{
            //    users = db.Query("SELECT * FROM Users WHERE UserID = " + WebSecurity.CurrentUserId);
            //    user = db.QuerySingle("SELECT * FROM Users WHERE UserID = " + WebSecurity.CurrentUserId);
                
            //    if (user != null)
            //    {
            //        strTemp = user["Email"];
            //    }
                
            //}
     
    [InitializeSimpleMembership]*/
    public class HomeController : Controller
    {
        private SL8VendorPortalDb db = new SL8VendorPortalDb();


        public ActionResult Index()
        {
            /*The below can also be done using Razr syntax on the view
             *  @if(Request.IsAuthenticated) {
                    <text>Welcome <strong>@User.Identity.Name</strong> 
                    @(User.IsInRole("Administrator") ? "(Administrator)" : String.Empty) 
                    [ @Html.ActionLink("Log Off", "LogOff", "Account") ]</text>
                }
                else {
                    @:[ @Html.ActionLink("Log On", "LogOn", "Account") ]
                }
             */
            db.Database.Initialize(true);
            if (Request.IsAuthenticated)
            {
                var context = new UsersContext();
                var username = User.Identity.Name;
                var user = context.UserProfiles.SingleOrDefault(u => u.UserName == username);
                var signupdate = user.StartDate;

            }

            //using (var db = new SytelineDbEntities())
            //{
            //    var POLines = db.Database.SqlQuery<POLine>(
            //        QueryDefinitions.GetQuery("SelectRegularPOLinesByVendor", new string[] { "504".PadLeft(7) }))
            //        .ToList();


            //    var temp = "Nothing";
            //}

            ViewBag.Message = "WireTech Fabricators ASP.NET MVC Vendor Portal Application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
