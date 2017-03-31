using YYQERP.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YYQERP.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        void RegisterAmended(IEntity entity, IUnitOfWorkRepository unitofWorkRepository);
        void RegisterNew(IEntity entity, IUnitOfWorkRepository unitofWorkRepository);
        void RegisterRemoved(IEntity entity, IUnitOfWorkRepository unitofWorkRepository);
        int Commit();
    }

}
