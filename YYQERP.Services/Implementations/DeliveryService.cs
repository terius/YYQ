using System;
using System.Collections.Generic;
using System.Linq;
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
    public class DeliveryService : BaseService<DeliverySet, int>, IDeliveryService
    {
        private readonly IRepository<DeliverySet, int> _Repository;
        private readonly IUnitOfWork _uow;
        private readonly ICommonCacheService _cacheService;
        private readonly IStockService _stockService;
        private readonly IRepository<StockSet, int> _stockRepository;
        private readonly IRepository<StockOutSet, int> _stockOutRepository;
        public DeliveryService(IRepository<DeliverySet, int> repository,
            IUnitOfWork uow,
            ICommonCacheService cacheService,
            IStockService stockService,
            IRepository<StockSet, int> stockRepository,
            IRepository<StockOutSet, int> stockOutRepository
       )
            : base(repository, uow)
        {
            _Repository = repository;
            _uow = uow;
            _cacheService = cacheService;
            _stockService = stockService;
            _stockRepository = stockRepository;
            _stockOutRepository = stockOutRepository;
        }

        public Search_Delivery_Response SearchDelivery(Search_Delivery_Request request)
        {

            Query<DeliverySet> q = new Query<DeliverySet>();
            if (request.STime != DateTime.MinValue)
            {
                q.And(d => d.Addtime >= request.STime);
            }
            if (request.ETime != DateTime.MinValue)
            {
                q.And(d => d.Addtime <= request.ETime);
            }

            q.OrderBy(d => new { d.Addtime }, true);
            int allcount = 0;
            var pageData = _Repository.PageQuery(q, request.page, request.rows, out allcount);
            var res = new Search_Delivery_Response();
            var unitList = _cacheService.GetCache_Unit();
            var userList = _cacheService.GetCache_User();
            res.rows = pageData.Convert_Delivery_To_DeliveryListView_List(unitList, userList);
            res.total = allcount;
            return res;
        }


        public void SaveAdd(Delivery_Add_View addInfo)
        {
            int id = 0;
            ElementType type = ElementType.Element;
            string msg = "";
            if (CheckSerialNoDup(addInfo.SerialNo))
            {
                throw new Exception("流水号:" + addInfo.SerialNo + "重复");
            }
            foreach (var item in addInfo.Details)
            {
                id = item.ElementId.HasValue ? item.ElementId.Value : item.ProductId.Value;
                type = item.ElementId.HasValue ? ElementType.Element : ElementType.Product;
                msg = _stockService.CheckStockQuantity(id, item.Quantity, type, item.Model);
                if (msg != "")
                {
                    throw new Exception(msg);
                }
            }
            DateTime dtNow = DateTime.Now;
            DeliverySet info = new DeliverySet();
            info.Addtime = dtNow;
            info.AddUserName = LoginUserName;
            info.Customer = addInfo.Customer;
            info.Manager = addInfo.Manager;
            info.OrderDate = Convert.ToDateTime(addInfo.OrderDate);
            info.OrderNo = addInfo.OrderNo;
            info.Remark = addInfo.Remark;
            info.Sender = addInfo.Sender;
            info.SerialNo = addInfo.SerialNo;
            info.DeliveryDetailSet = new List<DeliveryDetailSet>();
            DeliveryDetailSet detail;
            decimal allPrice = 0;
            Guid newGuid = Guid.NewGuid();
            foreach (var item2 in addInfo.Details)
            {
                detail = new DeliveryDetailSet();
                detail.Addtime = dtNow;
                detail.AddUserName = info.AddUserName;
                detail.ElementId = item2.ElementId;
                detail.Price = item2.Price;
                detail.ProductId = item2.ProductId;
                detail.Quantity = item2.Quantity;
                detail.Remark = item2.Remark;
                detail.TotalPrice = Math.Round(item2.Price * Convert.ToDecimal(item2.Quantity), 2);
                detail.UnitTypeCode = item2.UnitTypeCode;
                allPrice += detail.TotalPrice.Value;
                info.DeliveryDetailSet.Add(detail);

                //更新库存信息
                StockSet stockInfo = null;
                if (item2.ElementId.HasValue)
                {
                    stockInfo = _stockRepository.GetDbSetForEdit().FirstOrDefault(d => d.ElementId == item2.ElementId);
                }
                else
                {
                    stockInfo = _stockRepository.GetDbSetForEdit().FirstOrDefault(d => d.ProductId == item2.ProductId);
                }
                if (stockInfo == null)
                {
                    throw new Exception(item2.Model + "未入库");
                }
                stockInfo.Quantity = stockInfo.Quantity - item2.Quantity;
                stockInfo.LastOutTime = dtNow;
                stockInfo.LastOutQuantity = item2.Quantity;
                stockInfo.Modifytime = dtNow;
                stockInfo.ModifyUserName = info.AddUserName;
                _stockRepository.Save(stockInfo);


                //新增出库记录
                StockOutSet stockOutInfo = new StockOutSet();
                stockOutInfo.Addtime = dtNow;
                stockOutInfo.AddUserName = info.AddUserName;
                stockOutInfo.ElementId = item2.ElementId;
                stockOutInfo.ProductId = item2.ProductId;
                stockOutInfo.Quantity = item2.Quantity;
                stockOutInfo.Reason = "送货单出货，客户：" + info.Customer + " 订单号：" + info.OrderNo
                    + "  订单日期：" + info.OrderDate.ToString("yyyy-MM-dd");
                stockOutInfo.Remark = item2.Remark;
                stockOutInfo.ShelfId = stockInfo.ShelfId;
                stockOutInfo.GroupGuid = newGuid;
                stockOutInfo.ItemType = item2.ElementId.HasValue ? (int)ElementType.Element : (int)ElementType.Product;
                _stockOutRepository.Add(stockOutInfo);

            }
            info.TotalAmount = allPrice;
            info.TotalAmountUp = StringHelper.CmycurD(allPrice);
            _Repository.Add(info);
            _uow.Commit();
          

        }


        private bool CheckSerialNoDup(string sno)
        {
            return _Repository.GetDbQuerySet().Any(d => d.SerialNo == sno);
        }

        public string GetMaxSerialNo()
        {
            string nowString = DateTime.Now.ToString("yyyyMMdd");
            var maxNo = _Repository.GetDbQuerySet().Where(d => d.SerialNo.Contains(nowString)).Max(d => d.SerialNo);
            int itemp = 0;
            string lastNo = "001";
            if (maxNo != null && int.TryParse(maxNo.Substring(maxNo.Length - 3), out itemp))
            {
                lastNo = (itemp + 1).ToString().PadLeft(3, '0');
            }
            return nowString + lastNo;
        }


        public DeliveryForPrint GetDeliveryForPrint(int id)
        {
            var info = GetInfoByID(id);
            var unitList = _cacheService.GetCache_Unit();
            return info.Convert_Delivery_To_DeliveryForPrint(unitList);
        }


    }



}
