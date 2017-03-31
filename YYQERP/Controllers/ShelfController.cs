using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YYQERP.Services.Interfaces;
using YYQERP.Services.Messages;

namespace YYQERP.Controllers
{
    public class ShelfController : BaseController
    {

        private readonly IGoodsService _goodsService;

        public ShelfController(IGoodsService goodService)
        {
            _goodsService = goodService;
        }
        // GET: Shelfs
        public ActionResult Index()
        {
            ViewBag.Pers = GetUserOpers();
            return View();
        }

        public ActionResult GetPageList(Search_Shelf_Request request)
        {
            var list = _goodsService.SearchShelf(request);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }

        public string Add(Add_Shelf_Request request)
        {
            var res = _goodsService.AddShelf(request);
            return res.Message;
        }

        public string Edit(Edit_Shelf_Request request)
        {
            var res = _goodsService.EditShelf(request);
            return res.Message;
        }

        public string Delete(int Id)
        {
            var res = _goodsService.DeleteShelf(Id);
            return res.Message;
        }
    }
}