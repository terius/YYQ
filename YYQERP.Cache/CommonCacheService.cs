
using YYQERP.Cache.CacheStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYQERP.Infrastructure.Domain;
using YYQERP.Repository;
using YYQERP.Infrastructure;
using YYQERP.Infrastructure.Enums;


namespace YYQERP.Cache
{
    public class CommonCacheService : ICommonCacheService
    {
        ICacheStorage _cacheStorage;
        readonly object obDic = new object();
        readonly object obElement = new object();
        readonly object obUnit = new object();
        readonly object obPart = new object();
        readonly object obModel = new object();
        readonly object obBom = new object();
        readonly object obUser = new object();
        readonly IRepository<DicSet, int> _dicRepository;
        readonly IRepository<ElementSet, int> _eleRepository;
        readonly IRepository<PartSet, int> _partRepository;
        readonly IRepository<ModelSet, int> _modelRepository;
        readonly IRepository<BomSet, int> _bomRepository;
        readonly IRepository<UserSet, int> _userRepository;

        public CommonCacheService(ICacheStorage iCacheStorage,
            IRepository<DicSet, int> dicRepository,
            IRepository<ElementSet, int> eleRepository,
            IRepository<PartSet, int> partRepository,
            IRepository<ModelSet, int> modelRepository,
             IRepository<BomSet, int> bomRepository,
            IRepository<UserSet, int> userRepository
            )
        {
            _cacheStorage = iCacheStorage;
            _dicRepository = dicRepository;
            _eleRepository = eleRepository;
            _partRepository = partRepository;
            _modelRepository = modelRepository;
            _bomRepository = bomRepository;
            _userRepository = userRepository;
        }

        public IList<DicView> GetCache_Dic()
        {
            lock (obDic)
            {
                var views = _cacheStorage.Retrieve<IList<DicView>>(CacheKeys.Dic.ToString());
                if (views == null)
                {
                    var list = _dicRepository.GetDbQuerySet().Where(d => d.Enabled == true).ToList();
                    views = ConvertTo_ConvertTo_DicViews(list);
                    _cacheStorage.Store(CacheKeys.Dic.ToString(), views);
                }
                return views;
            }
        }

        public IList<ModelCacheView> GetCache_Model()
        {
            lock (obModel)
            {
                var views = _cacheStorage.Retrieve<IList<ModelCacheView>>(CacheKeys.Model.ToString());
                if (views == null)
                {
                    views = new List<ModelCacheView>();
                    var list = _modelRepository.FindAll();
                    foreach (var item in list)
                    {
                        views.Add(new ModelCacheView { Id = item.Id, Code = item.Code, Name = item.Name });
                    }
                    _cacheStorage.Store(CacheKeys.Model.ToString(), views);
                }
                return views;
            }
        }

        public IList<DicView> GetCache_Unit()
        {
            lock (obUnit)
            {
                var views = _cacheStorage.Retrieve<IList<DicView>>(CacheKeys.UnitDic.ToString());
                if (views == null)
                {
                    string unitCode = UnitCode.unit.ToString();
                    var list = _dicRepository.GetDbQuerySet().Where(d => d.ParentCode == unitCode && d.Enabled == true).ToList();
                    views = ConvertTo_ConvertTo_DicViews(list);
                    _cacheStorage.Store(CacheKeys.UnitDic.ToString(), views);
                }
                return views;
            }
        }

        public IList<PartCacheView> GetCache_Part()
        {
            lock (obPart)
            {
                var views = _cacheStorage.Retrieve<IList<PartCacheView>>(CacheKeys.Part.ToString());
                if (views == null)
                {
                    views = new List<PartCacheView>();
                    var list = _partRepository.FindAll();
                    string text = "";
                    foreach (var item in list)
                    {
                        text = item.Remark == null ? item.Name : (item.Name + "【" + item.Remark + "】");
                        views.Add(new PartCacheView { PartCode = item.Code, PartName = item.Name, Id = item.Id, Remark = item.Remark, Text = text });
                    }
                    _cacheStorage.Store(CacheKeys.Part.ToString(), views);
                }
                return views;
            }
        }

        public IList<BomCacheView> GetCache_Bom()
        {
            lock (obBom)
            {
                var views = _cacheStorage.Retrieve<IList<BomCacheView>>(CacheKeys.Bom.ToString());
                if (views == null)
                {
                    views = new List<BomCacheView>();
                    var list = _bomRepository.FindAll();
                    foreach (var item in list)
                    {
                        views.Add(new BomCacheView { Id = item.Id, Name = item.Name, Remark = item.Remark });
                    }
                    _cacheStorage.Store(CacheKeys.Bom.ToString(), views);
                }
                return views;
            }
        }

        public IList<UserCacheView> GetCache_User()
        {
            lock (obUser)
            {
                var views = _cacheStorage.Retrieve<IList<UserCacheView>>(CacheKeys.User.ToString());
                if (views == null)
                {
                    views = new List<UserCacheView>();
                    var list = _userRepository.FindAll();
                    foreach (var item in list)
                    {
                        views.Add(new UserCacheView { TrueName = item.TrueName, UserName = item.UserName });
                    }
                    _cacheStorage.Store(CacheKeys.User.ToString(), views);
                }
                return views;
            }
        }


        public DicView ConvertTo_DicView(DicSet source)
        {
            var view = new DicView();
            if (source == null)
            {
                return view;
            }
            view.Id = source.Id;
            view.Code = source.Code;
            view.Enabled = source.Enabled ? 1 : 0;
            view.Name = source.Name;
            view.ParentCode = source.ParentCode;

            return view;
        }

        public IList<DicView> ConvertTo_ConvertTo_DicViews(IEnumerable<DicSet> source)
        {
            var dest = new List<DicView>();
            foreach (var item in source)
            {
                dest.Add(ConvertTo_DicView(item));
            }
            return dest;
        }

        public void RemoveDicCache()
        {
            _cacheStorage.Remove(CacheKeys.Dic.ToString());
            _cacheStorage.Remove(CacheKeys.UnitDic.ToString());
        }

        public void RemoveElementCache()
        {
            _cacheStorage.Remove(CacheKeys.Element.ToString());
        }

        public void RemovePartCache()
        {
            _cacheStorage.Remove(CacheKeys.Part.ToString());
        }

        public void RemoveModelCache()
        {
            _cacheStorage.Remove(CacheKeys.Model.ToString());
        }

        public void RemoveBomCache()
        {
            _cacheStorage.Remove(CacheKeys.Bom.ToString());
        }

        public void RemoveUserCache()
        {
            _cacheStorage.Remove(CacheKeys.User.ToString());
        }


        public IList<ElementCacheView> GetCache_Element()
        {
            lock (obElement)
            {
                var views = _cacheStorage.Retrieve<IList<ElementCacheView>>(CacheKeys.Element.ToString());
                if (views == null)
                {
                    var list = _eleRepository.FindAll();
                    views = ConvertTo_ElementCacheViews(list);
                    _cacheStorage.Store(CacheKeys.Element.ToString(), views);
                }
                return views;
            }
        }


        public ElementCacheView ConvertTo_ElementCacheView(ElementSet source)
        {
            var view = new ElementCacheView();
            if (source == null)
            {
                return view;
            }
            view.ElementCode = source.Code;
            view.ElementId = source.Id;
            view.ElementName = source.Name + "【" + source.Code + "】";
            view.ShelfId = source.ShelfId;
            view.UnitCode = source.UnitTypeCode;
            return view;
        }

        public IList<ElementCacheView> ConvertTo_ElementCacheViews(IEnumerable<ElementSet> source)
        {
            var dest = new List<ElementCacheView>();
            foreach (var item in source)
            {
                dest.Add(ConvertTo_ElementCacheView(item));
            }
            return dest;
        }

    }
}
