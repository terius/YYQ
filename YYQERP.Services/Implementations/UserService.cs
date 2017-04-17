using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YYQERP.Cache;
using YYQERP.Infrastructure.Domain;
using YYQERP.Infrastructure.Helpers;
using YYQERP.Infrastructure.Querying;
using YYQERP.Infrastructure.UnitOfWork;
using YYQERP.Repository;
using YYQERP.Services.Interfaces;
using YYQERP.Services.Messages;
using YYQERP.Services.Views;

namespace YYQERP.Services.Implementations
{
    public class UserService : BaseService<UserSet, int>, IUserService
    {
        private readonly IRepository<UserSet, int> _Repository;
        private readonly IRepository<MenuSet, int> _MenuRepository;
        //  private readonly IRepository<UserRoleSet, int> _userRoleRepository;
        private readonly IRepository<RoleSet, int> _roleRepository;
        private readonly IUnitOfWork _uow;
        private readonly IRepository<RoleMenuSet, int> _roleMenuRepository;
        private readonly IRepository<MenuOperSet, int> _menuOperRepository;
        private readonly IRepository<RoleMenuOperSet, int> _roleMenuOperRepository;
        private readonly IRepository<OperSet, int> _operRepository;
        private readonly ICommonCacheService _cacheService;
        public UserService(IRepository<UserSet, int> repository, IUnitOfWork uow, IRepository<MenuSet, int> MenuRepository,
            IRepository<UserRoleSet, int> userRoleRepository,
            IRepository<RoleSet, int> roleRepository,
            IRepository<RoleMenuSet, int> roleMenuRepository,
            IRepository<MenuOperSet, int> menuOperRepository,
            IRepository<RoleMenuOperSet, int> roleMenuOperRepository,
            IRepository<OperSet, int> operRepository,
            ICommonCacheService cacheService
       )
            : base(repository, uow)
        {
            _Repository = repository;
            _uow = uow;
            _MenuRepository = MenuRepository;
            //    _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _roleMenuRepository = roleMenuRepository;
            _menuOperRepository = menuOperRepository;
            _roleMenuOperRepository = roleMenuOperRepository;
            _operRepository = operRepository;
            _cacheService = cacheService;
        }


     


        public IList<ZtreeMenuView> GetUserMenuList()
        {
            //var mlist = _MenuRepository.GetDbQuerySet().Where(d => d.ParentCode != null).ToList();
            //foreach (var item1 in mlist)
            //{
            //    MenuOperSet info = new MenuOperSet();
            //    info.MenuId = item1.Id;
            //    info.OperId = 15;
            //    _menuOperRepository.Add(info);
            //}
            //int edd = _uow.Commit();
            //setus();

            //   var userid = 1;
            //  var userInfo = GetInfoByID(userid);
            IList<ZtreeMenuView> list = new List<ZtreeMenuView>();
            var roleName = LoginRoleName;
            if (roleName == "Admin")
            {
                var menuList = _MenuRepository.GetDbQuerySet().Where(d => d.ShowInMenu == true).ToList();
                foreach (var item in menuList)
                {
                    list.Add(item.ConvertTo_ZtreeMenuView(item.Id));
                }
            }
            else
            {
                var roleMenus = LoginUserInfo;
                foreach (var item in roleMenus.UserMenus)
                {
                    if (item.ShowInMenu)
                    {
                        list.Add(item.ConvertTo_ZtreeMenuView());
                    }
                }
                //var userRoleInfo = userInfo.UserRoleSet.FirstOrDefault();
                //if (userRoleInfo != null)
                //{
                //var roleMenuList =  userInfo.RoleSet.RoleMenuSet.OrderBy(d => d.Sort);
                //foreach (var item in roleMenuList)
                //{
                //    if (item.MenuSet.ShowInMenu)
                //    {
                //        list.Add(item.MenuSet.ConvertTo_ZtreeMenuView(item.Sort.Value));
                //    }
                //}
                //  }
            }
            return list;
        }

        //private void setus()
        //{
        //    var list = _MenuRepository.FindAll();
        //    foreach (var item in list)
        //    {
        //        if (!string.IsNullOrEmpty(item.ParentCode))
        //        {
        //            MenuOperSet info = new MenuOperSet();
        //            info.MenuId = item.Id;
        //            info.OperId = 1;
        //            _menuOperRepository.Add(info);

        //            info = new MenuOperSet();
        //            info.MenuId = item.Id;
        //            info.OperId = 4;
        //            _menuOperRepository.Add(info);

        //            info = new MenuOperSet();
        //            info.MenuId = item.Id;
        //            info.OperId = 5;
        //            _menuOperRepository.Add(info);

        //            info = new MenuOperSet();
        //            info.MenuId = item.Id;
        //            info.OperId = 6;
        //            _menuOperRepository.Add(info);
        //        }
        //    }
        //    _uow.Commit();
        //}

     

        public UserRolePermission GetUserRoleAndMenuByUserId(int userId, string userName)
        {
            UserRolePermission userPers = new UserRolePermission();
            userPers.UserId = userId;
            userPers.UserName = userName;

            var userInfo = GetInfoByID(userId);
            userPers.UserTrueName = userInfo.TrueName;
            var roleInfo = userInfo.RoleSet;

            userPers.UserRole = roleInfo;
            userPers.RoleName = roleInfo.Name;
            userPers.RoleDescName = roleInfo.Description;
            userPers.AllOperCodes = GetAllOperCodes();
            var rmsList = userInfo.RoleSet.RoleMenuSet.OrderBy(d => d.Sort).ToList();
            if (rmsList != null && rmsList.Count > 0)
            {
                foreach (var rms in rmsList)
                {
                    int sort = rms.Sort.HasValue ? rms.Sort.Value : 0;
                    userPers.UserMenus.Add(rms.MenuSet.ConvertTo_MenuSetView(sort, rms.RoleMenuOperSet));
                }
            }

            return userPers;
        }

        private IList<string> GetAllOperCodes()
        {
            var list = _operRepository.GetDbQuerySet().Select(d => d.Code.ToLower()).ToList();
            return list;
        }

        public UserRolePermission LoginUserInfo
        {
            get
            {
                var info = HttpContext.Current.Session["UserRolesMenus"] as UserRolePermission;
                return info;
            }
        }

        public Search_User_Response SearchUser(Search_User_Request request)
        {
            Query<UserSet> q = new Query<UserSet>();
            if (!string.IsNullOrEmpty(request.Name))
            {
                q.And(d => d.TrueName.Contains(request.Name) || d.UserName.Contains(request.Name));
            }
            q.And(d => d.UserName != "admin");
            q.OrderBy(d => new { d.Id });
            int allcount = 0;
            var pageData = _Repository.PageQuery(q, request.page, request.rows, out allcount);
            var res = new Search_User_Response();
            res.rows = pageData.ConvertTo_UserListViews();
            res.total = allcount;
            return res;
        }

        public IEnumerable GetRoleSelectList()
        {
            //  IList<Default_SelectItem> items = new List<Default_SelectItem>();
            var list = _roleRepository.GetDbQuerySet().Select(d => new { id = d.Id, text = d.Description }).ToList();
            //foreach (var item in list)
            //{
            //    items.Add(new Default_SelectItem { id = item.Id, text = item.Description });
            //}
            return list;
        }

        public CEDResponse AddUser(Add_User_Request request)
        {
            CEDResponse res = new CEDResponse();
            var userInfo = request.ConvertTo_UserSet_forCreate(this.LoginUserName);
            if (CheckUserNameDup(userInfo.UserName))
            {
                res.Result = false;
                res.Message = "用户名不能重复";
                return res;
            }

            int rs = SaveAdd(userInfo);
            if (rs > 0)
            {
                res.Result = true;
                _cacheService.RemoveUserCache();
            }
            else
            {
                res.Result = false;
                res.Message = "保存失败";
                return res;
            }




            return res;

        }

        public CEDResponse EditUser(Edit_User_Request request)
        {
            CEDResponse res = new CEDResponse();
            var oldUserInfo = GetInfoByID(request.Id);
            var userInfo = request.ConvertTo_UserSet_forEdit(oldUserInfo, this.LoginUserName);
            if (CheckUserNameDup(userInfo.UserName, userInfo.Id))
            {
                res.Result = false;
                res.Message = "用户名不能重复";
                return res;
            }

            int rs = SaveEdit(userInfo);
            if (rs > 0)
            {
                res.Result = true;
                _cacheService.RemoveUserCache();
            }
            else
            {
                res.Result = false;
                res.Message = "保存失败";
                Remove(userInfo);
                return res;
            }
            return res;

        }

        public CEDResponse DeleteUser(int Id)
        {
            CEDResponse res = new CEDResponse();
            var info = GetInfoByID(Id);
            if (info != null)
            {
                int rs = Remove(info);
                if (rs > 0)
                {
                    res.Result = true;
                }
                else
                {
                    res.Message = "删除用户失败";
                }
            }
            return res;
        }

        private bool CheckUserNameDup(string newUserNames, int editId = 0)
        {
            var newName = newUserNames.ToUpper().Trim();
            if (editId <= 0)
            {
                return GetDbQuerySet().Any(d => d.UserName.Equals(newName));
            }
            else
            {
                return GetDbQuerySet().Any(d => d.UserName.Equals(newName) && d.Id != editId);
            }
        }

        public IList<RoleTreeView> GetRoleTreeList()
        {
            IList<RoleTreeView> views = new List<RoleTreeView>();
            var list = _roleRepository.GetDbQuerySet().Where(d => d.Name != "Admin" && d.Name != "Boss").ToList();
            foreach (var item in list)
            {
                views.Add(new RoleTreeView { RoleId = item.Id, RoleName = item.Description });
            }
            return views;
        }
        public IList<RoleMenuTreeView> GetRoleMenus(int roleId)
        {

            IList<RoleMenuTreeView> views = new List<RoleMenuTreeView>();
            if (roleId <= 0)
            {
                return views;
            }
            var RoleMenus = _roleRepository.Single(roleId).RoleMenuSet;
            var list = _MenuRepository.GetDbQuerySet().Where(d => d.ShowInMenu == true).ToList();
            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item.ParentCode))
                {
                    RoleMenuTreeView view = new RoleMenuTreeView();
                    view.id = item.Id;
                    view.text = item.Title;
                    view.children = list.Where(d => d.ParentCode == item.Code).ConvertTo_RoleMenuTreeViews(RoleMenus);
                    if (RoleMenus.Any(d => d.MenuId == item.Id))
                    {
                        view.@checked = true;
                    }
                    views.Add(view);
                }
            }
            return views;

        }

        public CEDResponse SaveRoleMenus(string[] menuTreeForms, int roleId)
        {
            CEDResponse res = new CEDResponse();
            IList<MenuTreeView> MenuIds = new List<MenuTreeView>();
            MenuTreeView view = null;
            var menuAllList = _MenuRepository.FindAll();
            foreach (var nameStr in menuTreeForms)
            {
                if (nameStr.StartsWith("mck-"))
                {
                    view = new MenuTreeView();
                    view.Id = Convert.ToInt32(nameStr.Split('-')[1]);
                    IList<int> ops = new List<int>();
                    foreach (var nameStr2 in menuTreeForms)
                    {
                        if (nameStr2.StartsWith("ck-" + view.Id + "-"))
                        {
                            ops.Add(Convert.ToInt32(nameStr2.Split('-')[2]));
                        }
                    }
                    if (ops.Count > 0)
                    {
                        view.OperIds = ops;
                    }


                    MenuIds.Add(view);
                    int parentid = GetParentId(menuAllList, view.Id);
                    if (parentid > 0 && !MenuIds.Any(d => d.Id == parentid))
                    {
                        view = new MenuTreeView();
                        view.Id = parentid;
                        MenuIds.Add(view);
                    }
                }
            }
            ////var info = _roleRepository.Single(roleId);
            ////info.RoleMenuSet = null;
            var logUserName = LoginUserName;
            DateTime dtNow = DateTime.Now;
            int i = 0;

            var rmList = _roleMenuRepository.GetDbQuerySet().Where(d => d.RoleId == roleId).ToList();
            foreach (var rm in rmList)
            {
                _roleMenuOperRepository.RemoveByWhere(d => d.RoleMenuId == rm.Id);
            }
            _roleMenuRepository.RemoveByWhere(d => d.RoleId == roleId);

            int mainid = GetMainPageId();
            if (mainid > 0)
            {
                MenuIds.Add(new MenuTreeView { Id = mainid });
            }
            foreach (var item in MenuIds)
            {
                RoleMenuSet mInfo = new RoleMenuSet();
                mInfo.Addtime = dtNow;
                mInfo.AddUserName = logUserName;
                mInfo.MenuId = item.Id;
                mInfo.RoleId = roleId;
                i++;
                mInfo.Sort = i;
                if (item.OperIds != null)
                {
                    IList<RoleMenuOperSet> temp = new List<RoleMenuOperSet>();
                    RoleMenuOperSet rmoInfo = null;
                    foreach (var operId in item.OperIds)
                    {
                        rmoInfo = new RoleMenuOperSet();
                        rmoInfo.OperId = operId;
                        rmoInfo.Addtime = dtNow;
                        rmoInfo.AddUserName = logUserName;
                        temp.Add(rmoInfo);
                    }
                    mInfo.RoleMenuOperSet = temp;
                }

                _roleMenuRepository.Add(mInfo);
            }
            //  _roleRepository.Save(info);
            int rs = _uow.Commit();
            res.Message = rs > 0 ? "" : "保存权限失败";
            return res;

        }

        private int GetParentId(IEnumerable<MenuSet> menuAllList, int menuId)
        {
            var info = menuAllList.FirstOrDefault(d => d.Id == menuId);
            if (info != null)
            {
                if (!string.IsNullOrEmpty(info.ParentCode))
                {
                    var parentInfo = menuAllList.FirstOrDefault(d => d.Code == info.ParentCode);
                    if (parentInfo != null)
                    {
                        return parentInfo.Id;
                    }
                }
            }
            return 0;
        }

        private int GetMainPageId()
        {
            var info = _MenuRepository.GetDbQuerySet().FirstOrDefault(d => d.Code == "9999");
            return info == null ? 0 : info.Id;
        }

        public CEDResponse ChangePassword(string newPwd)
        {
            CEDResponse res = new CEDResponse();
            var info = GetInfoByID(LoginUserInfo.UserId);
            if (info != null)
            {
                info.Password = StringHelper.Sha256(newPwd);
                info.ModifyTime = DateTime.Now;
                info.ModifyUserName = LoginUserName;
                var rs = SaveEdit(info);
                if (rs <= 0)
                {
                    res.Result = false;
                    res.Message = "修改密码失败";
                }
            }
            return res;
        }



    }

    public class MenuTreeView
    {
        public int Id { get; set; }

        public IList<int> OperIds { get; set; }
    }
}
