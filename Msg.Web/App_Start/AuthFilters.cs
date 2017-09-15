using System.Web.Mvc;

namespace Msg.Web.App_Start
{
    /// <summary>
    /// 一般登陆拦截器
    /// </summary>
    public class AuthFilters : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext ctx)
        {
        }
        public void OnActionExecuting(ActionExecutingContext ctx)
        {
            if (!UserAuth.IsAuthenticated)
            {
                var returnUrl = "";
                if (ctx.RequestContext.HttpContext.Request.Url != null)
                {
                    returnUrl = ctx.RequestContext.HttpContext.Request.Url.ToString();
                }
                
                ctx.Result = new RedirectResult("/account/login?returnUrl=" + returnUrl);
            }
        }
    }
    /// <summary>
    /// 一般登陆拦截器(手机网页)
    /// </summary>
    public class MobileAuthFilters : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext ctx)
        {
        }
        public void OnActionExecuting(ActionExecutingContext ctx)
        {
            if (!UserAuth.IsAuthenticated)
            {
                var returnUrl = "";
                if (ctx.RequestContext.HttpContext.Request.Url != null)
                {
                    returnUrl = ctx.RequestContext.HttpContext.Request.Url.ToString();
                }

                ctx.Result = new RedirectResult("/mobile/account/login?returnUrl=" + returnUrl);
            }
        }
    }

    /// <summary>
    /// 无需登陆拦截器
    /// </summary>
    public class NoAuthFilters : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext ctx)
        {
        }
        public void OnActionExecuting(ActionExecutingContext ctx)
        {
            if (UserAuth.IsAuthenticated)
            {
                ctx.Result = new RedirectResult("/Mobile");
            }
        }
    }


    /// <summary>
    /// 管理员无需登陆拦截器
    /// </summary>
    public class NoAdminAuthFilters : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext ctx)
        {
        }
        public void OnActionExecuting(ActionExecutingContext ctx)
        {
            var role = UserAuth.Role;
            if (UserAuth.IsAuthenticated && role != null && (role.RoleName == "管理员" || role.RoleName == "区域管理员"))
            {
                ctx.Result = new RedirectResult("/Admin/Home/Index");
            }
        }
    }

    /// <summary>
    /// 管理后台登陆拦截器
    /// </summary>
    public class AdminAuthFilter : FilterAttribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var role = UserAuth.Role;
            if (!UserAuth.IsAuthenticated || role == null || (role.RoleName != "管理员" && role.RoleName != "区域管理员"))
            {
                filterContext.Result = new RedirectResult("/Admin/Account/Login");
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }
    }

}