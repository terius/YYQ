using PagedList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YYQERP.Infrastructure.Domain;
using YYQERP.Infrastructure.Helpers;
using YYQERP.Infrastructure.UnitOfWork;

namespace YYQERP.Services.Implementations
{
    public class BaseService<T, Tid>
        where T : class,IEntity
    {
        IRepository<T, Tid> _IRepository;
        IUnitOfWork _uow;

        public BaseService(IRepository<T, Tid> iRepository, IUnitOfWork uow)
        {
            _IRepository = iRepository;
            _uow = uow;
        }
        public pages GetPagedListData(IPagedList<T> pgList)
        {
            var pageInfo = pgList.GetMetaData();
            int pageCount = pageInfo.PageCount;
            int pageNumber = pageInfo.PageNumber;
            int loopCount = Math.Min(5, pageCount);
            IList<int> pages = new List<int>();
            int startNum = 0;
            if (pageNumber <= 3)
            {
                startNum = 1;
            }
            else if (pageNumber >= (pageCount - 2))
            {
                startNum = pageCount - loopCount + 1;
            }
            else
            {
                startNum = pageNumber - 2;
            }

            for (int i = 0; i < loopCount; i++)
            {
                pages.Add(startNum);
                startNum++;
            }

            return new pages { PageInfo = pageInfo, PageNumList = pages };
        }


        public virtual int SaveAdd(T t)
        {
            _IRepository.Add(t);
            return _uow.Commit();
        }

      

        public IEnumerable<T> GetAllList()
        {
            IEnumerable<T> list = _IRepository.FindAll();
            return list;
        }

        public T GetInfoByID(Tid id)
        {
            T t = _IRepository.Single(id);
            return t;
        }

        public int DeleteInfoByID(Tid id)
        {
            T t = _IRepository.Single(id);
            if (t != null)
            {
                _IRepository.Remove(t);
                return _uow.Commit();
            }
            return 0;
        }


        public virtual int SaveEdit(T t)
        {
            _IRepository.Save(t);
            return _uow.Commit();
        }

        public int RemoveALL()
        {
            return _IRepository.RemoveALL();
        }

        public int Remove(T t)
        {
            _IRepository.Remove(t);
            return _uow.Commit();
        }

        //public IQueryable<T> GetObjectSet()
        //{
        //    return _IRepository.getd();
        //}

        public IQueryable<T> GetDbQuerySet()
        {
            return _IRepository.GetDbQuerySet();
        }

      

        public void CheckRowVersion(byte[] v1, byte[] v2)
        {
            if (!StringHelper.ByteEquals(v1, v2))
            {
                throw new Exception("当前页面信息已被他人修改，请重新打开此页面");
            }
        }


        public virtual IEnumerable GetListForSelect()
        {
            return null;
        }

        public string LoginRoleName { get { return GetCookieValue("UserInfo", "userRole"); } }


        private string GetCookieValue(string cookieName, string valueName)
        {
            var cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
            {
                return "";
            }
            var list = cookie.Value.Split('&');
            foreach (var item in list)
            {
                string name = item.Split('=')[0];
                string value = item.Split('=')[1];
                if (name.Equals(valueName))
                {
                    return value;
                }
            }
            return "";
        }

        public string LoginUserName { get { return HttpContext.Current.User.Identity.Name; } }


       

    }


}
