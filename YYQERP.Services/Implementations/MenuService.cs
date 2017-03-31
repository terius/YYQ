using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYQERP.Infrastructure.Domain;
using YYQERP.Infrastructure.UnitOfWork;
using YYQERP.Repository;
using YYQERP.Services.Interfaces;

namespace YYQERP.Services.Implementations
{
    public class MenuService : BaseService<MenuSet, int>, IMenuService
    {
        private readonly IRepository<MenuSet, int> _Repository;
        private readonly IUnitOfWork _uow;

        public MenuService(IRepository<MenuSet, int> repository, IUnitOfWork uow
       )
            : base(repository, uow)
        {
            _Repository = repository;
            _uow = uow;
        }
    }
}
