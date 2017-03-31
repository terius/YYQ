/*
路径：~/Areas/Sys/ViewJS/Base_Button.js
说明：权限按钮页面Base_Button的js文件
*/
//当前页面对象
"use strict";
var km = {};
km.model = null;
km.parent_model = null;

km.init = function () {
    com.initbuttons($('#km_toolbar'), km.model.buttons);
    km.init_parent_model();
    km.template.init();
    km.maingrid.init();
}

/*初始化iframe父页面的model对象，即：访问app.index.js文件中的客户端对象*/
km.init_parent_model = function () {
    //只有当前页面有父页面时，可以获取到父页面的model对象 parent.wrapper.model
    if (window != parent) {
        if (parent.wrapper) {
            km.parent_model = parent.wrapper.model;
            com.message('s', '获取到父页面的model对象：<br>' + JSON.stringify(km.parent_model));
        } else {
            com.showcenter('提示', "存在父页面，但未能获取到parent.wrapper对象");
        }
    } else {
        com.showcenter('提示', "当前页面已经脱离iframe，无法获得parent.wrapper对象！");
    }
}

//页面对象参数设置
km.settings = {};

//格式化数据
km.gridformat = {};

//百度模板引擎使用 详情：http://tangram.baidu.com/BaiduTemplate/
km.template = {
    tpl_add_html: '',//tpl_add模板的html
    tpl_icon_html: '',
    jq_add: null,
    jq_icon: null,
    initTemplate: function () {
        var data = { title: 'baiduTemplate', list: ['test data 1', 'test data 2', 'test data3'] };
        this.tpl_add_html = baidu.template('tpl_add', data);//使用baidu.template命名空间
        this.jq_add = $(this.tpl_add_html);

        var data16 = $.kmui.icons.all;
        var listData = { "title": 'icon16x16', "list": data16 }
        this.tpl_icon_html = baidu.template('tpl_icon', listData);
        this.jq_icon = $(this.tpl_icon_html);
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
            idField: 'ButtonCode',
            queryParams: { _t: com.settings.timestamp(), keyword: "" },
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
	            { field: 'ButtonCode', title: '按钮代码', width: 120, align: 'left', sortable: true },
                {
                    field: 'ButtonName', title: '按钮名称', width: 150, align: 'left', sortable: true, formatter: function (value, row, index) {
                        return '&nbsp;&nbsp;<a href="#"><span class="icon ' + row.IconClass + '">&nbsp;</span>&nbsp;' + value + '</a>';
                    }
                },
                { field: 'JsEvent', title: 'js事件', width: 150, align: 'left', sortable: true },
                {
                    field: 'ButtonType', title: '按钮类型', width: 80, align: 'center', sortable: true, formatter: function (value, row, index) {
                        //按钮类型 0=未定义 1=工具栏按钮 2=右键按钮 3=其他(待定)
                        var h = '未定义';
                        if (value == 1) { h = '工具栏按钮'; } else if (value == 2) { h = '右键按钮'; } else if (value == 3) { h = '其他'; }
                        return h;
                    }
                },
                { field: 'IconClass', title: '图标样式', width: 220, align: 'left', sortable: true },
                //{ field: 'IconUrl', title: '图标Url', width: 80, align: 'center' },
                { field: 'Sort', title: '排序', width: 80, align: 'center', sortable: true },
                //{ field: 'Split', title: '是否分隔符', width: 80, align: 'center' },
                { field: 'Enabled', title: '启用', width: 60, align: 'center', sortable: true, formatter: com.html_formatter.yesno },
                { field: 'Remark', title: '备注', width: 200, align: 'left', sortable: true }
                //{ field: 'AddBy', title: '创建人', width: 80, align: 'center' },
                //{ field: 'AddOn', title: '创建时间', width: 80, align: 'center' },
                //{ field: 'EditBy', title: '编辑人', width: 80, align: 'center' },
                //{ field: 'EditOn', title: '编辑时间', width: 80, align: 'center' },
            ]],
            onLoadSuccess: function () {
           //     alert('load data successfully!');
            },
            onLoadError: function(a,b,c) {
                alert('ajax执行出错');

            }
        });//end grid init
    },
    reload: function () { this.jq.datagrid('reload', { _t: com.settings.timestamp() }); },
    deleteRow: function (row) {
        var index = this.jq.datagrid('getRowIndex', row);
        this.jq.datagrid('deleteRow', index);
    },
    getSelectedRow: function () {
        //获取当前选中的行
        return this.jq.datagrid('getSelected');
    }
};

/*工具栏权限按钮事件*/
km.toolbar = {
    do_browse: function () { },
    do_refresh: function () {
        km.maingrid.reload(); //window.location.reload();
    },
    do_add: function () {
        km.template.jq_add.dialog_ext({
            title: '新增按钮', iconCls: 'icon-standard-add',
            onOpenEx: function (win) {
                win.find("#span_icon").removeClass();
                win.find("#span_icon").addClass("icon").addClass("icon-standard-bricks");
                win.find('#TPL_Enabled').combobox('setValue', 1);
                win.find('#TPL_ButtonType').combobox('setValue', 1);
                win.find('#TPL_Sort').numberbox('setValue', 100);
                win.find('#TPL_IconClass').textbox({
                    buttonText: '选择',
                    buttonIcon: '', editable: true, value: 'icon-standard-bricks',
                    onClickButton: function () {
                        km.initIcon(win);
                    }
                });
            },
            onClickButton: function (win) { //保存操作
                var flagValid = true;
                var jsonObject = win.find("#formadd").serializeJson();
                var jsonStr = JSON.stringify(jsonObject);
                if (jsonObject.ButtonCode == "" || jsonObject.ButtonCode == "" || jsonObject.JsEvent == "") {
                    flagValid = false;
                    com.message('e', '按钮代码或按钮名称或按钮js事件不能为空！'); return;
                }
                if (flagValid) {
                    com.ajax({
                        type: 'POST', url: km.model.urls["add"], data: jsonStr, success: function (result) {
                            if (result.s) {
                                com.message('s', result.emsg);
                                win.dialog('destroy');
                                km.maingrid.reload();
                            } else {
                                com.message('e', result.emsg);
                            }
                        }
                    });
                }
            }
        });
    },
    do_edit: function () {
        var sRow = km.maingrid.getSelectedRow();
        if (sRow == null) {
            layer.msg('请选择一条记录！'); return;
        }
        km.template.jq_add.dialog_ext({
            title: '编辑按钮【' + sRow.ButtonName + '】', iconCls: 'icon-standard-edit',
            onOpenEx: function (win) {
                win.find("#span_icon").removeClass();
                var icon = 'icon-standard-bricks';
                if (sRow.IconClass != "" && sRow.IconClass != null) {
                    icon = sRow.IconClass;
                }
                win.find("#span_icon").addClass("icon").addClass(icon);
                win.find('#TPL_ButtonCode').textbox().textbox('readonly', true);
                win.find('#TPL_IconClass').textbox({
                    buttonText: '选择', buttonIcon: '', editable: true, value: icon,
                    onClickButton: function () {
                        km.initIcon(win);
                    }
                }); 
                win.find('#formadd').form('load', sRow);
            },
            onClickButton: function (win) { //保存操作
                var flagValid = true;
                var jsonObject = win.find("#formadd").serializeJson();
                var jsonStr = JSON.stringify(jsonObject);
                //alert(jsonStr); return;
                if (jsonObject.ButtonCode == "" || jsonObject.ButtonCode == "" || jsonObject.JsEvent == "") {
                    flagValid = false;
                    com.message('e', '按钮代码或按钮名称或按钮js事件不能为空！'); return;
                }
                if (flagValid) {
                    com.ajax({
                        type: 'POST', url: km.model.urls["edit"], data: jsonStr, success: function (result) {
                            if (result.s) {
                                com.message('s', result.emsg);
                                win.dialog('destroy');
                                km.maingrid.reload();
                            } else {
                                com.message('e', result.emsg);
                            }
                        }
                    });
                }
            }
        });

    },
    do_delete: function () {
        var sRow = km.maingrid.getSelectedRow();
        if (sRow == null) {
            layer.msg('请选择一条记录！'); return;
        }
        var jsonParam = JSON.stringify(sRow);
        com.message('c', ' <b style="color:red">确定要删除按钮【' + sRow.ButtonName + '】吗？ </b>', function (b) {
            if (b) {
                com.ajax({
                    url: km.model.urls["delete"], data: jsonParam, success: function (result) {
                        //layer.msg(result.emsg); 
                        if (result.s) {
                            com.message('s', result.emsg);
                            km.maingrid.reload();
                        } else {
                            com.message('e', result.emsg);
                        }
                    }
                });
            }
        });
    },
    do_search: function () {

    }
};

//初始化图标选择
km.initIcon = function (addwin) {
    //addwin.find('#TPL_IconClass').textbox({
    //    buttonText: '选择',
    //    buttonIcon: '', editable: false,
    //    onClickButton: function () {
    //        km.initIcon(win);
    //    }
    //});
    var $this = addwin.find('#TPL_IconClass').textbox();
    var $spanicon = addwin.find('#span_icon');
    km.showIconSelector(km.template.tpl_icon_html, function (win) {
        var hwin = win.html();
        win.find("span").click(function () {
            //alert($(this)[0].id);
            var old_icontext = win.find("#icontext").text();
            if (old_icontext != "") {
                win.find("#" + old_icontext).removeClass("icon_selected");
            }
            var new_icontext = $(this)[0].id;
            $(this).addClass("icon_selected");
            win.find("#icontext").text(new_icontext);
            win.find("#span_selected_icon").removeClass();
            win.find("#span_selected_icon").addClass("icon ").addClass(new_icontext).addClass("iconbox");

        });
    }, function (win) {
        var selected_icontext = win.find("#icontext").text();
        if (selected_icontext == "") {
            com.message('w', '请选择一个图标');
            return;
        }
        //console.info($this);
        $this.textbox('setValue', selected_icontext);
        $spanicon.removeClass().addClass("icon").addClass(selected_icontext);
        $spanicon.attr('title', selected_icontext);
        parent.$(win).dialog('destroy');
        //alert(selected_icontext);
    });
}
//显示图标选择对话框
km.showIconSelector = function (html, before, dosave) {
    com.dialog({
        title: '图标选择',
        html: html, resizable: true, maximizable: true, width: 600,
        height: 400, btntext: '确定选择'
    }, before, dosave);
}

$(km.init);
