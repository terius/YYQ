using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Infrastructure.Enums
{
    public enum DBAction
    {
        Add,
        Edit,
        Delete,
        Other
    }


    public enum Role
    {
        Admin = 1,
        Finance,
        Product,
        Sale,
        Boss
    }


    public enum ElementType
    {
        [Description("原材料")]
        Element = 1,
        [Description("成品")]
        Product = 2,
        [Description("半成品")]
        HalfProduct = 3
    }

    public enum UnitCode
    {
        [Description("单位")]
        unit = 1

    }


    public enum PartCode
    {
        partOther
    }


    public enum ProductStatus
    {
        Created,
        StockIn,
        StockOut,
        Saled
    }
}
