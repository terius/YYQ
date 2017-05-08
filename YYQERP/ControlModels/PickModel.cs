using System.Collections.Generic;
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