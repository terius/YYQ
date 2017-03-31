using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYQERP.Repository;
using YYQERP.Services.Messages;
using YYQERP.Services.Views;

namespace YYQERP.Services.Interfaces
{
    public interface IUserService : IBaseService<UserSet, int>
    {
        IList<ZtreeMenuView> GetUserMenuList();

        UserRolePermission GetUserRoleAndMenuByUserId(int userId, string userName);


        Search_User_Response SearchUser(Search_User_Request request);

        IEnumerable GetRoleSelectList();

        CEDResponse AddUser(Add_User_Request request);

        CEDResponse EditUser(Edit_User_Request request);

        CEDResponse DeleteUser(int Id);

        string LoginUserName { get; }

        string LoginRoleName { get; }

        UserRolePermission LoginUserInfo { get; }

        IList<RoleTreeView> GetRoleTreeList();

        IList<RoleMenuTreeView> GetRoleMenus(int roleId);

        CEDResponse SaveRoleMenus(string[] menuIds, int roleId);

        CEDResponse ChangePassword(string newPwd);
    }
}
