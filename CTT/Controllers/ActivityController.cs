using System.Linq;
using System.Web.Mvc;
using CTT.Infrastructure;
using CTT.Models;


namespace CTT.Controllers
{
    [AdminAuthorize]
    public class ActivityController : CTTController
    {
        public ActionResult New()
        {
            PrepareData();
            return View("Edit", new Activity());
        }
        private void PrepareData()
        {
            ViewBag.Projects = RavenSession.Query<Project>().OrderBy(x => x.Name);
            ViewBag.Services = RavenSession.Query<Service>().OrderBy(x => x.Name);
        }
        public ActionResult Index(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var activity = RavenSession.Query<Activity>().FirstOrDefault(x => x.Id == id);
                if (activity!=null)
                {
                    PrepareData();
                    return View("Edit", activity);
                }
            }
            return View(RavenSession.Query<Activity>());
        }
        public ActionResult Save(string Id)
        {
            var activity = RavenSession.Query<Activity>().FirstOrDefault(x => x.Id == Id) ?? new Activity();
            UpdateModel(activity);
            var user = CurrentUser();
            if (string.IsNullOrEmpty(activity.UserId))
            {
                activity.UserId = user.Id;
            }
            RavenSession.Store(activity);
            return RedirectToAction("Index");
        }
       
        public ActionResult Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var activity = RavenSession.Query<Activity>().FirstOrDefault(x => x.Id == id);
                if (activity != null)
                {
                    RavenSession.Delete(activity);
                }
            }
            return RedirectToAction("Index");
        }
    }
}