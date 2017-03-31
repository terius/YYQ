using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YYQERP.Infrastructure.Logging;

namespace YYQERP.Filter
{
    public class CustomHandleError : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            LoggingFactory.GetLogger().Log("错误信息:\r\n" + filterContext.Exception.ToString());
            var request = filterContext.HttpContext.Request;
            if (request.IsAjaxRequest())
            {

                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.StatusCode = 200;
                ContentResult cr = new ContentResult();
                cr.Content = filterContext.Exception.InnerException == null ? filterContext.Exception.Message : (filterContext.Exception.Message + "\r\n内部错误信息 : " + filterContext.Exception.InnerException.Message);
                cr.ContentEncoding = System.Text.Encoding.UTF8;
                cr.ContentType = "text/xml";
                filterContext.Result = cr;
            }
            // filterContext.ExceptionHandled = true;
            //  filterContext.HttpContext.Response.StatusCode = 200;
            //  filterContext.Result = new RedirectResult("/Error/ErrorPage");

            if (filterContext.HttpContext.IsCustomErrorEnabled)
            {
                base.OnException(filterContext);
            }


        }
    }
}