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
using YYQERP.Infrastructure.Querying;
using YYQERP.Infrastructure.UnitOfWork;
using YYQERP.Repository;
using YYQERP.Services.Interfaces;
using YYQERP.Services.Messages;
using YYQERP.Services.Views;

namespace YYQERP.Services.Implementations
{
    public class DicService : BaseService<DicSet, int>, IDicService
    {
        private readonly IRepository<DicSet, int> _Repository;
        private readonly IUnitOfWork _uow;
        private readonly ICommonCacheService _cacheService;
        public DicService(IRepository<DicSet, int> repository, IUnitOfWork uow, ICommonCacheService cacheService
       )
            : base(repository, uow)
        {
            _Repository = repository;
            _uow = uow;
            _cacheService = cacheService;
        }

        public IList<DicView> GetUnitList()
        {
            var unitcode = UnitCode.unit.ToString();
            var list = _Repository.GetDbQuerySet().Where(d => d.ParentCode == unitcode && d.Enabled == true).ToList();
            var views = list.ConvertTo_ConvertTo_DicViews();
            return views;
        }


        public IEnumerable GetUnitSelectList()
        {

            var list = _cacheService.GetCache_Unit().Where(d => d.ParentCode == "unit").Select(d => new { unitcode = d.Code, unitname = d.Name }).ToList();
            return list;
        }

        public IList<DicTreeView> GetDicTreeViews(string parentCode)
        {
            IList<DicTreeView> views = new List<DicTreeView>();
            IList<DicSet> list = null;
            if (string.IsNullOrEmpty(parentCode))
            {
                list = GetDbQuerySet().Where(d => d.ParentCode == null).OrderBy(d => d.sort).ToList();
            }
            else
            {
                list = GetDbQuerySet().Where(d => d.ParentCode == parentCode).OrderBy(d => d.sort).ToList();
            }
            DicTreeView view = null;
            foreach (var item in list)
            {
                view = new DicTreeView();
                view.attributes.dict_code = item.Code;
                view.attributes.dict_enabled = item.Enabled ? 1 : 0;
                view.attributes.dict_pcode = item.ParentCode;
                view.attributes.dict_remark = item.Remark;
                view.attributes.dict_sort = item.sort;
                view.iconCls = "icon-standard-image";
                view.id = item.Id;
                view.state = "open";
                view.text = item.Name;
                //  view.children = GetDicTreeViews(item.Code);
                views.Add(view);
            }
            return views;
        }

        public IList<DicChildrenView> GetDicChildrenViews(string parentCode)
        {
            if (string.IsNullOrEmpty(parentCode))
            {
                return new List<DicChildrenView>();
            }
            var list = GetDbQuerySet().Where(d => d.ParentCode == parentCode).OrderBy(d => d.sort).ToList();
            var view = list.ConvertTo_DicChildrenViews();
            return view;
        }

        public CEDResponse AddDic(Add_Dic_Request request)
        {
            CEDResponse res = new CEDResponse();
            var Info = request.ConvertTo_DicSet_ForCreate();
            var msg = CheckDicAdd(Info);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }
            _Repository.Add(Info);
            int rs = _uow.Commit();
            if (rs > 0)
            {
                res.Result = true;
                _cacheService.RemoveDicCache();
            }
            else
            {
                res.Result = false;
                res.Message = "新增字典失败";
                return res;
            }


            return res;
        }

        private string CheckDicAdd(DicSet info)
        {
            if (string.IsNullOrEmpty(info.Code))
            {
                return "字典代码不能为空";
            }
            if (string.IsNullOrEmpty(info.Name))
            {
                return "字典名称不能为空";
            }
            string msg = CheckDicCodeDup(info.Code);
            if (msg != "")
            {
                return msg;
            }

            return "";
        }

        private string CheckDicEdit(DicSet info)
        {
            if (string.IsNullOrEmpty(info.Code))
            {
                return "字典代码不能为空";
            }
            if (string.IsNullOrEmpty(info.Name))
            {
                return "字典名称不能为空";
            }
            string msg = CheckDicCodeDup(info.Code,info.Id);
            if (msg != "")
            {
                return msg;
            }

            return "";
        }

        private string CheckDicCodeDup(string newCode, int editId = 0)
        {
            bool rs = false;
            if (editId <= 0)
            {
                rs = _Repository.GetDbQuerySet().Any(d => d.Code.Equals(newCode));
                if (rs)
                {
                    return "字典代码不能重复";
                }

            }
            else
            {
                rs = _Repository.GetDbQuerySet().Any(d => d.Name.Equals(newCode) && d.Id != editId);
                if (rs)
                {
                    return "Bom字典代码不能重复";
                }

            }
            return "";
        }

        public CEDResponse DeleteDic(int id)
        {
            CEDResponse res = new CEDResponse();
            var info = _Repository.Single(id);
            if (info != null)
            {
                _Repository.Remove(info);
                _Repository.RemoveByWhere(d => d.ParentCode == info.Code);
                int rs = _uow.Commit();
                if (rs > 0)
                {
                    res.Result = true;
                    _cacheService.RemoveDicCache();
                }
                else
                {
                    res.Message = "删除字典失败";
                }
            }
            else
            {
                res.Result = false;
                res.Message = "未找到所要删除的字典";
            }
            return res;
        }

        public CEDResponse EditDic(Edit_Dic_Request request)
        {
            CEDResponse res = new CEDResponse();
            var oldInfo = _Repository.Single(request.Id);
            if (oldInfo == null)
            {
                res.Result = false;
                res.Message = "未找到编辑的Dic信息";
                return res;
            }
            var eleInfo = request.ConvertTo_DicSet_ForEdit(oldInfo);
            var msg = CheckDicEdit(eleInfo);
            if (msg != "")
            {
                res.Result = false;
                res.Message = msg;
                return res;
            }
            _Repository.Save(eleInfo);
            int rs = _uow.Commit();
            if (rs > 0)
            {
                res.Result = true;
                _cacheService.RemoveDicCache();
            }
            else
            {
                res.Result = false;
                res.Message = "编辑Dic失败";
                return res;
            }


            return res;
        }
    }
}
