using System.Web.Mvc;
using Msg.Tools;
using Msg.Tools.Extensions;

namespace Msg.Web.App_Start
{
    public class ReturnHelper : Controller
    {
        /// <summary>
        /// The _instance
        /// </summary>
        private static ReturnHelper _instance;

        /// <summary>
        /// The instance
        /// </summary>
        public static ReturnHelper Instance
        {
            get { return _instance ?? new ReturnHelper(); }
            set { _instance = value; }
        }

        public JsonResult GetJsonResult(OperationResult result)
        {
            return Json(new
            {
                status = result.ResultType.Equals(OperationResultType.Success),
                msg = result.ResultType.ToDescription() + result.Message
            }, JsonRequestBehavior.AllowGet);
        }
    }
}