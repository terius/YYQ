using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public class Search_Stock_Request : PageRequest
    {
        public string ShelfCode { get; set; }
        public string ElementNameOrCode { get; set; }

        public string ProductCode { get; set; }
    }

    public class Search_StockWarn_Request : PageRequest
    {
        public string ShelfName { get; set; }
        public string ElementName { get; set; }


    }

    public class Export_Stock_Request : Search_Stock_Request
    {
        public string ColumnTitles { get; set; }

        public string Columns { get; set; }

        public string FileName { get; set; }

        public bool ExportPageData { get; set; }
    }
}
