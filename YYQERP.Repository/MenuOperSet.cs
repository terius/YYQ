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
    
    public partial class MenuOperSet
    {
        //public int Id { get; set; }
        public int MenuId { get; set; }
        public int OperId { get; set; }
    
        public virtual MenuSet MenuSet { get; set; }
        public virtual OperSet OperSet { get; set; }
    }
}
