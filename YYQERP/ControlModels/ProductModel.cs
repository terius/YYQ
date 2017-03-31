using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YYQERP.Infrastructure;
using YYQERP.Services.Views;

namespace YYQERP
{
    public class ProductModel
    {
        public IEnumerable ModelSelectList { get; set; }

        public IEnumerable UnitSelectList { get; set; }


        public IList<ElementCacheView> ElementCacheViews { get; set; }

        public IEnumerable BomSelectList { get; set; }


        //  public IList<ModelCacheView> ModelCacheViews { get; set; }

        public IEnumerable HalfProductSelectList { get; set; }
    }
}