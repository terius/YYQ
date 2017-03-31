using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYQERP.Cache;
using YYQERP.Infrastructure;
using YYQERP.Infrastructure.Domain;
using YYQERP.Infrastructure.Enums;
using YYQERP.Infrastructure.Helpers;
using YYQERP.Infrastructure.Querying;
using YYQERP.Infrastructure.UnitOfWork;
using YYQERP.Repository;
using YYQERP.Services.Interfaces;
using YYQERP.Services.Messages;
using YYQERP.Services.Views;

namespace YYQERP.Services.Implementations
{
    public class SaleService : BaseService<SaleReportSet, int>, ISaleService
    {
        private readonly IRepository<SaleReportSet, int> _Repository;
        private readonly IUnitOfWork _uow;
        //private readonly IRepository<CustomerSet, int> _customerRepository;
        public SaleService(IRepository<SaleReportSet, int> repository,
            IUnitOfWork uow
       //   IRepository<CustomerSet, int> customerRepository
       )
            : base(repository, uow)
        {
            _Repository = repository;
            _uow = uow;
            //  _customerRepository = customerRepository;
        }

        public Search_SaleReport_Response SearchSaleReport(Search_SaleReport_Request request, bool isPage = true)
        {
            var res = new Search_SaleReport_Response();
            Query<SaleReportSet> q = new Query<SaleReportSet>();
            if (request.STime != DateTime.MinValue)
            {
                q.And(d => d.SaleDate >= request.STime);
            }
            if (request.ETime != DateTime.MinValue)
            {
                q.And(d => d.SaleDate <= request.ETime);
            }

            if (request.filterRules != null)
            {
                foreach (var item in request.filterRules)
                {
                    GetFilter(item, q);
                }
            }
            //  q.And(d => d.QQ == "asdasd");
            var trueName = request.LoginTrueName;
            if (!request.QueryAll)
            {
                q.And(d => d.SaleLeader.Equals(trueName));
            }
            q.OrderBy(d => new { d.Addtime }, true);
            int allcount = 0;
            if (isPage)
            {
                var pageData = _Repository.PageQuery(q, request.page, request.rows, out allcount);
                res.rows = pageData.SaleReportSet_To_SaleReportViews();
            }
            else
            {
                var pageData = _Repository.Query(q).ToList();
                res.rows = pageData.SaleReportSet_To_SaleReportViews();

            }
            res.total = allcount;
            return res;
        }

        private void GetFilter(filterRule item, Query<SaleReportSet> q)
        {
            DateTime dtTemp = DateTime.MinValue;
            int itemp = 0;
            decimal priceTemp = 0;
            switch (item.field)
            {
                case "AfterSales":
                    q.And(d => d.AfterSales.Contains(item.value));
                    break;
                case "SaleDate":
                    dtTemp = StringHelper.SafeGetDateTimeFromObj(item.value);
                    q.And(d => d.SaleDate == dtTemp);
                    break;
                case "CustomerLevel":
                    q.And(d => d.CustomerLevel.Contains(item.value));
                    break;
                case "Company":
                    q.And(d => d.Company.Contains(item.value));
                    break;
                case "SaleLeader":
                    q.And(d => d.SaleLeader.Contains(item.value));
                    break;
                case "PayDate":
                    // dtTemp = StringHelper.SafeGetDateTimeFromObj(item.value);
                    q.And(d => d.PayDate.Contains(item.value));
                    break;
                case "PayWay":
                    q.And(d => d.PayWay.Contains(item.value));
                    break;
                case "CustomerType":
                    q.And(d => d.CustomerType.Contains(item.value));
                    break;
                case "MachineType":
                    q.And(d => d.MachineType.Contains(item.value));
                    break;
                case "Model":
                    q.And(d => d.Model.Contains(item.value));
                    break;
                case "GoodsSpec":
                    q.And(d => d.GoodsSpec.Contains(item.value));
                    break;
                case "GoodsNum":
                    itemp = StringHelper.SafeGetIntFromObj(item.value);
                    q.And(d => d.GoodsNum == itemp);
                    break;
                case "TaxPrice":
                    priceTemp = StringHelper.SafeGetDecimalFromObj(item.value);
                    q.And(d => d.TaxPrice == priceTemp);
                    break;
                case "AllTaxPrice":
                    priceTemp = StringHelper.SafeGetDecimalFromObj(item.value);
                    q.And(d => d.AllTaxPrice == priceTemp);
                    break;
                case "LogisticsCost":
                    priceTemp = StringHelper.SafeGetDecimalFromObj(item.value);
                    q.And(d => d.LogisticsCost == priceTemp);
                    break;
                case "NoTaxPrice":
                    priceTemp = StringHelper.SafeGetDecimalFromObj(item.value);
                    q.And(d => d.NoTaxPrice == priceTemp);
                    break;
                case "Invoice":
                    q.And(d => d.Invoice.Contains(item.value));
                    break;
                case "DeliveryNote":
                    q.And(d => d.DeliveryNote.Contains(item.value));
                    break;
                case "Billing":
                    q.And(d => d.Billing.Contains(item.value));
                    break;
                case "MachineNo":
                    q.And(d => d.MachineNo.Contains(item.value));
                    break;
                case "OrderNo":
                    q.And(d => d.OrderNo.Contains(item.value));
                    break;
                case "Remark":
                    q.And(d => d.Remark.Contains(item.value));
                    break;
                case "Contacts":
                    q.And(d => d.Contacts.Contains(item.value));
                    break;
                case "Phone":
                    q.And(d => d.Phone.Contains(item.value));
                    break;
                case "Tel":
                    q.And(d => d.Tel.Contains(item.value));
                    break;
                case "Email":
                    q.And(d => d.Email.Contains(item.value));
                    break;
                case "QQ":
                    q.And(d => d.QQ.Contains(item.value));
                    break;
                case "CompanyAddress":
                    q.And(d => d.CompanyAddress.Contains(item.value));
                    break;
                case "Fax":
                    q.And(d => d.Fax.Contains(item.value));
                    break;
                case "Bonus":
                    q.And(d => d.Bonus.Contains(item.value));
                    break;
                case "LogisticsCompany":
                    q.And(d => d.LogisticsCompany.Contains(item.value));
                    break;

                default:
                    break;
            }
        }


        public string ImportSaleReport(DataTable dt, string loginUserName)
        {

            DateTime dtNow = DateTime.Now;
            //  SaleReportSet info = null;
            Guid newGuid = Guid.NewGuid();
            foreach (DataRow dr in dt.Rows)
            {
                SaleReportSet info = new SaleReportSet();

                info.AfterSales = dr[0].ToString();
                info.SaleDate = StringHelper.SafeGetNullAbleDateTimeFromObj(dr[1]);
                info.CustomerLevel = dr[2].ToString();
                info.Company = dr[3].ToString();
                info.SaleLeader = dr[4].ToString();
                info.PayDate = dr[5].ToString();
                info.PayWay = dr[6].ToString();
                info.CustomerType = dr[7].ToString();
                info.MachineType = dr[8].ToString();
                info.Model = dr[9].ToString();
                info.GoodsSpec = dr[10].ToString();
                info.GoodsNum = StringHelper.SafeGetIntFromObj(dr[11], 0);
                info.TaxPrice = StringHelper.SafeGetNullAbleDecimalFromObj(dr[12]);
                info.AllTaxPrice = StringHelper.SafeGetNullAbleDecimalFromObj(dr[13]);
                info.LogisticsCost = StringHelper.SafeGetNullAbleDecimalFromObj(dr[14]);
                info.NoTaxPrice = StringHelper.SafeGetNullAbleDecimalFromObj(dr[15]);
                info.Invoice = dr[16].ToString();
                info.DeliveryNote = dr[17].ToString();
                info.Billing = dr[18].ToString();
                info.MachineNo = dr[19].ToString();
                info.OrderNo = dr[20].ToString();
                info.Remark = dr[21].ToString();
                info.Contacts = dr[22].ToString();
                info.Phone = dr[23].ToString();
                info.Tel = dr[24].ToString();
                info.Email = dr[25].ToString();
                info.QQ = dr[26].ToString();
                info.CompanyAddress = dr[27].ToString();
                info.Fax = dr[28].ToString();
                info.Bonus = dr[29].ToString();
                info.LogisticsCompany = dr[30].ToString();
                info.Addtime = dtNow;
                info.AddUserName = loginUserName;
                info.GroupGuid = newGuid;
                _Repository.Add(info);


            }
            int rs = _uow.Commit();
            return rs > 0 ? "" : "导入失败";
        }


        public CEDResponse Add(SaleReportSet newInfo, string loginUserName)
        {
            CEDResponse res = new CEDResponse();
            newInfo.GroupGuid = Guid.NewGuid();
            newInfo.Addtime = DateTime.Now;
            newInfo.AddUserName = loginUserName;
            int rs = SaveAdd(newInfo);
            if (rs > 0)
            {
                res.Result = true;
            }
            else
            {
                res.Result = false;
                res.Message = "新增销售信息失败";
                return res;
            }


            return res;
        }

        public CEDResponse Edit(SaleReportView newInfo, string loginUserName)
        {
            CEDResponse res = new CEDResponse();
            var oldInfo = GetInfoByID(newInfo.Id);
            oldInfo.AfterSales = newInfo.AfterSales;
            oldInfo.SaleDate = StringHelper.SafeGetNullAbleDateTimeFromObj(newInfo.SaleDate);
            oldInfo.CustomerLevel = newInfo.CustomerLevel;
            oldInfo.Company = newInfo.Company;
            oldInfo.SaleLeader = newInfo.SaleLeader;
            oldInfo.PayDate = newInfo.PayDate;
            oldInfo.PayWay = newInfo.PayWay;
            oldInfo.CustomerType = newInfo.CustomerType;
            oldInfo.MachineType = newInfo.MachineType;
            oldInfo.Model = newInfo.Model;
            oldInfo.GoodsSpec = newInfo.GoodsSpec;
            oldInfo.GoodsNum = StringHelper.SafeGetIntFromObj(newInfo.GoodsNum, 0);
            oldInfo.TaxPrice = StringHelper.SafeGetNullAbleDecimalFromObj(newInfo.TaxPrice);
            oldInfo.AllTaxPrice = StringHelper.SafeGetNullAbleDecimalFromObj(newInfo.AllTaxPrice);
            oldInfo.LogisticsCost = StringHelper.SafeGetNullAbleDecimalFromObj(newInfo.LogisticsCost);
            oldInfo.NoTaxPrice = StringHelper.SafeGetNullAbleDecimalFromObj(newInfo.AfterSales);
            oldInfo.Invoice = newInfo.Invoice;
            oldInfo.DeliveryNote = newInfo.DeliveryNote;
            oldInfo.Billing = newInfo.Billing;
            oldInfo.MachineNo = newInfo.MachineNo;
            oldInfo.OrderNo = newInfo.OrderNo;
            oldInfo.Remark = newInfo.Remark;
            oldInfo.Contacts = newInfo.Contacts;
            oldInfo.Phone = newInfo.Phone;
            oldInfo.Tel = newInfo.Tel;
            oldInfo.Email = newInfo.Email;
            oldInfo.QQ = newInfo.QQ;
            oldInfo.CompanyAddress = newInfo.CompanyAddress;
            oldInfo.Fax = newInfo.Fax;
            oldInfo.Bonus = newInfo.Bonus;
            oldInfo.LogisticsCompany = newInfo.LogisticsCompany;
            oldInfo.Modifytime = DateTime.Now;
            oldInfo.ModifyUserName = loginUserName;

            int rs = SaveEdit(oldInfo);
            if (rs > 0)
            {
                res.Result = true;
            }
            else
            {
                res.Result = false;
                res.Message = "编辑销售信息失败";
                return res;
            }


            return res;
        }

        public CEDResponse Delete(int Id)
        {
            CEDResponse res = new CEDResponse();
            var info = GetInfoByID(Id);
            if (info != null)
            {
                int rs = Remove(info);
                if (rs > 0)
                {
                    res.Result = true;
                }
                else
                {
                    res.Message = "删除销售记录失败";
                }
            }
            return res;
        }



    }



}
