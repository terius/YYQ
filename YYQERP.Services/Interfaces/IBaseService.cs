using YYQERP.Infrastructure.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Interfaces
{
    public interface IBaseService<T, Tid>
        where T : class,IEntity
    {
        IEnumerable<T> GetAllList();
        T GetInfoByID(Tid id);
        int SaveAdd(T t);

        IQueryable<T> GetDbQuerySet();

        IEnumerable GetListForSelect();
     }
}
