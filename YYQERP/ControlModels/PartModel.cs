using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YYQERP.Infrastructure;

namespace YYQERP
{
    public class PartModel
    {
        public PartModel()
        {
            ElementCacheViews = new List<ElementCacheView>();
        }
        public IList<ElementCacheView> ElementCacheViews { get; set; }

        public IList<PartCacheView> PartSelectViews { get; set; }
    }
}