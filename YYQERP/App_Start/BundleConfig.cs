using System.Web;
using System.Web.Optimization;

namespace YYQERP
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/css/base")
                .Include("~/Content/css/base.css", new CssRewriteUrlTransform())
                .Include("~/Content/css/cssbtn.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/css/easyui")
                       .Include("~/easyui/themes/bootstrap/easyui.css", new CssRewriteUrlTransform())
                       .Include("~/easyui/themes/icon.css", new CssRewriteUrlTransform())
                       .Include("~/easyui/themes/style.css", new CssRewriteUrlTransform())
                       .Include("~/easyui/144/icon-all.css", new CssRewriteUrlTransform())
                       .Include("~/easyui/144/plugin_extend.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/css/ztree").Include(
                "~/js/zTree_v3/css/zTreeStyle/zTreeStyle.css"
                ));


            bundles.Add(new StyleBundle("~/css/extend")
                    .Include("~/js/jquery.jnotify.css", new CssRewriteUrlTransform())
                    .Include("~/js/showLoading.css", new CssRewriteUrlTransform()));

            //js

            bundles.Add(new ScriptBundle("~/js/jquery").Include(
                 "~/easyui/jquery.min.js",
                 "~/js/jquery.form.js"
                 ));

            bundles.Add(new ScriptBundle("~/js/jquery-ext").Include(
                 "~/js/jquery.cookie.js",
                 "~/js/jquery.jnotify.js",
                 "~/js/jquery.showLoading.min.js"
                ));


            //easyui
            bundles.Add(new ScriptBundle("~/js/easyui").Include(
                      "~/easyui/jquery.easyui.min.js"
                ));

            bundles.Add(new ScriptBundle("~/js/easyui-cn").Include(
                     "~/easyui/locale/easyui-lang-zh_CN.js"
              ));

            bundles.Add(new ScriptBundle("~/js/easyui-ext").Include(
                  "~/easyui/144/jeasyui.extend.js"
                ));
            bundles.Add(new ScriptBundle("~/js/easyui-tab-ext").Include(
                  "~/easyui/144/jeasyui.tabs.extend.js"
               ));





            bundles.Add(new ScriptBundle("~/js/easyuiother").Include(
                  "~/easyui/locale/easyui-lang-zh_CN.js",
                   "~/easyui/144/jeasyui.extend.js",
                   "~/easyui/144/jeasyui.tabs.extend.js"
           ));





            bundles.Add(new ScriptBundle("~/bundles/js/easyui-datagrid-detailview").Include(
    "~/easyui/datagrid-detailview.js"));
            bundles.Add(new ScriptBundle("~/js/easyui-datagrid-groupview").Include(
  "~/easyui/datagrid-groupview.js"));
            bundles.Add(new ScriptBundle("~/js/datagrid-filter").Include(
 "~/easyui/datagrid-filter.js"));

            //其他
            bundles.Add(new ScriptBundle("~/js/home").Include(
                    "~/js/BaiduTemplate.js",
                    "~/js/utils.js",
                    "~/js/layer/layer.js",
                    "~/js/zTree_v3/js/jquery.ztree.all-3.5.min.js",
                    "~/js/common.js",
                    "~/js/app.index.js"));


            bundles.Add(new ScriptBundle("~/js/page").Include(
                    "~/js/BaiduTemplate.js",
                    "~/js/utils.js",
                    "~/js/layer/layer.js",
                    "~/js/zTree_v3/js/jquery.ztree.all-3.5.min.js",
                    "~/js/common.js",
                    "~/js/validate/jquery.validate.js",
                    "~/js/validate/validate-ex.js",
                    "~/js/Sys/Base_Index.js"
                    ));


            bundles.Add(new ScriptBundle("~/js/BaiduTemplate").Include(
                   "~/js/BaiduTemplate.js"
                ));

            bundles.Add(new ScriptBundle("~/js/utils").Include(
                 "~/js/utils.js"
              ));

            bundles.Add(new ScriptBundle("~/js/json2").Include(
              "~/js/json2.min.js"
             ));
            bundles.Add(new ScriptBundle("~/js/yxz").Include(
               "~/js/jqext.yxz.js"
             ));
            bundles.Add(new ScriptBundle("~/js/layer").Include(
          "~/js/layer/layer.js"
            ));

            bundles.Add(new ScriptBundle("~/js/ztree").Include(
           "~/js/zTree_v3/js/jquery.ztree.all-3.5.min.js"
           ));

            bundles.Add(new ScriptBundle("~/js/common").Include(
           "~/js/common.js"
           ));

            bundles.Add(new ScriptBundle("~/js/validate").Include(
                 "~/js/validate/jquery.validate.js", "~/js/validate/validate-ex.js"));



            //页面js
            bundles.Add(new ScriptBundle("~/js/basejs").Include(
          "~/js/Sys/Base_Index.js"));





            //bundles.Add(new ScriptBundle("~/js/page/home").Include(
            // "~/js/app.index.js"));


        }
    }
}
