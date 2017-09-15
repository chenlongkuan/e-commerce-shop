using System.Web.Optimization;

namespace Msg.Web.App_Start
{
    public class BundleConfig
    {
        // 有关 Bundling 的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Content/Css/normalize.css"
                ,"~/Content/Css/base.css"
                , "~/Content/Css/msg_miain.css"
                , "~/Content/AdminTemplate/bower_components/bootstrap/dist/css/bootstrap.min.css"
                , "~/Content/bootstrapdialog/dist/css/bootstrap-dialog.min.css"
                , "~/Content/ShowLoading/showLoading.css"));

            bundles.Add(new StyleBundle("~/bundles/loginCss").Include(
       "~/Content/Css/normalize.css"
       , "~/Content/Css/msg_miain.css"
       , "~/Content/css/login.css"
       , "~/Content/bower_components/bootstrap/dist/css/bootstrap.min.css"
       , "~/Content/bootstrapdialog/dist/css/bootstrap-dialog.min.css"
       , "~/Content/ShowLoading/showLoading.css"));
        }
    }
}