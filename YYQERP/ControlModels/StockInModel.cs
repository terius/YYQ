using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YYQERP.Infrastructure;
using YYQERP.Services.Views;

namespace YYQERP
{
    public class StockInModel
    {
        public IList<Default_SelectItem> StockProductSelectList { get; set; }

        public IEnumerable ShelfSelectList { get; set; }
    }
}