using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CTT.Models;

namespace CTT.Controllers
{
    public abstract class CTTController:RavenController
    {
        protected User CurrentUser()
        {
            return RavenSession.Query<User>().FirstOrDefault(x => x.Email == User.Identity.Name);
        }
    }
}