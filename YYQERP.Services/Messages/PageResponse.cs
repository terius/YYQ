using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Messages
{
    public class PageResponse<T> where T : class,new()
    {
        public IList<T> rows { get; set; }
        public int total { get; set; }
    }
}
