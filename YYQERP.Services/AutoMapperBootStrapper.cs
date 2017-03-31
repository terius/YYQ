using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYQERP.Repository;
using YYQERP.Services.Views;

namespace YYQERP.Services
{
    public class AutoMapperBootStrapper
    {
        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<SaleReportSet, SaleReportView>();
            Mapper.CreateMap<SaleReportView, SaleReportSet>();
        }
    }
}
