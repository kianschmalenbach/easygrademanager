using System.Web.Mvc;
using System.Web.Routing;

namespace EasyGradeManager
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Init",
                url: "Init",
                defaults: new { controller = "Home", action = "Init" }
            );

            routes.MapRoute(
                name: "Logout",
                url: "Logout",
                defaults: new { controller = "Home", action = "Logout" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{id}",
                defaults: new { controller = "Home", action = "Details", id = UrlParameter.Optional }
            );
        }
    }
}
