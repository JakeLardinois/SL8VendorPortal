﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SL8VendorPortal.Controllers
{
    public class AdminFunctionsController : Controller
    {
        //
        // GET: /AdminFunctions/
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

    }
}
