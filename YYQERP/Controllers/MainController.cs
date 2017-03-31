using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YYQERP.Services.Interfaces;
using YYQERP.Services.Messages;

namespace YYQERP.Controllers
{
    public class MainController : BaseController
    {

        private readonly IStockService _service;

        public MainController(IStockService service)
        {
            _service = service;
        }

        // GET: Main
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetPageList(Search_StockWarn_Request request)
        {
            var list = _service.SearchStockWarn(request);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }
    }
}