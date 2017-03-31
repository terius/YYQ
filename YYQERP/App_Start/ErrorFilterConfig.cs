using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YYQERP.Filter;

namespace YYQERP
{
    public class ErrorFilterConfig
    {
        public static void RegisterGlobalErrorFilters(GlobalFilterCollection filters)
        {
          //  filters.Add(new HandleErrorAttribute(), 1);
            filters.Add(new CustomHandleError { View = "ErrorPage" },2);
        }
    }
}