"use strict";
km.init = function () {
    com.initbuttons($('#km_toolbar'), km.model.buttons);
    //  com.initbuttons($('#km_toolbar_detail'), km.model.detailbuttons);
    km.init_parent_model();
    km.template.init();
    km.maingrid.init();
    km.detailgrid.init();
    com.CheckPer();
}


km.template = {
    tpl_add_html: '',
    jq_add: null,
    initTemplate: function () {
        this.tpl_add_html = baidu.template('tpl_add', {});//使用baidu.template命名空间
        this.jq_add = $(this.tpl_add_html);
    },
    init: function () {
        
        this.initTemplate();
    }
};




km.detailgrid = function () {
    var $grid = $("#bomDetailGrid");

    var editIndex = undefined;
    var endEditing = function () {
        if (editIndex == undefined) { return true }
        if ($grid.datagrid('validateRow', editIndex)) {
            $grid.datagrid('endEdit', editIndex);
            editIndex = undefined;
            return true;
        } else {
            return false;
        }
    }

    var do_accept = function () {
        if (endEditing()) {
            $grid.datagrid('acceptChanges');
        }
    }

    var do_aftersave = function () {
        com.message('s', "Bom明细保存成功");
        $grid.datagrid('reload');
    }


   
    return {
        init: function () {
            var onClickRow = function (index) {
                if (editIndex != index) {
                    if (endEditing()) {
                        $grid.datagrid('selectRow', index)
                                .datagrid('beginEdit', index);
                        editIndex = index;
                    } else {
                        $grid.datagrid('selectRow', editIndex);
                    }
                }
            }

            $grid.datagrid({
                iconCls: 'icon-edit',
                singleSelect: false,
                method: 'get',
                onClickRow: onClickRow,
                rownumbers: true,
                url: km.model.urls["bomdetail"],
                queryParams: { _t: com.settings.timestamp(), bomid: 0 },
                columns: [[
                    { field: 'IsSelect', title: '部件', width: 100, align: 'left', checkbox: true },
                    { field: 'PartName', title: '部件', width: 130, align: 'left' },
                    { field: 'ElementName', title: '原材料', width: 200, align: 'left' },
                    { field: 'ShelfName', title: '库位', width: 60, align: 'left' },
                    { field: 'Quantity', title: '数量', width: 50, align: 'left', editor: { type: 'numberbox', options: { required: true, precision: 2 } } },
                    { field: 'UnitName', title: '单位', width: 80, align: 'left' },
                    { field: 'Remark', title: '备注', width: 200, align: 'left', editor: { type: 'textbox' } },
                    { field: 'Addtime', title: '添加时间', width: 150, align: 'left' },
                    { field: 'ElementId', title: '原材料Id', width: 1, hidden: true },
                    { field: 'PartCode', title: '部件代码', width: 1, hidden: true }
                ]],
                onLoadSuccess: function (data) {
                    for (var i = 0; i < data.rows.length; ++i) {
                        if (data.rows[i]['IsSelect'] == 1) $(this).datagrid('checkRow', i);
                    }
                }

            });//end grid init
            $("#PartId").combobox('loadData', km.partList);
            $("#ElementId").combobox('loadData', km.eleData);
            com.mask($('#east_panel'), true);
        },

        reload: function (params) {
            var defaults = { _t: com.settings.timestamp() };
            if (params) {
                defaults = $.extend(defaults, params);
            }
            editIndex = undefined;
            $grid.datagrid('reload', defaults);

        },
        clear: function () {
            editIndex = undefined;
            km.bomid = 0;
            this.reload({ bomid: 0 });
        },

        do_addpart: function () {
            var partid = $("#PartId").combobox("getValue");

            if (partid) {
                $.getJSON(km.model.urls["getpartdetailele"], { partid: partid }, function (data) {
                    km.appendCheckRow(data, $grid);
                });
            }
        },
        do_addelement: function () {
            var eleid = $("#ElementId").combobox("getValue");
            if (eleid) {
                $.getJSON(km.model.urls["geteledetailforbom"], { eleid: eleid }, function (data) {
                    km.appendCheckRow(data, $grid);
                });
            }
        },
        do_savebomdetail: function () {
            do_accept();
            var data = $grid.datagrid('getChecked');
            if (km.CheckNullAndQuantity(data, "Quantity")) {
               
                com.SaveAjaxInfos(data, km.model.urls["savebomdetail"] + "?bomid=" + km.bomid, "", do_aftersave);
            }
        },
        do_export: function (exportPageData) {
           
            km.export.ColumnTitles = "型号,原材料,部件,数量,数量单位,库位,备注,添加时间";
            km.export.Columns = "BomName,ElementName,PartName,Quantity,UnitName,ShelfName,Remark,Addtime";
            km.export.FileName = "Bom明细";
            km.export.ExportPageData = exportPageData == 1 ? true : false;
            km.export.bomid = km.bomid;
   
            com.ExportToExcel("/Bom/ExportExcel", km.export);
        }

    }

}();

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
            queryParams: { _t: com.settings.timestamp(), ModelId: 0 },
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
                { field: 'Name', title: 'Bom名称', width: 200, align: 'left', sortable: true },
                { field: 'ModelName', title: '型号', width: 120, align: 'left', sortable: true },
                { field: 'Remark', title: '备注', width: 120, align: 'left', sortable: true },
                { field: 'Addtime', title: '创建时间', width: 180, align: 'left', sortable: true }

            ]],
            onLoadSuccess: function () {
                //     alert('load data successfully!');
            },
            onLoadError: function (a, b, c) {
                alert('ajax执行出错');

            },
            onClickRow: function (index, row) {
                km.maingrid.ShowBomDetail(row.Id);
            },
            toolbar: '#toolbar1'
        });

        $("#SearchModel").combobox('loadData', km.modelList);
    },

    search_data: function () {
        var modelid = com.trim($("#SearchModel").combobox("getValue"));
        var s_element = com.trim($("#s_element").val());
        this.reload({ ModelId: modelid, ElementNameOrCode: s_element });
    },
    reload: function (params) {
        var defaults = { _t: com.settings.timestamp() };
        if (params) {
            defaults = $.extend(defaults, params);
        }
        this.jq.datagrid('reload', defaults);
        km.detailgrid.clear();
    },
    deleteRow: function (row) {
        var index = this.jq.datagrid('getRowIndex', row);
        this.jq.datagrid('deleteRow', index);
    },
    getSelectedRow: function () {
        //获取当前选中的行
        return this.jq.datagrid('getSelected');
    },
    ShowBomDetail: function (id) {
        com.mask($('#east_panel'), false);
        km.bomid = id;
        km.detailgrid.reload({ bomid: id });
        //$.getJSON(km.model.urls["partdetail"], { id: id }, function (data) {

        //})
    }
};

/*工具栏按钮事件*/
km.toolbar = {
    $grid: $("#bomDetailGrid"),
    do_add: function () {
        //  var jq_add = $("#ele_add");
        km.template.jq_add.dialog_ext({
            title: '新增Bom', iconCls: 'icon-standard-add',
            onOpenEx: function (win) {
                win.find('#ModelId').combobox('loadData', km.modelList);
                //  win.find('#ShelfId').combobox('reload', '/Common/GetShelfSelectList');
            },
            onClickButton: function (win) { //保存操作
                if (com.CheckError(win)) {
                    return false;
                }
                com.jqFormOption.success = function (result) {
                    if (result == "") {
                        com.message('s', "新增Bom成功");
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
            title: '编辑Bom【' + sRow.Name + '】', iconCls: 'icon-standard-edit',
            onOpenEx: function (win) {
                win.find('#ModelId').combobox('loadData', km.modelList);
                win.find('#formadd').form('load', sRow);
            },
            onClickButton: function (win) { //保存操作
                if (com.CheckError(win)) {
                    return false;
                }
                com.jqFormOption.success = function (result) {
                    if (result == "") {
                        com.message('s', "编辑Bom成功");
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
        com.message('c', ' <b style="color:red">确定要删除Bom【' + sRow.Name + '】吗？ </b>', function (b) {
            if (b) {
                com.ajax({
                    url: km.model.urls["delete"], data: { Id: sRow.Id }, success: function (result) {
                        //layer.msg(result.emsg); 
                        if (result == "") {
                            com.message('s', "删除Bom【" + sRow.Name + "】成功");
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
