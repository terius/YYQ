using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YYQERP.Infrastructure;
using YYQERP.Infrastructure.UnitOfWork;
using YYQERP.Infrastructure.Querying;

using System.Data.Entity.Core.Objects;
using System.Linq.Expressions;
using YYQERP.Infrastructure.Domain;
using YYQERP.Infrastructure.Logging;
using YYQERP.Infrastructure.Enums;
using System.Data.Entity;

namespace YYQERP.Repository.Repositories
{
    public class Repository<T, EntityKey> : ReadOnlyRepository<T, EntityKey>, IRepository<T, EntityKey>, IUnitOfWorkRepository where T : class,IEntity, new()
    {
        private IUnitOfWork _uow;
        //   private readonly DbContext _context;// = new YYQERPEntities();

        public Repository(IUnitOfWork uow)
        {
            _uow = uow;

        }

        public DbSet<T> GetDbSet()
        {
            return DataContextFactory.GetDataContext().Set<T>();
        }

        public IQueryable<T> GetDbSetForEdit()
        {
            return DataContextFactory.GetDataContext().Set<T>();
        }

        public void Add(T entity)
        {
            entity.ThrowExceptionIfInvalid(DBAction.Add);
            //try
            //{
                _uow.RegisterNew(entity, this);
            //}
            //catch (Exception ex)
            //{
            //    LoggingFactory.GetLogger().Log(ex.ToString());
            //    throw;
            //}
        }

        public void Remove(T entity)
        {
            _uow.RegisterRemoved(entity, this);
        }

        public void Save(T entity)
        {
            entity.ThrowExceptionIfInvalid(DBAction.Edit);
            _uow.RegisterAmended(entity, this);

            // Do nothing as EF tracks changes
        }

        //public override DbSet<T> GetDbSet()
        //{
        //    return DataContextFactory.GetDataContext().Set<T>();
        //}

        public void PersistCreationOf(IEntity entity)
        {
            GetDbSet().Add((T)entity);
        }

        public void PersistUpdateOf(IEntity entity)
        {

            // Do nothing as EF tracks changes
            // DataContextFactory.GetDataContext().
        }

        public void PersistDeletionOf(IEntity entity)
        {
            GetDbSet().Remove((T)entity);
        }




        public int RemoveALL()
        {
            int icount = 0;
            var list = GetDbSet();
            foreach (var item in list)
            {
                Remove(item);
                icount++;
            }
            return icount;
        }

        public int RemoveByWhere(Func<T, bool> wherefun)
        {
            int icount = 0;
            GetDbSet().Where(wherefun).ToList().ForEach(d => { Remove(d); icount++; });
            return icount;
        }


    }
}
