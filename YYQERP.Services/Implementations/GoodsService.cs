using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
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
    public class GoodsService : BaseService<ElementSet, int>, IGoodsService
    {
        private readonly IRepository<ElementSet, int> _Repository;
        private readonly IUnitOfWork _uow;
        private readonly ICommonCacheService _cacheService;
        private readonly IRepository<ShelfSet, int> _shelfRepository;
        private readonly IRepository<ModelSet, int> _modelRepository;
        private readonly IRepository<PartSet, int> _partRepository;
        private readonly IRepository<PartDetailSet, int> _partDetailRepository;

        private readonly IRepository<BomSet, int> _bomRepository;
        private readonly IRepository<BomDetailSet, int> _bomDetailRepository;
        private readonly IStockService _stockService;
        private readonly IRepository<ShelfSet, int> _ShelfRepository;
        private readonly IRepository<ProductSet, int> _productRepository;
        private readonly IRepository<ProductDetailSet, int> _productDetailRepository;
        private readonly IRepository<StockSet, int> _stockRepository;
        private readonly IRepository<DicSet, int> _dicRepository;
        public GoodsService(IRepository<ElementSet, int> repository, IUnitOfWork uow,
            ICommonCacheService cacheService,
            IRepository<ShelfSet, int> shelfRepository,
            IRepository<ModelSet, int> modelRepository,
            IRepository<PartSet, int> partRepository,
            IRepository<PartDetailSet, int> partDetailRepository,
            IRepository<BomSet, int> bomRepository,
            IRepository<BomDetailSet, int> bomDetailRepository,
            IStockService stockService,
            IRepository<ShelfSet, int> ShelfRepository,
            IRepository<ProductSet, int> productRepository,
            IRepository<ProductDetailSet, int> productDetailRepository,
            IRepository<StockSet, int> stockRepository,
            IRepository<DicSet, int> dicRepository

       )
            : base(repository, uow)
        {
            _Repository = repository;
            _uow = uow;
            _cacheService = cacheService;
            _shelfRepository = shelfRepository;
            _modelRepository = modelRepository;
            _partRepository = partRepository;
            _partDetailRepository = partDetailRepository;
            _bomRepository = bomRepository;
            _bomDetailRepository = bomDetailRepository;
            _stockService = stockService;
            _ShelfRepository = ShelfRepository;
            _productRepository = productRepository;
            _productDetailRepository = productDetailRepository;
            _stockRepository = stockRepository;
            _dicRepository = dicRepository;
        }




        public IEnumerable GetShelfSelectList()
        {
            var list = _shelfRepository.GetDbQuerySet().Select(d => new { shelfid = d.Id, shelftext = d.Code }).ToList();
            return list;
        }

        #region 原材料方法
        public IEnumerable GetElementSelectList()
        {
            var list = _Repository.GetDbQuerySet().Select(d => new { elementid = d.Id, elementtext = d.Name + "【" + d.Code + "】", shelfid = d.ShelfId, unitcode = d.UnitTypeCode }).ToList();
            return list;
        }
        public Search_Element_Response SearchElement(Search_Element_Request request, bool isPage = true)
        {
            var res = new Search_Element_Response();
            Query<ElementSet> q = new Query<ElementSet>();
            if (!string.IsNullOrEmpty(request.ShelfName))
            {
                q.And(d => d.ShelfSet.Code.Contains(request.ShelfName));
            }
            if (!string.IsNullOrEmpty(request.ElementName))
            {
                q.And(d => d.Name.Contains(request.ElementName) || d.Code.Contains(request.ElementName));
            }
            q.OrderBy(d => new { d.Addtime }, true);
            int allcount = 0;
            var dicList = _cacheService.GetCache_Dic();
            if (isPage)
            {
                var pageData = _Repository.PageQuery(q, request.page, request.rows, out allcount);
                res.rows = pageData.ConvertTo_ElementListViews(dicList);
            }
            else
            {
                var pageData = _Repository.Query(q).ToList();
                res.rows = pageData.ConvertTo_ElementListViews(dicList);
            }

            res.total = allcount;
            return res;
        }


        public CEDResponse AddElement(Add_Element_Request request)
        {
            CEDResponse res = new CEDResponse();
            var eleInfo = request.ConvertTo_ElementSet_ForCreate();
            var msg = CheckEleAdd(eleInfo);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }
            int rs = SaveAdd(eleInfo);
            if (rs > 0)
            {
                res.Result = true;
                _cacheService.RemoveElementCache();
            }
            else
            {
                res.Result = false;
                res.Message = "新增原材料失败";
                return res;
            }


            return res;
        }

        public CEDResponse EditElement(Edit_Element_Request request)
        {
            CEDResponse res = new CEDResponse();
            var oldInfo = GetInfoByID(request.Id);
            if (oldInfo == null)
            {
                res.Result = false;
                res.Message = "未找到编辑的原材料信息";
                return res;
            }
            var eleInfo = request.ConvertTo_ElementSet_ForEdit(oldInfo);
            var msg = CheckEleEdit(eleInfo);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }
            int rs = SaveEdit(eleInfo);
            if (rs > 0)
            {
                res.Result = true;
                _cacheService.RemoveElementCache();
            }
            else
            {
                res.Result = false;
                res.Message = "编辑原材料失败";
                return res;
            }


            return res;
        }


        private string CheckEleEdit(ElementSet eleInfo)
        {
            string msg = CheckElementNameOrCodeDup(eleInfo.Name, eleInfo.Code, eleInfo.Id);
            if (msg != "")
            {
                return msg;
            }

            return "";
        }

        private string CheckEleAdd(ElementSet eleInfo)
        {
            string msg = CheckElementNameOrCodeDup(eleInfo.Name, eleInfo.Code);
            if (msg != "")
            {
                return msg;
            }

            return "";
        }


        private string CheckElementNameOrCodeDup(string newEleName, string newEleCode, int editId = 0)
        {
            bool rs = false;
            if (editId <= 0)
            {
                rs = GetDbQuerySet().Any(d => d.Name.Equals(newEleName));
                if (rs)
                {
                    return "原材料名称不能重复";
                }
                else
                {
                    rs = GetDbQuerySet().Any(d => d.Code.Equals(newEleCode));
                    if (rs)
                    {
                        return "原材料料号不能重复";
                    }
                }
            }
            else
            {
                rs = GetDbQuerySet().Any(d => d.Name.Equals(newEleName) && d.Id != editId);
                if (rs)
                {
                    return "原材料名称不能重复";
                }
                else
                {
                    rs = GetDbQuerySet().Any(d => d.Code.Equals(newEleCode) && d.Id != editId);
                    if (rs)
                    {
                        return "原材料料号不能重复";
                    }
                }
            }
            return "";
        }

        public CEDResponse DeleteElement(int id)
        {
            CEDResponse res = new CEDResponse();
            var info = GetInfoByID(id);
            if (info != null)
            {
                int rs = Remove(info);
                if (rs > 0)
                {
                    res.Result = true;
                    _cacheService.RemoveElementCache();
                }
                else
                {
                    res.Message = "删除用户失败";
                }
            }
            return res;
        }

        public string ImportElement(DataTable dt, string loginUserName)
        {
            //System.Threading.Thread.Sleep(10000);
            //  return "";
            CreateNewUnit(dt, loginUserName);
            CreateNewShelf(dt, loginUserName);
            ElementSet info = null;
            DateTime dtNow = DateTime.Now;
            var shelfList = _shelfRepository.GetDbQuerySet().Select(d => new { d.Id, d.Code }).ToList();
            var unitList = _cacheService.GetCache_Unit();
            var eleList = _Repository.GetDbQuerySet().Select(d => d.Code).ToList();
            string eleCode = "";
            string eleName = "";
            string unitName = "";
            string shelfCode = "";
            int colCount = dt.Columns.Count;
            int icount = 0;
            foreach (DataRow dr in dt.Rows)
            {
                icount++;
                eleCode = dr[1].ToString();
                eleName = dr[0].ToString();
                unitName = dr[2].ToString();
                shelfCode = dr[3].ToString();
                if (string.IsNullOrEmpty(eleCode) || string.IsNullOrEmpty(eleName))
                {
                    continue;
                }
                if (string.IsNullOrEmpty(unitName) || string.IsNullOrEmpty(shelfCode))
                {
                    return ExpErr(icount, dr);
                }
                bool isEleExist = CheckEleExist(eleCode, eleList);


                //if (isEleExist)
                //{
                //    info = _Repository.GetDbQuerySet().FirstOrDefault(d => d.Code == code);
                //}
                info = new ElementSet();
                info.Code = eleCode;
                info.Name = eleName;
                if (!isEleExist)
                {

                    info.Addtime = dtNow;
                    info.AddUserName = loginUserName;
                    info.Price = StringToDecimal(dr[4].ToString());
                    info.ShelfId = shelfList.FirstOrDefault(d => d.Code == shelfCode).Id;
                    //string msg = GetOrCreateShelfFromCode(info, dr[3].ToString(), shelfList);
                    //if (msg != "")
                    //{
                    //    return "库位错误！数据信息：" + msg;// string.Join(",", dr.ItemArray);
                    //}
                    info.UnitTypeCode = unitList.FirstOrDefault(d => d.Name == unitName).Code; // GetOrCreateUnitCodeFromName(unitList, dr[2].ToString(), loginUserName);
                    if (string.IsNullOrEmpty(info.UnitTypeCode))
                    {
                        return "数量单位错误！数据信息：" + string.Join(",", dr.ItemArray);
                    }
                    info.WarningQuantity = StringHelper.SafeGetDoubleFromObj(dr[5], 0);
                    info.Remark = "导入创建  " + dtNow.ToString("yyyy-MM-dd HH:mm:ss");

                    _Repository.Add(info);
                }
                //else
                //{
                //    info = _Repository.GetDbSetForEdit().FirstOrDefault(d => d.Code == eleCode);
                //    info.Modifytime = dtNow;
                //    info.ModifyUserName = loginUserName;
                //    _Repository.Save(info);
                //}




                //if (isEleExist)
                //{
                //    info.BomDetailSet = null;
                //    info.PartDetailSet = null;
                //    info.ProductDetailSet = null;
                //    info.ShelfSet = null;
                //    info.StockOutSet = null;
                //    info.StockInSet = null;
                //    info.StockSet.Clear();
                //    _Repository.Save(info);
                //}
                //if (!isEleExist)
                //{

                //}
                if (colCount >= 7)
                {
                    CreateElementStock(info, dr[6].ToString(), isEleExist, dtNow, loginUserName);
                }


            }
            int rs = _uow.Commit();
            return rs > 0 ? "" : "导入数据为0";
        }

        public string RestoreElement(DataTable dt, string loginUserName)
        {
            DateTime dtNow = DateTime.Now;
            string eleCode = "";
            string shelfCode = "";
            double Quantity = 0;
            foreach (DataRow dr in dt.Rows)
            {
                eleCode = dr[1].ToString();
                shelfCode = dr[2].ToString();
                Quantity = StringHelper.SafeGetDoubleFromObj(dr[5], 0);
                if (string.IsNullOrEmpty(eleCode) || string.IsNullOrEmpty(shelfCode))
                {
                    continue;
                }
                var stockInfo = _stockRepository.GetDbSetForEdit().FirstOrDefault(d => d.ElementSet.Code == eleCode && d.ShelfSet.Code == shelfCode);
                if (stockInfo != null)
                {
                    stockInfo.Remark = loginUserName + " 更改库存数，原数量：" + stockInfo.Quantity + " 更改后数量：" + Quantity+"  更改时间:" + dtNow.ToString("yyyy-MM-dd HH:mm:ss");
                    stockInfo.Quantity = Quantity;
                    stockInfo.Modifytime = dtNow;
                    stockInfo.ModifyUserName = loginUserName;
                }
                
            }
            int rs = _uow.Commit();
            return rs > 0 ? "" : "更新数据为0";
        }

      

        private string ExpErr(int icount, DataRow dr)
        {
            string msg = string.Format("第{0}行数据错误，行数据：原材料名称:{1},料号:{2},数量单位:{3},库位:{4},价格:{5},预警数量:{6},库存数量:{7}",
                icount, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString()
                );
            return msg;
        }

        private void CreateNewUnit(DataTable dt, string loginUserName)
        {
            string name = "";
            var unitList = _cacheService.GetCache_Unit();
            DateTime dtNow = DateTime.Now;
            DicSet newinfo = null;
            foreach (DataRow dr in dt.Rows)
            {
                name = dr[2].ToString();
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }
                var info = unitList.FirstOrDefault(d => d.Name == name);
                if (info == null)
                {
                    newinfo = new DicSet();
                    newinfo.Name = name;
                    newinfo.Code = "unit" + dtNow.ToString("yyyyMMddHHmmssfff");
                    newinfo.Addtime = dtNow;
                    newinfo.AddUserName = loginUserName;
                    newinfo.Enabled = true;
                    newinfo.ParentCode = "unit";
                    newinfo.sort = 100;
                    newinfo.Remark = "导入创建" + dtNow.ToString("yyyyMMddHHmmssfff");
                    _dicRepository.Add(newinfo);
                    unitList.Add(new DicView { Code = newinfo.Code, Enabled = 1, Name = name, ParentCode = "unit" });
                }
            }
            _uow.Commit();
            _cacheService.RemoveDicCache();
        }
        private void CreateNewShelf(DataTable dt, string loginUserName)
        {
            var shelfList = _shelfRepository.GetDbQuerySet().Select(d => d.Code).ToList();
            string code = "";
            ShelfSet shelfInfo = null;
            DateTime dtNow = DateTime.Now;
            foreach (DataRow dr in dt.Rows)
            {
                code = dr[3].ToString();
                if (string.IsNullOrEmpty(code))
                {
                    continue;
                }
                if (!CheckShelfCodeExist(code, shelfList))
                {
                    shelfList.Add(code);
                    shelfInfo = new ShelfSet();
                    shelfInfo.Name = shelfInfo.Code = code;
                    shelfInfo.Remark = "导入创建  " + dtNow.ToString("yyyyMMddHHmmssfff");
                    shelfInfo.Addtime = dtNow;
                    shelfInfo.AddUserName = loginUserName;
                    _shelfRepository.Add(shelfInfo);
                }

            }
            _uow.Commit();
        }

        private void CreateElementStock(ElementSet info, string stockNum, bool isEleExist, DateTime dtNow, string loginUserName)
        {
            double num = StringHelper.SafeGetDoubleFromObj(stockNum, 0);
            if (num > 0)
            {
                //StockSet sInfo = null;
                //if (info.StockSet != null)
                //{
                //    sInfo = info.StockSet.FirstOrDefault();
                //}

                var sInfo = _stockRepository.GetDbSetForEdit().FirstOrDefault(d => d.ElementSet.Code == info.Code);
                if (sInfo != null)
                {
                    sInfo.Quantity += num;
                    //sInfo.ElementSet = null;
                    //   sInfo.ProductSet = null;
                    //  sInfo.ShelfSet = null;
                    sInfo.LastInQuantity = num;
                    sInfo.LastInTime = dtNow;
                    _stockRepository.Save(sInfo);

                }
                else
                {
                    if (isEleExist)
                    {
                        info = _Repository.GetDbSetForEdit().FirstOrDefault(d => d.Code == info.Code);
                        info.Modifytime = dtNow;
                        info.ModifyUserName = loginUserName;
                        _Repository.Save(info);
                    }
                    sInfo = new StockSet();
                    sInfo.Addtime = info.Addtime;
                    sInfo.AddUserName = info.AddUserName;
                    //   sInfo.ElementId
                    sInfo.FirstInTime = info.Addtime.Value;
                    sInfo.LastInTime = info.Addtime;

                    sInfo.ItemType = (int)ElementType.Element;
                    sInfo.LastInQuantity = sInfo.Quantity = num;
                    sInfo.ShelfId = info.ShelfId;
                    sInfo.UnitTypeCode = info.UnitTypeCode;
                    if (info.StockSet == null)
                    {
                        info.StockSet = new List<StockSet>();
                    }
                    info.StockSet.Add(sInfo);

                }

            }

        }

        private bool CheckEleExist(string code, IList<string> eleList)
        {
            foreach (var item in eleList)
            {
                if (code == item)
                {
                    return true;
                }
            }
            return false;
        }

        private string GetOrCreateUnitCodeFromName(IList<DicView> unitList, string name, string loginUserName)
        {
            string newName = name ?? "个";
            var info = unitList.FirstOrDefault(d => d.Name == newName);
            if (info == null)
            {
                DateTime dtNow = DateTime.Now;
                DicSet newinfo = new DicSet();
                newinfo.Name = newName;
                newinfo.Code = "unit" + dtNow.ToString("yyyyMMddHHmmssfff");
                newinfo.Addtime = dtNow;
                newinfo.AddUserName = loginUserName;
                newinfo.Enabled = true;
                newinfo.ParentCode = "unit";
                newinfo.sort = 100;
                newinfo.Remark = "导入创建" + dtNow.ToString("yyyyMMddHHmmssfff");
                _dicRepository.Add(newinfo);
                unitList.Add(new DicView { Code = newinfo.Code, Enabled = 1, Name = newName, ParentCode = "unit" });
                _cacheService.RemoveDicCache();
                return newinfo.Code;
            }
            else
            {
                return info.Code;
            }
        }


        private bool CheckShelfCodeExist(string code, IList<string> list)
        {
            foreach (var item in list)
            {
                if (code == item)
                {
                    return true;
                }
            }
            return false;
        }

        private string GetOrCreateShelfFromCode(ElementSet eInfo, string code, IList<string> list)
        {
            var shelfInfo = eInfo.ShelfSet;
            if (shelfInfo != null)
            {
                if (!string.IsNullOrEmpty(code) && shelfInfo.Code != code)
                {
                    return string.Format("原材料库位不一致！原材料：{0}【{1}】，原库位：{2}，导入库位{3}", eInfo.Name, eInfo.Code, shelfInfo.Code, code);
                }
                //  return shelfInfo.Id;
            }
            else
            {
                if (!CheckShelfCodeExist(code, list))
                {
                    list.Add(code);
                    shelfInfo = new ShelfSet();
                    shelfInfo.Name = shelfInfo.Code = code;
                    shelfInfo.Remark = "导入创建  " + eInfo.Addtime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    shelfInfo.Addtime = eInfo.Addtime;
                    shelfInfo.AddUserName = eInfo.AddUserName;
                    eInfo.ShelfSet = shelfInfo;
                }
                // _shelfRepository.Add(info);
            }
            return "";
            //var info = list.FirstOrDefault(d => d.Code == code);
            //if (info == null)
            //{
            //    info = new ShelfSet();
            //    info.Name = info.Code = code;
            //    info.Remark = "导入创建  " + dtNow.ToString("yyyy-MM-dd HH:mm:ss");
            //    info.Addtime = dtNow;
            //    info.AddUserName = loginUserName;
            //    _shelfRepository.Add(info);
            //    _uow.Commit();
            //}
            //return info.Id;
        }

        private decimal StringToDecimal(string str)
        {
            decimal price = 0;
            decimal.TryParse(str, out price);
            return price;
        }

        #endregion


        #region 型号方法

        public Search_Model_Response SearchModels(Search_Model_Request request)
        {
            var q = new Query<ModelSet>();
            q.OrderBy(d => new { d.Addtime }, true);
            int allcount = 0;
            var pageData = _modelRepository.PageQuery(q, request.page, request.rows, out allcount);
            var res = new Search_Model_Response();
            res.rows = pageData.ConvertTo_ModelListViews();
            res.total = allcount;
            return res;
        }

        public CEDResponse AddModel(Add_Model_Request request)
        {
            CEDResponse res = new CEDResponse();
            var Info = request.ConvertTo_ModelSet_ForCreate();
            var msg = CheckModelAdd(Info);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }
            _modelRepository.Add(Info);
            int rs = _uow.Commit();
            if (rs > 0)
            {
                res.Result = true;
                _cacheService.RemoveModelCache();
            }
            else
            {
                res.Result = false;
                res.Message = "新增型号失败";
                return res;
            }


            return res;
        }


        public CEDResponse EditModel(Edit_Model_Request request)
        {
            CEDResponse res = new CEDResponse();
            var oldInfo = _modelRepository.Single(request.Id);
            if (oldInfo == null)
            {
                res.Result = false;
                res.Message = "未找到编辑的型号信息";
                return res;
            }
            var eleInfo = request.ConvertTo_ModelSet_ForEdit(oldInfo);
            var msg = CheckModelEdit(eleInfo);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }
            _modelRepository.Save(eleInfo);
            int rs = _uow.Commit();
            if (rs > 0)
            {
                res.Result = true;
                _cacheService.RemoveModelCache();
            }
            else
            {
                res.Result = false;
                res.Message = "编辑型号失败";
                return res;
            }


            return res;
        }


        private string CheckModelEdit(ModelSet eleInfo)
        {
            string msg = CheckModelNameOrCodeDup(eleInfo.Name, eleInfo.Code, eleInfo.Id);
            if (msg != "")
            {
                return msg;
            }

            return "";
        }

        private string CheckModelAdd(ModelSet eleInfo)
        {
            string msg = CheckModelNameOrCodeDup(eleInfo.Name, eleInfo.Code);
            if (msg != "")
            {
                return msg;
            }

            return "";
        }


        private string CheckModelNameOrCodeDup(string newName, string newCode, int editId = 0)
        {
            bool rs = false;
            if (editId <= 0)
            {
                rs = _modelRepository.GetDbQuerySet().Any(d => d.Name.Equals(newName));
                if (rs)
                {
                    return "型号名称不能重复";
                }
                else
                {
                    rs = _modelRepository.GetDbQuerySet().Any(d => d.Code.Equals(newCode));
                    if (rs)
                    {
                        return "型号代码不能重复";
                    }
                }
            }
            else
            {
                rs = _modelRepository.GetDbQuerySet().Any(d => d.Name.Equals(newName) && d.Id != editId);
                if (rs)
                {
                    return "型号名称不能重复";
                }
                else
                {
                    rs = _modelRepository.GetDbQuerySet().Any(d => d.Code.Equals(newCode) && d.Id != editId);
                    if (rs)
                    {
                        return "型号代码不能重复";
                    }
                }
            }
            return "";
        }

        public CEDResponse DeleteModel(int id)
        {
            CEDResponse res = new CEDResponse();
            var info = _modelRepository.Single(id);
            if (info != null)
            {
                _modelRepository.Remove(info);
                int rs = _uow.Commit();
                if (rs > 0)
                {
                    res.Result = true;
                    _cacheService.RemoveModelCache();
                }
                else
                {
                    res.Message = "删除型号失败";
                }
            }
            else
            {
                res.Result = false;
                res.Message = "未找到所要删除的型号";
            }
            return res;
        }

        #endregion


        #region 部件方法

        public Search_Part_Response SearchPart(Search_Part_Request request)
        {
            var q = new Query<PartSet>();
            q.OrderBy(d => new { d.Addtime }, true);
            int allcount = 0;
            var pageData = _partRepository.PageQuery(q, request.page, request.rows, out allcount);
            var res = new Search_Part_Response();
            res.rows = pageData.ConvertTo_PartListViews();
            res.total = allcount;
            return res;
        }

        public CEDResponse AddPart(Add_Part_Request request)
        {
            CEDResponse res = new CEDResponse();
            var Info = request.ConvertTo_PartSet_ForCreate();
            var msg = CheckPartAdd(Info);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }
            _partRepository.Add(Info);
            int rs = _uow.Commit();
            if (rs > 0)
            {
                res.Result = true;
                _cacheService.RemovePartCache();
            }
            else
            {
                res.Result = false;
                res.Message = "新增部件失败";
                return res;
            }


            return res;
        }


        public CEDResponse EditPart(Edit_Part_Request request)
        {
            CEDResponse res = new CEDResponse();
            var oldInfo = _partRepository.Single(request.Id);
            if (oldInfo == null)
            {
                res.Result = false;
                res.Message = "未找到编辑的部件信息";
                return res;
            }
            var eleInfo = request.ConvertTo_PartSet_ForEdit(oldInfo);
            var msg = CheckPartEdit(eleInfo);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }
            _partRepository.Save(eleInfo);
            int rs = _uow.Commit();
            if (rs > 0)
            {
                res.Result = true;
                _cacheService.RemovePartCache();
            }
            else
            {
                res.Result = false;
                res.Message = "编辑部件失败";
                return res;
            }


            return res;
        }


        private string CheckPartEdit(PartSet eleInfo)
        {
            string msg = CheckPartNameOrCodeDup(eleInfo.Name, eleInfo.Code, eleInfo.Id);
            if (msg != "")
            {
                return msg;
            }

            return "";
        }

        private string CheckPartAdd(PartSet eleInfo)
        {
            string msg = CheckPartNameOrCodeDup(eleInfo.Name, eleInfo.Code);
            if (msg != "")
            {
                return msg;
            }

            return "";
        }


        private string CheckPartNameOrCodeDup(string newName, string newCode, int editId = 0)
        {
            bool rs = false;
            if (editId <= 0)
            {
                rs = _partRepository.GetDbQuerySet().Any(d => d.Name.Equals(newName));
                if (rs)
                {
                    return "部件名称不能重复";
                }
                else
                {
                    rs = _partRepository.GetDbQuerySet().Any(d => d.Code.Equals(newCode));
                    if (rs)
                    {
                        return "部件代码不能重复";
                    }
                }
            }
            else
            {
                rs = _partRepository.GetDbQuerySet().Any(d => d.Name.Equals(newName) && d.Id != editId);
                if (rs)
                {
                    return "部件名称不能重复";
                }
                else
                {
                    rs = _partRepository.GetDbQuerySet().Any(d => d.Code.Equals(newCode) && d.Id != editId);
                    if (rs)
                    {
                        return "部件代码不能重复";
                    }
                }
            }
            return "";
        }

        public CEDResponse DeletePart(int id)
        {
            CEDResponse res = new CEDResponse();
            var info = _partRepository.Single(id);
            if (info != null)
            {
                _partRepository.Remove(info);
                int rs = _uow.Commit();
                if (rs > 0)
                {
                    res.Result = true;
                    _cacheService.RemovePartCache();
                }
                else
                {
                    res.Message = "删除部件失败";
                }
            }
            else
            {
                res.Result = false;
                res.Message = "未找到所要删除的部件";
            }
            return res;
        }

        public IList<PartDetailEditView> GetPartDetail(int partId)
        {
            var partInfo = _partRepository.Single(partId);
            var views = partInfo.PartDetailSet.ConvertTo_PartDetailEditViews();
            return views;
        }


        public CEDResponse SavePartDetail(List<PartDetailEditView> infos, int partid)
        {
            if (partid <= 0)
            {
                return new CEDResponse { Result = false, Message = "请先选择部件" };
            }
            _partDetailRepository.RemoveByWhere(d => d.PartId == partid);
            int rs = 0;
            var res = new CEDResponse();
            PartDetailSet info = null;
            Guid newGuid = Guid.NewGuid();
            DateTime dtNow = DateTime.Now;
            var currUserName = HttpContext.Current.User.Identity.Name;
            foreach (PartDetailEditView view in infos)
            {
                if (CheckPartDetailInData(view))
                {
                    info = new PartDetailSet();
                    info.Addtime = dtNow;
                    info.AddUserName = currUserName;
                    info.ElementId = view.ElementId;
                    info.PartId = partid;
                    info.Quantity = view.Quantity;
                    _partDetailRepository.Add(info);

                }

            }
            rs = _uow.Commit();
            if (rs > 0)
            {
                res.Result = true;
            }
            else
            {
                res.Message = "部件明细保存失败";
            }
            return res;
        }



        private bool CheckPartDetailInData(PartDetailEditView data)
        {
            if (data.ElementId <= 0)
            {
                return false;
            }



            if (data.Quantity <= 0)
            {
                return false;
            }


            return true;
        }

        public IEnumerable GetPartSelectList()
        {
            var list = _partRepository.GetDbQuerySet().Select(d => new { partid = d.Id, partname = d.Name + "【" + d.Code + "】" }).ToList();
            return list;
        }


        public IList<BomListInPartView> GetBomListInPart(string partCode)
        {
            IList<BomListInPartView> views = new List<BomListInPartView>();
            var list = _bomRepository.GetDbQuerySet().Where(d => d.BomDetailSet.Any(f => f.PartCode == partCode)).ToList();
            foreach (var item in list)
            {
                views.Add(new BomListInPartView { Id = item.Id, BomName = item.Name, Remark = item.Remark });
            }
            return views;
        }

        public IList<BomDetailEditView> GetBomDetailListInPart(int bomId)
        {
            var list = _bomDetailRepository.GetDbQuerySet().Where(d => d.BomId == bomId).OrderBy(d => d.PartCode).ToList();
            var partList = _cacheService.GetCache_Part();
            var unitList = _cacheService.GetCache_Unit();
            var views = list.ConvertTo_BomDetailEditViews(partList, unitList);
            return views;
        }

        #endregion


        #region Bom方法

        public Search_Bom_Response SearchBom(Search_Bom_Request request)
        {
            var q = new Query<BomSet>();
            if (request.ModelId > 0)
            {
                q.And(d => d.ModelId == request.ModelId);
            }
            q.OrderBy(d => new { d.Addtime }, true);
            int allcount = 0;
            var pageData = _bomRepository.PageQuery(q, request.page, request.rows, out allcount);
            var res = new Search_Bom_Response();
            res.rows = pageData.ConvertTo_BomListViews();
            res.total = allcount;
            return res;
        }

        public CEDResponse AddBom(Add_Bom_Request request)
        {
            CEDResponse res = new CEDResponse();
            var Info = request.ConvertTo_BomSet_ForCreate();
            var msg = CheckBomAdd(Info);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }
            _bomRepository.Add(Info);
            int rs = _uow.Commit();
            if (rs > 0)
            {
                res.Result = true;
                _cacheService.RemoveBomCache();
            }
            else
            {
                res.Result = false;
                res.Message = "新增Bom失败";
                return res;
            }


            return res;
        }


        public CEDResponse EditBom(Edit_Bom_Request request)
        {
            CEDResponse res = new CEDResponse();
            var oldInfo = _bomRepository.Single(request.Id);
            if (oldInfo == null)
            {
                res.Result = false;
                res.Message = "未找到编辑的Bom信息";
                return res;
            }
            var eleInfo = request.ConvertTo_BomSet_ForEdit(oldInfo);
            var msg = CheckBomEdit(eleInfo);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }
            _bomRepository.Save(eleInfo);
            int rs = _uow.Commit();
            if (rs > 0)
            {
                res.Result = true;
                _cacheService.RemoveBomCache();
            }
            else
            {
                res.Result = false;
                res.Message = "编辑Bom失败";
                return res;
            }


            return res;
        }


        private string CheckBomEdit(BomSet eleInfo)
        {
            string msg = CheckBomNameDup(eleInfo.Name, eleInfo.Id);
            if (msg != "")
            {
                return msg;
            }

            return "";
        }

        private string CheckBomAdd(BomSet eleInfo)
        {
            string msg = CheckBomNameDup(eleInfo.Name);
            if (msg != "")
            {
                return msg;
            }

            return "";
        }


        private string CheckBomNameDup(string newName, int editId = 0)
        {
            bool rs = false;
            if (editId <= 0)
            {
                rs = _bomRepository.GetDbQuerySet().Any(d => d.Name.Equals(newName));
                if (rs)
                {
                    return "Bom名称不能重复";
                }

            }
            else
            {
                rs = _bomRepository.GetDbQuerySet().Any(d => d.Name.Equals(newName) && d.Id != editId);
                if (rs)
                {
                    return "Bom名称不能重复";
                }

            }
            return "";
        }

        public CEDResponse DeleteBom(int id)
        {
            CEDResponse res = new CEDResponse();
            var info = _bomRepository.Single(id);
            if (info != null)
            {
                _bomRepository.Remove(info);
                int rs = _uow.Commit();
                if (rs > 0)
                {
                    res.Result = true;
                }
                else
                {
                    res.Message = "删除Bom失败";
                }
            }
            else
            {
                res.Result = false;
                res.Message = "未找到所要删除的Bom";
            }
            return res;
        }

        public IList<BomDetailEditView> GetBomDetail(int BomId)
        {
            var BomInfo = _bomRepository.Single(BomId);
            var DicList = _cacheService.GetCache_Dic();
            var partList = _cacheService.GetCache_Part();
            var views = BomInfo.BomDetailSet.OrderBy(d => d.PartCode).OrderBy(d => d.ElementId).ConvertTo_BomDetailEditViews(partList, DicList);
            return views;
        }

        public IList<Export_Bom_View> GetBomDetailExport(int BomId)
        {
            var BomInfo = _bomRepository.Single(BomId);
            var DicList = _cacheService.GetCache_Dic();
            var partList = _cacheService.GetCache_Part();
            var views = BomInfo.BomDetailSet.OrderBy(d => d.PartCode).OrderBy(d => d.ElementId).BomDetailSet_To_Export_Bom_Views(partList, DicList);
            return views;
        }


        public CEDResponse SaveBomDetail(List<BomDetailEditView> infos, int Bomid)
        {
            if (Bomid <= 0)
            {
                return new CEDResponse { Result = false, Message = "请先选择Bom" };
            }
            _bomDetailRepository.RemoveByWhere(d => d.BomId == Bomid);
            int rs = 0;
            var res = new CEDResponse();
            BomDetailSet info = null;
            Guid newGuid = Guid.NewGuid();
            DateTime dtNow = DateTime.Now;
            var currUserName = HttpContext.Current.User.Identity.Name;
            foreach (BomDetailEditView view in infos)
            {
                if (CheckBomDetailInData(view))
                {
                    info = new BomDetailSet();
                    info.Addtime = dtNow;
                    info.AddUserName = currUserName;
                    info.ElementId = view.ElementId;
                    info.BomId = Bomid;
                    info.Quantity = view.Quantity;
                    info.PartCode = view.PartCode;
                    info.Remark = view.Remark;
                    _bomDetailRepository.Add(info);

                }

            }
            rs = _uow.Commit();
            if (rs > 0)
            {
                res.Result = true;
            }
            else
            {
                res.Message = "Bom明细保存失败";
            }
            return res;
        }

        public IEnumerable GetBomSelectList()
        {
            var list = _bomRepository.GetDbQuerySet().Select(d => new { bomid = d.Id, bomname = d.Name }).ToList();
            return list;
        }

        private bool CheckBomDetailInData(BomDetailEditView data)
        {
            if (data.ElementId <= 0)
            {
                return false;
            }



            if (data.Quantity <= 0)
            {
                return false;
            }


            return true;
        }

        public IEnumerable GetModelSelectList()
        {
            var list = _modelRepository.GetDbQuerySet().Select(d => new { modelid = d.Id, modelname = d.Name + "【" + d.Code + "】" }).ToList();
            return list;
        }


        public IList<BomDetailEditView> GetPartDetailInBomDetail(int partId)
        {
            var partInfo = _partRepository.Single(partId);
            var dicList = _cacheService.GetCache_Dic();
            var views = partInfo.PartDetailSet.ConvertTo_BomDetailEditViews(partInfo, dicList);
            return views;
        }


        public BomDetailEditView GetEleDetailInBomDetail(int eleid)
        {
            var eleInfo = GetInfoByID(eleid);
            var dicList = _cacheService.GetCache_Dic();
            var view = eleInfo.ConvertTo_BomDetailEditView(dicList);
            return view;
        }
        public IList<StockOut_ForAdd_View> GetStockOutListByBomId(int bomid, int num)
        {
            var list = _bomDetailRepository.GetDbQuerySet().Where(d => d.BomId == bomid).ToList();
            var dicList = _cacheService.GetCache_Unit();
            var partList = _cacheService.GetCache_Part();
            var views = list.ConvertTo_StockOut_ForAdd_Views(dicList, partList, _stockService, num);
            return views;
        }

        public StockOut_ForAdd_View GetStockOutListByEleId(int eleid)
        {
            var eleInfo = GetInfoByID(eleid);
            var dicList = _cacheService.GetCache_Dic();
            var view = eleInfo.ConvertTo_StockOut_ForAdd_View(dicList, _stockService);
            return view;
        }



        #endregion

        #region 库位方法

        public Search_Shelf_Response SearchShelf(Search_Shelf_Request request)
        {
            var q = new Query<ShelfSet>();
            if (!string.IsNullOrEmpty(request.Code))
            {
                q.And(d => d.Code.Contains(request.Code));
            }
            q.OrderBy(d => new { d.Addtime }, true);
            int allcount = 0;
            var pageData = _ShelfRepository.PageQuery(q, request.page, request.rows, out allcount);
            var res = new Search_Shelf_Response();
            res.rows = pageData.ConvertTo_ShelfListViews();
            res.total = allcount;
            return res;
        }

        public CEDResponse AddShelf(Add_Shelf_Request request)
        {
            CEDResponse res = new CEDResponse();
            var Info = request.ConvertTo_ShelfSet_ForCreate();
            var msg = CheckShelfAdd(Info);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }
            _ShelfRepository.Add(Info);
            int rs = _uow.Commit();
            if (rs > 0)
            {
                res.Result = true;

            }
            else
            {
                res.Result = false;
                res.Message = "新增型号失败";
                return res;
            }


            return res;
        }


        public CEDResponse EditShelf(Edit_Shelf_Request request)
        {
            CEDResponse res = new CEDResponse();
            var oldInfo = _ShelfRepository.Single(request.Id);
            if (oldInfo == null)
            {
                res.Result = false;
                res.Message = "未找到编辑的型号信息";
                return res;
            }
            var eleInfo = request.ConvertTo_ShelfSet_ForEdit(oldInfo);
            var msg = CheckShelfEdit(eleInfo);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }
            _ShelfRepository.Save(eleInfo);
            int rs = _uow.Commit();
            if (rs > 0)
            {
                res.Result = true;

            }
            else
            {
                res.Result = false;
                res.Message = "编辑型号失败";
                return res;
            }


            return res;
        }


        private string CheckShelfEdit(ShelfSet info)
        {
            string msg = CheckShelfCodeDup(info.Code, info.Id);
            if (msg != "")
            {
                return msg;
            }

            return "";
        }

        private string CheckShelfAdd(ShelfSet info)
        {
            string msg = CheckShelfCodeDup(info.Code);
            if (msg != "")
            {
                return msg;
            }

            return "";
        }


        private string CheckShelfCodeDup(string newCode, int editId = 0)
        {
            bool rs = false;
            if (editId <= 0)
            {

                rs = _ShelfRepository.GetDbQuerySet().Any(d => d.Code.Equals(newCode));
                if (rs)
                {
                    return "库位不能重复";
                }

            }
            else
            {

                rs = _ShelfRepository.GetDbQuerySet().Any(d => d.Code.Equals(newCode) && d.Id != editId);
                if (rs)
                {
                    return "库位不能重复";
                }

            }
            return "";
        }

        public CEDResponse DeleteShelf(int id)
        {
            CEDResponse res = new CEDResponse();
            var info = _ShelfRepository.Single(id);
            if (info != null)
            {
                _ShelfRepository.Remove(info);
                int rs = _uow.Commit();
                if (rs > 0)
                {
                    res.Result = true;
                }
                else
                {
                    res.Message = "删除库位失败";
                }
            }
            else
            {
                res.Result = false;
                res.Message = "未找到所要删除的库位";
            }
            return res;
        }

        #endregion


        #region 产品方法

        public Search_Product_Response SearchProduct(Search_Product_Request request)
        {
            var q = new Query<ProductSet>();
            if (!string.IsNullOrEmpty(request.Aliases))
            {
                q.And(d => d.Aliases.Contains(request.Aliases));
            }
            if (!string.IsNullOrEmpty(request.ModelName))
            {
                q.And(d => d.ModelSet.Name.Contains(request.ModelName));
            }
            q.OrderBy(d => new { d.Addtime }, true);
            int allcount = 0;
            var pageData = _productRepository.PageQuery(q, request.page, request.rows, out allcount);
            var res = new Search_Product_Response();
            res.rows = pageData.ConvertTo_ProductListViews();
            res.total = allcount;
            return res;
        }
        public IList<ProductDetailListView> GetProductDetail(int ProductId)
        {
            var info = _productRepository.Single(ProductId);
            if (info == null)
            {
                return new List<ProductDetailListView>();
            }
            var unitList = _cacheService.GetCache_Unit();
            var views = info.ProductDetailSet.ConvertTo_ProductDetailListViews(unitList);
            return views;
        }


        public CEDResponse AddProduct(Add_Product_Request request)
        {
            CEDResponse res = new CEDResponse();
            var Info = request.ConvertTo_ProductSet_ForCreate();
            var msg = CheckProductAdd(Info);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }
            _productRepository.Add(Info);
            int rs = _uow.Commit();
            if (rs > 0)
            {
                res.Result = true;
            }
            else
            {
                res.Result = false;
                res.Message = "新增产品失败";
                return res;
            }


            return res;
        }

        public CEDResponse EditProduct(Edit_Product_Request request)
        {
            CEDResponse res = new CEDResponse();
            var oldInfo = _productRepository.Single(request.Id);
            if (oldInfo == null)
            {
                res.Result = false;
                res.Message = "未找到编辑的产品信息";
                return res;
            }
            var eleInfo = request.ConvertTo_ProductSet_ForEdit(oldInfo);
            var msg = CheckProductEdit(eleInfo);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }
            _productRepository.Save(eleInfo);
            int rs = _uow.Commit();
            if (rs > 0)
            {
                res.Result = true;

            }
            else
            {
                res.Result = false;
                res.Message = "编辑产品失败";
                return res;
            }


            return res;
        }

        private string CheckProductAdd(ProductSet info)
        {
            string msg = CheckProductNameDup(info.Aliases);
            if (msg != "")
            {
                return msg;
            }

            return "";
        }

        private string CheckProductEdit(ProductSet info)
        {
            string msg = CheckProductNameDup(info.Aliases, info.Id);
            if (msg != "")
            {
                return msg;
            }

            return "";
        }

        private string CheckProductNameDup(string newName, int editId = 0)
        {
            bool rs = false;
            if (editId <= 0)
            {
                rs = _productRepository.GetDbQuerySet().Any(d => d.Aliases.Equals(newName));
                if (rs)
                {
                    return "产品序列号不能重复";
                }

            }
            else
            {
                rs = _productRepository.GetDbQuerySet().Any(d => d.Aliases.Equals(newName) && d.Id != editId);
                if (rs)
                {
                    return "产品序列号不能重复";
                }

            }
            return "";
        }
        public IList<Default_SelectItem> GetProductSelectListForStockIn()
        {
            var InStatus = (int)ProductStatus.StockIn;
            var list = _productRepository.GetDbQuerySet().Where(d => d.Status != InStatus).ToList();
            IList<Default_SelectItem> selecters = new List<Default_SelectItem>();
            foreach (var item in list)
            {
                selecters.Add(new Default_SelectItem { id = item.Id, text = item.Aliases + "【" + item.ModelSet.Name + "】" });
            }
            return selecters;
        }

        public IList<Default_SelectItem> GetProductSelectListForDelivery()
        {
            var list = _productRepository.GetDbQuerySet().ToList();
            IList<Default_SelectItem> selecters = new List<Default_SelectItem>();
            foreach (var item in list)
            {
                selecters.Add(new Default_SelectItem { id = item.Id, text = item.Aliases + "【" + item.ModelSet.Name + "】" });
            }
            return selecters;
        }

        public StockIn_ForAdd_ByProductView GetStockInAddItemByProductId(int pid)
        {
            var info = _productRepository.Single(pid);
            var unitList = _cacheService.GetCache_Unit();
            var view = info.ConvertTo_StockIn_ForAdd_ByProductView(unitList);
            return view;
        }


        public IEnumerable GetHalfProductSelectList()
        {
            int halfP = (int)ElementType.HalfProduct;
            var list = _productRepository.GetDbQuerySet().Where(d => d.ItemType == halfP).
                Select(d => new { productid = d.Id, producttext = d.Aliases + "【" + d.ModelSet.Name + "】" }).ToList();
            return list;
        }


        public IList<ProductDetailListView> GetElementListByBomId_In_ProductDetail(int bomid)
        {
            var list = _bomDetailRepository.GetDbQuerySet().Where(d => d.BomId == bomid).ToList();
            var dicList = _cacheService.GetCache_Unit();
            var views = list.ConvertTo_ProductDetailListViews(dicList);
            return views;
        }

        public ProductDetailListView GetElementByEleId_In_ProductDetail(int eleid)
        {
            var eleInfo = GetInfoByID(eleid);
            var dicList = _cacheService.GetCache_Dic();
            var view = eleInfo.ConvertTo_ProductDetailListView(dicList);
            return view;
        }


        public IList<ProductDetailListView> GetElementListByHalfProductId_In_ProductDetail(int productId)
        {
            var list = _productDetailRepository.GetDbQuerySet().Where(d => d.ProductId == productId).ToList();
            var dicList = _cacheService.GetCache_Unit();
            var views = list.ConvertTo_ProductDetailListViews(dicList);
            return views;
        }


        public CEDResponse SaveProductDetail(IList<ProductDetailListView> list, string currUserName, int productId)
        {
            var res = new CEDResponse();
            var msg = CheckQuantity(list);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }
            var pInfo = _productRepository.Single(productId);
            if (pInfo == null)
            {
                res.Result = false;
                res.Message = "产品信息不能为空";
                return res;
            }
            pInfo.HalfProductDetailSet = null;
            pInfo.ProductDetailSet = null;
            var info = new ProductDetailSet();
            DateTime dtNow = DateTime.Now;
            int rs = 0;
            _productDetailRepository.RemoveByWhere(d => d.ProductId == productId);
            foreach (var item in list)
            {
                info = new ProductDetailSet();
                info.ProductId = productId;
                info.Addtime = dtNow;
                info.AddUserName = currUserName;
                info.ElementId = item.ElementId;
                if (item.BomId > 0)
                {
                    info.BomId = item.BomId;
                }

                if (pInfo.ItemType == (int)ElementType.HalfProduct)
                {
                    info.HalfProductId = productId;
                }
                else if (item.HalfProductId > 0)
                {
                    info.HalfProductId = item.HalfProductId;
                }

                info.Quantity = item.Quantity;
                _productDetailRepository.Add(info);
            }
            rs = _uow.Commit();
            if (rs > 0)
            {
                res.Result = true;
            }
            else
            {
                res.Message = "产品明细保存失败";
            }
            return res;
        }

        private string CheckQuantity(IList<ProductDetailListView> list)
        {
            foreach (var item in list)
            {
                var num = item.Quantity;
                if (num <= 0)
                {
                    return item.ElementName + "数量不能为0";
                }
            }
            return "";
        }

        #endregion

        #region 领料方法
        public IList<Pick_ForAdd_View> GetListByBomId_For_PickAdd(int bomid, int num)
        {
            var list = _bomDetailRepository.GetDbQuerySet().Where(d => d.BomId == bomid).ToList();
            var dicList = _cacheService.GetCache_Unit();
            var partList = _cacheService.GetCache_Part();
            var views = list.ConvertTo_Pick_ForAdd_Views(dicList, partList, _stockService, num);
            return views;
        }

        public IList<Pick_ForAdd_View> GetListByPartId_For_PickAdd(int partid, int num)
        {
            var list = _partDetailRepository.GetDbQuerySet().Where(d => d.PartId == partid).ToList();
            var dicList = _cacheService.GetCache_Unit();
            var partList = _cacheService.GetCache_Part();
            var views = list.PartDetailSet_To_Pick_ForAdd_Views(dicList, partList, _stockService, num);
            return views;
        }

        public Pick_ForAdd_View GetViewByEleId_For_PickAdd(int eleid)
        {
            var eleInfo = GetInfoByID(eleid);
            var dicList = _cacheService.GetCache_Dic();
            var view = eleInfo.ElementSet_To_Pick_ForAdd_View(dicList, _stockService);
            return view;
        }
        #endregion

        #region 送货单方法
        public DeliveryDetail_ForAdd_View AddProductItem_For_Delivery(int pid)
        {
            var info = _productRepository.GetDbQuerySet().FirstOrDefault(d => d.Id == pid);
            var dicList = _cacheService.GetCache_Unit();
            var modelList = _cacheService.GetCache_Model();
            var views = info.Convert_Product_To_DeliveryDetail_ForAdd_View(dicList, modelList);
            return views;
        }


        public DeliveryDetail_ForAdd_View AddElementItem_For_Delivery(int eleid)
        {
            var eleInfo = GetInfoByID(eleid);
            var dicList = _cacheService.GetCache_Dic();
            var view = eleInfo.Convert_Element_To_DeliveryDetail_ForAdd_View(dicList);
            return view;
        }

        #endregion

    }
}
