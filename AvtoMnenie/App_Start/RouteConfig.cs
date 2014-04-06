using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AvtoMnenie
{
  public class RouteConfig
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      routes.MapRoute(
        name: "SalonSavePostCommentURL",
        url: "Home/SaveComment/{name}",
        defaults: new { controller = "Home", action = "SaveComment", name = UrlParameter.Optional }
        );
      routes.MapRoute(
        name: "SalonPostCommentURL",
        url: "Salon/PostComment/{name}",
        defaults: new { controller = "Home", action = "PostComment", name = UrlParameter.Optional }
        );
      routes.MapRoute(
        name: "SalonCommentsURL",
        url: "Salon/Comments/{name}",
        defaults: new { controller = "Home", action = "ShowSalonComments", name = UrlParameter.Optional }
        );
      routes.MapRoute(
          name: "SalonURL",
          url: "Salon/{name}",
          defaults: new { controller = "Home", action = "ShowSalonByName", name = UrlParameter.Optional }
          );
      routes.MapRoute(
          name: "NewsURL",
          url: "News/{name}",
          defaults: new { controller = "Home", action = "ShowNewsByName", name = UrlParameter.Optional }
          );

      routes.MapRoute(
          name: "Default",
          url: "{controller}/{action}/{id}",
          defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}