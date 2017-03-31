using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YYQERP.Cache;
using YYQERP.Infrastructure.Helpers;
using YYQERP.Services.Interfaces;
using YYQERP.Services.Messages;
using YYQERP.Services.Views;

namespace YYQERP.Controllers
{
    public class PickController : BaseController
    {

        private readonly IPickService _pickService;
        private readonly ICommonCacheService _cacheService;
        private readonly IGoodsService _goodsService;

        public PickController(IPickService pickService, ICommonCacheService cacheService, IGoodsService goodsService)
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


        #region 发料
        public ActionResult PickOut()
        {
            ViewBag.Pers = GetUserOpers();
            var model = new PickOutModel();
            model.UserSelectList = _cacheService.GetCache_User();
            return View(model);
        }

        public ActionResult GetPickOutPageList(Search_PickOut_Request request)
        {
            var list = _pickService.SearchPickOut(request);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }

        public ActionResult GetPickDetail_ForAdd(int parentid)
        {
            var list = _pickService.GetPickDetail_ForAdd(parentid);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }

        public string SavePickDetail(int parentid)
        {
            var pgInfo = GetInfoByStream<List<PickOut_ForAdd_View>>();
            var res = _pickService.SavePickOut(pgInfo, LoginUserName, parentid);
            return res.Message;
        }


        public string DeletePick(int id)
        {
            var msg = _pickService.DeletePick(id);
            return msg;
        }


        public FileResult ExportExcel(Export_PickOut_Request request)
        {
            var data = JsonHelper.DeserializeObj<List<PickOut_ForAdd_View>>(Server.UrlDecode(request.ExpData));
            return Export<PickOut_ForAdd_View>(data, request.Columns, request.ColumnTitles, request.FileName);
        }
        #endregion


    }
}