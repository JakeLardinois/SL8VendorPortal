using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;

using WebMatrix.WebData;
using SL8VendorPortal.Models;
using System.Web.Security;
using System.Linq;

namespace SL8VendorPortal.Filters
{
    /*This class is implemented as a data annotation on the AccountController.cs class. So as soon as any view that references that controller is called, then this the Account Controller will be initialized
     * thereby initializing this class. Note that you could also annotate any other controller with this class in order to force this class to initialize and create the required database for controlling the
     * login functions (usernames, passwords, profiles, etc.)
     */
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                Database.SetInitializer<UsersContext>(null);

                try
                {
                    using (var context = new UsersContext())
                    {
                        if (!context.Database.Exists())
                        {
                            // Create the SimpleMembership database without Entity Framework migration schema
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }

                    /*The below is where I specify that I am using the database specified by the connection string 'SecurityConnection' for my users and thier passwords. So I am telling it that my users are located
                     * in the 'Users' table where the 'UserID' column is being used as the UserID (note that it could be named anything) and the 'UserName' column is being used to hold my Users Usernames. The last value gives
                     * the applicaiton the permission to create the other tables that are required for the authentication functions. Note also that all it took to get this working was to rt. click on 'App_Data' and add a new item
                     * and select a SQL CE database... I then just had to add my connection string to web.config and then add my table with the values I specified below (I also added a couple other columns to the table, but the 2
                     * columns that are specified below are the only necessary ones). I also changed the UsersContext():base("DefaultConnection") intitializer in AccountModels to UsersContext():base("SecurityConnection") and 
                     * deleted the "DefaultConnection" connection string in web.config
                     */
                    WebSecurity.InitializeDatabaseConnection("SecurityConnection", "Users", "UserID", "UserName", autoCreateTables: true);
                    SeedMembership();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }

            private void SeedMembership()
            {
                //This could be called here, but I already call it in the SimpleMembershipInitializer() method.
                //WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);

                var roles = (SimpleRoleProvider)Roles.Provider;
                var membership = (SimpleMembershipProvider)Membership.Provider;

                if (!roles.RoleExists("Admin"))//Create an admin role for the database if it doesn't already exist...
                {
                    roles.CreateRole("Admin");
                }
                if (!roles.RoleExists("User"))//Create a user role for the database if it doesn't already exist...
                {
                    roles.CreateRole("User");
                }
                if (!roles.RoleExists("QueueAdmin"))//Create a QueueAdmin role for the database if it doesn't already exist...
                {
                    roles.CreateRole("QueueAdmin");
                }
                if (membership.GetUser("Administrator", false) == null)//Create the Administrator user...
                {
                    //WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, StartDate = DateTime.Now });
                    WebSecurity.CreateUserAndAccount("Administrator", "wh0r353", new { Email = "Administrator@WireTechFab.com", 
                        FirstName = "Jake", LastName = "Lardinois", StartDate = DateTime.Now });
                }
                if (!roles.GetRolesForUser("Administrator").Contains("Admin"))
                {
                    roles.AddUsersToRoles(new[] { "Administrator" }, new[] { "Admin" });//Note that this single statement could also be used to add multiple users to multiple roles by leveraging the arrays.
                }
            }
        }
    }
}
