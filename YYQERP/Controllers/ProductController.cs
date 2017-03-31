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
    public class ProductController : BaseController
    {

        private readonly IGoodsService _goodsService;
        private readonly IDicService _dicService;
        private readonly ICommonCacheService _cacheService;
        public ProductController(IGoodsService goodService, IDicService dicService, ICommonCacheService cacheService)
        {
            _goodsService = goodService;
            _dicService = dicService;
            _cacheService = cacheService;
        }
        // GET: Products
        public ActionResult Index()
        {
            ViewBag.Pers = GetUserOpers();
            var model = new ProductModel();
         
            model.ModelSelectList = _goodsService.GetModelSelectList();
            model.UnitSelectList = _dicService.GetUnitSelectList();
            model.ElementCacheViews = _cacheService.GetCache_Element();
            model.BomSelectList = _goodsService.GetBomSelectList();
            model.HalfProductSelectList = _goodsService.GetHalfProductSelectList();
            return View(model);
        }

        public ActionResult GetPageList(Search_Product_Request request)
        {
            var list = _goodsService.SearchProduct(request);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }

        public ActionResult GetProductDetail(int pid)
        {
            var list = _goodsService.GetProductDetail(pid);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }

        public string Add(Add_Product_Request request)
        {
            var res = _goodsService.AddProduct(request);
            return res.Message;
        }

        public string Edit(Edit_Product_Request request)
        {
            var res = _goodsService.EditProduct(request);
            return res.Message;
        }



        public ActionResult GetDetailByBomId(int bomid)
        {
            var list = _goodsService.GetElementListByBomId_In_ProductDetail(bomid);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetailByEleId(int eleid)
        {
            var list = _goodsService.GetElementByEleId_In_ProductDetail(eleid);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetailByProductId(int pid)
        {
            var list = _goodsService.GetElementListByHalfProductId_In_ProductDetail(pid);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public string SaveProductDetail(int pid)
        {
            var pgInfo = GetInfoByStream<List<ProductDetailListView>>();
            var res = _goodsService.SaveProductDetail(pgInfo, LoginUserName,pid);
            return res.Message;
        }
    }
}