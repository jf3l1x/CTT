using System.Linq;
using System.Web.Mvc;
using CTT.Infrastructure;
using CTT.Models;

namespace CTT.Controllers
{
    [AdminAuthorize]
    public class ProjectController : CTTController
    {
        private void FillServices(Project p)
        {
            var services = RavenSession.Query<Service>().ToList();
            ViewData["services"] = services;
            foreach (var service in services)
            {
                var preco = p.Precos.FirstOrDefault(x => x.ServicoId == service.Id);
                if (preco==null)
                {
                    p.Precos.Add(new Preco(){ServicoId = service.Id});
                }
            }
        }
        public ActionResult New()
        {
            ViewData["users"] = RavenSession.Query<User>().ToList();
            var p = new Project();
            FillServices(p);
            return View("Edit", p);
        }

        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
                return View(RavenSession.Query<Project>().OrderBy(x => x.Name).ToList());
            Project project = RavenSession.Query<Project>().FirstOrDefault(x => x.Id == id);
            if (project != null)
            {
                FillServices(project);
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