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
    public class PickService : BaseService<PickSet, int>, IPickService
    {
        private readonly IRepository<PickSet, int> _Repository;
        private readonly IUnitOfWork _uow;
        private readonly ICommonCacheService _cacheService;
        private readonly IStockService _stockService;
        //   private readonly IGoodsService _goodsService;
        private readonly IRepository<PickMainSet, int> _pickMainRepository;
        public PickService(IRepository<PickSet, int> repository,
            IUnitOfWork uow,
            ICommonCacheService cacheService,
            IStockService stockService,
            IRepository<PickMainSet, int> pickMainRepository
       )
            : base(repository, uow)
        {
            _Repository = repository;
            _uow = uow;
            _cacheService = cacheService;
            _stockService = stockService;
            _pickMainRepository = pickMainRepository;
        }

        public Search_Pick_Response SearchPick(Search_Pick_Request request)
        {

            Query<PickSet> q = new Query<PickSet>();
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
            var res = new Search_Pick_Response();
            var unitList = _cacheService.GetCache_Unit();
            var partList = _cacheService.GetCache_Part();
            var userList = _cacheService.GetCache_User();
            res.rows = pageData.ConvertTo_PickListViews(unitList, partList, userList);
            res.total = allcount;
            return res;
        }


        public string GetPickDetailHtml(int id)
        {
            var info = GetInfoByID(id);
            var unitList = _cacheService.GetCache_Unit();
            var partList = _cacheService.GetCache_Part();
            var userList = _cacheService.GetCache_User();
            var view = info.ConvertTo_PickDetailView(unitList, partList, userList, _stockService);
            string s = MyHtmlHelper.CreateDetailHtml<PickDetailView>(view);
            return s;
        }

        private string CheckQuantity(IList<Pick_ForAdd_View> list)
        {
            foreach (var item in list)
            {
                double num = item.Quantity;
                if (num <= 0)
                {
                    return "原材料" + item.ElementName + "申请数量不能为0";
                }

            }
            return "";
        }


        public CEDResponse SavePick(IList<Pick_ForAdd_View> list, string currUserName, string Purpose)
        {
            var res = new CEDResponse();
            var msg = CheckQuantity(list);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }

            PickSet info = null;
            DateTime dtNow = DateTime.Now;
            var info2 = new PickMainSet();
            info2.Purpose = Purpose;
            info2.Addtime = dtNow;
            info2.AddUserName = currUserName;
            int rs = 0;
            IList<PickSet> infoList = new List<PickSet>();
            foreach (var item in list)
            {
                info = new PickSet();
                info.Addtime = dtNow;
                info.AddUserName = currUserName;
                info.ElementId = item.ElementId;
                if (item.BomId > 0)
                {
                    info.BomId = item.BomId;
                }
                info.Quantity = item.Quantity;
                info.Purpose = Purpose;
                info.PartCode = item.PartCode;
              
                infoList.Add(info);
            }
            info2.PickSet = infoList;
            _pickMainRepository.Add(info2);
          
            rs = _uow.Commit();
            if (rs > 0)
            {
                res.Result = true;
            }
            else
            {
                //  DeletePickMain(MainId);
                res.Message = "申请领料失败";
            }
            return res;
        }


        public string DeletePick(int id)
        {
            var info = GetInfoByID(id);
            if (info != null)
            {
                if (info.IsFeedback)
                {
                    return "此原材料已发料，禁止删除此条记录";
                }
                int rs = Remove(info);
                if (rs <= 0)
                {
                    return "删除失败";
                }
            }
            else
            {
                return "未找到此条记录";
            }
            return "";
        }

        //private void DeletePickMain(int MainId)
        //{
        //    var info = _pickMainRepository.Single(MainId);
        //    if (info != null)
        //    {
        //        _pickMainRepository.Remove(info);
        //        _uow.Commit();
        //    }
        //}

        //private int CreatePickMain(string Purpose, DateTime dtNow, string currUserName)
        //{
        //    var info = new PickMainSet();
        //    info.Purpose = Purpose;
        //    info.Addtime = dtNow;
        //    info.AddUserName = currUserName;
        //    _pickMainRepository.Add(info);
        //    _uow.Commit();
        //    return info.Id;
        //}

        //public IEnumerable GetPickSelect()
        //{
        //    var list = _Repository.GetDbQuerySet().GroupBy(d => new { d.AddUserName, d.Addtime, d.Purpose, d.GroupGuid }).ToList();
        //    IList<Default_SelectItem> selecter = new List<Default_SelectItem>();
        //    foreach (var item in list)
        //    {
        //        selecter.Add(item.Key.
        //    }
        //    var aa = list;
        //}


        #region 发料

        public Search_PickOut_Response SearchPickOut(Search_PickOut_Request request)
        {
            var res = new Search_PickOut_Response();

            Query<PickMainSet> q = new Query<PickMainSet>();
            if (!string.IsNullOrEmpty(request.UserName))
            {
                q.And(d => d.AddUserName == request.UserName);
            }
            q.OrderBy(d => new { d.Addtime }, true);
            int allcount = 0;
            var pageData = _pickMainRepository.PageQuery(q, request.page, request.rows, out allcount);

            var userList = _cacheService.GetCache_User();
            res.rows = pageData.PickMainSet_To_PickOutListViews(userList);
            res.total = allcount;
            return res;
        }

        public IList<PickOut_ForAdd_View> GetPickDetail_ForAdd(int ParentId)
        {
            if (ParentId <= 0)
            {
                return new List<PickOut_ForAdd_View>();
            }
            var list = _Repository.GetDbQuerySet().Where(d => d.ParentId == ParentId).ToList();
            var unitList = _cacheService.GetCache_Unit();
            var partList = _cacheService.GetCache_Part();
            var userList = _cacheService.GetCache_User();
            var views = list.PickSet_To_PickOut_ForAdd_Views(unitList, partList, _stockService);
            return views;
        }


        private string CheckQuantity(IList<PickOut_ForAdd_View> list)
        {
            foreach (var item in list)
            {
                double stockOutNum = item.StockOutQuantity;
                if (stockOutNum <= 0)
                {
                    return "原材料" + item.ElementName + "发料数量不能为0";
                }

                var stockNum = _stockService.GetStockQuantity(item.ElementId);
                if (stockNum <= 0)
                {
                    return "原材料" + item.ElementName + "库存数量为0";
                }
                if (stockOutNum > stockNum)
                {
                    return "原材料" + item.ElementName + "发料数量超过库存数量";
                }

                if (stockOutNum + item.ALStockOutQuantity > item.Quantity)
                {
                    return "原材料" + item.ElementName + "发料数量超过申请数量";
                }


            }
            return "";
        }

        public CEDResponse SavePickOut(IList<PickOut_ForAdd_View> list, string currUserName, int mainId)
        {
            var res = new CEDResponse();
            var msg = CheckQuantity(list);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }

            //  PickSet info = null;
            DateTime dtNow = DateTime.Now;

            var info2 = _pickMainRepository.Single(mainId);
            info2.StockOutTime = dtNow;
            info2.StockOutUserName = currUserName;
            info2.IsFeedback = true;
            int rs = 0;

            foreach (var info in info2.PickSet)
            {
                foreach (var item in list)
                {
                    if (item.Id == info.Id)
                    {
                        info.StockOutQuantity = info.StockOutQuantity + item.StockOutQuantity;
                        info.StockOutTime = dtNow;
                        info.StockOutUserName = currUserName;
                        info.IsFeedback = true;
                        break;
                    }
                }

            }
            //  info2.PickSet = infoList;
            _pickMainRepository.Save(info2);
            // rs = _uow.Commit();
            // if (rs > 0)
            //  {
            res = AddStockOutRecord(list, currUserName, info2);
            //   res.Result = true;
            //}
            //else
            //{
            //    //  DeletePickMain(MainId);
            //    res.Message = "发料失败";
            //}
            return res;
        }

        private CEDResponse AddStockOutRecord(IList<PickOut_ForAdd_View> list, string currUserName, PickMainSet mainInfo)
        {
            IList<StockOut_ForAdd_View> outList = new List<StockOut_ForAdd_View>();
            StockOut_ForAdd_View info = null;
            // var view = list.First();
            var userList = _cacheService.GetCache_User();
            var userCache = userList.FirstOrDefault(d => d.UserName == mainInfo.AddUserName);
            var userName = userCache == null ? "" : userCache.TrueName;
            string Reason = "销售申请发料，申请人：" + userName + "，申请时间：" + mainInfo.Addtime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "，申请目的：" + mainInfo.Purpose;
            foreach (var item in list)
            {
                // info = new StockOutSet();
                //info.Addtime = dtNow;
                //info.AddUserName = currUserName;
                //info.ElementId = item.ElementId;
                //if (item.BomId > 0)
                //{
                //    info.BomId = item.BomId;
                //}
                //info.GroupGuid = groupGuid;
                //info.Quantity = item.Quantity;
                //info.Remark = item.Remark;
                //info.ShelfId = item.ShelfId;
                //info.ItemType = item.ItemType;
                //info.Reason = Reason;


                info = new StockOut_ForAdd_View();
                info.BomId = item.BomId;
                info.ElementId = item.ElementId;
                info.Quantity = item.StockOutQuantity;
                info.Remark = Reason;
                info.ShelfId = item.ShelfId;
                info.ItemType = (int)ElementType.Element;
                info.PickId = item.Id;
                outList.Add(info);
            }

            return _stockService.SaveStockOut(outList, currUserName, Reason);
        }



        #endregion


    }



}
