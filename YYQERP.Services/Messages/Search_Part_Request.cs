using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public class Search_Part_Request : PageRequest
    {
        public string PartName { get; set; }
        public string PartCode { get; set; }
    }
}
