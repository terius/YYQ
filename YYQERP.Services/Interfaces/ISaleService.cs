using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYQERP.Repository;
using YYQERP.Services.Messages;
using YYQERP.Services.Views;

namespace YYQERP.Services.Interfaces
{
    public interface ISaleService : IBaseService<SaleReportSet, int>
    {
        Search_SaleReport_Response SearchSaleReport(Search_SaleReport_Request request,bool isPage=true);

        string ImportSaleReport(DataTable dt, string loginUserName);

        CEDResponse Add(SaleReportSet newInfo, string loginUserName);

        CEDResponse Edit(SaleReportView newInfo, string loginUserName);

        CEDResponse Delete(int Id);
    }
}
