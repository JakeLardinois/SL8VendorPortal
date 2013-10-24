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
    #region Notes
    /*The [InitializeSimpleMembership] data annotation is what is required in order to call anything from the 'WebSecurity' class. You can annotate any controller with this class in order to call the class in 
     * /Filters/InitializeSimpleMembershipAttribute.cs. Of also importance when searching for data about the currently logged in user are
     * the 'Roles', 'User', and 'Request' classes (ex Request.IsAuthenticated, User.Identity.Name, Roles.IsUserInRole, etc.)*/
    //Accessing individual or profile collection (I used better methods on the controllers classes, though...)
            /*dynamic users;
            dynamic user;
            var Username = WebSecurity.CurrentUserName;
            using (var db = Database.Open("SecurityConnection"))
            {
                users = db.Query("SELECT * FROM Users WHERE UserID = " + WebSecurity.CurrentUserId);
                user = db.QuerySingle("SELECT * FROM Users WHERE UserID = " + WebSecurity.CurrentUserId);
                
                if (user != null)
                {
                    strTemp = user["Email"];
                }
                
            }*/
     
    //Accessing a users Account Info via Razor syntax...
             /*  @if(Request.IsAuthenticated) {
                    <text>Welcome <strong>@User.Identity.Name</strong> 
                    @(User.IsInRole("Administrator") ? "(Administrator)" : String.Empty) 
                    [ @Html.ActionLink("Log Off", "LogOff", "Account") ]</text>
                }
                else {
                    @:[ @Html.ActionLink("Log On", "LogOn", "Account") ]
                }
             */
    //Accessing a users Account Info via C#...
    /*db.Database.Initialize(true);
    if (Request.IsAuthenticated)
    {
        var context = new UsersContext();
        var username = User.Identity.Name;
        var user = context.UserProfiles.SingleOrDefault(u => u.UserName == username);
        var signupdate = user.StartDate;

    }*/
    #endregion

    public class HomeController : Controller
    {
        private SL8VendorPortalDb db = new SL8VendorPortalDb();


        public ActionResult Index()
        {
            //This forces the database to reinitialize
            var result = db.VendorRequests
                .Where(p => p.RequestCategoryCode.Equals("TOReciept"))
                .ToList();

            ViewBag.Message = "WireTech Fabricators ASP.NET MVC Vendor Portal Application.";

            //The below code can be used to reset a users password...
            //WebSecurity.InitializeDatabaseConnection("SecurityConnection", "Users", "UserID", "UserName", autoCreateTables: false);
            //var token = WebSecurity.GeneratePasswordResetToken("Del");
            //WebSecurity.ResetPassword(token, "123456789");

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
