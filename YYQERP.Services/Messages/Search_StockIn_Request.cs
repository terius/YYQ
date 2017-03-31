using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public class Search_StockIn_Request : PageRequest
    {
        public string ShelfCode { get; set; }
        public string ElementCode { get; set; }

        
    }

    public class Search_StockOut_Request : PageRequest
    {
        public string ShelfCode { get; set; }
        public string ElementCode { get; set; }


    }

    public class Export_StockIn_Request : Search_StockIn_Request
    {
        public string ColumnTitles { get; set; }

        public string Columns { get; set; }

        public string FileName { get; set; }

        public bool ExportPageData { get; set; }
    }


    public class Export_StockOut_Request : Search_StockOut_Request
    {
        public string ColumnTitles { get; set; }

        public string Columns { get; set; }

        public string FileName { get; set; }

        public bool ExportPageData { get; set; }
    }
}
