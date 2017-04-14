﻿using System.Collections.Generic;
using System.Web.Mvc;
using YYQERP.Cache;
using YYQERP.Infrastructure.Helpers;
using YYQERP.Services.Interfaces;
using YYQERP.Services.Messages;
using YYQERP.Services.Views;

namespace YYQERP.Controllers
{
    public class DeliveryController : BaseController
    {

        private readonly IPickService _pickService;
        private readonly ICommonCacheService _cacheService;
        private readonly IGoodsService _goodsService;

        public DeliveryController(IPickService pickService, ICommonCacheService cacheService, IGoodsService goodsService)
        {
            _pickService = pickService;
            _cacheService = cacheService;
            _goodsService = goodsService;
        }
        // GET: Pick
        public ActionResult Index()
        {

            ViewBag.Pers = GetUserOpers();
            var model = new PickModel();
            model.ElementSelectList = _cacheService.GetCache_Element();
            model.BomSelectList = _cacheService.GetCache_Bom();
            model.PartSelectList = _cacheService.GetCache_Part();
            return View(model);
        }

        public ActionResult GetPickPageList(Search_Pick_Request request)
        {
            var list = _pickService.SearchPick(request);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }

        public string GetPickDetail(int id)
        {
            var html = _pickService.GetPickDetailHtml(id);
            return html;
        }

        public ActionResult GetAddTemplate()
        {
            IList<Pick_ForAdd_View> list = new List<Pick_ForAdd_View>();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetListByBomId_For_Add(int bomid, int num)
        {
            var list = _goodsService.GetListByBomId_For_PickAdd(bomid, num);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetListByPartId_For_Add(int partid, int num)
        {
            var list = _goodsService.GetListByPartId_For_PickAdd(partid, num);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetByEleId_For_Add(int eleid)
        {
            var info = _goodsService.GetViewByEleId_For_PickAdd(eleid);
            return Json(info, JsonRequestBehavior.AllowGet);
        }

        public string SavePick(string purpose)
        {
            var pgInfo = GetInfoByStream<List<Pick_ForAdd_View>>();
            var res = _pickService.SavePick(pgInfo, LoginUserName, purpose);
            return res.Message;
        }


      


    }
}