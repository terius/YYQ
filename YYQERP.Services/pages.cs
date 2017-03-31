using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services
{
    public class pages
    {
        public IPagedList PageInfo { get; set; }

        public IList<int> PageNumList { get; set; }
    }
}
