using System;
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
            model.MaxSerialNo = _deliveryService.GetMaxSerialNo();
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

        public void SaveAdd()
        {
            var addInfo = GetInfoByStream<Delivery_Add_View>();
            _deliveryService.SaveAdd(addInfo);
            //  var res = _pickService.SavePick(pgInfo, LoginUserName, purpose);
            //  return res.Message;

        }

        public FileResult ExportExcel(int id,bool isOut)
        {

            string excelFile = Server.MapPath("~/ExcelTemplate/送货单模板.xlsx");
            string fileName = "送货单" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            string destFile = Server.MapPath("~/Output/" + fileName);
            var info = _deliveryService.GetDeliveryForPrint(id);
            ExcelHelper.ExportInvoice(excelFile, destFile, info);
            var result= File(destFile, "application/ms-excel", fileName);
            _deliveryService.UpdateIsOut(id, isOut);
            return result;
        }

    }
}