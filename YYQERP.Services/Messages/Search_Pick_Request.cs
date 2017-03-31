using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public class Search_Pick_Request : PageRequest
    {
        public DateTime STime { get; set; }
        public DateTime ETime { get; set; }
    }

 
}
