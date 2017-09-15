using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Msg.Bll.Helpers;
using Msg.Bll.Models;
using Msg.Tools;
using Msg.Web.App_Start;

namespace Msg.Web.Areas.Admin.Controllers
{
    public partial class AccountController : Controller
    {
        //
        // GET: /Admin/Account/
        private readonly UsersHelper _userHelper = UnityConfig.GetConfiguredContainer().Resolve<UsersHelper>();

        [NoAdminAuthFilters]
        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        [NoAdminAuthFilters]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel login)
        {
            var result = _userHelper.Login(login);
            if (result.ResultType.Equals(OperationResultType.Success))
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            ViewBag.ErrMsg = result.Message;
            return View("Login");
        }
    }
}
