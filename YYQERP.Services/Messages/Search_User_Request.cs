using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public class Search_User_Request : PageRequest
    {
        public string Name { get; set; }
    }
}
