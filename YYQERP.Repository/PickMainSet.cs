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
    
    public partial class PickMainSet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PickMainSet()
        {
            this.PickSet = new HashSet<PickSet>();
        }
    
        //public int Id { get; set; }
        public string Purpose { get; set; }
        public Nullable<System.DateTime> Addtime { get; set; }
        public string AddUserName { get; set; }
        public bool IsFeedback { get; set; }
        public Nullable<System.DateTime> StockOutTime { get; set; }
        public string StockOutUserName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PickSet> PickSet { get; set; }
    }
}