using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YYQERP.Services.Interfaces;
using YYQERP.Services.Messages;

namespace YYQERP.Controllers
{
    public class ModelsController : BaseController
    {

        private readonly IGoodsService _goodsService;

        public ModelsController(IGoodsService goodService)
        {
            _goodsService = goodService;
        }
        // GET: Models
        public ActionResult Index()
        {
            ViewBag.Pers = GetUserOpers();
            return View();
        }

        public ActionResult GetPageList(Search_Model_Request request)
        {
            var list = _goodsService.SearchModels(request);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }

        public string Add(Add_Model_Request request)
        {
            var res = _goodsService.AddModel(request);
            return res.Message;
        }

        public string Edit(Edit_Model_Request request)
        {
            var res = _goodsService.EditModel(request);
            return res.Message;
        }

        public string Delete(int Id)
        {
            var res = _goodsService.DeleteModel(Id);
            return res.Message;
        }
    }
}