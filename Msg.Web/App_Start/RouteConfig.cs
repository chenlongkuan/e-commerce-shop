using System.Web.Mvc;
using System.Web.Routing;

namespace Msg.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "snacksRoute",
                url: "snacks/{cate}/{page}",
                defaults:
                    new
                    {
                        controller = "Home",
                        action = "Snacks",
                        cate = UrlParameter.Optional,
                        page = UrlParameter.Optional
                    },
                namespaces: new[] { "Msg.Web.Controllers" }
                );


            routes.MapRoute(
                name: "milkRoute",
                url: "milk/{brand}/{page}",
                defaults:
                    new
                    {
                        controller = "Home",
                        action = "Milk",
                        brand = UrlParameter.Optional,
                        page = UrlParameter.Optional
                    },
                namespaces: new[] { "Msg.Web.Controllers" }
                );


            routes.MapRoute(
                 name: "shortDefault",
                 url: "item/{id}",
                 defaults: new { controller = "Home", action = "Details", id = UrlParameter.Optional },
                 namespaces: new[] { "Msg.Web.Controllers" }
                 );

            routes.MapRoute(
                 name: "Live",
                 url: "live/{id}",
                 defaults: new { controller = "Home", action = "Live", id = UrlParameter.Optional },
                 namespaces: new[] { "Msg.Web.Controllers" }
                 );

            routes.MapRoute(
                 name: "Travels",
                 url: "travels/{id}",
                 defaults: new { controller = "Home", action = "Travels", id = UrlParameter.Optional },
                 namespaces: new[] { "Msg.Web.Controllers" }
                 );

            routes.MapRoute(
                 name: "Tickets",
                 url: "tickets/{id}",
                 defaults: new { controller = "Home", action = "Tickets", id = UrlParameter.Optional },
                 namespaces: new[] { "Msg.Web.Controllers" }
                 );

            routes.MapRoute(
                 name: "Market",
                 url: "market/{id}",
                 defaults: new { controller = "Home", action = "Market", id = UrlParameter.Optional },
                 namespaces: new[] { "Msg.Web.Controllers" }
                 );

            routes.MapRoute(
                name: "CheckOut",
                url: "checkOut/{id}",
                defaults: new { controller = "Home", action = "CheckOut", id = UrlParameter.Optional },
                namespaces: new[] { "Msg.Web.Controllers" }
                );     
            
            routes.MapRoute(
                 name: "Search",
                 url: "search/{key}/{page}",
                 defaults: new
                 {
                     controller = "Home"
                     , action = "Search"
                     ,key = UrlParameter.Optional
                     ,page = UrlParameter.Optional
                 },
                namespaces: new[] { "Msg.Web.Controllers" }
              );
            routes.MapRoute(
              name: "Default",
              url: "{controller}/{action}/{id}",
              defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
              namespaces: new[] { "Msg.Web.Controllers" }
              );
            //routes.MapRoute(
            //    name: "shortAccountRoute",
            //    url: "{action}/{id}",
            //    defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new[] { "Msg.Web.Controllers" }
            //    );



        }
    }
}