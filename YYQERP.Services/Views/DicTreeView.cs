using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Views
{
    public class DicTreeView
    {
        public DicTreeView()
        {
            this.attributes = new attributes();
            this.children = new List<DicTreeView>();
        }
        public int id { get; set; }

        public string text { get; set; }

        public string iconCls { get; set; }

        public string state { get; set; }

        public attributes attributes { get; set; }

        public IList<DicTreeView> children { get; set; }
    }

   

    public class attributes
    {
        public string dict_code { get; set; }
        public string dict_pcode { get; set; }
        public int dict_sort { get; set; }
        public int dict_enabled { get; set; }
        public string dict_remark { get; set; }  
    }

    public class DicChildrenView
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int sort { get; set; }
        public int Enabled { get; set; }
        public string Remark { get; set; }

        public string ParentCode { get; set; }

    }
}
