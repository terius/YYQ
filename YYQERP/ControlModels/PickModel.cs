﻿using System.Collections.Generic;
using YYQERP.Infrastructure;

namespace YYQERP
{
    public class PickModel
    {
        public IList<ElementCacheView> ElementSelectList { get; set; }

        public IList<BomCacheView> BomSelectList { get; set; }

        public IList<PartCacheView> PartSelectList { get; set; }
    }
}