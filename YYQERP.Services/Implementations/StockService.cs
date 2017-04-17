using System;
using System.Collections;
using System.Collections.Generic;
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
    public class StockService : BaseService<StockSet, int>, IStockService
    {
        private readonly IRepository<StockSet, int> _Repository;
        private readonly IRepository<DicSet, int> _dicRepository;
        private readonly IRepository<StockInSet, int> _stockInRepository;
        private readonly IRepository<StockOutSet, int> _stockOutRepository;
        //  private readonly IRepository<StockOutMainSet, int> _stockOutMainRepository;
        private readonly IUnitOfWork _uow;
        private readonly IDicService _dicService;
        private readonly ICommonCacheService _cacheService;
        public StockService(IRepository<StockSet, int> repository, IUnitOfWork uow,
            IRepository<ElementSet, int> eleRepository,
            IRepository<ShelfSet, int> shelfRepository,
             IRepository<DicSet, int> dicRepository,
            IRepository<StockInSet, int> stockInRepository,
            IDicService dicService,
            ICommonCacheService cacheService,
              IRepository<StockOutSet, int> stockOutRepository
       )
            : base(repository, uow)
        {
            _Repository = repository;
            _uow = uow;
            _dicRepository = dicRepository;
            _stockInRepository = stockInRepository;
            _dicService = dicService;
            _cacheService = cacheService;
            _stockOutRepository = stockOutRepository;
            // _stockOutMainRepository = stockOutMainRepository;
        }

        public Search_Stock_Response SearchStock(Search_Stock_Request request, bool isPage)
        {
            var res = new Search_Stock_Response();
            Query<StockSet> q = new Query<StockSet>();
            if (!string.IsNullOrEmpty(request.ShelfCode))
            {
                q.And(d => d.ShelfSet.Code.Contains(request.ShelfCode));
            }
            if (!string.IsNullOrEmpty(request.ElementNameOrCode))
            {
                q.And(d => d.ElementSet.Name.Contains(request.ElementNameOrCode) || d.ElementSet.Code.Contains(request.ElementNameOrCode));
                //  q.And(d => d.el.Name.Contains(request.ShelfName));
            }

            if (!string.IsNullOrEmpty(request.ProductCode))
            {
                q.And(d => d.ProductSet.Aliases.Contains(request.ProductCode));
                //  q.And(d => d.el.Name.Contains(request.ShelfName));
            }

            q.OrderBy(d => new { d.Id });
            int allcount = 0;
            IList<StockSet> pageData = null;
            if (isPage)
            {
                pageData = _Repository.PageQuery(q, request.page, request.rows, out allcount);
            }
            else
            {
                pageData = _Repository.Query(q).ToList();
            }

            var dicList = _cacheService.GetCache_Dic();
            res.rows = pageData.ConvertTo_StockListViews(dicList);
            res.total = allcount;
            return res;
        }


        public Search_StockWarn_Response SearchStockWarn(Search_StockWarn_Request request)
        {
            Query<StockSet> q = new Query<StockSet>();
            if (!string.IsNullOrEmpty(request.ShelfName))
            {
                q.And(d => d.ShelfSet.Code.Contains(request.ShelfName));
            }
            if (!string.IsNullOrEmpty(request.ElementName))
            {
                q.And(d => d.ElementSet.Name.Contains(request.ElementName));
                //  q.And(d => d.el.Name.Contains(request.ShelfName));
            }
            q.And(d => d.ElementId > 0 && (d.ElementSet.WarningQuantity > d.Quantity || d.ElementSet.PickQuantity > d.Quantity));
            q.OrderBy(d => new { d.Id });
            int allcount = 0;
            var pageData = _Repository.PageQuery(q, request.page, request.rows, out allcount);
            var res = new Search_StockWarn_Response();
            var dicList = _cacheService.GetCache_Dic();
            res.rows = pageData.ConvertTo_StockWarnListViews(dicList);
            res.total = allcount;
            return res;
        }





        public CEDResponse SaveStockIn(PageResponse<StockInView> pgInfo, string currUserName, string reason)
        {
            int rs = 0;
            var res = new CEDResponse();
            StockInSet info = null;
            Guid newGuid = Guid.NewGuid();
            DateTime dtNow = DateTime.Now;
            foreach (StockInView view in pgInfo.rows)
            {
                if (CheckStockInData(view))
                {
                    info = new StockInSet();
                    info.Addtime = dtNow;
                    info.AddUserName = currUserName;
                    info.ElementId = view.ElementId;
                    info.GroupGuid = newGuid;
                    info.Quantity = view.Quantity;
                    info.ShelfId = view.ShelfId;
                    // info.UnitTypeCode = view.UnitTypeCode;
                    info.ItemType = (int)ElementType.Element;
                    info.Reason = reason;
                    _stockInRepository.Add(info);
                    int ed = _uow.Commit();
                    if (ed > 0)
                    {
                        ed = UpdateStock(info, view.UnitTypeCode);
                        if (ed <= 0)
                        {
                            _stockInRepository.Remove(info);
                            _uow.Commit();
                        }
                        rs += ed;
                    }
                }

            }

            if (rs > 0)
            {
                res.Result = true;
            }
            else
            {
                res.Message = "入库保存失败";
            }
            return res;
        }

        private int UpdateStock(StockInSet info, string unitCode)
        {
            int rs = 0;
            StockSet stockInfo = null;

            if (info.ElementId.HasValue)
            {
                stockInfo = _Repository.GetDbSetForEdit().Where(d => d.ElementId == info.ElementId.Value && d.ShelfId == info.ShelfId).FirstOrDefault();
            }
            else
            {
                stockInfo = _Repository.GetDbSetForEdit().Where(d => d.ProductId == info.ProductId.Value && d.ShelfId == info.ShelfId).FirstOrDefault();
            }
            if (stockInfo == null)
            {
                stockInfo = new StockSet();
                stockInfo.Addtime = info.Addtime.Value;
                stockInfo.AddUserName = info.AddUserName;
                stockInfo.ElementId = info.ElementId;
                stockInfo.FirstInTime = info.Addtime.Value;
                stockInfo.ItemType = info.ItemType;
                stockInfo.LastInQuantity = stockInfo.Quantity = info.Quantity;
                stockInfo.LastInTime = info.Addtime.Value;
                stockInfo.ShelfId = info.ShelfId;
                stockInfo.UnitTypeCode = unitCode;
                stockInfo.ProductId = info.ProductId;
                rs = SaveAdd(stockInfo);
            }
            else
            {
                stockInfo.LastInQuantity = info.Quantity;
                stockInfo.LastInTime = info.Addtime.Value;
                stockInfo.Modifytime = DateTime.Now;
                stockInfo.ModifyUserName = info.AddUserName;
                stockInfo.Quantity = stockInfo.Quantity + info.Quantity;
                stockInfo.ElementSet = null;
                stockInfo.ProductSet = null;
                stockInfo.ShelfSet = null;
                rs = SaveEdit(stockInfo);

            }
            return rs;
        }


        private string UpdateStockByStockOut(StockOutSet info, bool isProduct)
        {

            StockSet stockInfo = null;
            if (!isProduct)
            {
                stockInfo = _Repository.GetDbSetForEdit().Where(d => d.ElementId == info.ElementId && d.ShelfId == info.ShelfId).FirstOrDefault();
            }
            else
            {
                stockInfo = _Repository.GetDbSetForEdit().Where(d => d.ProductId == info.ProductId && d.ShelfId == info.ShelfId).FirstOrDefault();
            }
            if (stockInfo != null)
            {
                stockInfo.LastOutQuantity = info.Quantity;
                stockInfo.LastOutTime = info.Addtime.Value;
                stockInfo.Modifytime = info.Addtime.Value;
                stockInfo.ModifyUserName = info.AddUserName;
                stockInfo.Quantity = stockInfo.Quantity - info.Quantity;
                if (stockInfo.Quantity < 0)
                {
                    string error = isProduct ? "产品" + stockInfo.ProductSet.Aliases + "库存不足" : "原材料" + stockInfo.ElementSet.Name + "库存不足";
                    return error;
                }
                stockInfo.ElementSet = null;
                stockInfo.ProductSet = null;
                stockInfo.ShelfSet = null;
                _Repository.Save(stockInfo);
            }
            return "";

        }

        private bool CheckStockInData(StockInView data)
        {
            if (data.ElementId <= 0)
            {
                return false;
            }

            if (data.ShelfId <= 0)
            {
                return false;
            }

            if (data.Quantity <= 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(data.UnitTypeCode))
            {
                return false;
            }
            return true;
        }


        public Search_StockIn_Response SearchStockIn(Search_StockIn_Request request, bool isPage)
        {
            Query<StockInSet> q = new Query<StockInSet>();
            if (!string.IsNullOrEmpty(request.ElementCode))
            {
                q.And(d => d.ElementSet.Code.Contains(request.ElementCode));
            }
            if (!string.IsNullOrEmpty(request.ShelfCode))
            {
                q.And(d => d.ShelfSet.Code.Contains(request.ShelfCode));
            }
            q.OrderBy(d => new { d.Addtime }, true);
            int allcount = 0;
            IList<StockInSet> pageData = null;
            if (isPage)
            {
                pageData = _stockInRepository.PageQuery(q, request.page, request.rows, out allcount);
            }
            else
            {
                pageData = _stockInRepository.Query(q).ToList();
            }
            var res = new Search_StockIn_Response();
            var unitList = _dicService.GetUnitList();
            res.rows = pageData.ConvertTo_StockInListViews(unitList, this);
            res.total = allcount;
            return res;
        }


        public string DeleteStockIn(int id)
        {
            var info = _stockInRepository.Single(id);
            if (info != null)
            {
                bool isEle = info.ElementId.HasValue ? true : false;
                int itemid = info.ElementId.HasValue ? info.ElementId.Value : info.ProductId.Value;
                int shelfid = info.ShelfId;
                _stockInRepository.Remove(info);
                StockSet stockInfo = null;
                if (isEle)
                {
                    stockInfo = _Repository.GetDbSetForEdit().Where(d => d.ElementId == itemid && d.ShelfId == shelfid).FirstOrDefault();
                }
                else
                {
                    stockInfo = _Repository.GetDbSetForEdit().Where(d => d.ProductId == itemid && d.ShelfId == shelfid).FirstOrDefault();
                }
                if (stockInfo != null)
                {
                    stockInfo.Quantity -= info.Quantity;
                    if (stockInfo.Quantity < 0)
                    {
                        return "库存不足，禁止删除";
                    }
                    _Repository.Save(stockInfo);
                }
                int rs = _uow.Commit();
                if (rs <= 0)
                {
                    return "删除失败";
                }
            }
            else
            {
                return "未找到删除的入库记录";
            }
            return "";
        }

        public Search_StockOut_Response SearchStockOut(Search_StockOut_Request request, bool isPage)
        {
            Query<StockOutSet> q = new Query<StockOutSet>();
            if (!string.IsNullOrEmpty(request.ElementCode))
            {
                q.And(d => d.ElementSet.Code.Contains(request.ElementCode) || d.ElementSet.Name.Contains(request.ElementCode));
            }
            if (!string.IsNullOrEmpty(request.ShelfCode))
            {
                q.And(d => d.ShelfSet.Code.Contains(request.ShelfCode));
            }
            q.OrderBy(d => new { d.Addtime }, true);
            int allcount = 0;
            IList<StockOutSet> pageData = null;
            if (isPage)
            {
                pageData = _stockOutRepository.PageQuery(q, request.page, request.rows, out allcount);
            }
            else
            {
                pageData = _stockOutRepository.Query(q).ToList();
            }
            var res = new Search_StockOut_Response();
            var unitList = _dicService.GetUnitList();
            res.rows = pageData.ConvertTo_StockOutDetailListViews(unitList, this);
            res.total = allcount;
            return res;


            //Query<StockOutMainSet> q = new Query<StockOutMainSet>();
            //if (!string.IsNullOrEmpty(request.ElementCode))
            //{
            //    q.And(d => d.StockOutSet.Any(f => f.ElementSet.Code.Contains(request.ElementCode) || f.ElementSet.Name.Contains(request.ElementCode)));
            //}
            //if (!string.IsNullOrEmpty(request.ShelfCode))
            //{
            //    q.And(d => d.StockOutSet.Any(f => f.ShelfSet.Code.Contains(request.ShelfCode)));
            //}
            //q.OrderBy(d => new { d.Addtime }, true);
            //int allcount = 0;
            //var pageData = _stockOutMainRepository.PageQuery(q, request.page, request.rows, out allcount);
            //var res = new Search_StockOut_Response();
            //var unitList = _dicService.GetUnitList();
            //res.rows = pageData.ConvertTo_StockOutListViews(unitList, this);
            //res.total = allcount;
            //return res;
        }

        public string GetStockDetailHtml(int id)
        {
            var info = GetInfoByID(id);
            var dicCache = _cacheService.GetCache_Dic();
            var view = info.ConvertTo_StockDetailView(dicCache);
            string s = MyHtmlHelper.CreateDetailHtml<StockDetailView>(view);
            return s;
        }

        public CEDResponse SaveStockOut(IList<StockOut_ForAdd_View> list, string currUserName, string Reason)
        {
            var res = new CEDResponse();
            var msg = CheckQuantity(list);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }

            var info = new StockOutSet();
            DateTime dtNow = DateTime.Now;
            //  int MainId = CreateStockOutMain(Reason, dtNow, currUserName);
            Guid groupGuid = Guid.NewGuid();
            int rs = 0;
            foreach (var item in list)
            {
                info = new StockOutSet();
                info.Addtime = dtNow;
                info.AddUserName = currUserName;
                info.ElementId = item.ElementId;
                if (item.BomId > 0)
                {
                    info.BomId = item.BomId;
                }
                info.GroupGuid = groupGuid;
                info.Quantity = item.Quantity;
                info.Remark = item.Remark;
                info.ShelfId = item.ShelfId;
                info.ItemType = item.ItemType;
                info.Reason = Reason;
                info.PickId = item.PickId;
                _stockOutRepository.Add(info);
                //int ed = _uow.Commit();
                //if (ed > 0)
                //{
                msg = UpdateStockByStockOut(info, false);
                if (msg != "")
                {
                    res.Result = false;
                    res.Message = msg;
                    return res;
                }
                //if (ed <= 0)
                //{
                //    _stockOutRepository.Remove(info);
                //    _uow.Commit();
                //}
                //    rs += ed;
                //}
            }
            rs = _uow.Commit();
            if (rs > 0)
            {
                res.Result = true;
            }
            else
            {
                res.Message = "出库保存失败";
            }
            return res;
        }

        //private int CreateStockOutMain(string Reason, DateTime dtNow, string currUserName)
        //{
        //    var info = new StockOutMainSet();
        //    info.Reason = Reason;
        //    info.Addtime = dtNow;
        //    info.AddUserName = currUserName;
        //    _stockOutMainRepository.Add(info);
        //    _uow.Commit();
        //    return info.Id;
        //}


        public CEDResponse SaveStockOutByProduct(IList<StockOut_ForAdd_ByProductView> list, string currUserName, string Reason)
        {
            var res = new CEDResponse();
            var msg = CheckQuantity(list);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }
            var info = new StockOutSet();
            DateTime dtNow = DateTime.Now;
            Guid guid = Guid.NewGuid();
            int rs = 0;
            // int mainId = CreateStockOutMain(Reason, dtNow, currUserName);
            Guid groupGuid = Guid.NewGuid();
            foreach (var item in list)
            {
                info = new StockOutSet();
                info.Addtime = dtNow;
                info.AddUserName = currUserName;
                info.ProductId = item.ProductId;
                info.GroupGuid = groupGuid;
                info.Quantity = item.Quantity;
                info.Remark = item.Remark;
                info.ShelfId = item.ShelfId;
                info.ItemType = item.ItemType;
                info.Reason = Reason;
                _stockOutRepository.Add(info);
                //int ed = _uow.Commit();
                //if (ed > 0)
                //{
                msg = UpdateStockByStockOut(info, true);
                if (msg != "")
                {
                    res.Result = false;
                    res.Message = msg;
                    return res;
                }
                //if (ed <= 0)
                //{
                //    _stockOutRepository.Remove(info);
                //    _uow.Commit();
                //}
                //rs += ed;
                //}
            }
            rs = _uow.Commit();
            if (rs > 0)
            {
                res.Result = true;
            }
            else
            {
                res.Message = "出库保存失败";
            }
            return res;
        }

        private string CheckQuantity(IList<StockOut_ForAdd_View> list)
        {
            foreach (var item in list)
            {
                var num = item.Quantity;
                if (num <= 0)
                {
                    return "原材料" + item.ElementName + "出库数量不能为0";
                }
                string stockNum = GetStockQuantity(item.ElementId, item.ShelfId);
                if (stockNum == "未入库")
                {
                    return "原材料" + item.ElementName + "库存不足，需求：" + num + "  库存：" + stockNum + "  库位：" + item.ShelfName;
                }
                else
                {
                    double snum = Convert.ToDouble(stockNum);
                    if (num > snum)
                    {
                        return "原材料" + item.ElementName + "库存不足，需求：" + num + "  库存：" + stockNum + "  库位：" + item.ShelfName;
                    }
                }
            }
            return "";
        }

        private string CheckQuantity(IList<StockOut_ForAdd_ByProductView> list)
        {
            foreach (var item in list)
            {
                double num = item.Quantity;
                string stockNum = GetStockQuantity(item.ProductId, item.ShelfId, true);
                if (stockNum == "未入库")
                {
                    return "产品" + item.ProductName + "库存不足，需求：" + num + "  库存：" + stockNum + "  库位：" + item.ShelfName;
                }
                else
                {
                    double snum = Convert.ToDouble(stockNum);
                    if (num > snum)
                    {
                        return "产品" + item.ProductName + "库存不足，需求：" + num + "  库存：" + stockNum + "  库位：" + item.ShelfName;
                    }
                }
            }
            return "";
        }

        private string CheckShelfByStockIn(IList<StockIn_ForAdd_ByProductView> list)
        {
            foreach (var item in list)
            {
                if (item.ShelfId <= 0)
                {
                    return item.ProductName + "存储的库位错误";
                }
            }
            return "";
        }

        public string GetStockQuantity(int eleId, int shelfId, bool isProduct = false)
        {
            StockSet info = null;
            if (!isProduct)
            {
                info = _Repository.GetDbQuerySet().Where(d => d.ElementId == eleId && d.ShelfId == shelfId).FirstOrDefault();
            }
            else
            {
                info = _Repository.GetDbQuerySet().Where(d => d.ProductId == eleId && d.ShelfId == shelfId).FirstOrDefault();
            }
            return info == null ? "未入库" : info.Quantity.ToString();

        }


        public double GetStockQuantityNum(int eleId, int shelfId, bool isProduct = false)
        {
            StockSet info = null;
            if (!isProduct)
            {
                info = _Repository.GetDbQuerySet().Where(d => d.ElementId == eleId && d.ShelfId == shelfId).FirstOrDefault();
            }
            else
            {
                info = _Repository.GetDbQuerySet().Where(d => d.ProductId == eleId && d.ShelfId == shelfId).FirstOrDefault();
            }
            return info == null ? 0 : info.Quantity;

        }

        public double GetStockQuantity(int eleId)
        {
            StockSet info = _Repository.GetDbQuerySet().Where(d => d.ElementId == eleId).FirstOrDefault();
            return info == null ? 0 : info.Quantity;
        }

        public string DeleteStockOut(int id)
        {
            var info = _stockOutRepository.Single(id);
            if (info != null)
            {
                bool isEle = info.ElementId.HasValue ? true : false;
                int itemid = info.ElementId.HasValue ? info.ElementId.Value : info.ProductId.Value;
                int shelfid = info.ShelfId;
                _stockOutRepository.Remove(info);
                StockSet stockInfo = null;
                if (isEle)
                {
                    stockInfo = _Repository.GetDbSetForEdit().Where(d => d.ElementId == itemid && d.ShelfId == shelfid).FirstOrDefault();
                }
                else
                {
                    stockInfo = _Repository.GetDbSetForEdit().Where(d => d.ProductId == itemid && d.ShelfId == shelfid).FirstOrDefault();
                }
                if (stockInfo != null)
                {
                    stockInfo.Quantity += info.Quantity;
                    _Repository.Save(stockInfo);
                }
                int rs = _uow.Commit();
                if (rs <= 0)
                {
                    return "删除失败";
                }
            }
            else
            {
                return "未找到删除的出库记录";
            }
            return "";
        }

        //public string DeleteStockOutMain(int mainId)
        //{
        //    string rsMsg = "删除失败";
        //    int icount = 0;
        //    var list = _stockOutRepository.GetDbQuerySet().Where(d => d.MainId == mainId).ToList();
        //    IList<StockOutDetailDeleteView> delList = new List<StockOutDetailDeleteView>();
        //    StockOutDetailDeleteView dInfo = null;
        //    foreach (var item in list)
        //    {
        //        dInfo = new StockOutDetailDeleteView();
        //        dInfo.IsEle = item.ElementId.HasValue ? true : false;
        //        dInfo.ElementId = item.ElementId;
        //        dInfo.ProductId = item.ProductId;
        //        dInfo.Quantity = item.Quantity;
        //        dInfo.ShelfId = item.ShelfId;
        //        delList.Add(dInfo);
        //    }
        //    var info = _stockOutMainRepository.Single(mainId);
        //    if (info != null)
        //    {
        //        _stockOutMainRepository.Remove(info);
        //        UpdateStockQuantity(delList);
        //        icount = _uow.Commit();
        //        if (icount> 0)
        //        {
        //            rsMsg = "";
        //        }
        //    }

        //    return rsMsg;
        //}

        private void UpdateStockQuantity(IList<StockOutDetailDeleteView> delList)
        {
            StockSet stockInfo = null;
            foreach (var item in delList)
            {
                if (item.IsEle)
                {
                    stockInfo = _Repository.GetDbSetForEdit().Where(d => d.ElementId == item.ElementId && d.ShelfId == item.ShelfId).FirstOrDefault();
                }
                else
                {
                    stockInfo = _Repository.GetDbSetForEdit().Where(d => d.ProductId == item.ProductId && d.ShelfId == item.ShelfId).FirstOrDefault();
                }
                if (stockInfo != null)
                {
                    stockInfo.Quantity += item.Quantity;
                    _Repository.Save(stockInfo);
                }
            }
        }

        public IList<Default_SelectItem> GetProductSelectListForStockOut()
        {
            var list = _Repository.GetDbQuerySet().Where(d => d.ProductId > 0 && d.Quantity > 0).ToList();
            IList<Default_SelectItem> selecters = new List<Default_SelectItem>();
            Default_SelectItem info = null;
            foreach (var item in list)
            {
                info = new Default_SelectItem();
                info.id = item.ProductId.Value;
                info.text = string.Format("{0} 库位：{1} 库存数：{2}", item.ProductSet.Aliases + "【" + item.ProductSet.ModelSet.Name + "】", item.ShelfSet.Code, item.Quantity);
                info.extdata = item.Id.ToString();
                selecters.Add(info);
            }
            return selecters;
        }




        public StockOut_ForAdd_ByProductView GetStockOutAddItemByProductId(int stockid)
        {
            var info = GetInfoByID(stockid);
            var unitList = _cacheService.GetCache_Unit();
            var view = info.ConvertTo_StockOut_ForAdd_ByProductView(unitList);
            return view;
        }


        public CEDResponse SaveStockInByProduct(IList<StockIn_ForAdd_ByProductView> list, string currUserName, string reason)
        {
            var res = new CEDResponse();
            var msg = CheckShelfByStockIn(list);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }
            var info = new StockInSet();
            DateTime dtNow = DateTime.Now;
            Guid guid = Guid.NewGuid();
            int rs = 0;
            foreach (var item in list)
            {
                info = new StockInSet();
                info.Addtime = dtNow;
                info.AddUserName = currUserName;
                info.ProductId = item.ProductId;
                info.GroupGuid = guid;
                info.Quantity = item.Quantity;
                info.Remark = item.Remark;
                info.ShelfId = item.ShelfId;
                info.ItemType = item.ItemType;
                info.Reason = reason;
                _stockInRepository.Add(info);
                int ed = _uow.Commit();
                if (ed > 0)
                {
                    ed = UpdateStock(info, item.UnitTypeCode);
                    if (ed <= 0)
                    {
                        _stockInRepository.Remove(info);
                        _uow.Commit();
                    }
                    rs += ed;
                }
            }
            if (rs > 0)
            {
                res.Result = true;
            }
            else
            {
                res.Message = "入库保存失败";
            }
            return res;
        }

        public string CheckStockQuantity(int id, double Quantity, ElementType type, string model = "未知")
        {
          
            double allQuantity = 0;
            if (type == ElementType.Element)
            {
                var eleInfo = _Repository.GetDbSetForEdit().FirstOrDefault(d => d.ElementId == id);
                if (eleInfo == null)
                {
                    return "原材料" + model + "未入库!";
                }
                allQuantity = eleInfo.Quantity;
                if (Quantity > allQuantity)
                {
                    return model + "库存不足！";
                }
                eleInfo.Quantity = allQuantity - Quantity;
                _Repository.Save(eleInfo);

            }
            else
            {
                var prodInfo = _Repository.GetDbQuerySet().FirstOrDefault(d => d.ProductId == id);
                if (prodInfo == null)
                {
                    return "成品" + model + "未入库!";
                }
                allQuantity = prodInfo.Quantity;
                if (Quantity > allQuantity)
                {
                    return model + "库存不足！";
                }
                prodInfo.Quantity = allQuantity - Quantity;
                _Repository.Save(prodInfo);
            }
           
            return "";

        }


     

       // public void AddStockOutForDelivery()


    }
}
