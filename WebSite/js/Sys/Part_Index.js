"use strict";
km.init = function () {
    com.initbuttons($('#km_toolbar'), km.model.buttons);
    com.initbuttons($('#km_toolbar_detail'), km.model.detailbuttons);
    km.init_parent_model();
    km.template.init();
    km.maingrid.init();
    km.detailgrid.init();
    km.bomgrid.init();
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


var editIndex = undefined;


km.detailgrid = function () {
    var $grid = $("#partDetailGrid"), editIndex = undefined;


    var endEditing = function () {
        if (editIndex == undefined) { return true }
        if ($grid.datagrid('validateRow', editIndex)) {
            var ed = $grid.datagrid('getEditor', { index: editIndex, field: 'ElementId' });
            var name = $(ed.target).combobox('getText');
            $grid.datagrid('getRows')[editIndex]['ElementName'] = name;




            $grid.datagrid('endEdit', editIndex);
            editIndex = undefined;
            return true;
        } else {
            return false;
        }
    }
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

    var do_aftersave = function () {
        com.message('s', "部件明细保存成功");
        $grid.datagrid('reload');
        $("#PartId").combobox('clear');
    }


    var getPartList = function (partId) {
        var list = [];
        if (partId) {
            for (var i = 0; i < km.partList.length; i++) {
                if (km.partList[i].Id != partId) {
                    list.push(km.partList[i]);
                }

            }
        }
        return list;
    }

    return {
        init: function () {

            $grid.datagrid({
                iconCls: 'icon-edit',
                singleSelect: false,
                method: 'get',
                onClickRow: onClickRow,
                rownumbers: true,
                url: km.model.urls["partdetail"],
                queryParams: { _t: com.settings.timestamp(), partid: 0 },
                columns: [[
                    { field: 'IsSelect', title: '部件', width: 100, align: 'left', checkbox: true },
                    {
                        field: 'ElementId', title: '原材料', width: 350, align: 'left',
                        formatter: function (value, row) {
                            return row.ElementName;
                        },
                        editor: {
                            type: 'combobox',
                            options: {
                                valueField: 'ElementId',
                                textField: 'ElementName',
                                data: km.eleData,
                                required: true
                            }
                        }
                    },

                    { field: 'Quantity', title: '数量', width: 120, align: 'left', editor: { type: 'numberbox', options: { required: true, precision: 2 } } }

                ]],
                onLoadSuccess: function (data) {
                    for (var i = 0; i < data.rows.length; ++i) {
                        if (data.rows[i]['IsSelect'] == 1) $(this).datagrid('checkRow', i);
                    }
                }

            });//end grid init
            
        },
        reload: function (params) {
            var defaults = { _t: com.settings.timestamp() };
            if (params) {
                defaults = $.extend(defaults, params);
            }
            editIndex = undefined;
            $grid.datagrid('reload', defaults);
            $("#PartId").combobox('loadData', getPartList(defaults.partid));
            $("#PartId").combobox('clear');
        },
        do_appendrow: function () {

            if (endEditing()) {
                $grid.datagrid('appendRow', {});
                editIndex = $grid.datagrid('getRows').length - 1;
                $grid.datagrid('selectRow', editIndex)
                        .datagrid('beginEdit', editIndex);
            }
        },
        do_removerow: function () {
            if (editIndex == undefined) { return }
            $grid.datagrid('cancelEdit', editIndex)
                    .datagrid('deleteRow', editIndex);
            editIndex = undefined;
        },
        do_accept: function () {
            if (endEditing()) {
                $grid.datagrid('acceptChanges');
            }
        },
        do_save: function () {
            this.do_accept();
            var data = $grid.datagrid('getChecked');
            if (data && data.length > 0) {
                var dup = km.checkEleDup(data, "ElementId");
                if (dup)
                {
                    com.message('e', "列表中原材料不能重复");
                    return false;
                }

                com.SaveAjaxInfos(data, km.model.urls["savepartdetail"] + "?partid=" + km.partid, "部件明细保存成功", do_aftersave);
            }
        },
        do_copypart: function () {
            var partid = $("#PartId").combobox("getValue");

            if (partid) {
                this.do_accept();
                //var isExist = CheckExistByBom(bomid);
                //if (isExist) {
                //    com.message('e', "请勿重复添加Bom");
                //    return false;
                //}
              
                $.getJSON(km.model.urls["partdetail"], {
                    partId: partid
                }, function (data) {
                    //  alert(JSON.stringify(data));
                    var indexMax = $grid.datagrid('getRows').length - 1;
                    for (var i = 0; i < data.length; i++) {
                        $grid.datagrid('appendRow', data[i]);
                        indexMax++;
                        if (data[i]['IsSelect'] == 1) $grid.datagrid('checkRow', indexMax);
                    }

                });
            }
        }


    }
}();

km.maingrid = {
    jq: null,
    init: function () {
        this.jq = $("#maingrid").datagrid(km.gridOption({
            queryParams: { Name: "" },
            url: km.model.urls["pagelist"],
            columns: [[
                { field: 'Name', title: '部件名称', width: 180, align: 'left', sortable: true },
                { field: 'Code', title: '部件代码', width: 120, align: 'left', sortable: true },
                { field: 'Remark', title: '备注', width: 120, align: 'left', sortable: true },
                { field: 'Addtime', title: '创建时间', width: 150, align: 'left', sortable: true }

            ]],
            onClickRow: function (index, row) {
                //   km.settings.partCode = row.Code;
                com.mask($('#east_panel'), false);
                km.maingrid.ShowPartDetail(row.Id, row.Code);
            },
        }));//end grid init
        com.mask($('#east_panel'), true);
    },

    search_data: function () {
          var name = com.trim($("#partName").val());
          var code = com.trim($("#partCode").val());
          this.reload({ PartName: name, PartCode: code });
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
    },
    ShowPartDetail: function (id, Code) {
        km.partid = id;
        km.detailgrid.reload({ partid: id });
        km.bomgrid.reload({ partCode: Code });
        //$.getJSON(km.model.urls["partdetail"], { id: id }, function (data) {

        //})
    }
};


km.bomgrid = function () {
    var $grid = $("#bomGrid");
    return {
        init: function () {
            $grid.datagrid(km.gridOption({
                fitColumns: true,
                pagination: false,
                queryParams: { partCode: "" },
                url: km.model.urls["getBomByPart"],
                columns: [[
               { field: 'BomName', title: 'Bom名称', width: 220, align: 'left' },
               { field: 'Remark', title: '备注', width: 300, align: 'left' }

                ]],
                view: detailview,
                detailFormatter: function (index, row) {
                    return '<div style="padding:5px"><table class="ddv"></table></div>';
                },
                onExpandRow: function (index, row) {
                    var ddv = $(this).datagrid('getRowDetail', index).find('table.ddv');
                    ddv.datagrid({
                        queryParams: { _t: com.settings.timestamp(), bomId: row.Id },
                        url: km.model.urls["getBomDetail"],
                        fitColumns: true,
                        singleSelect: true,
                        rownumbers: true,
                        border: false,
                        loadMsg: '',
                        height: 'auto',
                        columns: [[
                            { field: 'PartName', title: '部件名称', width: 100, align: 'left' },
                            { field: 'ElementName', title: '原材料名称', width: 200, align: 'left' },
                            { field: 'Quantity', title: '数量', width: 100, align: 'left' },
                            { field: 'UnitName', title: '数量单位', width: 100, align: 'left' },
                            { field: 'Remark', title: '备注', width: 200, align: 'left' }
                        ]],
                        onResize: function () {
                            $grid.datagrid('fixDetailRowHeight', index);
                        },
                        onLoadSuccess: function () {
                            setTimeout(function () {
                                $grid.datagrid('fixDetailRowHeight', index);
                            }, 0);
                        }
                    });
                    $grid.datagrid('fixDetailRowHeight', index);
                }

            }));

        },
        reload: function (params) {
            var defaults = { _t: com.settings.timestamp() };
            if (params) {
                defaults = $.extend(defaults, params);
            }
            $grid.datagrid('reload', defaults);

        }
    }
}();

/*工具栏按钮事件*/
km.toolbar = {
    $grid: $("#partDetailGrid"),
    do_add: function () {
        //  var jq_add = $("#ele_add");
        km.template.jq_add.dialog_ext({
            title: '新增部件种类', iconCls: 'icon-standard-add',
            onOpenEx: function (win) {
                //  win.find('#UnitTypeCode').combobox('reload', '/Common/GetUnitSelectList');
                //  win.find('#ShelfId').combobox('reload', '/Common/GetShelfSelectList');
            },
            onClickButton: function (win) { //保存操作
                if (com.CheckError(win)) {
                    return false;
                }
                com.jqFormOption.success = function (result) {
                    if (result == "") {
                        com.message('s', "新增部件成功");
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
            title: '编辑部件【' + sRow.Name + '】', iconCls: 'icon-standard-edit',
            onOpenEx: function (win) {
                //  win.find('#UnitTypeCode').combobox('reload', '/Common/GetUnitSelectList');
                // win.find('#ShelfId').combobox('reload', '/Common/GetShelfSelectList');
                win.find('#formadd').form('load', sRow);
            },
            onClickButton: function (win) { //保存操作
                if (com.CheckError(win)) {
                    return false;
                }
                com.jqFormOption.success = function (result) {
                    if (result == "") {
                        com.message('s', "编辑部件成功");
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
        com.message('c', ' <b style="color:red">确定要删除部件【' + sRow.Name + '】吗？ </b>', function (b) {
            if (b) {
                com.ajax({
                    url: km.model.urls["delete"], data: { Id: sRow.Id }, success: function (result) {
                        //layer.msg(result.emsg); 
                        if (result == "") {
                            com.message('s', "删除部件【" + sRow.Name + "】成功");
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
