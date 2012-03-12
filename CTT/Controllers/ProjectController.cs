using System.Linq;
using System.Web.Mvc;
using CTT.Infrastructure;
using CTT.Models;

namespace CTT.Controllers
{
    [AdminAuthorize]
    public class ProjectController : CTTController
    {
        public ActionResult New()
        {
            ViewData["users"] = RavenSession.Query<User>().ToList();
            return View("Edit", new Project());
        }

        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
                return View(RavenSession.Query<Project>().OrderBy(x => x.Name).ToList());
            Project project = RavenSession.Query<Project>().FirstOrDefault(x => x.Id == id);
            if (project != null)
            {
                ViewData["users"] = RavenSession.Query<User>().ToList();
                return View("Edit", project);
            }
            return HttpNotFound();
        }
        public ActionResult Save(Project project)
        {
            project.AllowedUsers.Clear();
            if (Request.Form["user"]!=null)
            {
                foreach (var userId in Request.Form.GetValues("user"))
                {
                    project.AllowedUsers.Add(userId);
                }
            }
            
            RavenSession.Store(project);
            return RedirectToAction("Index");
        }
    }
}