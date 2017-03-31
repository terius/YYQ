using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public class Search_Element_Request : PageRequest
    {
        public string ShelfName { get; set; }
        public string ElementName { get; set; }


    }

    public class Export_Element_Request : Search_Element_Request
    {
        public string ColumnTitles { get; set; }

        public string Columns { get; set; }

        public string FileName { get; set; }

        public bool ExportPageData { get; set; }
    }
}
