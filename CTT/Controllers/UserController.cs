using System.Linq;
using System.Web.Mvc;
using CTT.Infrastructure;
using CTT.Models;

namespace CTT.Controllers
{
    [Authorize]
    public class UserController : CTTController
    {
        [AdminAuthorize]
        public ActionResult List()
        {
            return View(RavenSession.Query<User>().ToList());
        }

        [AdminAuthorize]
        public ActionResult Index(string id)
        {
            User user = RavenSession.Query<User>().FirstOrDefault(x => x.Id == id);
            ViewData["currentUser"] = CurrentUser();
            return View(user);
        }
       
        public ActionResult Profile()
        {
            User user = CurrentUser();
            ViewData["currentUser"] = user;
            return View("Index", user);
        }

        public ActionResult Save(User user)
        {
            User current = CurrentUser();
            if (!current.Id.Equals(user.Id) && !current.IsAdmin)
            {
                return new HttpUnauthorizedResult();
            }
            User entity = RavenSession.Query<User>().FirstOrDefault(x => x.Id == user.Id);
            if (entity != null)
            {
                entity.FullName = user.FullName;
                entity.NickName = user.NickName;
                if (current.IsAdmin)
                {
                    bool isAdmin = user.IsAdmin;
                    if (entity.IsAdmin && !user.IsAdmin)
                    {
                        //Check id exist another admin
                        if (!RavenSession.Query<User>().Any(x => x.IsAdmin && x.Id != user.Id))
                        {
                            isAdmin = true;
                        }
                    }
                    entity.IsAdmin = isAdmin;
                    entity.Comissao = user.Comissao;
                }
                RavenSession.Store(entity);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}