using System.Linq;
using System.Web;
using System.Web.Mvc;
using CTT.Controllers;
using CTT.Models;
using Raven.Client;

namespace CTT.Infrastructure
{
    public class AdminAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                using (IDocumentSession session = RavenController.DocumentStore.OpenSession())
                {
                    User user = session.Query<User>().FirstOrDefault(x => x.Email == httpContext.User.Identity.Name);
                    if (user == null || !user.IsAdmin)
                    {
                        return false;
                    }
                }
            }
            return base.AuthorizeCore(httpContext);
        }
    }
}