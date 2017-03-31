using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YYQERP.Infrastructure;

namespace YYQERP
{
    public class BomModel
    {
        public BomModel()
        {
            ElementCacheViews = new List<ElementCacheView>();
        }
        public IList<ElementCacheView> ElementCacheViews { get; set; }

        public IEnumerable ModelSelectList { get; set; }

        public IEnumerable PartSelectList { get; set; }
    }
}