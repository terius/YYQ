using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYQERP.Repository;

namespace YYQERP.Services.Views
{
    public class UserRolePermission
    {

        public UserRolePermission()
        {
            UserRole = new RoleSet();
            UserMenus = new List<MenuSetView>();
        }
        public int UserId { get; set; }

        public IList<string> AllOperCodes { get; set; }

        public string UserName { get; set; }

        public string UserTrueName { get; set; }

        public string RoleName { get; set; }

        public string RoleDescName { get; set; }


        public RoleSet UserRole { get; set; }

        public IList<MenuSetView> UserMenus { get; set; }
    }

    public class MenuSetView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string ParentCode { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public int MenuLevel { get; set; }
    
        public string IconClass { get; set; }
 
        public bool ShowInMenu { get; set; }

        public int Sort { get; set; }

        public IList<string> Opers { get; set; }
    }


   
}
