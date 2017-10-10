"use strict";
km.init = function () {
    com.initbuttons($('#km_toolbar'), km.model.buttons);
    km.init_parent_model();
    km.template.init();
    km.maingrid.init();
    com.CheckPer();
    // km.rolelist.init("Role");
}

km.rolelist =
{
    init: function (selectId) {
        var list = $.getJSON("/User/GetRoleSelectList", function (d) {
            $("#" + selectId).empty();
            alert(document.getElementById(selectId).outerHTML);
            if (d.length > 0) {
                var html = "";
                for (var i = 0; i < d.length; i++) {
                    html += "<option value=\"" + d[i].id + "\">" + d[i].text + "</option>";
                }
                $("#" + selectId).html(html);
            }
        })
    }
}

//百度模板引擎使用 详情：http://tangram.baidu.com/BaiduTemplate/
km.template = {
    tpl_add_html: '',//tpl_add模板的html
    //  tpl_icon_html: '',
    jq_add: null,
    //  jq_icon: null,
    initTemplate: function () {
        var data = { title: 'baiduTemplate', list: ['test data 1', 'test data 2', 'test data3'] };
        this.tpl_add_html = baidu.template('tpl_add', data);//使用baidu.template命名空间
        this.jq_add = $(this.tpl_add_html);

        // var data16 = $.kmui.icons.all;
        // var listData = { "title": 'icon16x16', "list": data16 }
        // this.tpl_icon_html = baidu.template('tpl_icon', listData);
        // this.jq_icon = $(this.tpl_icon_html);
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
	            { field: 'UserName', title: '用户名', width: 120, align: 'left', sortable: true },
                { field: 'TrueName', title: '姓名', width: 120, align: 'left', sortable: true },
                { field: 'RoleName', title: '所属角色', width: 120, align: 'left', sortable: true },
                { field: 'Addtime', title: '添加时间', width: 150, align: 'left', sortable: true }
            ]],
            onLoadSuccess: function () {
                //     alert('load data successfully!');
            },
            onLoadError: function (a, b, c) {
                alert('ajax执行出错');

            },
            toolbar: '#toolbar1'
        });//end grid init
    },

    search_data: function () {
        var name = com.trim($("#keyword").val());
        this.reload({ Name: name });
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
    do_browse: function () { },
    do_refresh: function () {
        km.maingrid.reload(); //window.location.reload();
    },
    do_add: function () {
        km.template.jq_add.dialog_ext({
            title: '新增用户', iconCls: 'icon-standard-add',
            onOpenEx: function (win) {
                win.find('#RoleId').combobox('reload', '/User/GetRoleSelectList');
                win.find('#tr-resetpwd').hide();
            },
            onClickButton: function (win) { //保存操作
                if (com.CheckError(win)) {
                    return false;
                }
                com.jqFormOption.success = function (result) {
                    if (result == "") {
                        com.message('s', "新增用户成功");
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
        km.selectUserId = sRow.Id;
        km.template.jq_add.dialog_ext({
            title: '编辑用户【' + sRow.TrueName + '】', iconCls: 'icon-standard-edit',
            onOpenEx: function (win) {
                win.find('#RoleId').combobox('reload', '/User/GetRoleSelectList')
                win.find('#formadd').form('load', sRow);
                win.find('#btn_resetpwd').on("click", function () {
                    km.toolbar.resetpwd();
                })
                win.find('#tr-resetpwd').show();
               
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
        com.message('c', ' <b style="color:red">确定要删除用户【' + sRow.TrueName + '】吗？ </b>', function (b) {
            if (b) {
                com.ajax({
                    url: km.model.urls["delete"], data: { Id: sRow.Id }, success: function (result) {
                        //layer.msg(result.emsg); 
                        if (result =="") {
                            com.message('s', "删除用户【" + sRow.TrueName +"】成功");
                            km.maingrid.reload();
                        } else {
                            com.message('e', result);
                        }
                    }
                });
            }
        });
    },
    resetpwd: function () {
        com.message('c', ' <b style="color:red">确定要重置该用户密码吗？ </b>', function (b) {
            if (b) {
                $.get(km.model.urls.resetpwd, { id: km.selectUserId }, function (result) {
                    if (result == "") {
                        com.message('s', "用户重置密码成功");

                    } else {
                        com.message('e', result);
                    }
                })
            }
        });
    }
};

$(km.init);
