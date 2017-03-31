using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Infrastructure.Domain
{
    public interface IRepository<T, EntityKey> : IReadOnlyRepository<T, EntityKey> where T : class, IEntity
    {
        void Add(T entity);
        void Remove(T entity);
        void Save(T entity);
        int RemoveALL();

        int RemoveByWhere(Func<T, bool> wherefun);

        IQueryable<T> GetDbQuerySet();

        IQueryable<T> GetDbSetForEdit();
    }
}
