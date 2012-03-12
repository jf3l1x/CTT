using System.Linq;
using System.Web.Mvc;
using CTT.Infrastructure;
using CTT.Models;


namespace CTT.Controllers
{
    [Authorize]
    public class ActivityController : CTTController
    {
        public ActionResult New()
        {
            PrepareData();
            return View("Edit", new Activity());
        }
        private void PrepareData()
        {
            ViewBag.Projects = RavenSession.Advanced.LuceneQuery<Project>("AllowedProjectUsers").WhereEquals("UserId", CurrentUser().Id).WaitForNonStaleResults().ToList();
            ViewBag.Services = RavenSession.Query<Service>().OrderBy(x => x.Name);
        }
        public ActionResult Index(string id)
        {
            var user = CurrentUser();
            if (!string.IsNullOrEmpty(id))
            {
                var activity = RavenSession.Query<Activity>().FirstOrDefault(x => x.Id == id);
                if (activity!=null)
                {
                    PrepareData();
                    return View("Edit", activity);
                }
            }
            return View(RavenSession.Query<Activity>().Where(x=>x.UserId==user.Id).OrderByDescending(x=>x.Start).ToList());
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