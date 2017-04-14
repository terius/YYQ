using System;
using YYQERP.Cache;
using YYQERP.Infrastructure.Domain;
using YYQERP.Infrastructure.Querying;
using YYQERP.Infrastructure.UnitOfWork;
using YYQERP.Repository;
using YYQERP.Services.Interfaces;
using YYQERP.Services.Messages;

namespace YYQERP.Services.Implementations
{
    public class DeliveryService : BaseService<DeliverySet, int>, IDeliveryService
    {
        private readonly IRepository<DeliverySet, int> _Repository;
        private readonly IUnitOfWork _uow;
        private readonly ICommonCacheService _cacheService;
        private readonly IStockService _stockService;
        public DeliveryService(IRepository<DeliverySet, int> repository,
            IUnitOfWork uow,
            ICommonCacheService cacheService,
            IStockService stockService
       )
            : base(repository, uow)
        {
            _Repository = repository;
            _uow = uow;
            _cacheService = cacheService;
            _stockService = stockService;
        }

        public Search_Delivery_Response SearchDelivery(Search_Delivery_Request request)
        {

            Query<DeliverySet> q = new Query<DeliverySet>();
            if (request.STime != DateTime.MinValue)
            {
                q.And(d => d.Addtime >= request.STime);
            }
            if (request.ETime != DateTime.MinValue)
            {
                q.And(d => d.Addtime <= request.ETime);
            }

            q.OrderBy(d => new { d.Addtime }, true);
            int allcount = 0;
            var pageData = _Repository.PageQuery(q, request.page, request.rows, out allcount);
            var res = new Search_Delivery_Response();
            var unitList = _cacheService.GetCache_Unit();
            var userList = _cacheService.GetCache_User();
            res.rows = pageData.Convert_Delivery_To_DeliveryListView_List(unitList, userList);
            res.total = allcount;
            return res;
        }





    }



}
