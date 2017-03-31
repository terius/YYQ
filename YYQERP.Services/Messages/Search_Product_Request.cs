using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public class Search_Product_Request : PageRequest
    {
        public string Aliases { get; set; }

        public string ModelName { get; set; }
    }
}
