using YYQERP.Infrastructure.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace YYQERP.Infrastructure.Domain
{
    public interface IReadOnlyRepository<T, EntityKey> where T : class,IEntity
    {

        T Single(EntityKey Id);

        IQueryable<T> Query(Query<T> query);

        IEnumerable<T> FindAll();

        IList<T> PageQuery(Query<T> query,int pageIndex,int pageSize,out int allCount);

        bool CheckExist(Expression<Func<T, bool>> predicate);
    }
}
