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
    public class StockController : BaseController
    {


        private readonly IStockService _service;
        private readonly ICommonCacheService _cacheService;
        private readonly IGoodsService _goodsService;

        public StockController(IStockService service, ICommonCacheService cacheService, IGoodsService goodsService)
        {
            _service = service;
            _cacheService = cacheService;
            _goodsService = goodsService;
        }
        
        #region 库存

        public ActionResult Index()
        {
            ViewBag.Pers = GetUserOpers();
            return View();
        }

        public ActionResult GetPageList(Search_Stock_Request request)
        {
            var list = _service.SearchStock(request);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }
        public ActionResult GetStockDetail(int id)
        {
            var html = _service.GetStockDetailHtml(id);
            return Content(html);
        }

      

        public FileResult ExportExcelForStock(Export_Stock_Request request)
        {
            var data = _service.SearchStock(request, request.ExportPageData);
            return Export<StockListView>(data.rows, request.Columns, request.ColumnTitles, request.FileName);
        }


        public string DeleteStock(int id)
        {
            var msg = _service.DeleteStock(id);
            return msg;
        }

        #endregion


        #region 入库

        public ActionResult StockIn()
        {
            ViewBag.Pers = GetUserOpers();
            var model = new StockInModel();
            model.StockProductSelectList = _goodsService.GetProductSelectListForStockIn();
            model.ShelfSelectList = _goodsService.GetShelfSelectList();
            return View(model);
        }

        public ActionResult GetStockInPageList(Search_StockIn_Request request)
        {
            var list = _service.SearchStockIn(request);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }
        public ActionResult GetStockInTempleteList()
        {
            IList<StockInView> list = new List<StockInView>();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStockInTempleteListByProduct()
        {
            IList<StockIn_ForAdd_ByProductView> list = new List<StockIn_ForAdd_ByProductView>();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveStockIn(string reason)
        {
            var pgInfo = GetInfoByStream<PageResponse<StockInView>>();
            var res = _service.SaveStockIn(pgInfo, LoginUserName, reason);
            if (res.Result)
            {
                return Content("");
            }
            return Content(res.Message);
        }

        public ActionResult GetStockInListByProductId(int pid)
        {
            var list = _goodsService.GetStockInAddItemByProductId(pid);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public string SaveStockInByProduct(string reason)
        {
            var pgInfo = GetInfoByStream<List<StockIn_ForAdd_ByProductView>>();
            var res = _service.SaveStockInByProduct(pgInfo, LoginUserName, reason);
            return res.Message;
        }
        public FileResult ExportExcelForStockIn(Export_StockIn_Request request)
        {
            var data = _service.SearchStockIn(request, request.ExportPageData);
            return Export<StockInListView>(data.rows, request.Columns, request.ColumnTitles, request.FileName);
        }

        public string DeleteStockIn(int id)
        {
            var msg = _service.DeleteStockIn(id);
            return msg;
        }

        #endregion


        #region 出库

        public ActionResult StockOut()
        {
            ViewBag.Pers = GetUserOpers();
            var model = new StockOutModel();
            model.ElementCacheViews = _cacheService.GetCache_Element();
            model.BomSelectList = _goodsService.GetBomSelectList();
            //   model.ModelCacheViews = _cacheService.GetCache_Model();
            model.StockProductSelectList = _service.GetProductSelectListForStockOut();
            return View(model);
        }
        public ActionResult GetStockOutPageList(Search_StockOut_Request request)
        {
            var list = _service.SearchStockOut(request);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }
        public ActionResult GetStockOutTempleteList()
        {
            IList<StockOut_ForAdd_View> list = new List<StockOut_ForAdd_View>();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStockOutTempleteListByProduct()
        {
            IList<StockOut_ForAdd_ByProductView> list = new List<StockOut_ForAdd_ByProductView>();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStockOutListByBomId(int bomid,int num)
        {
            var list = _goodsService.GetStockOutListByBomId(bomid, num);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStockOutListByEleId(int eleid)
        {
            var list = _goodsService.GetStockOutListByEleId(eleid);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStockOutListByProductId(int stockid)
        {
            var list = _service.GetStockOutAddItemByProductId(stockid);
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public string SaveStockOut(string reason)
        {
            var pgInfo = GetInfoByStream<List<StockOut_ForAdd_View>>();
            var res = _service.SaveStockOut(pgInfo, LoginUserName, reason);
            return res.Message;
        }

        public string SaveStockOutByProduct(string reason)
        {
            var pgInfo = GetInfoByStream<List<StockOut_ForAdd_ByProductView>>();
            var res = _service.SaveStockOutByProduct(pgInfo, LoginUserName, reason);
            return res.Message;
        }
        public string DeleteStockOut(int id)
        {
            var msg = _service.DeleteStockOut(id);
            return msg;
        }

        public FileResult ExportExcelForStockOut(Export_StockOut_Request request)
        {
            var data = _service.SearchStockOut(request, request.ExportPageData);
            return Export<StockOutListDetailView>(data.rows, request.Columns, request.ColumnTitles, request.FileName);
        }

        //public string DeleteStockOutMain(int id)
        //{
        //    var msg = _service.DeleteStockOutMain(id);
        //    return msg;
        //}

        #endregion
        

      
    }
}