using System.Web.Mvc;

namespace Msg.Web.Areas.Mobile
{
    public class MobileAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Mobile";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
       
            context.MapRoute(
                name: "Mobile_default",
                url: "Mobile/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Msg.Web.Areas.Mobile.Controllers" }
            );
        }
    }
}
