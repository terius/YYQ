//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace YYQERP.Repository
{
    using System;
    using System.Collections.Generic;
    
    public partial class BomDetailSet
    {
        //public int Id { get; set; }
        public int BomId { get; set; }
        public int ElementId { get; set; }
        public string PartCode { get; set; }
        public double Quantity { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> Addtime { get; set; }
        public Nullable<System.DateTime> Modifytime { get; set; }
        public string AddUserName { get; set; }
        public string ModifyUserName { get; set; }
    
        public virtual ElementSet ElementSet { get; set; }
        public virtual BomSet BomSet { get; set; }
    }
}
