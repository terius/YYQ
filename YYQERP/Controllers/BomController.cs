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
    public class BomController : BaseController
    {

        private readonly IGoodsService _goodsService;
        private readonly ICommonCacheService _cacheService;
        public BomController(IGoodsService goodService, ICommonCacheService cacheService)
        {
            _goodsService = goodService;
            _cacheService = cacheService;
        }
        // GET: Bom
        public ActionResult Index()
        {
            ViewBag.Pers = GetUserOpers();
            var model = new BomModel();
            model.ElementCacheViews = _cacheService.GetCache_Element();
            model.ModelSelectList = _goodsService.GetModelSelectList();
            model.PartSelectList = _goodsService.GetPartSelectList();
            return View(model);
        }

        public ActionResult GetPageList(Search_Bom_Request request)
        {
            var list = _goodsService.SearchBom(request);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }

        public string Add(Add_Bom_Request request)
        {
            var res = _goodsService.AddBom(request);
            return res.Message;
        }

        public string Edit(Edit_Bom_Request request)
        {
            var res = _goodsService.EditBom(request);
            return res.Message;
        }

        public string Delete(int Id)
        {
            var res = _goodsService.DeleteBom(Id);
            return res.Message;
        }


        public ActionResult GetBomDetail(int bomid)
        {
            IList<BomDetailEditView> list = new List<BomDetailEditView>();
            if (bomid > 0)
            {
                list = _goodsService.GetBomDetail(bomid);
            }
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }


        public string SaveBomDetail(int bomid)
        {
            var pgInfo = GetInfoByStream<List<BomDetailEditView>>();
            var res = _goodsService.SaveBomDetail(pgInfo, bomid);
            return res.Message;
        }


        public ActionResult GetPartDetail(int partid)
        {
            IList<BomDetailEditView> list = new List<BomDetailEditView>();
            if (partid > 0)
            {
                list = _goodsService.GetPartDetailInBomDetail(partid);
            }
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }

        public ActionResult GetEleDetailForBom(int eleid)
        {
            var view = _goodsService.GetEleDetailInBomDetail(eleid);
            var json = Json(view, JsonRequestBehavior.AllowGet);
            return json;
        }

        public FileResult ExportExcel(Export_Bom_Request request)
        {
            IList<Export_Bom_View> list = new List<Export_Bom_View>();
            if (request.bomid > 0)
            {
                list = _goodsService.GetBomDetailExport(request.bomid);
            }
           
            return Export<Export_Bom_View>(list, request.Columns, request.ColumnTitles, request.FileName);
        }
    }
}