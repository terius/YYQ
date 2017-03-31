using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YYQERP.Cache;
using YYQERP.Services.Interfaces;
using YYQERP.Services;
using YYQERP.Services.Messages;

namespace YYQERP.Controllers
{
    public class DicController : BaseController
    {

        private readonly IDicService _dicService;
        private readonly ICommonCacheService _cacheService;
        public DicController(IDicService dicService, ICommonCacheService cacheService)
        {
            _dicService = dicService;
            _cacheService = cacheService;
        }
        // GET: Dic
        public ActionResult Index()
        {
            ViewBag.Pers = GetUserOpers();
            return View();
        }

        public ActionResult GetDicTreeViews()
        {
            var list = _dicService.GetDicTreeViews();
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }

        public ActionResult GetDicChildren(string parentCode)
        {
            var list = _dicService.GetDicChildrenViews(parentCode);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }

        public string Add(Add_Dic_Request request)
        {
            var res = _dicService.AddDic(request);
            return res.Message;
        }


        public string Delete(int Id)
        {
            var res = _dicService.DeleteDic(Id);
            return res.Message;
        }

        public string Edit(Edit_Dic_Request request)
        {
            var res = _dicService.EditDic(request);
            return res.Message;
        }
    }
}