using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CTT.Controllers;
using CTT.Infrastructure.Indexes;
using CTT.Services;
using MvcContrib;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace CTT
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        public MvcApplication()
        {
            BeginRequest +=
                (sender, args) =>
                    {
                        HttpContext.Current.Items["CurrentRequestRavenSession"] =
                            RavenController.DocumentStore.OpenSession();
                    };
            EndRequest += (sender, args) =>
                              {
                                  using (
                                      var session =
                                          (IDocumentSession) HttpContext.Current.Items["CurrentRequestRavenSession"])
                                  {
                                      if (session == null)
                                          return;

                                      if (Server.GetLastError() != null)
                                          return;

                                      session.SaveChanges();
                                  }
                              };
        }

        public static IDocumentStore DocumentStore { get; private set; }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Activity", action = "Index", id = UrlParameter.Optional} // Parameter defaults
                );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            InitializeDocumentStore();
            RegisterRoutes(RouteTable.Routes);
            Bus.AddMessageHandler(typeof (AuthenticatedMessageHandler));
            Bus.AddMessageHandler(typeof(ClaimsRequestMessageHandler));
            RavenController.DocumentStore = DocumentStore;
        }

        private static void InitializeDocumentStore()
        {
            if (DocumentStore != null) return; // prevent misuse

            DocumentStore = new DocumentStore
                                {
                                    ConnectionStringName = "RavenDB"
                                }.Initialize();
            IndexCreation.CreateIndexes(typeof(AllowedProjectUsers).Assembly,DocumentStore);

            DocumentStore.Conventions.IdentityPartsSeparator = "-";
            
            //RavenProfiler.InitializeFor(DocumentStore,
            //    //Fields to filter out of the output
            //                            "Email", "HashedPassword", "AkismetKey", "GoogleAnalyticsKey", "ShowPostEvenIfPrivate",
            //                            "PasswordSalt", "UserHostAddress");
        }
    }
}