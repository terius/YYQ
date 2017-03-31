using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Infrastructure.Logger
{
    public class HtmlPager
    {
        public static string CreatePagesHtml(int totalPageNo, int currPageNo, string jsClickName = "doSearch")
        {
            if (currPageNo > totalPageNo || currPageNo < 1 || totalPageNo == 1) return "";
            string res = "";
            if (currPageNo > 3) res += LiForFirst(jsClickName);//有跳转到第一页
            //if (currPageNo > 2) res += LiForPrev(currPageNo - 1, jsClickName);//跳转到前一页
            if (currPageNo > 4) res += LiForOthers();//无跳转，显示中间有页面
            if (currPageNo > 2) res += LiForPage(currPageNo - 2, jsClickName);//前两页
            if (currPageNo > 1) res += LiForPage(currPageNo - 1, jsClickName);//前一页

            res += LiForCurr(currPageNo);//当前页

            if (currPageNo < totalPageNo) res += LiForPage(currPageNo + 1, jsClickName);//下一页
            if (currPageNo < totalPageNo - 1) res += LiForPage(currPageNo + 2, jsClickName);//下两页
            if (currPageNo < totalPageNo - 3) res += LiForOthers();//无跳转，显示中间有页面
            //if (currPageNo < totalPageNo - 1) res += LiForNext(currPageNo + 1, jsClickName);//跳转到后一页
            if (currPageNo < totalPageNo - 2) res += LiForLast(totalPageNo, jsClickName);//跳转到最后页
            return string.Format(@"
    <div class=""pagination-container"">
        <ul class=""pagination"">
            {0}
        </ul>
    </div>"
                , res);
        }
        static string GetPageItem(int pageNo, string text, string jsClickName, string classStr = null, string rel = null)
        {
            return string.Format(@"<li{0}><a href=""#""{1}{2}>{3}</a></li>"
                , string.IsNullOrWhiteSpace(classStr) ? "" : " class=\"" + classStr + "\""
                , string.IsNullOrWhiteSpace(rel) ? "" : " rel=\"" + rel + "\""
                , string.IsNullOrWhiteSpace(jsClickName) ? "" : string.Format(" onclick=\"{0}({1});\"", jsClickName, pageNo)
                , text
                );
        }
        static string LiForFirst(string jsClickName) { return GetPageItem(1, "1"/*"««"*/, jsClickName, classStr: "PagedList-skipToFirst"); }
        static string LiForPrev(int pageNo, string jsClickName) { return GetPageItem(pageNo, "«", jsClickName, classStr: "PagedList-skipToPrevious", rel: "prev"); }
        static string LiForNext(int pageNo, string jsClickName) { return GetPageItem(pageNo, "»", jsClickName, classStr: "PagedList-skipToNext", rel: "next"); }
        static string LiForLast(int totalPageNo, string jsClickName) { return GetPageItem(totalPageNo, totalPageNo.ToString()/*"»»"*/, jsClickName, classStr: "PagedList-skipToLast"); }
        static string LiForPage(int pageNo, string jsClickName) { return GetPageItem(pageNo, pageNo.ToString(), jsClickName); }
        static string LiForCurr(int pageNo) { return GetPageItem(pageNo, pageNo.ToString(), null, classStr: "active"); }
        static string LiForOthers() { return GetPageItem(0, "&#8230;", null, classStr: "disabled PagedList-ellipses"); }
    }
}
