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
    public class PartController : BaseController
    {

        private readonly IGoodsService _goodsService;
        private readonly ICommonCacheService _cacheService;
        public PartController(IGoodsService goodService, ICommonCacheService cacheService)
        {
            _goodsService = goodService;
            _cacheService = cacheService;
        }
        // GET: Part
        public ActionResult Index()
        {
            ViewBag.Pers = GetUserOpers();
            var model = new PartModel();
            model.ElementCacheViews = _cacheService.GetCache_Element();
            model.PartSelectViews = _cacheService.GetCache_Part();
            return View(model);
        }


        public ActionResult GetPageList(Search_Part_Request request)
        {
            var list = _goodsService.SearchPart(request);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }

        public string Add(Add_Part_Request request)
        {
            var res = _goodsService.AddPart(request);
            return res.Message;
        }

        public string Edit(Edit_Part_Request request)
        {
            var res = _goodsService.EditPart(request);
            return res.Message;
        }

        public string Delete(int Id)
        {
            var res = _goodsService.DeletePart(Id);
            return res.Message;
        }


        public ActionResult GetPartDetail(int partid)
        {
            IList<PartDetailEditView> list = new List<PartDetailEditView>();
            if (partid > 0)
            {
                list = _goodsService.GetPartDetail(partid);
            }
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }


        public string SavePartDetail(int partid)
        {
            var pgInfo = GetInfoByStream<List<PartDetailEditView>>();
            var res = _goodsService.SavePartDetail(pgInfo,partid);
            return res.Message;
        }
        public ActionResult GetBomByPart(string partCode)
        {
            IList<BomListInPartView> list = new List<BomListInPartView>();
            if (!string.IsNullOrEmpty(partCode))
            {
                list = _goodsService.GetBomListInPart(partCode);
            }
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }

     

        public ActionResult GetBomDetail(int bomId)
        {
            IList<BomDetailEditView> list = new List<BomDetailEditView>();
            if (bomId > 0)
            {
                list = _goodsService.GetBomDetailListInPart(bomId);
            }
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }

   

    }
}