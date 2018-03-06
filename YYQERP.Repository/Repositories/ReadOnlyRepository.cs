using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using System.Linq.Expressions;
using YYQERP.Infrastructure.Domain;
using YYQERP.Infrastructure.Querying;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using YYQERP.Repository;


namespace YYQERP.Repository.Repositories
{

    public class ReadOnlyRepository<T, EntityKey> : IReadOnlyRepository<T, EntityKey> where T : class,IEntity
    {
        public IQueryable<T> GetDbQuerySet()
        {
            return DataContextFactory.GetDataContext().Set<T>().AsNoTracking();
        }



        public T Single(EntityKey Id)
        {
            Contract.Requires(Id != null);
            var instance = DataContextFactory.GetDataContext().Set<T>().Find(Id);
            return instance;
        }



        public IEnumerable<T> FindAll()
        {
            return GetDbQuerySet().ToList<T>();
        }

        public bool CheckExist(Expression<Func<T, bool>> predicate)
        {
            return GetDbQuerySet().Any(predicate);
        }


        //public IEnumerable<T> FindBy(Query<T> query)
        //{
        //    var db = GetObjectSet();
        //    var rs = query.BuilderQuery == null ? db : db.Where(query.BuilderQuery);
        //    if (query.OrderByList != null)
        //    {
        //        int i = 0;
        //        foreach (var item in query.OrderByList)
        //        {

        //            if (i == 0)
        //            {
        //                rs = item.IsDesc ? rs.OrderByDescending(item.Orderby) : rs.OrderBy(item.Orderby);
        //            }
        //            else
        //            {
        //                rs = item.IsDesc ? ((IOrderedQueryable<T>)rs).ThenByDescending(item.Orderby) : ((IOrderedQueryable<T>)rs).ThenBy(item.Orderby);

        //            }
        //            i++;
        //        }
        //    }
        //    return rs;

        //    //ObjectQuery<T> efQuery = TranslateIntoObjectQueryFrom(query);

        //    //return efQuery.ToList<T>();
        //}



        public IQueryable<T> Query(Query<T> query)
        {
            var db = GetDbQuerySet();
            var rs = query.BuilderQuery == null ? db : db.Where(query.BuilderQuery);
            if (query.OrderByList != null)
            {
                int i = 0;
                foreach (var item in query.OrderByList)
                {

                    if (i == 0)
                    {
                        rs = item.IsDesc ? rs.OrderByDescending(item.Orderby) : rs.OrderBy(item.Orderby);
                    }
                    else
                    {
                        rs = item.IsDesc ? ((IOrderedQueryable<T>)rs).ThenByDescending(item.Orderby) : ((IOrderedQueryable<T>)rs).ThenBy(item.Orderby);

                    }
                    i++;
                }
            }
            return rs;
        }


        public IList<T> PageQuery(Query<T> query, int pageIndex, int pageSize, out int allCount)
        {
            var IQuery = Query(query);
            allCount = IQuery.Count();
            int index = pageIndex - 1 > 0 ? (pageIndex - 1) : 0;
            var list = IQuery.Skip(index * pageSize).Take(pageSize).ToList();
            return list;
        }


        //public PageData<T> QueryBySQL(Query query, int pageIndex, int pageSize)
        //{

        //    return new SqlTranslatorQuery().GetPageDataBySQLQuery<T>(query, pageIndex, pageSize, GetEntitySetName());

        //}




    }
}
