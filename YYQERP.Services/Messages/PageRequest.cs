using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public abstract class PageRequest
    {
        public int page { get; set; }
        public int rows { get; set; }
    }
}
