using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Views
{
    public class RoleMenuTreeView
    {
        public int id { get; set; }
        public bool @checked { get; set; }

        public string text { get; set; }

        public IList<OperView> OperViews { get; set; }

        public IList<RoleMenuTreeView> children { get; set; }

    
    }

    public class OperView
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool @checked { get; set; }
    }
}
