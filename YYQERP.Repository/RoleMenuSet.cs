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
    
    public partial class RoleMenuSet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RoleMenuSet()
        {
            this.RoleMenuOperSet = new HashSet<RoleMenuOperSet>();
        }
    
        //public int Id { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public Nullable<System.DateTime> Addtime { get; set; }
        public Nullable<int> Sort { get; set; }
        public string AddUserName { get; set; }
        public Nullable<System.DateTime> Modifytime { get; set; }
        public string ModifyUserName { get; set; }
    
        public virtual MenuSet MenuSet { get; set; }
        public virtual RoleSet RoleSet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RoleMenuOperSet> RoleMenuOperSet { get; set; }
    }
}
