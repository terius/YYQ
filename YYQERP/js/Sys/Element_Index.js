"use strict";
km.init = function () {
    com.initbuttons($('#km_toolbar'), km.model.buttons);
    km.init_parent_model();
    km.template.init();
    km.maingrid.init();
    com.CheckPer();
}




km.template = {
    jq_add: null,
    jq_file: null,
    jq_file2:null,
    initTemplate: function () {
        var html = baidu.template('tpl_add', {});//使用baidu.template命名空间
        this.jq_add = $(html);

        html = baidu.template('tpl_file', {});//使用baidu.template命名空间
        this.jq_file = $(html);
        html = baidu.template('tpl_file2', {});//使用baidu.template命名空间
        this.jq_file2 = $(html);
    },
    init: function () {
        this.initTemplate();
    }
};

km.maingrid = function () {

    var $grid = $("#maingrid");
    return {
        init: function () {
            var QuantityWarning = function (value, row, index) {
                if (row.StockQuantity == "未入库" || row.StockQuantity <= row.WarningQuantity) {
                    return 'background-color:#f24b21;color:#fff;';
                }
            }
            $grid.datagrid(km.gridOption({
                fitColumns: true,
                queryParams: { _t: com.settings.timestamp(), Name: "" },
                url: km.model.urls["pagelist"],
                columns: [[
                    { field: 'Name', title: '原材料名称', width: 180, align: 'left', sortable: true },
                    { field: 'Code', title: '料号', width: 120, align: 'left', sortable: true },
                    { field: 'ShelfName', title: '库位', width: 60, align: 'left', sortable: true },
                    { field: 'UnitName', title: '单位规格', width: 120, align: 'left', sortable: true },
                    { field: 'WarningQuantity', title: '预警数量', width: 120, align: 'left', sortable: true },
                    { field: 'StockQuantity', title: '当前库存', width: 120, align: 'left', sortable: true, styler: QuantityWarning },
                    { field: 'PriceFormat', title: '单价', width: 150, align: 'left', sortable: true },
                    { field: 'Remark', title: '备注', width: 200, align: 'left', sortable: true },
                    { field: 'Addtime', title: '创建时间', width: 150, align: 'left', sortable: true }

                ]],
                toolbar: '#toolbar1',
                onClickRow: function (index, row) {
                    // km.maingrid.ShowStockDetail(row.Id);
                    // alert(JSON.stringify(row));
                    //km.maingrid.selectedIndex = index;
                    //km.maingrid.selectedRow = row;
                    //km.rolegrid.setUserRoles(row);
                    //if (km.maingrid.selectedRow)
                    //    km.set_mode('show');
                },
            }));//end grid init
        },

        search_data: function () {
            var elename = com.trim($("#elementCode").val());
            var shelfcode = com.trim($("#shelfCode").val());
            this.reload({ ElementName: elename, ShelfName: shelfcode });
        },
        reload: function (params) {
            var defaults = { _t: com.settings.timestamp() };
            if (params) {
                defaults = $.extend(defaults, params);
            }
            $grid.datagrid('reload', defaults);

        },
        deleteRow: function (row) {
            var index = $grid.datagrid('getRowIndex', row);
            $grid.datagrid('deleteRow', index);
        },
        getSelectedRow: function () {
            //获取当前选中的行
            return $grid.datagrid('getSelected');
        },
        do_export: function (exportPageData) {
            km.GetExportParams($grid, "原材料", exportPageData);
            km.export.ElementName = com.trim($("#elementCode").val());
            km.export.ShelfName = com.trim($("#shelfCode").val());
            com.ExportToExcel("/Element/ExportExcel", km.export);
        }
     
    }
}();

/*工具栏按钮事件*/
km.toolbar = {
    do_add: function () {
        //  var jq_add = $("#ele_add");
        km.template.jq_add.dialog_ext({
            title: '新增原材料种类', iconCls: 'icon-standard-add',
            onOpenEx: function (win) {
                win.find('#UnitTypeCode').combobox('reload', '/Common/GetUnitSelectList');
                win.find('#ShelfId').combobox('reload', '/Common/GetShelfSelectList');
            },
            onClickButton: function (win) { //保存操作
                if (com.CheckError(win)) {
                    return false;
                }
                com.jqFormOption.success = function (result) {
                    if (result == "") {
                        com.message('s', "新增原材料成功");
                        win.dialog('destroy');
                        km.maingrid.reload();
                    }
                    else {
                        com.message('e', result);
                    }
                }
                com.jqFormOption.url = km.model.urls["add"];
                var form = win.find("#formadd");
                form.ajaxSubmit(com.jqFormOption);
            }
        });
    },
    do_edit: function () {
        var sRow = km.maingrid.getSelectedRow();
        if (sRow == null) {
            layer.msg('请选择一条记录！'); return;
        }
        km.template.jq_add.dialog_ext({
            title: '编辑原材料【' + sRow.Name + '】', iconCls: 'icon-standard-edit',
            onOpenEx: function (win) {
                win.find('#UnitTypeCode').combobox('reload', '/Common/GetUnitSelectList');
                win.find('#ShelfId').combobox('reload', '/Common/GetShelfSelectList');
                win.find('#formadd').form('load', sRow);
            },
            onClickButton: function (win) { //保存操作
                if (com.CheckError(win)) {
                    return false;
                }
                com.jqFormOption.success = function (result) {
                    if (result == "") {
                        com.message('s', "编辑原材料成功");
                        win.dialog('destroy');
                        km.maingrid.reload();
                    }
                    else {
                        com.message('e', result);
                    }
                }
                com.jqFormOption.url = km.model.urls["edit"];
                com.jqFormOption.data.Id = sRow.Id;
                var form = win.find("#formadd");
                form.ajaxSubmit(com.jqFormOption);
            }
        });

    },
    do_delete: function () {
        var sRow = km.maingrid.getSelectedRow();
        if (sRow == null) {
            layer.msg('请选择一条记录！'); return;
        }
        // var jsonParam = JSON.stringify(sRow);
        com.message('c', ' <b style="color:red">确定要删除原材料【' + sRow.Name + '】吗？如果删除其相关信息将一并删除，请谨慎操作！！！ </b>', function (b) {
            if (b) {
                com.ajax({
                    url: km.model.urls["delete"], data: { Id: sRow.Id }, success: function (result) {
                        //layer.msg(result.emsg); 
                        if (result == "") {
                            com.message('s', "删除原材料【" + sRow.Name + "】成功");
                            km.maingrid.reload();
                        } else {
                            com.message('e', result);
                        }
                    }
                });
            }
        });
    },
    do_import: function () {
        var checkFile = function (win) {
            var file = win.find("#importfile");
            return false;

        }
        km.template.jq_file.dialog_ext({
            title: '导入原材料', iconCls: 'icon-standard-page-white-excel',
            onOpenEx: function (win) {

            },
            onClickButton: function (win) { //保存操作
                //if (checkFile(win)) {
                //    return false;
                //}

                com.jqFormOption.success = function (result) {

                    if (result == "") {
                        com.message('s', "导入成功");
                        win.dialog('destroy');
                        km.maingrid.reload();
                        //   loading.fadeOut();
                    }
                    else {
                        com.message('e', result);
                    }
                }
                com.jqFormOption.url = km.model.urls["importFile"];
                var form = win.find("#formfile");
                form.ajaxSubmit(com.jqFormOption);
            }
        });
    },
    do_restore: function () {
        var checkFile = function (win) {
            var file = win.find("#importfile2");
            return false;

        }
        km.template.jq_file2.dialog_ext({
            title: '原材料库存数量调整', iconCls: 'icon-standard-page-white-excel',
            onOpenEx: function (win) {

            },
            onClickButton: function (win) { //保存操作
                //if (checkFile(win)) {
                //    return false;
                //}
                com.message('c', ' <b style="color:red">是否确定要更改库存数？ </b>', function (b) {
                    if (b) {
                        com.jqFormOption.success = function (result) {

                            if (result == "") {
                                com.message('s', "导入成功");
                                win.dialog('destroy');
                                km.maingrid.reload();
                                //   loading.fadeOut();
                            }
                            else {
                                com.message('e', result);
                            }
                        }
                        com.jqFormOption.url = km.model.urls["restoreFile"];
                        var form = win.find("#formfile2");
                        form.ajaxSubmit(com.jqFormOption);
                    }
                })
            }
        });
    }

};



$(km.init);
