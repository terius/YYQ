using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Views
{
    public class ZtreeMenuView
    {
        public string MenuCode { get; set; }
        public string MenuName { get; set; }
        public string ParentCode { get; set; }

        public int MenuType { get; set; }
        public string Url { get; set; }

        public string IconClass { get; set; }

        public string icon { get; set; }

        public int Sort { get; set; }
    }
}
