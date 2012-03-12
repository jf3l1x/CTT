using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CTT.Infrastructure;

namespace CTT.Controllers
{
    [AdminAuthorize]
    public class ReportController : Controller
    {
        //
        // GET: /Report/

        public ActionResult Index()
        {
            return View();
        }

    }
}
