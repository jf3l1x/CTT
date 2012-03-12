using System.Linq;
using System.Web.Mvc;
using CTT.Infrastructure;
using CTT.Models;

namespace CTT.Controllers
{
    [AdminAuthorize]
    public class ServiceController : CTTController
    {
        public ActionResult Index(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var service = RavenSession.Query<Service>().FirstOrDefault(x => x.Id == id);
                if (service!=null)
                {
                    return View("Edit", service);
                }
            }
            return View(RavenSession.Query<Service>());
        }
        public ActionResult Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var service = RavenSession.Query<Service>().FirstOrDefault(x => x.Id == id);
                if (service != null)
                {
                    RavenSession.Delete(service);
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult New()
        {
            return View("Edit", new Service());
        }

        public ActionResult Save(Service service)
        {
            RavenSession.Store(service);
            return RedirectToAction("Index");
        }
    }
}