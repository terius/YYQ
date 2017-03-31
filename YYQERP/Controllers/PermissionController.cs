using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YYQERP.Cache;
using YYQERP.Services.Interfaces;
using YYQERP.Services.Messages;
using YYQERP.Services.Views;

namespace YYQERP.Controllers
{
    public class PermissionController : BaseController
    {

        private readonly IUserService _userService;
        public PermissionController(IUserService userService)
        {
            _userService = userService;
        }
        // GET: Bom
        public ActionResult Index()
        {
            ViewBag.Pers = GetUserOpers();
            return View();
        }

        public ActionResult GetList()
        {
            var list = _userService.GetRoleTreeList();
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }

        public ActionResult GetRoleMenus(int roleId)
        {
            var list = _userService.GetRoleMenus(roleId);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }

        //public string SaveRoleMenus(int roleId)
        //{
        //    var pgInfo = GetInfoByStream<List<idClass>>();
        //    var res = _userService.SaveRoleMenus(pgInfo, roleId);
        //    return res.Message;
        //}

        public string SaveRoleMenus(int roleId)
        {
            var form = Request.Form;
            var res = _userService.SaveRoleMenus(form.AllKeys, roleId);
            return "";
        }
    }


}