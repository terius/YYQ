using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YYQERP.Infrastructure;
using YYQERP.Services.Views;

namespace YYQERP
{
    public class StockOutModel
    {
        public StockOutModel()
        {
            ElementCacheViews = new List<ElementCacheView>();
        }
        public IList<ElementCacheView> ElementCacheViews { get; set; }

        public IEnumerable BomSelectList { get; set; }


      //  public IList<ModelCacheView> ModelCacheViews { get; set; }

        public IList<Default_SelectItem> StockProductSelectList { get; set; }

        public IEnumerable PickSelectList { get; set; }
    }
}