using EasyGradeManager.Models;
using System.Data.Entity;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace EasyGradeManager
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //IDatabaseInitializer<EasyGradeManagerContext> init = new DropCreateDatabaseAlways<EasyGradeManagerContext>();
            IDatabaseInitializer<EasyGradeManagerContext> init = new DropCreateDatabaseIfModelChanges<EasyGradeManagerContext>();
            Database.SetInitializer(init);
            init.InitializeDatabase(new EasyGradeManagerContext());
            HttpConfiguration config = GlobalConfiguration.Configuration;
            config.Formatters.JsonFormatter
                        .SerializerSettings
                        .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            config.Formatters.JsonFormatter
                        .SerializerSettings
                        .DateFormatString = "yyyy-MM-dd";
        }
    }
}
