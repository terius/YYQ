using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYQERP.Services.Views;

namespace YYQERP.Services.Messages
{
    public class Search_Stock_Response : PageResponse<StockListView>
    {
    }

    public class Search_StockWarn_Response : PageResponse<StockWarnListView>
    {
    }
}
