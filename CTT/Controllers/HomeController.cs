using System.Web.Mvc;

namespace CTT.Controllers
{
    [Authorize]
    public class HomeController : CTTController
    {
        

        public ActionResult Index()
        {
            return View();
        }
    }
}