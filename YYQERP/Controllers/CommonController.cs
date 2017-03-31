using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YYQERP.Services.Interfaces;

namespace YYQERP.Controllers
{
    public class CommonController : BaseController
    {

        private readonly IStockService _stockService;
        private readonly IDicService _dicService;
        private readonly IGoodsService _goodsService;

        public CommonController(IStockService stockService, IDicService dicService, IGoodsService goodsService)
        {
            _stockService = stockService;
            _dicService = dicService;
            _goodsService = goodsService;
        }
        // GET: Common
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetUnitSelectList()
        {
            var list = _dicService.GetUnitSelectList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetShelfSelectList()
        {
            var list = _goodsService.GetShelfSelectList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }


   

    }
}