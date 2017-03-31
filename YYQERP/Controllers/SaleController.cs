using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using YYQERP.Cache;
using YYQERP.Infrastructure.Helpers;
using YYQERP.Repository;
using YYQERP.Services.Interfaces;
using YYQERP.Services.Messages;
using YYQERP.Services.Views;

namespace YYQERP.Controllers
{
    public class SaleController : BaseController
    {

        private readonly ISaleService _saleService;
        private readonly IUserService _userService;

        public SaleController(ISaleService saleService, IUserService userService)
        {
            _saleService = saleService;
            _userService = userService;
        }
        // GET: Pick
        public ActionResult Index()
        {
            ViewBag.Pers = GetUserOpers();
            ViewBag.MenuCode = Request["menucode"];
            return View();
        }

        public ActionResult GetSaleReportPageList(Search_SaleReport_Request request)
        {
            request.filterRules = GetFilterRules();
            request.QueryAll = CheckIfCanQueryAll(request);
            var list = _saleService.SearchSaleReport(request);
            var json = Json(list, JsonRequestBehavior.AllowGet);
            return json;
        }

        private bool CheckIfCanQueryAll(Search_SaleReport_Request request)
        {
         
            foreach (var item in GetUserOpers())
            {
                if (item.Equals("QueryAll",StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            var info = _userService.LoginUserInfo;
            request.LoginTrueName = info.UserTrueName;
            return false;
        }

        private IList<filterRule> GetFilterRules()
        {
            var rules = Request["filterRules"];
            IList<filterRule> list = string.IsNullOrEmpty(rules) ? null : JsonHelper.DeserializeObj<List<filterRule>>(rules);
            return list;
        }

        public string ImportFile()
        {
            string msg = "";
            var newFilePath = UploadFile("SaleReportUpload", out msg);
            if (msg != "")
            {
                return msg;
            }
            var data = ExcelHelper.GetSaleReportData(newFilePath);
            msg = _saleService.ImportSaleReport(data, LoginUserName);
            return msg;
        }

        public FileResult ExportExcel(Export_SaleReport_Request request)
        {
            request.filterRules = GetFilterRules();
            var data = _saleService.SearchSaleReport(request, request.ExportPageData);
            return Export<SaleReportView>(data.rows, request.Columns, request.ColumnTitles, request.FileName);
        }

        public string Add(SaleReportSet info)
        {

            var res = _saleService.Add(info,LoginUserName);
            return res.Message;
        }

        public string Edit(SaleReportView info)
        {
            var res = _saleService.Edit(info, LoginUserName);
            return res.Message;
        }


        public string Delete(int Id)
        {
            var res = _saleService.Delete(Id);
            return res.Message;
        }


    }
}