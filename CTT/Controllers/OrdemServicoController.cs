using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CTT.Models;

namespace CTT.Controllers
{
    public class OrdemServicoController : CTTController
    {
        public ActionResult List()
        {
            LoadLists();
            var user = CurrentUser();
            var ordens =
                RavenSession.Query<OrdemServico>().Where(x => x.UserId == user.Id).ToList().OrderByDescending(
                    x => x.Data);
            return View("List",ordens);
        }
        private void LoadLists()
        {
            ViewData["users"] = RavenSession.Query<User>().ToList();
            ViewData["projetos"] = RavenSession.Query<Project>().ToList();
        }
        public ActionResult All()
        {
            LoadLists();
            var user = CurrentUser();
            var ordens =
                RavenSession.Query<OrdemServico>().ToList().OrderByDescending(
                    x => x.Data);
            return View("List",ordens);
        }
        public ActionResult New()
        {
            LoadLists();
            ViewData["currentUser"] = CurrentUser();
            return View();
        }
        public ActionResult Delete(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                var ordem = RavenSession.Query<OrdemServico>().FirstOrDefault(x => x.Id == Id);
                if (ordem != null)
                {
                    RavenSession.Delete(ordem);
                    var activities = RavenSession.Query<Activity>().Where(x => x.OrdemServicoId == Id);
                    foreach (var activity in activities)
                    {
                        activity.OrdemServicoId = null;
                    }
                    RavenSession.SaveChanges();
                }
            }
            return List();

        }
        public ActionResult Index(string Id)
        {
            var ordem = RavenSession.Load<OrdemServico>(Id);
            if (ordem!=null)
            {
                SetViewDataIndex(ordem);
                return View(ordem);
            }
            return HttpNotFound();
        }

        private void SetViewDataIndex(OrdemServico ordem)
        {
            var projeto = RavenSession.Load<Project>(ordem.ProjetoId);
            ViewBag.Projeto = projeto;
            ViewData["services"] = RavenSession.Query<Service>().ToList();
            ViewData["users"] = RavenSession.Query<User>().ToList();
        }

        public ActionResult Create(DateTime Inicio,DateTime Fim,string ProjectId)
        {
            var ordem = new OrdemServico();
            ordem.UserId = CurrentUser().Id;
            ordem.Inicio = Inicio;
            ordem.Fim = Fim.Date.AddDays(1).Subtract(TimeSpan.FromSeconds(1));
            ordem.Data = DateTime.Now;
            ordem.ProjetoId = ProjectId;
            var users=new List<string>();
            if (Request.Form["user"] != null)
            {
                foreach (var userId in Request.Form.GetValues("user"))
                {
                    users.Add(userId);
                }
            }
            else
            {
                users.Add(CurrentUser().Id);
            }

            ordem.Atividades =
                RavenSession.Query<Activity>().Where(
                    x =>
                    x.Start >= ordem.Inicio && x.End <= ordem.Fim && x.ProjectId == ProjectId).ToList().Where(x=>users.Contains(x.UserId) && string.IsNullOrEmpty(x.OrdemServicoId) ).ToList();
            RavenSession.Store(ordem);
            foreach (var atividade in ordem.Atividades)
            {
                atividade.OrdemServicoId = ordem.Id;
                
            }
            RavenSession.SaveChanges();
            SetViewDataIndex(ordem);
            return View("Index", ordem);
        }

    }
}