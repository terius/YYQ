using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using YYQERP.Infrastructure.Helpers;
using YYQERP.Services.Interfaces;


namespace YYQERP.Controllers
{
   
    public class AccountController : Controller
    {

        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            HttpCookie myCookie = new HttpCookie("UserInfo");
            myCookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(myCookie);
            return Redirect("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string UserName, string Password)
        {

            Password = StringHelper.Sha256(Password);
            var userinfo = _userService.GetDbQuerySet().FirstOrDefault(d => d.UserName.Equals(UserName) && d.Password.Equals(Password));
            if (userinfo != null)
            {
                GetRoles(userinfo.Id, UserName);
                FormsAuthentication.SetAuthCookie(userinfo.UserName, false);

                return Redirect("~/Home/Index");
            }
            ModelState.AddModelError("", "用户名或密码错误。");
            return View();
        }

        private void GetRoles(int userId, string userName)
        {
            var roles = _userService.GetUserRoleAndMenuByUserId(userId, userName);
            AddUserInfoCookie(userName, roles.RoleName);
            Session["UserRolesMenus"] = roles;
        }

        private void AddUserInfoCookie(string userName, string rolename)
        {
            HttpCookie cookie = new HttpCookie("UserInfo");//定义cookie对象以及名为Info的项
            cookie.Path = "/";
            cookie.Values.Add("userName", userName);//增加属性
            cookie.Values.Add("userRole", rolename);//增加属性
            Response.AppendCookie(cookie);//确定写入cookie中  
        }

        public ActionResult NoPermissions()
        {
            ViewBag.LastUrl = Request["returnUrl"];
            return View();
        }

     



        public ActionResult ErrorPage()
        {
            return View();
        }

    }
}