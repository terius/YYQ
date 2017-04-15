using System.Collections.Generic;
using YYQERP.Infrastructure;
using YYQERP.Services.Views;

namespace YYQERP
{
    public class DeliveryViewModel
    {
        public IList<ElementCacheView> ElementSelectList { get; set; }


        public IList<Default_SelectItem> ProductSelectList { get; set; }
    }
}