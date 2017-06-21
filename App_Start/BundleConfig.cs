using System.Web;
using System.Web.Optimization;

namespace BS.Microservice.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/css/layout").Include(
                        "~/Resource/js/plugins/chosen/chosen.css",
                        "~/Resource/css/style.min.css",
                        "~/Resource/css/bootstrap.min.css",
                        "~/Resource/css/jquery-ui.min.css",
                        "~/Resource/css/ui.jqgrid-bootstrap.css",
                        "~/Resource/css/font-awesome.min.css",
                        "~/Resource/css/animate.min.css",
                        "~/Resource/css/iconfont.css",
                       "~/Resource/css/font-icoMoon.css"
                       ));
            bundles.Add(new ScriptBundle("~/bundles/js/layout").Include(
                "~/Resource/js/grid.locale-cn.js",
                "~/Resource/js/bootstrap.min.js",
                "~/Resource/js/Comm.js",
                "~/Resource/js/json2.js",
                "~/Resource/js/jquery.form.js",
                "~/Resource/js/plugins/chosen/chosen.jquery.js",
                "~/Resource/js/plugins/iCheck/iCheck.min.js",
                "~/Resource/js/jquery.jqGrid.min.js",
                "~/Resource/js/JQ2.js"
                ));
            bundles.Add(new ScriptBundle("~/bundles/js/html5").Include(
               "~/Resource/js/html5.js",
               "~/Resource/js/respond.min.js",
               "~/Resource/js/PIE_IE678.js"
               ));
            bundles.Add(new StyleBundle("~/bundles/css/default").Include(
                       "~/Resource/content/bootstrap.css",
                       "~/Resource/css/font-awesome.min.css",
                       "~/Resource/css/animate.min.css",
                       "~/Resource/css/style.min.css",
                       "~/Resource/css/font-icoMoon.css"
                      ));
        }
    }
}