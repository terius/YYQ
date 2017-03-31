using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YYQERP.Infrastructure.Helpers
{
    public static class PriceHelper
    {

        public static string FormatMoney(this decimal? price)
        {
            return price.HasValue ? string.Format("{0:C2}", price.Value) : "";
        }

        public static string FormatMoney(this decimal price)
        {
            return string.Format("{0:C2}", price);
        }
    }

}
