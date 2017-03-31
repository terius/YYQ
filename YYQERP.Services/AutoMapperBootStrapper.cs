using AutoMapper;
using YYQERP.Repository;
using YYQERP.Services.Views;

namespace YYQERP.Services
{
    public class AutoMapperBootStrapper
    {
        public static void ConfigureAutoMapper()
        {
            Mapper.Initialize(cfg => {
                cfg.CreateMap<SaleReportSet, SaleReportView>();
                cfg.CreateMap<SaleReportView, SaleReportSet>();
            });
            //Mapper.Map<SaleReportSet, SaleReportView>();
           // Mapper.CreateMap<SaleReportView, SaleReportSet>();
        }
    }
}
