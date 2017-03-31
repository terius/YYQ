using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Infrastructure
{
    public class BomCacheView
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }
    }

    public class UserCacheView
    {
        public string UserName { get; set; }

        public string TrueName { get; set; }
    }
}
