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
    
    public partial class PartDetailSet
    {
        //public int Id { get; set; }
        public int PartId { get; set; }
        public int ElementId { get; set; }
        public double Quantity { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> Addtime { get; set; }
        public string AddUserName { get; set; }
        public Nullable<System.DateTime> Modifytime { get; set; }
        public string ModifyUserName { get; set; }
    
        public virtual ElementSet ElementSet { get; set; }
        public virtual PartSet PartSet { get; set; }
    }
}
