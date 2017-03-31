// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace YYQERP.DependencyResolution
{
    using StructureMap;
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;
    using System.Data.Entity;
    using YYQERP.Cache;
    using YYQERP.Cache.CacheStorage;
    using YYQERP.Infrastructure.Configuration;
    using YYQERP.Infrastructure.Domain;
    using YYQERP.Infrastructure.Logging;
    using YYQERP.Infrastructure.UnitOfWork;
    using YYQERP.Repository;
    using YYQERP.Repository.Repositories;
    using YYQERP.Services.Implementations;
    using YYQERP.Services.Interfaces;

    public class DefaultRegistry : Registry
    {

        #region Constructors and Destructors

        public DefaultRegistry()
        {

            Scan(
                scan =>
                {
                     //scan.Assembly("YYQERP.Services");
                     //scan.Assembly("YYQERP.Repository");
                     //scan.Assembly("YYQERP.Cache");

                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                    scan.With(new ControllerConvention());
                });
       


            For<ICacheStorage>().Use<HttpContextCacheAdapter>();
            For<IApplicationSettings>().Use<WebConfigApplicationSettings>();
            // Logger
            For<ILogger>().Use<Log4NetAdapter>();
            For<IUnitOfWork>().Use<EFUnitOfWork>();
            For<DbContext>().Use<YYQERPEntities>();

            For(typeof(IRepository<,>)).Use(typeof(Repository<,>));

            //ЗўЮёзЂВс
            For<IUserService>().Use<UserService>();
            For<IMenuService>().Use<MenuService>();
            For<IStockService>().Use<StockService>();
            For<IDicService>().Use<DicService>();
            For<ICommonCacheService>().Use<CommonCacheService>();
            For<IGoodsService>().Use<GoodsService>();
            For<IPickService>().Use<PickService>();
            For<ISaleService>().Use<SaleService>();
           
        }

        #endregion
    }
}