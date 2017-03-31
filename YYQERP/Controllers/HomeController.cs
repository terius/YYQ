using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YYQERP.Infrastructure.Helpers;
using YYQERP.Repository;
using YYQERP.Services.Interfaces;

namespace YYQERP.Controllers
{
    [MyAuthorize]
    public class HomeController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IMenuService _menuService;
       

        public HomeController(IUserService userService, IMenuService menuService)
        {
            _userService = userService;
            _menuService = menuService;
          
        }
        public ActionResult Index()
        {
          
           // var list = _userService.GetUserMenuList();
          //  ViewBag.MenuList =  JsonHelper.SerializeObj(list);
            var info = _userService.LoginUserInfo;
            ViewBag.UserRole = info.RoleDescName;
            ViewBag.UserName = info.UserTrueName;
            return View();
        }


        public ActionResult GetMenuTree()
        {
           
             var list = _userService.GetUserMenuList();

             return Json(list, JsonRequestBehavior.AllowGet);
        }


        public string ChangePassword(string newPassword)
        {
            var res = _userService.ChangePassword(newPassword);

            return res.Message;
        }

    }
}