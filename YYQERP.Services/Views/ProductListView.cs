using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Services.Views
{
    public class ProductListView
    {
        public int Id { get; set; }

        public string ModelName { get; set; }

        public string Aliases { get; set; }

        public string CreateDate { get; set; }

        public string Price { get; set; }

        public int ItemType { get; set; }

        public string ItemTypeText { get; set; }

        public int ModelId { get; set; }

        public string UnitTypeCode { get; set; }
        public string UnitName { get; set; }

        public string Remark { get; set; }
    }
}
