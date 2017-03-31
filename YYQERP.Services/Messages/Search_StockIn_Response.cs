using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYQERP.Services.Views;

namespace YYQERP.Services.Messages
{
    public class Search_StockIn_Response : PageResponse<StockInListView>
    {
    }

    public class Search_StockOut_Response : PageResponse<StockOutListDetailView>
    {
    }
}
