using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using YYQERP.Infrastructure.Enums;
using YYQERP.Repository;
using YYQERP.Services.Views;

namespace YYQERP
{
    public class MyAuthorize : AuthorizeAttribute
    {
        //private string _role = "";

        //public MyAuthorize()
        //{

        //}

        //public MyAuthorize(Role role)
        //    : this(role.ToString())
        //{

        //}
        //public MyAuthorize(string role)
        //{
        //    _role = role;
        //}




        /// <summary>  
        /// 重写时，提供一个入口点用于进行自定义授权检查【入口点】。  
        /// </summary>  
        /// <param name="httpContext">HTTP 上下文，它封装有关单个 HTTP 请求的所有 HTTP 特定的信息。</param>  
        /// <returns>如果用户已经过授权，则为 true；否则为 false。</returns>  
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            bool result = false;
            if (HttpContext.Current == null) return result = false;
            if (!HttpContext.Current.User.Identity.IsAuthenticated) return result = false;
            if (string.IsNullOrEmpty(Roles))
            {
                return true;
            }
            else
            {
                string[] allowRoles = Roles.Split(',');
                var userRoleMenus = httpContext.Session["UserRolesMenus"] as UserRolePermission;
                foreach (string allowRole in allowRoles)
                {

                    if (userRoleMenus.RoleName.Equals(allowRole, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return true;
                    }

                }
            }

            return result;
        }

        /// <summary>
        /// 提供一个入口点用于自定义授权检查，通过为true
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.RequestContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login", returnUrl = filterContext.HttpContext.Request.Url, returnMessage = "您无权查看." }));
                return;
            }
            //string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            //string actionName = filterContext.ActionDescriptor.ActionName;
            //var userRoleMenus = filterContext.HttpContext.Session["UserRolesMenus"] as UserRolePermission;
            //if (userRoleMenus.RoleName != "Admin")
            //{
            //    bool hasPer = false;
            //    string url = (controllerName + "/" + actionName).ToLower();
            //    foreach (var menu in userRoleMenus.UserMenus)
            //    {
            //        if (menu.Url != null && menu.Url.ToLower().Contains(url))
            //        {
            //            hasPer = true;
            //            break;
            //        }
            //    }

            //    if (!hasPer)
            //    {
            //        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "NoPermissions", returnUrl = filterContext.HttpContext.Request.Url, returnMessage = "您无权查看." }));
            //        return;
            //    }
            //}
            base.OnAuthorization(filterContext);
        }

        /// <summary>  
        /// 处理授权失败的 HTTP 请求。  
        /// </summary>  
        /// <param name="filterContext">封装用于 System.Web.Mvc.AuthorizeAttribute 的信息。 filterContext 对象包括控制器、HTTP 上下文、请求上下文、操作结果和路由数据。</param>  
        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            //异步请求  
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                filterContext.Result = new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        ErrorType = this.GetType().Name,
                        Action = filterContext.ActionDescriptor.ActionName,
                        Message = "执行错误"
                    }
                };
            }
            else
            {
                // string MyAuthError = ConfigurationManager.AppSettings["NoPermissionUrl"];// +@"?returnUrl=" + myolurl;  
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "NoPermissions", returnUrl = filterContext.HttpContext.Request.Url, returnMessage = "您无权查看." }));
                // filterContext.Result = new RedirectResult(MyAuthError);
            }
        }
    }
}