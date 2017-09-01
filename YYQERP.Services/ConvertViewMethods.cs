using AutoMapper;
using YYQERP.Infrastructure.Enums;
using YYQERP.Infrastructure.Helpers;
using YYQERP.Repository;
using YYQERP.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYQERP.Services.Views;
using YYQERP.Services.Messages;
using YYQERP.Infrastructure;
using System.Web;

namespace YYQERP.Services
{
    public static class ConvertViewMethods
    {
        public static ZtreeMenuView ConvertTo_ZtreeMenuView(this MenuSetView source)
        {
            var view = new ZtreeMenuView();
            if (source == null)
            {
                return view;
            }
            view.icon = source.Icon;
            view.IconClass = source.IconClass;
            view.MenuCode = source.Code;
            view.MenuName = source.Title;
            view.MenuType = source.MenuLevel;
            view.ParentCode = source.ParentCode;
            view.Url = source.Url;
            view.Sort = source.Sort;

            return view;
        }

        public static ZtreeMenuView ConvertTo_ZtreeMenuView(this MenuSet source, int sort)
        {
            var view = new ZtreeMenuView();
            if (source == null)
            {
                return view;
            }
            view.icon = source.Icon;
            view.IconClass = source.IconClass;
            view.MenuCode = source.Code;
            view.MenuName = source.Title;
            view.MenuType = source.MenuLevel;
            view.ParentCode = source.ParentCode;
            view.Url = source.Url;
            view.Sort = sort;
            return view;
        }



        public static UserListView ConvertTo_UserListView(this UserSet source)
        {
            var view = new UserListView();
            if (source == null)
            {
                return view;
            }
            var roleInfo = source.RoleSet;
            view.Addtime = source.AddTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            view.Id = source.Id;
            view.RoleName = roleInfo == null ? "" : roleInfo.Description;
            view.TrueName = source.TrueName;
            view.UserName = source.UserName;
            view.RoleId = roleInfo == null ? 0 : roleInfo.Id;
            return view;
        }

        public static IList<UserListView> ConvertTo_UserListViews(this IList<UserSet> source)
        {
            var dest = new List<UserListView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_UserListView());
            }
            return dest;
        }

        public static UserSet ConvertTo_UserSet_forCreate(this Add_User_Request source, string currUserName)
        {
            var info = new UserSet();
            if (source == null)
            {
                return info;
            }
            info.AddTime = DateTime.Now;
            info.AddUserName = currUserName;
            info.Password = StringHelper.Sha256("123456");
            info.TrueName = source.TrueName;
            info.UserName = source.UserName;
            info.RoleId = source.RoleId;
            return info;
        }

        public static UserSet ConvertTo_UserSet_forEdit(this Edit_User_Request source, UserSet oldInfo, string currUserName)
        {
            var info = oldInfo;
            if (source == null)
            {
                return info;
            }
            info.ModifyTime = DateTime.Now;
            info.ModifyUserName = currUserName;
            info.TrueName = source.TrueName;
            info.UserName = source.UserName;
            info.RoleId = source.RoleId;
            return info;
        }


        public static StockListView ConvertTo_StockListView(this StockSet source, IList<DicView> dicList)
        {
            var view = new StockListView();
            if (source == null)
            {
                return view;
            }

            view.FirstInTime = source.FirstInTime.ToString("yyyy-MM-dd HH:mm:ss");
            view.Id = source.Id;
            view.ItemTypeText = StringHelper.GetEnumDescription((ElementType)source.ItemType);
            view.LastInTime = source.LastInTime.ToStringWithtime();
            view.LastOutTime = source.LastOutTime.ToStringWithtime();
            if (source.ElementId.HasValue && source.ElementId.Value > 0)
            {
                view.ElementName = GetElementShowName(source.ElementSet);
            }
            else
            {
                view.ElementName = GetProductShowName(source.ProductSet);
            }
            view.UnitType = GetDicNameByCode(source.UnitTypeCode, dicList);
            view.Quantity = source.Quantity;
            view.ShelfName = GetShelfShowName(source.ShelfSet);
            view.WarnQuantity = source.ElementId.HasValue ? source.ElementSet.WarningQuantity : 0;
            return view;
        }

        public static IList<StockListView> ConvertTo_StockListViews(this IList<StockSet> source, IList<DicView> dicList)
        {
            var dest = new List<StockListView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_StockListView(dicList));
            }
            return dest;
        }


        public static StockWarnListView ConvertTo_StockWarnListView(this StockSet source, IList<DicView> dicList)
        {
            var view = new StockWarnListView();
            if (source == null)
            {
                return view;
            }


            view.Id = source.Id;
            view.ItemTypeText = StringHelper.GetEnumDescription((ElementType)source.ItemType);
            view.LastOutTime = source.LastOutTime.HasValue ? source.LastOutTime.Value.ToStringWithtime() : "";
            if (source.ElementId.HasValue && source.ElementId.Value > 0)
            {
                view.ElementName = GetElementShowName(source.ElementSet);
            }
            else
            {
                view.ElementName = GetProductShowName(source.ProductSet);
            }
            view.UnitType = GetDicNameByCode(source.UnitTypeCode, dicList);
            view.Quantity = source.Quantity;
            view.ShelfName = GetShelfShowName(source.ShelfSet);
            view.LastOutQuantity = source.LastOutQuantity.HasValue ? source.LastOutQuantity.Value : 0;
            view.WarnQuantity = source.ItemType == 1 ? source.ElementSet.WarningQuantity : 0;
            view.PickQuantity = source.ElementId.HasValue ? source.ElementSet.PickQuantity : 0;
            return view;
        }


        public static IList<StockWarnListView> ConvertTo_StockWarnListViews(this IList<StockSet> source, IList<DicView> dicList)
        {
            var dest = new List<StockWarnListView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_StockWarnListView(dicList));
            }
            return dest;
        }




        public static StockInListView ConvertTo_StockInListView(this StockInSet source, IList<DicView> unitList, IStockService stockService)
        {
            var view = new StockInListView();
            if (source == null)
            {
                return view;
            }
            view.Addtime = source.Addtime.Value.ToStringWithtime();
            view.ItemName = source.ItemType == 1 ? GetElementShowName(source.ElementSet) : GetProductShowName(source.ProductSet);

            view.GroupGuid = source.GroupGuid.ToString();
            view.Id = source.Id;
            view.Quantity = source.Quantity;
            view.ShelfName = GetShelfShowName(source.ShelfSet);
            var unitCode = source.ItemType == 1 ? source.ElementSet.UnitTypeCode : source.ProductSet.UnitTypeCode;
            view.UnitName = GetDicNameByCode(unitCode, unitList);
            view.ItemTypeText = StringHelper.GetEnumDescription((ElementType)source.ItemType);
            int itemid = source.ElementId.HasValue ? source.ElementId.Value : source.ProductId.Value;
            bool isProduct = source.ElementId.HasValue ? false : true;
            view.StockQuantity = stockService.GetStockQuantity(itemid, source.ShelfId, isProduct);
            view.ShowWarn = GetShowWarn(view.StockQuantity, source.ElementSet);
            view.Reason = source.Reason;
            return view;
        }

        public static IList<StockInListView> ConvertTo_StockInListViews(this IList<StockInSet> source, IList<DicView> unitList, IStockService stockService)
        {
            var dest = new List<StockInListView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_StockInListView(unitList, stockService));
            }
            return dest;
        }


        public static StockOutListDetailView ConvertTo_StockOutDetailListView(this StockOutSet source, IList<DicView> unitList, IStockService stockService)
        {
            var view = new StockOutListDetailView();
            if (source == null)
            {
                return view;
            }
            view.Addtime = source.Addtime.Value.ToStringWithtime();
            view.ElementName = source.ElementSet == null ? GetProductShowName(source.ProductSet) : GetElementShowName(source.ElementSet);
            //   view.GroupGuid = source.GroupGuid.ToString();
            view.Id = source.Id;
            view.GroupGuid = source.GroupGuid.ToString();
            view.Quantity = source.Quantity;
            view.ShelfName = GetShelfShowName(source.ShelfSet);
            var unitCode = source.ElementSet == null ? source.ProductSet.UnitTypeCode : source.ElementSet.UnitTypeCode;
            view.UnitName = GetDicNameByCode(unitCode, unitList);
            view.BomName = source.BomSet == null ? "" : source.BomSet.Name;
            view.ItemTypeText = StringHelper.GetEnumDescription((ElementType)source.ItemType);
            int itemid = source.ElementId.HasValue ? source.ElementId.Value : source.ProductId.Value;
            bool isProduct = source.ElementId.HasValue ? false : true;
            view.StockQuantity = stockService.GetStockQuantity(itemid, source.ShelfId, isProduct);
            view.ShowWarn = GetShowWarn(view.StockQuantity, source.ElementSet);
            view.Reason = source.Reason;

            return view;
        }

        private static int GetShowWarn(string StockQuantity, ElementSet eleInfo)
        {
            if (eleInfo == null)
            {
                return 0;
            }
            if (StockQuantity == "未入库")
            {
                return 0;
            }
            var iStockQuantity = Convert.ToDouble(StockQuantity);
            return iStockQuantity <= eleInfo.WarningQuantity ? 1 : 0;
        }

        //public static IList<StockOutListView> ConvertTo_StockOutListViews(this IList<StockOutMainSet> source, IList<DicView> unitList, IStockService stockService)
        //{
        //    var dest = new List<StockOutListView>();
        //    StockOutListView info = null;
        //    foreach (var item in source)
        //    {
        //        info = new StockOutListView();
        //        info.Addtime = item.Addtime.Value.ToStringWithtime();
        //        info.Reason = item.Reason;
        //        info.Id = item.Id;
        //        info.Details = item.StockOutSet.ToList().ConvertTo_StockOutDetailListViews(unitList, stockService);
        //        dest.Add(info);
        //    }
        //    return dest;
        //}

        public static IList<StockOutListDetailView> ConvertTo_StockOutDetailListViews(this IList<StockOutSet> source, IList<DicView> unitList, IStockService stockService)
        {
            var dest = new List<StockOutListDetailView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_StockOutDetailListView(unitList, stockService));
            }
            return dest;
        }








        public static DicView ConvertTo_DicView(this DicSet source)
        {
            var view = new DicView();
            if (source == null)
            {
                return view;
            }
            view.Id = source.Id;
            view.Code = source.Code;
            view.Enabled = source.Enabled ? 1 : 0;
            view.Name = source.Name;
            view.ParentCode = source.ParentCode;

            return view;
        }

        public static IList<DicView> ConvertTo_ConvertTo_DicViews(this IEnumerable<DicSet> source)
        {
            var dest = new List<DicView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_DicView());
            }
            return dest;
        }

        public static StockDetailView ConvertTo_StockDetailView(this StockSet source, IList<DicView> dicList)
        {
            var view = new StockDetailView();
            if (source == null)
            {
                return view;
            }
            view.Addtime = source.Addtime.Value.ToStringWithtime();
            view.AddUserName = source.AddUserName;
            if (source.ElementId.HasValue && source.ElementId.Value > 0)
            {
                view.ElementName = GetElementShowName(source.ElementSet);
            }
            else
            {
                view.ElementName = GetProductShowName(source.ProductSet);
            }
            view.FirstInTime = source.FirstInTime.ToStringWithtime();
            view.ItemTypeText = StringHelper.GetEnumDescription((ElementType)source.ItemType);
            view.LastInQuantity = source.LastInQuantity.ToString();
            view.LastInTime = source.LastInTime.Value.ToStringWithtime();
            view.LastOutQuantity = source.LastOutQuantity.HasValue ? source.LastOutQuantity.Value.ToString() : "";
            view.LastOutTime = source.LastOutTime.HasValue ? source.LastOutTime.Value.ToStringWithtime() : "";
            view.Modifytime = source.Modifytime.HasValue ? source.Modifytime.Value.ToStringWithtime() : "";
            view.ModifyUserName = source.ModifyUserName;
            view.Quantity = source.Quantity.ToString();
            view.ShelfName = GetShelfShowName(source.ShelfSet);
            view.UnitType = GetDicNameByCode(source.UnitTypeCode, dicList);
            view.Remark = source.Remark;


            return view;
        }

        public static MenuSetView ConvertTo_MenuSetView(this MenuSet source, int sort, ICollection<RoleMenuOperSet> opers)
        {
            var view = new MenuSetView();
            if (source == null)
            {
                return view;
            }
            view.Code = source.Code;
            view.Icon = source.Icon;
            view.IconClass = source.IconClass;
            view.Id = source.Id;
            view.MenuLevel = source.MenuLevel;
            view.ParentCode = source.ParentCode;
            view.ShowInMenu = source.ShowInMenu;
            view.Sort = sort;
            view.Title = source.Title;
            view.Url = source.Url;
            IList<string> Opers = new List<string>();
            foreach (var item in opers)
            {
                Opers.Add(item.OperSet.Code.ToLower());
            }
            view.Opers = Opers;
            return view;
        }


        #region 原材料转换方法

        public static ElementListView ConvertTo_ElementListView(this ElementSet source, IList<DicView> dicList)
        {
            var view = new ElementListView();
            if (source == null)
            {
                return view;
            }

            view.Addtime = source.Addtime.Value.ToStringWithtime();
            view.Name = source.Name;
            view.Code = source.Code;
            view.Id = source.Id;
            view.PriceFormat = source.Price.HasValue ? string.Format("{0:C2}", source.Price.Value) : "";
            view.ShelfName = GetShelfShowName(source.ShelfSet);
            view.UnitName = GetDicNameByCode(source.UnitTypeCode, dicList);
            view.UnitTypeCode = source.UnitTypeCode;
            view.ShelfId = source.ShelfId;
            view.UnitTypeCode = source.UnitTypeCode;
            view.Price = source.Price.ToString();
            view.WarningQuantity = source.WarningQuantity;
            view.Remark = source.Remark;
            string num = "0";
            if (source.StockSet.Count == 0)
            {
                num = "未入库";
            }
            else
            {
                double inum = 0;
                foreach (var item in source.StockSet)
                {
                    inum += item.Quantity;
                }
                num = inum.ToString();
            }
            view.StockQuantity = num;
            return view;
        }


        public static IList<ElementListView> ConvertTo_ElementListViews(this IEnumerable<ElementSet> source, IList<DicView> dicList)
        {
            var dest = new List<ElementListView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_ElementListView(dicList));
            }
            return dest;
        }


        public static ElementSet ConvertTo_ElementSet_ForCreate(this Add_Element_Request source)
        {
            var view = new ElementSet();
            if (source == null)
            {
                return view;
            }

            view.Addtime = DateTime.Now;
            view.AddUserName = HttpContext.Current.User.Identity.Name;
            view.Code = source.Code;
            view.Name = source.Name;
            if (!string.IsNullOrEmpty(source.Price))
            {
                view.Price = Convert.ToDecimal(source.Price);
            }
            view.ShelfId = source.ShelfId;
            view.UnitTypeCode = source.UnitTypeCode;
            view.WarningQuantity = source.WarningQuantity;
            view.Remark = source.Remark;

            return view;
        }

        public static ElementSet ConvertTo_ElementSet_ForEdit(this Edit_Element_Request source, ElementSet oldInfo)
        {
            if (oldInfo == null)
            {
                return null;
            }

            oldInfo.Modifytime = DateTime.Now;
            oldInfo.ModifyUserName = HttpContext.Current.User.Identity.Name;
            oldInfo.Code = source.Code;
            oldInfo.Name = source.Name;
            if (!string.IsNullOrEmpty(source.Price))
            {
                oldInfo.Price = Convert.ToDecimal(source.Price);
            }
            oldInfo.ShelfId = source.ShelfId;
            oldInfo.UnitTypeCode = source.UnitTypeCode;
            oldInfo.WarningQuantity = source.WarningQuantity;
            oldInfo.Remark = source.Remark;
            return oldInfo;
        }

        #endregion

        #region 型号转换方法
        public static ModelListView ConvertTo_ModelListView(this ModelSet source)
        {
            var view = new ModelListView();
            if (source == null)
            {
                return view;
            }

            view.Code = source.Code;
            view.Id = source.Id;
            view.Name = source.Name;
            view.Remark = source.Remark;
            view.Addtime = source.Addtime.Value.ToStringWithtime();
            return view;
        }

        public static IList<ModelListView> ConvertTo_ModelListViews(this IEnumerable<ModelSet> source)
        {
            var dest = new List<ModelListView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_ModelListView());
            }
            return dest;
        }

        public static ModelSet ConvertTo_ModelSet_ForCreate(this Add_Model_Request source)
        {
            var view = new ModelSet();
            if (source == null)
            {
                return view;
            }

            view.Addtime = DateTime.Now;
            view.AddUserName = HttpContext.Current.User.Identity.Name;
            view.Code = source.Code;
            view.Name = source.Name;
            view.Remark = source.Remark;

            return view;
        }

        public static ModelSet ConvertTo_ModelSet_ForEdit(this Edit_Model_Request source, ModelSet oldInfo)
        {
            if (oldInfo == null)
            {
                return null;
            }

            oldInfo.Modifytime = DateTime.Now;
            oldInfo.ModifyUserName = HttpContext.Current.User.Identity.Name;
            oldInfo.Code = source.Code;
            oldInfo.Name = source.Name;
            oldInfo.Remark = source.Remark;

            return oldInfo;
        }

        #endregion

        #region 部件转换方法
        public static PartListView ConvertTo_PartListView(this PartSet source)
        {
            var view = new PartListView();
            if (source == null)
            {
                return view;
            }

            view.Code = source.Code;
            view.Id = source.Id;
            view.Name = source.Name;
            view.Remark = source.Remark;
            view.Addtime = source.Addtime.Value.ToStringWithtime();
            return view;
        }

        public static IList<PartListView> ConvertTo_PartListViews(this IEnumerable<PartSet> source)
        {
            var dest = new List<PartListView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_PartListView());
            }
            return dest;
        }

        public static PartSet ConvertTo_PartSet_ForCreate(this Add_Part_Request source)
        {
            var view = new PartSet();
            if (source == null)
            {
                return view;
            }

            view.Addtime = DateTime.Now;
            view.AddUserName = HttpContext.Current.User.Identity.Name;
            view.Code = source.Code;
            view.Name = source.Name;
            view.Remark = source.Remark;

            return view;
        }

        public static PartSet ConvertTo_PartSet_ForEdit(this Edit_Part_Request source, PartSet oldInfo)
        {
            if (oldInfo == null)
            {
                return null;
            }

            oldInfo.Modifytime = DateTime.Now;
            oldInfo.ModifyUserName = HttpContext.Current.User.Identity.Name;
            oldInfo.Code = source.Code;
            oldInfo.Name = source.Name;
            oldInfo.Remark = source.Remark;

            return oldInfo;
        }


        public static PartDetailEditView ConvertTo_PartDetailEditView(this PartDetailSet source)
        {
            var view = new PartDetailEditView();
            if (source == null)
            {
                return view;
            }

            view.ElementId = source.ElementId;
            view.Id = source.Id;
            view.PartId = source.PartId;
            view.Quantity = source.Quantity;
            view.ElementName = GetElementShowName(source.ElementSet);
            view.IsSelect = 1;
            return view;
        }

        public static IList<PartDetailEditView> ConvertTo_PartDetailEditViews(this IEnumerable<PartDetailSet> source)
        {
            var dest = new List<PartDetailEditView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_PartDetailEditView());
            }
            return dest;
        }

        #endregion


        #region Bom转换方法
        public static BomListView ConvertTo_BomListView(this BomSet source)
        {
            var view = new BomListView();
            if (source == null)
            {
                return view;
            }

            view.ModelName = GetModelShowName(source.ModelSet);
            view.ModelId = source.ModelId;
            view.Id = source.Id;
            view.Name = source.Name;
            view.Remark = source.Remark;
            view.Addtime = source.Addtime.Value.ToStringWithtime();
            return view;
        }

        public static IList<BomListView> ConvertTo_BomListViews(this IEnumerable<BomSet> source)
        {
            var dest = new List<BomListView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_BomListView());
            }
            return dest;
        }

        public static BomSet ConvertTo_BomSet_ForCreate(this Add_Bom_Request source)
        {
            var view = new BomSet();
            if (source == null)
            {
                return view;
            }

            view.Addtime = DateTime.Now;
            view.AddUserName = HttpContext.Current.User.Identity.Name;
            view.ModelId = source.ModelId;
            view.Name = source.Name;
            view.Remark = source.Remark;

            return view;
        }

        public static BomSet ConvertTo_BomSet_ForEdit(this Edit_Bom_Request source, BomSet oldInfo)
        {
            if (oldInfo == null)
            {
                return null;
            }

            oldInfo.Modifytime = DateTime.Now;
            oldInfo.ModifyUserName = HttpContext.Current.User.Identity.Name;
            oldInfo.ModelId = source.ModelId;
            oldInfo.Name = source.Name;
            oldInfo.Remark = source.Remark;

            return oldInfo;
        }


        public static BomDetailEditView ConvertTo_BomDetailEditView(this BomDetailSet source, IList<PartCacheView> partList, IList<DicView> dicList)
        {
            var view = new BomDetailEditView();
            if (source == null)
            {
                return view;
            }

            view.ElementId = source.ElementId;
            view.ElementName = GetElementShowName(source.ElementSet);
            view.Id = source.Id;
            //   view.BomId = source.BomId;
            view.Quantity = source.Quantity;
            view.Addtime = source.Addtime.Value.ToStringWithtime();
            view.PartCode = source.PartCode;
            view.PartName = GetPartNameByPartList(partList, source.PartCode);
            view.Remark = source.Remark;
            // view.UnitTypeCode = source.ElementSet.UnitTypeCode;
            view.UnitName = GetDicNameByCode(source.ElementSet.UnitTypeCode, dicList);
            view.ShelfName = GetShelfShowName(source.ElementSet.ShelfSet);
            view.IsSelect = 1;

            return view;
        }


        public static IList<BomDetailEditView> ConvertTo_BomDetailEditViews(this IEnumerable<BomDetailSet> source, IList<PartCacheView> partList, IList<DicView> dicList)
        {
            var dest = new List<BomDetailEditView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_BomDetailEditView(partList, dicList));
            }
            return dest;
        }


        public static Export_Bom_View BomDetailSet_To_Export_Bom_View(this BomDetailSet source, IList<PartCacheView> partList, IList<DicView> dicList)
        {
            var view = new Export_Bom_View();
            if (source == null)
            {
                return view;
            }
            view.BomName = source.BomSet.ModelSet.Name;
            view.ElementName = GetElementShowName(source.ElementSet);
            view.Quantity = source.Quantity;
            view.Addtime = source.Addtime.Value.ToStringWithtime();
            view.PartName = GetPartNameByPartList(partList, source.PartCode);
            view.Remark = source.Remark;
            view.UnitName = GetDicNameByCode(source.ElementSet.UnitTypeCode, dicList);
            view.ShelfName = GetShelfShowName(source.ElementSet.ShelfSet);

            return view;
        }

        public static IList<Export_Bom_View> BomDetailSet_To_Export_Bom_Views(this IEnumerable<BomDetailSet> source, IList<PartCacheView> partList, IList<DicView> dicList)
        {
            var dest = new List<Export_Bom_View>();
            foreach (var item in source)
            {
                dest.Add(item.BomDetailSet_To_Export_Bom_View(partList, dicList));
            }
            return dest;
        }


        public static BomDetailEditView ConvertTo_BomDetailEditView(this PartDetailSet source, PartSet partInfo, IList<DicView> dicList)
        {
            var view = new BomDetailEditView();
            if (source == null)
            {
                return view;
            }

            view.ElementId = source.ElementId;
            view.ElementName = GetElementShowName(source.ElementSet);
            view.Id = source.Id;
            //   view.BomId = source.BomId;
            view.Quantity = source.Quantity;
            //  view.Addtime = source.Addtime.Value.ToStringWithtime();
            view.PartCode = partInfo.Code;
            view.PartName = partInfo.Name;
            view.Remark = source.Remark;
            // view.UnitTypeCode = source.ElementSet.UnitTypeCode;
            view.UnitName = GetDicNameByCode(source.ElementSet.UnitTypeCode, dicList);
            view.ShelfName = GetShelfShowName(source.ElementSet.ShelfSet);
            view.IsSelect = 1;

            return view;
        }

        public static IList<BomDetailEditView> ConvertTo_BomDetailEditViews(this IEnumerable<PartDetailSet> source, PartSet partInfo, IList<DicView> dicList)
        {
            var dest = new List<BomDetailEditView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_BomDetailEditView(partInfo, dicList));
            }
            return dest;
        }

        public static BomDetailEditView ConvertTo_BomDetailEditView(this ElementSet source, IList<DicView> dicList)
        {
            var view = new BomDetailEditView();
            if (source == null)
            {
                return view;
            }

            view.ElementId = source.Id;
            view.ElementName = GetElementShowName(source);
            view.Id = source.Id;
            view.Quantity = 0;
            view.PartCode = PartCode.partOther.ToString();
            view.PartName = "其他";
            view.Remark = "";
            view.UnitName = GetDicNameByCode(source.UnitTypeCode, dicList);
            view.ShelfName = GetShelfShowName(source.ShelfSet);
            view.IsSelect = 1;
            return view;
        }


        public static StockOut_ForAdd_View ConvertTo_StockOut_ForAdd_View(this ElementSet source, IList<DicView> dicList, IStockService stockService)
        {
            var view = new StockOut_ForAdd_View();
            if (source == null)
            {
                return view;
            }

            view.ElementId = source.Id;
            view.ElementName = GetElementShowName(source);
            view.Quantity = 0;
            view.BomId = 0;
            view.BomName = "";


            view.Remark = "";
            view.ShelfId = source.ShelfId;
            view.ShelfName = GetShelfShowName(source.ShelfSet);
            view.UnitName = GetDicNameByCode(source.UnitTypeCode, dicList);
            view.PartName = "其他";
            view.StockQuantity = stockService.GetStockQuantity(view.ElementId, view.ShelfId);
            view.ItemType = (int)ElementType.Element;
            view.IsSelect = 1;
            if (view.StockQuantity == "未入库")
            {
                view.IsSelect = 0;
            }



            return view;
        }


        public static StockOut_ForAdd_View ConvertTo_StockOut_ForAdd_View(this BomDetailSet source, IList<DicView> dicList, IList<PartCacheView> partList, IStockService stockService, int num)
        {
            var view = new StockOut_ForAdd_View();
            if (source == null)
            {
                return view;
            }
            var eleInfo = source.ElementSet;
            view.ElementId = eleInfo.Id;
            view.ElementName = GetElementShowName(eleInfo);
            if (num < 1)
            {
                num = 1;
            }
            view.Quantity = source.Quantity * num;
            view.BomId = source.BomId;
            view.BomName = source.BomSet.Name;


            view.Remark = source.Remark;
            view.ShelfId = eleInfo.ShelfId;
            view.ShelfName = GetShelfShowName(eleInfo.ShelfSet);
            view.UnitName = GetDicNameByCode(eleInfo.UnitTypeCode, dicList);
            view.PartName = GetPartNameByPartList(partList, source.PartCode);
            view.StockQuantity = stockService.GetStockQuantity(view.ElementId, view.ShelfId);
            view.ItemType = (int)ElementType.Element;
            view.IsSelect = 1;
            if (view.StockQuantity == "未入库")
            {
                view.IsSelect = 0;
            }
            else if (view.Quantity > Convert.ToDouble(view.StockQuantity))
            {
                view.IsSelect = 0;
            }


            return view;
        }

        public static IList<StockOut_ForAdd_View> ConvertTo_StockOut_ForAdd_Views(this IEnumerable<BomDetailSet> source, IList<DicView> dicList, IList<PartCacheView> partList, IStockService stockService, int num)
        {
            var dest = new List<StockOut_ForAdd_View>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_StockOut_ForAdd_View(dicList, partList, stockService, num));
            }
            return dest;
        }




        #endregion



        #region 字典转换方法
        // IList<DicTreeView>

        public static DicChildrenView ConvertTo_DicChildrenView(this DicSet source)
        {
            var view = new DicChildrenView();
            if (source == null)
            {
                return view;
            }

            view.Code = source.Code;
            view.Name = source.Name;
            view.Enabled = source.Enabled ? 1 : 0;
            view.Id = source.Id;
            view.Remark = source.Remark;
            view.sort = source.sort;
            view.ParentCode = source.ParentCode;
            return view;
        }

        public static IList<DicChildrenView> ConvertTo_DicChildrenViews(this IEnumerable<DicSet> source)
        {
            var dest = new List<DicChildrenView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_DicChildrenView());
            }
            return dest;
        }


        public static DicSet ConvertTo_DicSet_ForCreate(this Add_Dic_Request source)
        {
            var info = new DicSet();
            if (source == null)
            {
                return info;
            }

            info.Addtime = DateTime.Now;
            info.AddUserName = HttpContext.Current.User.Identity.Name;
            info.Code = source.Code;
            info.Enabled = source.Enabled == 1 ? true : false;
            info.Name = source.Name;
            info.ParentCode = string.IsNullOrEmpty(source.ParentCode) ? null : source.ParentCode;
            info.Remark = source.Remark;
            info.sort = source.sort;

            return info;
        }


        public static DicSet ConvertTo_DicSet_ForEdit(this Edit_Dic_Request source, DicSet oldInfo)
        {
            if (oldInfo == null)
            {
                return null;
            }

            oldInfo.Modifytime = DateTime.Now;
            oldInfo.ModifyUserName = HttpContext.Current.User.Identity.Name;
            oldInfo.Code = source.Code;
            oldInfo.Enabled = source.Enabled == 1 ? true : false;
            oldInfo.Name = source.Name;
            oldInfo.ParentCode = string.IsNullOrEmpty(source.ParentCode) ? null : source.ParentCode;
            oldInfo.Remark = source.Remark;
            oldInfo.sort = source.sort;


            return oldInfo;
        }

        #endregion


        #region 库位转换方法
        public static ShelfListView ConvertTo_ShelfListView(this ShelfSet source)
        {
            var view = new ShelfListView();
            if (source == null)
            {
                return view;
            }

            view.Code = source.Code;
            view.Id = source.Id;
            view.Remark = source.Remark;
            view.Addtime = source.Addtime.Value.ToStringWithtime();
            return view;
        }

        public static IList<ShelfListView> ConvertTo_ShelfListViews(this IEnumerable<ShelfSet> source)
        {
            var dest = new List<ShelfListView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_ShelfListView());
            }
            return dest;
        }

        public static ShelfSet ConvertTo_ShelfSet_ForCreate(this Add_Shelf_Request source)
        {
            var view = new ShelfSet();
            if (source == null)
            {
                return view;
            }

            view.Addtime = DateTime.Now;
            view.AddUserName = HttpContext.Current.User.Identity.Name;
            view.Code = source.Code;
            view.Name = source.Code;
            view.Remark = source.Remark;

            return view;
        }

        public static ShelfSet ConvertTo_ShelfSet_ForEdit(this Edit_Shelf_Request source, ShelfSet oldInfo)
        {
            if (oldInfo == null)
            {
                return null;
            }

            oldInfo.Modifytime = DateTime.Now;
            oldInfo.ModifyUserName = HttpContext.Current.User.Identity.Name;
            oldInfo.Code = source.Code;
            oldInfo.Name = source.Code;
            oldInfo.Remark = source.Remark;

            return oldInfo;
        }

        #endregion



        #region 产品转换方法
        public static ProductListView ConvertTo_ProductListView(this ProductSet source)
        {
            var view = new ProductListView();
            if (source == null)
            {
                return view;
            }
            view.Aliases = source.Aliases;
            view.CreateDate = source.CreateDate.ToStringWithtime();
            view.Id = source.Id;
            view.ModelName = GetModelShowName(source.ModelSet);
            view.Price = MoneyToString(source.Price);
            view.ItemType = source.ItemType;
            view.ItemTypeText = StringHelper.GetEnumDescription((ElementType)source.ItemType);
            view.ModelId = source.ModelId;
            view.UnitTypeCode = source.UnitTypeCode;
            view.Remark = source.Remark;
            return view;
        }

        public static IList<ProductListView> ConvertTo_ProductListViews(this IEnumerable<ProductSet> source)
        {
            var dest = new List<ProductListView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_ProductListView());
            }
            return dest;
        }


        public static ProductDetailListView ConvertTo_ProductDetailListView(this ProductDetailSet source, IList<DicView> unitList)
        {
            var view = new ProductDetailListView();
            if (source == null)
            {
                return view;
            }
            view.ElementId = source.ElementId;
            view.ElementName = GetElementShowName(source.ElementSet);
            view.Quantity = source.Quantity;
            view.UnitName = GetDicNameByCode(source.ElementSet.UnitTypeCode, unitList);
            if (source.BomId.HasValue)
            {
                view.ItemName = "来自BOM，Bom：" + source.BomSet.Name;
            }
            else if (source.HalfProductId.HasValue)
            {
                view.ItemName = "来自半成品,半成品：" + source.ProductSet1.Aliases + "【" + source.ProductSet1.ModelSet.Name + "】";
            }
            else
            {
                view.ItemName = "原材料";
            }
            view.BomId = source.BomId.HasValue ? source.BomId.Value : 0;
            view.HalfProductId = source.HalfProductId.HasValue ? source.HalfProductId.Value : 0;
            view.IsSelect = 1;
            return view;
        }

        public static IList<ProductDetailListView> ConvertTo_ProductDetailListViews(this IEnumerable<ProductDetailSet> source, IList<DicView> unitList)
        {
            var dest = new List<ProductDetailListView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_ProductDetailListView(unitList));
            }
            return dest;
        }


        //public static StockOut_ForAdd_ByProductView ConvertTo_StockOut_ForAdd_ByProductView(this ProductSet source, IList<DicView> dicList, IStockService stockService)
        //{
        //    var view = new StockOut_ForAdd_ByProductView();
        //    if (source == null)
        //    {
        //        return view;
        //    }

        //    view.ProductId = source.Id;
        //    view.ProductName = GetProductShowName(source);
        //    view.Quantity = 1;
        //    view.Remark = "";
        //    view.ShelfId = source.ShelfId.Value;
        //    view.ShelfName = GetShelfShowName(source.ShelfSet);
        //    view.UnitName = GetDicNameByCode(source.UnitTypeCode, dicList);
        //    view.StockQuantity = stockService.GetStockQuantity(view.ProductId, view.ShelfId,true);
        //    view.IsSelect = 1;
        //    if (view.StockQuantity == "未入库")
        //    {
        //        view.IsSelect = 0;
        //    }

        //    return view;
        //}

        public static StockOut_ForAdd_ByProductView ConvertTo_StockOut_ForAdd_ByProductView(this StockSet source, IList<DicView> dicList)
        {
            var view = new StockOut_ForAdd_ByProductView();
            if (source == null)
            {
                return view;
            }

            view.ProductId = source.ProductId.Value;
            view.ProductName = GetProductShowName(source.ProductSet);
            view.Quantity = 1;
            view.Remark = source.ProductSet.Remark;
            view.ShelfId = source.ShelfId;
            view.ShelfName = GetShelfShowName(source.ShelfSet);
            view.UnitName = GetDicNameByCode(source.UnitTypeCode, dicList);
            view.StockQuantity = source.Quantity.ToString();
            view.ItemType = source.ItemType;
            view.IsSelect = source.Quantity <= 0 ? 0 : 1;

            return view;
        }

        public static StockIn_ForAdd_ByProductView ConvertTo_StockIn_ForAdd_ByProductView(this ProductSet source, IList<DicView> dicList)
        {
            var view = new StockIn_ForAdd_ByProductView();
            if (source == null)
            {
                return view;
            }

            view.ProductId = source.Id;
            view.ProductName = GetProductShowName(source);
            view.Quantity = 1;
            view.Remark = source.Remark;
            view.ShelfId = 0;
            view.UnitName = GetDicNameByCode(source.UnitTypeCode, dicList);

            view.ItemType = source.ItemType;
            view.IsSelect = 1;
            view.ItemTypeText = StringHelper.GetEnumDescription((ElementType)source.ItemType);
            view.UnitTypeCode = source.UnitTypeCode;

            return view;
        }



        public static ProductSet ConvertTo_ProductSet_ForCreate(this Add_Product_Request source)
        {
            var view = new ProductSet();
            if (source == null)
            {
                return view;
            }

            view.Addtime = DateTime.Now;
            view.AddUserName = HttpContext.Current.User.Identity.Name;
            view.ModelId = source.ModelId;
            view.Aliases = source.Aliases;
            if (!string.IsNullOrEmpty(source.Price))
            {
                view.Price = Convert.ToDecimal(source.Price);
            }
            view.ItemType = source.ItemType;
            view.UnitTypeCode = source.UnitTypeCode;
            if (!string.IsNullOrEmpty(source.CreateDate))
            {
                view.CreateDate = Convert.ToDateTime(source.CreateDate);
            }
            view.Remark = source.Remark;

            return view;
        }


        public static ProductSet ConvertTo_ProductSet_ForEdit(this Edit_Product_Request source, ProductSet oldInfo)
        {
            if (oldInfo == null)
            {
                return null;
            }

            oldInfo.Modifytime = DateTime.Now;
            oldInfo.ModifyUserName = HttpContext.Current.User.Identity.Name;
            oldInfo.ModelId = source.ModelId;
            oldInfo.Aliases = source.Aliases;
            if (!string.IsNullOrEmpty(source.CreateDate))
            {
                oldInfo.CreateDate = Convert.ToDateTime(source.CreateDate);
            }
            if (!string.IsNullOrEmpty(source.Price))
            {
                oldInfo.Price = Convert.ToDecimal(source.Price);
            }
            oldInfo.ItemType = source.ItemType;
            oldInfo.UnitTypeCode = source.UnitTypeCode;
            oldInfo.Remark = source.Remark;
            return oldInfo;
        }


        public static ProductDetailListView ConvertTo_ProductDetailListView(this BomDetailSet source, IList<DicView> dicList)
        {
            var view = new ProductDetailListView();
            if (source == null)
            {
                return view;
            }
            var eleInfo = source.ElementSet;
            view.ElementId = eleInfo.Id;
            view.ElementName = GetElementShowName(eleInfo);
            view.Quantity = source.Quantity;
            view.BomId = source.BomId;
            view.ItemName = "Bom：" + source.BomSet.Name;
            view.UnitName = GetDicNameByCode(eleInfo.UnitTypeCode, dicList);
            view.IsSelect = 1;


            return view;
        }

        public static IList<ProductDetailListView> ConvertTo_ProductDetailListViews(this IEnumerable<BomDetailSet> source, IList<DicView> dicList)
        {
            var dest = new List<ProductDetailListView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_ProductDetailListView(dicList));
            }
            return dest;
        }


        public static ProductDetailListView ConvertTo_ProductDetailListView(this ElementSet source, IList<DicView> dicList)
        {
            var view = new ProductDetailListView();
            if (source == null)
            {
                return view;
            }

            view.ElementId = source.Id;
            view.ElementName = GetElementShowName(source);
            view.Quantity = 0;
            view.BomId = 0;
            view.UnitName = GetDicNameByCode(source.UnitTypeCode, dicList);
            view.ItemName = "原材料";
            view.IsSelect = 1;




            return view;
        }


        #endregion


        #region 用户及权限转换方法
        public static IList<OperView> ConvertTo_OperViews(this MenuSet source, IEnumerable<RoleMenuSet> roleMenus)
        {
            var dest = new List<OperView>();
            OperView info = null;
            var MenuOperSetList = source.MenuOperSet.OrderBy(d => d.OperId);
            foreach (var item in MenuOperSetList)
            {
                info = new OperView();
                info.Id = item.OperId;
                info.Name = item.OperSet.Name;
                var roleMenuSetInfo = roleMenus.FirstOrDefault(d => d.MenuId == item.MenuId);
                if (roleMenuSetInfo != null)
                {
                    info.@checked = roleMenuSetInfo.RoleMenuOperSet.Any(d => d.OperId == item.OperId);
                }
                dest.Add(info);

            }
            return dest;
        }

        public static RoleMenuTreeView ConvertTo_RoleMenuTreeView(this MenuSet source, IEnumerable<RoleMenuSet> roleMenus)
        {
            var view = new RoleMenuTreeView();
            if (source == null)
            {
                return view;
            }
            view.children = null;
            view.id = source.Id;
            view.text = source.Title;
            view.OperViews = source.ConvertTo_OperViews(roleMenus);
            if (roleMenus.Any(d => d.MenuId == source.Id))
            {

                view.@checked = true;
            }
            return view;
        }

        public static IList<RoleMenuTreeView> ConvertTo_RoleMenuTreeViews(this IEnumerable<MenuSet> source, IEnumerable<RoleMenuSet> roleMenus)
        {
            var dest = new List<RoleMenuTreeView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_RoleMenuTreeView(roleMenus));
            }
            return dest;
        }


        #endregion

        #region 领料转换方法

        public static PickListView ConvertTo_PickListView(this PickSet source, IList<DicView> unitList, IList<PartCacheView> partList, IList<UserCacheView> userList)
        {
            var view = new PickListView();
            if (source == null)
            {
                return view;
            }
            view.Addtime = source.Addtime.Value.ToStringWithtime();
            view.AddUserName = GetUserTrueName(source.AddUserName, userList);
            view.BomName = source.BomId.HasValue ? source.BomSet.Name : "";
            view.ElementName = GetElementShowName(source.ElementSet);
            view.ParentId = source.ParentId;
            view.Id = source.Id;
            view.IsFeedback = source.IsFeedback ? "是" : "否";
            view.PartName = GetPartNameByPartList(partList, source.PartCode);
            view.Purpose = source.Purpose;
            view.Quantity = source.Quantity;
            view.StockOutQuantity = source.StockOutQuantity;
            view.StockOutTime = source.StockOutTime.HasValue ? source.StockOutTime.Value.ToStringWithtime() : "";
            view.StockOutUserName = GetUserTrueName(source.StockOutUserName, userList);

            return view;
        }

        public static IList<PickListView> ConvertTo_PickListViews(this IList<PickSet> source, IList<DicView> unitList, IList<PartCacheView> partList, IList<UserCacheView> userList)
        {
            var dest = new List<PickListView>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_PickListView(unitList, partList, userList));
            }
            return dest;
        }


        public static PickDetailView ConvertTo_PickDetailView(this PickSet source, IList<DicView> unitList,
            IList<PartCacheView> partList, IList<UserCacheView> userList, IStockService stockService)
        {
            var view = new PickDetailView();
            if (source == null)
            {
                return view;
            }
            view.Addtime = source.Addtime.Value.ToStringWithtime();
            view.AddUserName = GetUserTrueName(source.AddUserName, userList);
            view.BomName = source.BomId.HasValue ? source.BomSet.Name : "";
            view.ElementName = GetElementShowName(source.ElementSet);

            view.IsFeedback = source.IsFeedback ? "是" : "否";
            view.PartName = GetPartNameByPartList(partList, source.PartCode);
            view.Purpose = source.Purpose;
            view.Quantity = source.Quantity;
            view.StockOutQuantity = source.StockOutQuantity;
            view.StockOutTime = source.StockOutTime.HasValue ? source.StockOutTime.Value.ToStringWithtime() : "";
            view.StockOutUserName = GetUserTrueName(source.StockOutUserName, userList);
            view.UnitName = GetDicNameByCode(source.ElementSet.UnitTypeCode, unitList);
            view.StockQuantity = stockService.GetStockQuantity(source.ElementId, source.ElementSet.ShelfId);
            return view;
        }

        public static Pick_ForAdd_View ConvertTo_Pick_ForAdd_View(this BomDetailSet source, IList<DicView> dicList, IList<PartCacheView> partList, IStockService stockService, int num)
        {
            var view = new Pick_ForAdd_View();
            if (source == null)
            {
                return view;
            }
            var eleInfo = source.ElementSet;
            view.ElementId = eleInfo.Id;
            view.ElementName = GetElementShowName(eleInfo);
            if (num < 1)
            {
                num = 1;
            }
            view.Quantity = source.Quantity * num;
            view.BomId = source.BomId;
            view.BomName = source.BomSet.Name;
            view.UnitName = GetDicNameByCode(eleInfo.UnitTypeCode, dicList);
            view.PartName = GetPartNameByPartList(partList, source.PartCode);
            view.StockQuantity = stockService.GetStockQuantity(view.ElementId, eleInfo.ShelfId);
            view.IsSelect = 1;
            view.PartCode = source.PartCode;
            return view;
        }


        public static IList<Pick_ForAdd_View> ConvertTo_Pick_ForAdd_Views(this IEnumerable<BomDetailSet> source, IList<DicView> dicList, IList<PartCacheView> partList, IStockService stockService, int num)
        {
            var dest = new List<Pick_ForAdd_View>();
            foreach (var item in source)
            {
                dest.Add(item.ConvertTo_Pick_ForAdd_View(dicList, partList, stockService, num));
            }
            return dest;
        }


        public static Pick_ForAdd_View PartDetailSet_To_Pick_ForAdd_View(this PartDetailSet source, IList<DicView> dicList, IList<PartCacheView> partList, IStockService stockService, int num)
        {
            var view = new Pick_ForAdd_View();
            if (source == null)
            {
                return view;
            }
            var eleInfo = source.ElementSet;
            view.ElementId = eleInfo.Id;
            view.ElementName = GetElementShowName(eleInfo);
            if (num < 1)
            {
                num = 1;
            }
            view.Quantity = source.Quantity * num;
            view.BomId = 0;
            view.BomName = "";
            view.UnitName = GetDicNameByCode(eleInfo.UnitTypeCode, dicList);
            view.PartCode = source.PartSet.Code;
            view.PartName = GetPartNameByPartList(partList, view.PartCode);
            view.StockQuantity = stockService.GetStockQuantity(view.ElementId, eleInfo.ShelfId);
            view.IsSelect = 1;

            return view;
        }

        public static IList<Pick_ForAdd_View> PartDetailSet_To_Pick_ForAdd_Views(this IEnumerable<PartDetailSet> source, IList<DicView> dicList, IList<PartCacheView> partList, IStockService stockService, int num)
        {
            var dest = new List<Pick_ForAdd_View>();
            foreach (var item in source)
            {
                dest.Add(item.PartDetailSet_To_Pick_ForAdd_View(dicList, partList, stockService, num));
            }
            return dest;
        }

        public static Pick_ForAdd_View ElementSet_To_Pick_ForAdd_View(this ElementSet source, IList<DicView> dicList, IStockService stockService)
        {
            var view = new Pick_ForAdd_View();
            if (source == null)
            {
                return view;
            }

            view.ElementId = source.Id;
            view.ElementName = GetElementShowName(source);
            view.Quantity = 0;
            view.BomId = 0;
            view.BomName = "";
            view.UnitName = GetDicNameByCode(source.UnitTypeCode, dicList);
            view.PartCode = "";
            view.PartName = "";
            view.StockQuantity = stockService.GetStockQuantity(view.ElementId, source.ShelfId);
            view.IsSelect = 1;
            return view;
        }



        public static PickOutListView PickMainSet_To_PickOutListView(this PickMainSet source, IList<UserCacheView> userList)
        {
            var view = new PickOutListView();
            if (source == null)
            {
                return view;
            }
            view.Addtime = source.Addtime.Value.ToStringWithtime();
            view.AddUserName = GetUserTrueName(source.AddUserName, userList);
            view.Id = source.Id;
            view.IsFeedback = source.IsFeedback ? "是" : "否";
            view.Purpose = source.Purpose;
            view.StockOutTime = source.StockOutTime.HasValue ? source.StockOutTime.Value.ToStringWithtime() : "";
            view.StockOutUserName = GetUserTrueName(source.StockOutUserName, userList);

            return view;
        }

        public static IList<PickOutListView> PickMainSet_To_PickOutListViews(this IEnumerable<PickMainSet> source, IList<UserCacheView> userList)
        {
            var dest = new List<PickOutListView>();
            foreach (var item in source)
            {
                dest.Add(item.PickMainSet_To_PickOutListView(userList));
            }
            return dest;
        }


        public static PickOut_ForAdd_View PickSet_To_PickOut_ForAdd_View(this PickSet source, IList<DicView> dicList, IList<PartCacheView> partList, IStockService stockService)
        {
            var view = new PickOut_ForAdd_View();
            if (source == null)
            {
                return view;
            }
            var eleInfo = source.ElementSet;
            view.ElementName = GetElementShowName(eleInfo);
            view.Quantity = source.Quantity;
            view.BomName = source.BomId.HasValue ? source.BomSet.Name : "";
            view.UnitName = GetDicNameByCode(eleInfo.UnitTypeCode, dicList);
            view.PartName = GetPartNameByPartList(partList, source.PartCode);
            view.StockQuantity = stockService.GetStockQuantity(source.ElementId, eleInfo.ShelfId);
            view.IsSelect = 1;
            view.StockOutQuantity = source.Quantity - source.StockOutQuantity;
            view.Id = source.Id;
            view.ShelfName = GetShelfShowName(eleInfo.ShelfSet);
            view.ALStockOutQuantity = source.StockOutQuantity;
            view.ElementId = source.ElementId;
            view.BomId = source.BomId.HasValue ? source.BomId.Value : 0;
            view.Addtime = source.Addtime.Value.ToStringWithtime();
            view.AddUserName = source.AddUserName;
            view.ShelfId = eleInfo.ShelfId;

            if (view.StockQuantity == "未入库" || Convert.ToDouble(view.StockQuantity) <= 0 || view.StockOutQuantity <= 0)
            {
                view.IsSelect = 0;
            }

            return view;
        }

        public static IList<PickOut_ForAdd_View> PickSet_To_PickOut_ForAdd_Views(this IEnumerable<PickSet> source, IList<DicView> dicList, IList<PartCacheView> partList, IStockService stockService)
        {
            var dest = new List<PickOut_ForAdd_View>();
            foreach (var item in source)
            {
                dest.Add(item.PickSet_To_PickOut_ForAdd_View(dicList, partList, stockService));
            }
            return dest;
        }

        public static SaleReportView SaleReportSet_To_SaleReportView(this SaleReportSet source)
        {
            var view = new SaleReportView();
            if (source == null)
            {
                return view;
            }
            view = Mapper.Map<SaleReportSet, SaleReportView>(source);
            view.SaleDate = source.SaleDate.ToStringWithDate();
            view.PayDate = source.PayDate;
            view.TaxPrice = source.TaxPrice.FormatMoney();
            view.AllTaxPrice = source.AllTaxPrice.FormatMoney();
            view.LogisticsCost = source.LogisticsCost.FormatMoney();
            view.NoTaxPrice = source.NoTaxPrice.FormatMoney();
            view.Addtime = source.Addtime.ToStringWithtime();
            return view;
        }
        public static IList<SaleReportView> SaleReportSet_To_SaleReportViews(this IEnumerable<SaleReportSet> source)
        {

            var dest = new List<SaleReportView>();
            foreach (var item in source)
            {
                dest.Add(item.SaleReportSet_To_SaleReportView());
            }
            return dest;
        }


        #endregion


        #region 销售方法

        //public static SaleReportSet SaleReportSet_To_SaleReportSetEdit(this SaleReportSet source)
        //{
        //    view = Mapper.Map<SaleReportSet, SaleReportView>(source);
        //}

        #endregion


        #region 通用方法

        private static string GetUserTrueName(string userName, IList<UserCacheView> userList)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return "";
            }
            foreach (var item in userList)
            {
                if (item.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return item.TrueName;
                }
            }
            return userName;
        }

        private static string MoneyToString(decimal? price)
        {
            return price.HasValue ? string.Format("{0:C2}", price.Value) : "";
        }

        private static string GetDicNameByCode(string code, IList<DicView> dicList)
        {
            if (string.IsNullOrEmpty(code))
            {
                return "";
            }
            var info = dicList.Where(d => d.Code == code).FirstOrDefault();
            return info == null ? "" : info.Name;
        }


        private static string GetProductShowName(ProductSet info)
        {
            if (info == null)
            {
                return "";
            }

            return info.Aliases + "【" + info.ModelSet.Name + "】";

        }

        private static string GetElementShowName(ElementSet info)
        {
            return info == null ? "" : (info.Name + "【" + info.Code + "】");
        }

        private static string GetModelShowName(ModelSet info)
        {
            return info == null ? "" : (info.Name + "【" + info.Code + "】");
        }

        private static string GetShelfShowName(ShelfSet info)
        {
            if (info == null)
            {
                return "";
            }
            //if (!string.IsNullOrEmpty(info.Name))
            //{
            //    return info.Code;
            //}
            return info.Code;
        }



        private static string GetPartNameByPartList(IList<PartCacheView> list, string partCode)
        {
            if (string.IsNullOrEmpty(partCode))
            {
                return "";
            }
            if (partCode == PartCode.partOther.ToString())
            {
                return "其他";
            }
            var pinfo = list.FirstOrDefault(d => d.PartCode == partCode);
            return pinfo == null ? "" : pinfo.PartName;
        }

        #endregion



        #region 送货单

        public static IList<DeliveryListView> Convert_Delivery_To_DeliveryListView_List(this IList<DeliverySet> source,
            IList<DicView> unitList, IList<UserCacheView> userList)
        {
            var dest = new List<DeliveryListView>();
            DeliveryListView view;
            DeliveryDetailView detailView;
            foreach (var item in source)
            {
                view = new DeliveryListView();
                view.Addtime = item.Addtime.ToStringWithtime();
                view.AddUserName = GetUserTrueName(item.AddUserName, userList);
                view.Customer = item.Customer;
                view.Id = item.Id;
                view.Manager = item.Manager;
                view.OrderNo = item.OrderNo;
                view.OrderDate = item.OrderDate.ToStringWithDate();
                view.Remark = item.Remark;
                view.Sender = item.Sender;
                view.TotalAmount = item.TotalAmount.ToStringText();
                view.SerialNo = item.SerialNo;
                if (item.IsOut.HasValue && item.IsOut.Value)
                {
                    view.IsOut = "是";
                }
                else
                {
                    view.IsOut = "否";
                }
               
                view.Details = new List<DeliveryDetailView>();
                foreach (var detail in item.DeliveryDetailSet)
                {
                    detailView = new DeliveryDetailView();
                    detailView.Type = detail.ElementId.HasValue ? "原材料" : "成品";
                    if (detailView.Type == "原材料")
                    {
                        detailView.Model = detail.ElementSet.Name;
                    }
                    else
                    {
                        detailView.Model = detail.ProductSet.ModelSet.GetModelText();
                    }
                    detailView.Quantity = detail.Quantity.ToStringText();
                    detailView.Remark = detail.Remark;
                    detailView.Price = detail.Price.ToStringText();
                    detailView.TotalPrice = detail.TotalPrice.ToStringText();
                    detailView.Unit = unitList.GetName(detail.UnitTypeCode);
                    view.Details.Add(detailView);
                    //detailView.TotalPrice = detail.


                }
                dest.Add(view);
                //dest.Add(item.ConvertTo_PickListView(unitList, partList, userList));
            }
            return dest;
        }


        public static DeliveryDetail_ForAdd_View Convert_Product_To_DeliveryDetail_ForAdd_View(this ProductSet source,
            IList<DicView> dicList, IList<ModelCacheView> modelList)
        {
            var dest = new DeliveryDetail_ForAdd_View();
            if (source == null)
            {
                return dest;
            }
            dest.Model = modelList.GetModelText(source.ModelId);
            dest.Price = source.Price.HasValue ? source.Price.Value : 0;
            dest.ProductId = source.Id;
            dest.Type = "成品";
            dest.Unit = dicList.GetName(source.UnitTypeCode);
            dest.UnitTypeCode = source.UnitTypeCode;
        

            return dest;

        }

        public static DeliveryDetail_ForAdd_View Convert_Element_To_DeliveryDetail_ForAdd_View(this ElementSet source,
          IList<DicView> dicList)
        {
            var dest = new DeliveryDetail_ForAdd_View();
            if (source == null)
            {
                return dest;
            }
            dest.Model = source.Name;
            dest.Price = source.Price.HasValue ? source.Price.Value : 0;
            dest.ElementId = source.Id;
            dest.Type = "原材料";
            dest.Unit = dicList.GetName(source.UnitTypeCode);
            dest.UnitTypeCode = source.UnitTypeCode;


            return dest;

        }

        public static DeliveryForPrint Convert_Delivery_To_DeliveryForPrint(this DeliverySet source, IList<DicView> unitList)
        {
            var dest = new DeliveryForPrint();
            if (source == null)
            {
                return dest;
            }
            dest.Customer = source.Customer;
            dest.Manager = source.Manager;
            dest.OrderNo = source.OrderNo;
            dest.OrderDate = source.OrderDate.ToString("yyyy年MM月dd日");
            dest.Sender = source.Sender;
            dest.TotalAmount = source.TotalAmount.ToStringText();
            dest.SerialNo = source.SerialNo;
            dest.TotalAmountUp = source.TotalAmountUp;
            dest.Details = new List<DeliveryForPrint_Detail>();
            DeliveryForPrint_Detail detailView;
            foreach (var detail in source.DeliveryDetailSet)
            {
                detailView = new DeliveryForPrint_Detail();
                if (detail.ElementId.HasValue)
                {
                    detailView.Model = detail.ElementSet.Name;
                }
                else
                {
                    detailView.Model = detail.ProductSet.ModelSet.GetModelText();
                }
                detailView.Quantity = detail.Quantity.ToStringText();
                detailView.Remark = detail.Remark;
                detailView.Price = detail.Price.ToStringText();
                detailView.TotalPrice = detail.TotalPrice.ToStringText();
                detailView.Unit = unitList.GetName(detail.UnitTypeCode);
                dest.Details.Add(detailView);
            }
            return dest;
        }
        #endregion




        #region 转换方法
        private static string ToStringText(this decimal? price)
        {
            return price.HasValue ? string.Format("{0:C2}", price.Value) : "";
        }

        private static string ToStringText(this decimal price)
        {
            return string.Format("{0:C2}", price);
        }

        private static string ToStringText(this double? data)
        {
            return data.HasValue ? data.ToString() : "";
        }

        private static string GetName(this IList<DicView> list, string code)
        {
            return list.Where(d => d.Code == code).Select(d => d.Name).FirstOrDefault();
        }

        private static string ToStringWithtime(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private static string ToStringWithtime(this DateTime? dt)
        {
            if (dt == null || dt.Value == DateTime.MinValue || dt.Value == DateTime.MaxValue)
            {
                return "";
            }
            return dt.Value.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private static string ToStringWithDate(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        private static string ToStringWithDate(this DateTime? dt)
        {
            if (dt == null || dt.Value == DateTime.MinValue || dt.Value == DateTime.MaxValue)
            {
                return "";
            }
            return dt.Value.ToString("yyyy-MM-dd");
        }

        private static string GetModelText(this IList<ModelCacheView> list, int id)
        {
            var info = list.FirstOrDefault(d => d.Id == id);
            return info == null ? "" : (info.Name + "【" + info.Code + "】");
        }
        #endregion

    }
}
