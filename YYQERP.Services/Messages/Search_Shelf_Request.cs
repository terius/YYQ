using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public class Search_Shelf_Request : PageRequest
    {
        public string Code { get; set; }
    }
}
