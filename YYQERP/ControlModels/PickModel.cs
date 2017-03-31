using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YYQERP.Infrastructure;
using YYQERP.Services.Views;

namespace YYQERP
{
    public class PickModel
    {
        public IList<ElementCacheView> ElementSelectList { get; set; }

        public IList<BomCacheView> BomSelectList { get; set; }

        public IList<PartCacheView> PartSelectList { get; set; }
    }
}