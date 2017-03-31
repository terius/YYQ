

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
    initTemplate: function () {
        var html = baidu.template('tpl_add', {});//使用baidu.template命名空间
        this.jq_add = $(html);

        html = baidu.template('tpl_file', {});//使用baidu.template命名空间
        this.jq_file = $(html);
    },
    init: function () {
        this.initTemplate();
    }
};

km.maingrid = function () {
    var $grid = $("#maingrid");
    var reload = function (params) {
        var defaults = { _t: com.settings.timestamp() };
        if (params) {
            defaults = $.extend(defaults, params);
        }
        $grid.datagrid('reload', defaults);

    }
    return {
        init: function () {


            $grid.datagrid(km.gridOption({
                // fitColumns: true,
                queryParams: { STime: "", ETime: "", menucode: km.menucode },
                url: km.model.urls["pagelist"],
                remoteFilter: true,
                columns: [[
                    { field: 'AfterSales', title: '售后', width: 100, align: 'left', sortable: true },
                    { field: 'SaleDate', title: '日期', width: 100, align: 'left', sortable: true },
                    { field: 'CustomerLevel', title: '客户等级', width: 100, align: 'left', sortable: true },
                    { field: 'Company', title: '公司名称', width: 200, align: 'left', sortable: true },
                    { field: 'SaleLeader', title: '销售负责人', width: 100, align: 'left', sortable: true },
                    { field: 'PayDate', title: '付款日期', width: 100, align: 'left', sortable: true },
                    { field: 'PayWay', title: '付款方式', width: 100, align: 'left', sortable: true },
                    { field: 'CustomerType', title: '客户分类', width: 100, align: 'left', sortable: true },
                    { field: 'MachineType', title: '机器分类', width: 100, align: 'left', sortable: true },
                     { field: 'Model', title: '型号', width: 100, align: 'left', sortable: true },
                      { field: 'GoodsSpec', title: '货物名称规格', width: 200, align: 'left', sortable: true },
                       { field: 'GoodsNum', title: '数量', width: 100, align: 'left', sortable: true },
                        { field: 'TaxPrice', title: '含税单价', width: 100, align: 'left', sortable: true },
                         { field: 'AllTaxPrice', title: '金额（数量*单价）', width: 100, align: 'left', sortable: true },
                          { field: 'LogisticsCost', title: '物流运费', width: 100, align: 'left', sortable: true },
                           { field: 'NoTaxPrice', title: '未税金额', width: 100, align: 'left', sortable: true },
                            { field: 'Invoice', title: '发票号码', width: 100, align: 'left', sortable: true },
                             { field: 'DeliveryNote', title: '送货单号码', width: 100, align: 'left', sortable: true },
                              { field: 'Billing', title: '结款', width: 100, align: 'left', sortable: true },
                               { field: 'MachineNo', title: '机器序列号', width: 100, align: 'left', sortable: true },
                                { field: 'OrderNo', title: '订单号', width: 100, align: 'left', sortable: true },
                                 { field: 'Remark', title: '备注', width: 100, align: 'left', sortable: true },
                                  { field: 'Contacts', title: '客户联系人', width: 100, align: 'left', sortable: true },
                                   { field: 'Phone', title: '手机号码', width: 100, align: 'left', sortable: true },
                                    { field: 'Tel', title: '固定电话', width: 100, align: 'left', sortable: true },
                                     { field: 'Email', title: '邮箱', width: 100, align: 'left', sortable: true },
                                      { field: 'QQ', title: 'QQ号码', width: 100, align: 'left', sortable: true },
                                       { field: 'CompanyAddress', title: '公司地址', width: 100, align: 'left', sortable: true },
                                        { field: 'Fax', title: '传真号', width: 100, align: 'left', sortable: true },
                                         { field: 'Bonus', title: '提成', width: 100, align: 'left', sortable: true },
                                          { field: 'LogisticsCompany', title: '物流公司', width: 100, align: 'left', sortable: true },
                ]],
                toolbar: '#toolbar1'

            }));

            $grid.datagrid('enableFilter', []);


        },

        search_data: function () {
            var stime = com.trim($("#STime").datebox("getValue"));
            var etime = com.trim($("#ETime").datebox("getValue"));

            reload({ STime: stime, ETime: etime, menucode: km.menucode });
        },
        do_import: function () {
            var checkFile = function (win) {
                var file = win.find("#importfile");
                return false;

            }
            km.template.jq_file.dialog_ext({
                title: '导入销售报表', iconCls: 'icon-standard-page-white-excel',
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
                            reload();
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
        do_export: function (exportPageData) {
            km.GetExportParams($grid, "销售报表", exportPageData);
            km.export.STime = com.trim($("#STime").datebox("getValue"));
            km.export.ETime = com.trim($("#ETime").datebox("getValue"));
            var option = $grid.datagrid("options");
            km.export.filterRules = JSON.stringify(option.filterRules);
            com.ExportToExcel("/Sale/ExportExcel", km.export);
        },
        getSelectedRow: function () {
            //获取当前选中的行
            return $grid.datagrid('getSelected');
        }

    }
}();

/*工具栏按钮事件*/
km.toolbar = {
    do_add: function () {
        //  var jq_add = $("#ele_add");
        km.template.jq_add.dialog_ext({
            title: '新增销售信息', iconCls: 'icon-standard-add',
            onOpenEx: function (win) {

            },
            onClickButton: function (win) { //保存操作
                if (com.CheckError(win)) {
                    return false;
                }
                com.jqFormOption.success = function (result) {
                    if (result == "") {
                        com.message('s', "新增销售信息成功");
                        win.dialog('destroy');
                        km.maingrid.search_data();
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
            title: '编辑销售信息', iconCls: 'icon-standard-edit',
            onOpenEx: function (win) {
                var form = win.find('#formadd');
                form.form('load', sRow);
                var price = com.MoneyStringToInt(sRow.TaxPrice);
                form.find("#TaxPrice").numberbox("setValue", price);

                price = com.MoneyStringToInt(sRow.AllTaxPrice);
                form.find("#AllTaxPrice").numberbox("setValue", price);

                price = com.MoneyStringToInt(sRow.LogisticsCost);
                form.find("#LogisticsCost").numberbox("setValue", price);

                price = com.MoneyStringToInt(sRow.NoTaxPrice);
                form.find("#NoTaxPrice").numberbox("setValue", price);
            },
            onClickButton: function (win) { //保存操作
                if (com.CheckError(win)) {
                    return false;
                }
                com.jqFormOption.success = function (result) {
                    if (result == "") {
                        com.message('s', "编辑销售信息成功");
                        win.dialog('destroy');
                        km.maingrid.search_data();
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
        com.message('c', ' <b style="color:red">确定要删除此条销售记录吗？ </b>', function (b) {
            if (b) {
                com.ajax({
                    url: km.model.urls["delete"], data: { Id: sRow.Id }, success: function (result) {
                        //layer.msg(result.emsg); 
                        if (result == "") {
                            com.message('s', "删除销售记录成功");
                            km.maingrid.search_data();
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
