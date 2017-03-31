"use strict";
km.init = function () {
    km.init_parent_model();
    km.maintree.init();
    km.detailgrid.init();
    com.CheckPer();
}


km.maintree = {
    jq: null,
    treedata: null,
    selectedNode: null,
    selectedData: null,
    setRowData: function (node) {
        var rowData = {
            RoleId: node.RoleId,
            RoleName: node.RoleName
        };
        km.maintree.selectedData = rowData;
    },
    init: function () {
        this.treedata = null;
        this.selectedNode = null;
        this.selectedData = null;
        this.jq = $("#maintree").tree({
            method: 'GET',
            animate: true,
            url: km.model.urls["list"],
            formatter: function (node) {
                return node.RoleName;
            },
            onClick: function (node) {// 在用户点击的时候提示
                //alert(JSON.stringify(node));   
                //km.maintree.selectedNode = node;
                km.maintree.setRowData(node);
                //km.set_mode('show');
                km.detailgrid.reload({ roleId: node.RoleId });
            },
            onLoadSuccess: function (node, data) { // $("#div_content").html(JSON.stringify(data));
                km.maintree.treedata = data;
                //km.maintree.jq.tree('collapseAll');
            }
        });//end tree init
    }
};


km.detailgrid = function () {
    var jq = null, $grid = $("#MenuTree");
    var do_aftersave = function () {
        com.message('s', "角色权限保存成功");
        var roleid = km.maintree.selectedData.RoleId;
        km.detailgrid.reload({ roleId: roleid });
    }
    return {
        init: function () {
            jq = $grid.treegrid({
                fitColumns:true,
                method: 'get',
                animate: true,
                url: km.model.urls["getRoleMenus"],
                idField: 'id',
                treeField: 'text',
                queryParams: { _t: com.settings.timestamp(), roleId: 0 },
                //  checkbox: true,
                //  cascadeCheck: false,
                columns: [[
                   {
                       title: '模块名称', field: 'text', width: 100,
                       formatter: function (value, row, index) {
                           if (row._parentId == null) {
                               return "<span>" + value + "</span>";
                           }
                           else {
                               return "<label><input type=\"checkbox\" name=\"mck-" + row.id + "\"" + (row.checked ? "checked" : "") + " >" + row.text+"</lable>";
                           }
                       }
                   },
                   {
                       title: '权限', field: 'OperViews', width: 300, formatter: function (value, row, index) {
                           //<div style="text-align:left;white-space:normal;height:auto;" class="datagrid-cell datagrid-cell-c1-Rights">
                           //<input type="checkbox" id="allck-sys_sysparam" menucode="sys_sysparam" onclick="km.maingrid.allActionCheckboxClick(this,'sys_sysparam')">
                           //    <span style="font-weight:bold; color:#6C1899;">全选</span>&nbsp;&nbsp;|&nbsp;&nbsp;
                           //        <input type="checkbox" id="ck-sys_sysparam-add" name="ck-sys_sysparam" menucode="sys_sysparam" buttoncode="add" onclick="km.maingrid.actionCheckboxClick(this)">
                           //<span class="icon icon-standard-add" style=" position:relative;top:5px"></span>新增 &nbsp;&nbsp;<input type="checkbox" id="ck-sys_sysparam-edit" name="ck-sys_sysparam" menucode="sys_sysparam" buttoncode="edit" onclick="km.maingrid.actionCheckboxClick(this)"><span class="icon icon-standard-pencil" style=" position:relative;top:5px"></span>编辑 &nbsp;&nbsp;<input type="checkbox" id="ck-sys_sysparam-delete" name="ck-sys_sysparam" menucode="sys_sysparam" buttoncode="delete" onclick="km.maingrid.actionCheckboxClick(this)"><span class="icon icon-standard-cross" style=" position:relative;top:5px"></span>删除 &nbsp;&nbsp;<input type="checkbox" id="ck-sys_sysparam-refresh" name="ck-sys_sysparam" menucode="sys_sysparam" buttoncode="refresh" onclick="km.maingrid.actionCheckboxClick(this)"><span class="icon icon-standard-arrow-refresh" style=" position:relative;top:5px"></span>刷新 </div>

                           if (value && value.length > 0) {
                               var html = "";
                               for (var i = 0; i < value.length; i++) {
                                   html += "<label><input type=\"checkbox\" name=\"ck-" + row.id + "-" + value[i].Id + "\" " + (value[i].checked ? "checked" : "") + "  >" + value[i].Name + "</label>&nbsp;&nbsp";
                               }
                               return html;
                           }
                       }
                   }
                ]],
                onLoadSuccess: function (row, data) {
                    //  alert(JSON.stringify(data));
                    //for (var i = 0; i < data.length; i++) {
                    //    if (data[i].Checked == 1) {
                    //        $grid.treegrid("checkNode", data[i].Id);
                    //    }
                    //    for (var j = 0; j < data[i].children.length; j++) {
                    //        if (data[i].children[j].Checked == 1) {
                    //            $grid.treegrid("checkNode", data[i].children[j].Id);
                    //        }
                    //    }
                    //}

                },
                onCheckNode: function (row, checked) {
                    // alert(JSON.stringify(checked));
                    //var parent = $grid.tree("getParent", node.target);
                    //if (parent) {
                    //    // alert(JSON.stringify(parent));
                    //    $grid.tree("check", parent.target);
                    //}
                    //   alert(JSON.stringify(node));
                }
            });
        },
        reload: function (queryParams) {
            var default_QueryParams = { _t: com.settings.timestamp() };
            if (queryParams) {
                default_QueryParams = $.extend(default_QueryParams, queryParams);
            }
            $grid.treegrid('reload', default_QueryParams);
            //$.getJSON(km.model.urls["getRoleMenus"], default_QueryParams, function (data) {
            //    $grid.tree('loadData', data);
            //})

        },
        do_savePermisson: function () {
            var roleId = km.maintree.selectedData ? km.maintree.selectedData.RoleId : 0;
            if (roleId) {
                com.jqFormOption.success = function (result) {
                    if (result == "") {
                        com.message('s', "角色权限保存成功");
                        km.detailgrid.reload({ roleId: roleId });
                        //   km.maingrid.reload();
                    }
                    else {
                        com.message('e', result);
                    }
                }
                com.jqFormOption.url = km.model.urls["saveRoleMenus"];
                com.jqFormOption.data.roleId = km.maintree.selectedData.RoleId;
                var form = $("#formtree");
                form.ajaxSubmit(com.jqFormOption);
                //var data = $grid.treegrid('getCheckedNodes'); 

                ////alert(JSON.stringify(nodes));
                //var ids = [];
                //for (var i = 0; i < data.length; i++) {
                //    var childs = $grid.treegrid('getChildren', data[i].id);
                //    ids.push({ id: data[i].id });
                //}
                //com.SaveAjaxInfos(ids, km.model.urls["saveRoleMenus"] + "?roleId=" + roleId, "Bom明细保存成功", do_aftersave);
            }
        }
    }
}();

//km.detailgrid = function () {
//    var jq = null, $grid = $("#MenuTree");
//    var do_aftersave = function () {
//        com.message('s', "角色权限保存成功");
//        var roleid = km.maintree.selectedData.RoleId;
//        km.detailgrid.reload({ roleId: roleid });
//    }
//    return {
//        init: function () {
//            jq = $grid.tree({
//                method: 'get',
//                animate: true,
//                url: km.model.urls["getRoleMenus"],
//                //idField: 'Id',
//                //treeField: 'Name',
//                queryParams: { _t: com.settings.timestamp(), roleId: 0 },
//                checkbox: true,
//                cascadeCheck: false,
//                onLoadSuccess: function (row, data) {
//                    //  alert(JSON.stringify(data));
//                    //for (var i = 0; i < data.length; i++) {
//                    //    if (data[i].Checked == 1) {
//                    //        $grid.treegrid("checkNode", data[i].Id);
//                    //    }
//                    //    for (var j = 0; j < data[i].children.length; j++) {
//                    //        if (data[i].children[j].Checked == 1) {
//                    //            $grid.treegrid("checkNode", data[i].children[j].Id);
//                    //        }
//                    //    }
//                    //}

//                },
//                onCheck: function (node, checked) {
//                    var parent = $grid.tree("getParent", node.target);
//                    if (parent) {
//                        // alert(JSON.stringify(parent));
//                        $grid.tree("check", parent.target);
//                    }
//                    //   alert(JSON.stringify(node));
//                }
//            });
//        },
//        reload: function (queryParams) {
//            var default_QueryParams = { _t: com.settings.timestamp() };
//            if (queryParams) {
//                default_QueryParams = $.extend(default_QueryParams, queryParams);
//            }
//            // $grid.treegrid('loadData', []);
//            $.getJSON(km.model.urls["getRoleMenus"], default_QueryParams, function (data) {
//                $grid.tree('loadData', data);
//            })

//        },
//        do_savePermisson: function () {
//            var roleId = km.maintree.selectedData ? km.maintree.selectedData.RoleId : 0;
//            if (roleId) {
//                var data = $grid.tree('getChecked');
//                var ids = [];
//                for (var i = 0; i < data.length; i++) {
//                    ids.push({ id: data[i].id });
//                }
//                com.SaveAjaxInfos(ids, km.model.urls["saveRoleMenus"] + "?roleId=" + roleId, "Bom明细保存成功", do_aftersave);
//            }
//        }
//    }
//}();

$(km.init);
