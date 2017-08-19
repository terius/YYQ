using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public class Search_Bom_Request : PageRequest
    {
        public int ModelId { get; set; }
        public string ElementNameOrCode { get; set; }
    }
}
