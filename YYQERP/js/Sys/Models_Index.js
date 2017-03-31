"use strict";
km.init = function () {
    com.initbuttons($('#km_toolbar'), km.model.buttons);
    km.init_parent_model();
    km.template.init();
    km.maingrid.init();
    com.CheckPer();
}


km.template = {
    tpl_add_html: '',
    jq_add: null,
    initTemplate: function () {
        var data = //{ title: 'baiduTemplate', list: ['test data 1', 'test data 2', 'test data3'] };
        this.tpl_add_html = baidu.template('tpl_add', {});//使用baidu.template命名空间
        this.jq_add = $(this.tpl_add_html);
    },
    init: function () {
        this.initTemplate();
    }
};

km.maingrid = {
    jq: null,
    init: function () {
        this.jq = $("#maingrid").datagrid({
            fit: true,
            border: false,
            singleSelect: true,
            rownumbers: true,
            remoteSort: false,
            cache: false,
            method: 'get',
            idField: 'Id',
            queryParams: { _t: com.settings.timestamp(), Name: "" },
            url: km.model.urls["pagelist"],
            pagination: true,
            pageList: [5, 10, 15, 20, 30, 50, 100],
            pageSize: 15,
            rowStyler: function (row) {
                if (row.Enabled == 0) {
                    return 'color:red;';
                }
            },
            columns: [[
                { field: 'Name', title: '型号名称', width: 180, align: 'left', sortable: true },
                { field: 'Code', title: '型号代码', width: 120, align: 'left', sortable: true },
                { field: 'Remark', title: '备注', width: 120, align: 'left', sortable: true },
                { field: 'Addtime', title: '创建时间', width: 150, align: 'left', sortable: true }

            ]],
            onLoadSuccess: function () {
                //     alert('load data successfully!');
            },
            onLoadError: function (a, b, c) {
                alert('ajax执行出错');

            }
        });//end grid init
    },

    search_data: function () {
      //  var elename = com.trim($("#elementCode").val());
       // var shelfcode = com.trim($("#shelfCode").val());
        this.reload();
    },
    reload: function (params) {
        var defaults = { _t: com.settings.timestamp() };
        if (params) {
            defaults = $.extend(defaults, params);
        }
        this.jq.datagrid('reload', defaults);

    },
    deleteRow: function (row) {
        var index = this.jq.datagrid('getRowIndex', row);
        this.jq.datagrid('deleteRow', index);
    },
    getSelectedRow: function () {
        //获取当前选中的行
        return this.jq.datagrid('getSelected');
    }
};

/*工具栏按钮事件*/
km.toolbar = {
    do_add: function () {
      //  var jq_add = $("#ele_add");
        km.template.jq_add.dialog_ext({
            title: '新增型号种类', iconCls: 'icon-standard-add',
            onOpenEx: function (win) {
               // win.find('#UnitTypeCode').combobox('reload', '/Common/GetUnitSelectList');
              //  win.find('#ShelfId').combobox('reload', '/Common/GetShelfSelectList');
            },
            onClickButton: function (win) { //保存操作
                if (com.CheckError(win)) {
                    return false;
                }
                com.jqFormOption.success = function (result) {
                    if (result == "") {
                        com.message('s', "新增型号成功");
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
            title: '编辑用户【' + sRow.Name + '】', iconCls: 'icon-standard-edit',
            onOpenEx: function (win) {
               // win.find('#UnitTypeCode').combobox('reload', '/Common/GetUnitSelectList');
               // win.find('#ShelfId').combobox('reload', '/Common/GetShelfSelectList');
                win.find('#formadd').form('load', sRow);
            },
            onClickButton: function (win) { //保存操作
                if (com.CheckError(win)) {
                    return false;
                }
                com.jqFormOption.success = function (result) {
                    if (result == "") {
                        com.message('s', "编辑用户成功");
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
        com.message('c', ' <b style="color:red">确定要删除型号【' + sRow.Name + '】吗？ </b>', function (b) {
            if (b) {
                com.ajax({
                    url: km.model.urls["delete"], data: { Id: sRow.Id }, success: function (result) {
                        //layer.msg(result.emsg); 
                        if (result == "") {
                            com.message('s', "删除型号【" + sRow.Name + "】成功");
                            km.maingrid.reload();
                        } else {
                            com.message('e', result);
                        }
                    }
                });
            }
        });
    }
};



$(km.init);
