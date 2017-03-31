using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using YYQERP.Infrastructure.Helpers;
using YYQERP.Services.Interfaces;
using YYQERP.Services.Messages;

namespace YYQERP.Controllers
{

    public class UserController : BaseController
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }



        // GET: User
        public ActionResult Index()
        {
            ViewBag.Pers = GetUserOpers();
            return View();
        }


        public ActionResult GetRoleSelectList()
        {
            var list = _userService.GetRoleSelectList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPageList(Search_User_Request request)
        {
            var list = _userService.SearchUser(request);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }


        public ActionResult AddUser(Add_User_Request request)
        {
            var res = _userService.AddUser(request);
            if (res.Result)
            {
                return Content("");
            }
            return Content(res.Message);

        }

        public ActionResult EditUser(Edit_User_Request request)
        {
            var res = _userService.EditUser(request);
            if (res.Result)
            {
                return Content("");
            }
            return Content(res.Message);

        }

        public ActionResult DeleteUser(int Id)
        {
            var res = _userService.DeleteUser(Id);
            if (res.Result)
            {
                return Content("");
            }
            return Content(res.Message);

        }
    }

    
}