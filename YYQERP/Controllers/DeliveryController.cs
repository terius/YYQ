using System.Collections.Generic;
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

        private readonly IDeliveryService _deliveryService;
        private readonly ICommonCacheService _cacheService;
        private readonly IGoodsService _goodsService;

        public DeliveryController(IDeliveryService deliveryService, ICommonCacheService cacheService, IGoodsService goodsService)
        {
            _deliveryService = deliveryService;
            _cacheService = cacheService;
            _goodsService = goodsService;
        }
        // GET: Pick
        public ActionResult Index()
        {

            ViewBag.Pers = GetUserOpers();
            var model = new DeliveryViewModel();
            model.ElementSelectList = _cacheService.GetCache_Element();
            model.ProductSelectList = _goodsService.GetProductSelectListForDelivery();
            model.AddView = new Delivery_Add_View();
            model.AddView.Details = new List<DeliveryDetail_ForAdd_View>();
            return View(model);
        }

        public ActionResult GetPageList(Search_Delivery_Request request)
        {
            var list = _deliveryService.SearchDelivery(request);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }


        public ActionResult GetTemplate()
        {
            Delivery_Add_View view = new Delivery_Add_View();
            view.Details = new List<DeliveryDetail_ForAdd_View>();
           
            return Json(view, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddProductItem(int pid)
        {
            var view = _goodsService.AddProductItem_For_Delivery(pid);
            return Json(view, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddElementItem(int eleid)
        {
            var view = _goodsService.AddElementItem_For_Delivery(eleid);
            return Json(view, JsonRequestBehavior.AllowGet);
        }

        //public string SaveAdd(string purpose)
        //{
        //    var pgInfo = GetInfoByStream<List<Pick_ForAdd_View>>();
        //    var res = _pickService.SavePick(pgInfo, LoginUserName, purpose);
        //    return res.Message;
        //}

    }
}