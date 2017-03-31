using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YYQERP.Infrastructure.Helpers;
using YYQERP.Services.Interfaces;
using YYQERP.Services.Messages;
using YYQERP.Services.Views;

namespace YYQERP.Controllers
{
    public class ElementController : BaseController
    {

        private readonly IGoodsService _goodsService;

        public ElementController(IGoodsService goodService)
        {
            _goodsService = goodService;
        }
        // GET: Element
        public ActionResult Index()
        {
           
            ViewBag.Pers = GetUserOpers();
            return View();
        }

        public ActionResult GetElementPageList(Search_Element_Request request)
        {
            var list = _goodsService.SearchElement(request);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }

        public ActionResult GetElementSelectList()
        {
            var list = _goodsService.GetElementSelectList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public string Add(Add_Element_Request request)
        {
            var res = _goodsService.AddElement(request);
            return res.Message;
        }

        public string Edit(Edit_Element_Request request)
        {
            var res = _goodsService.EditElement(request);
            return res.Message;
        }

        public string Delete(int Id)
        {
            var res = _goodsService.DeleteElement(Id);
            return res.Message;
        }

        public string ImportFile()
        {
            string msg = "";
            var newFilePath = UploadFile("EleUpload",out msg);
            if (msg != "")
            {
                return msg;
            }
            var data = ExcelHelper.GetData(newFilePath);
            msg = _goodsService.ImportElement(data, LoginUserName);
            return msg;
        }
        public string RestoreFile()
        {
            string msg = "";
            var newFilePath = UploadFile("EleUpload", out msg);
            if (msg != "")
            {
                return msg;
            }
            var data = ExcelHelper.GetData(newFilePath);
            msg = _goodsService.RestoreElement(data, LoginUserName);
            return msg;
        }

        public FileResult ExportExcel(Export_Element_Request request)
        {
            var data = _goodsService.SearchElement(request, request.ExportPageData);
            return Export<ElementListView>(data.rows, request.Columns, request.ColumnTitles, request.FileName);
        }

        
    }
}