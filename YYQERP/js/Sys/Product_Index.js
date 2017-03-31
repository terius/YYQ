"use strict";
km.init = function () {
    com.initbuttons($('#km_toolbar'), km.model.buttons);
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
            queryParams: { _t: com.settings.timestamp(), Aliases: "", ModelName: "" },
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
                { field: 'ModelName', title: '型号', width: 160, align: 'left', sortable: true },
                { field: 'Aliases', title: '名称', width: 120, align: 'left', sortable: true },
                { field: 'CreateDate', title: '生产日期', width: 160, align: 'left', sortable: true },
                { field: 'ItemTypeText', title: '类型', width: 120, align: 'left', sortable: true }

            ]],
            onLoadSuccess: function () {
                //     alert('load data successfully!');
            },
            onLoadError: function (a, b, c) {
                alert('ajax执行出错');

            },
            toolbar: '#toolbar1',
            onClickRow: function (index, row) {
                km.maingrid.ShowDetail(row.Id);
            },
        });//end grid init
    },

    search_data: function () {
        var Aliases = com.trim($("#SearchAliases").val());
        var ModelName = com.trim($("#SearchModel").val());
        this.reload({ Aliases: Aliases, ModelName: ModelName });
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
    ShowDetail: function (id) {
        com.mask($('#east_panel'), false);
        km.pid = id;
        km.detailgrid.reload({ pid: id });
    }
};

km.detailgrid = function () {
    var $grid = $("#detailgrid"), editIndex = undefined;

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
    var CheckExistByBom = function (bomid) {

        var gridData = $grid.datagrid("getData");
        if (gridData.total > 0) {
            for (var i = 0; i < gridData.rows.length; i++) {
                if (bomid == gridData.rows[i].BomId) {
                    return true;
                }
            }
        }
        return false;
    }

    var CheckExistByEle = function (eleid) {
        var gridData = $grid.datagrid("getData");
        if (gridData.total > 0) {
            for (var i = 0; i < gridData.rows.length; i++) {
                if (eleid == gridData.rows[i].ElementId) {
                    return true;
                }
            }
        }
        return false;
    }

    var CheckExistByProduct = function (pid) {
        var gridData = $grid.datagrid("getData");
        if (gridData.total > 0) {
            for (var i = 0; i < gridData.rows.length; i++) {
                if (pid == gridData.rows[i].HalfProductId) {
                    return true;
                }
            }
        }
        return false;
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
        com.message('s', "产品明细保存成功");
        window.location.reload();
    }
    return {
        init: function () {
            $grid.datagrid({
                iconCls: 'icon-edit',
                singleSelect: false,
                method: 'get',
                rownumbers: true,
                url: km.model.urls["pdetail"],
                onClickRow: onClickRow,
                queryParams: { _t: com.settings.timestamp(), pid: 0 },
                columns: [[
                    { field: 'IsSelect', title: '选择', width: 20, align: 'left', checkbox: true },
                    { field: 'ElementName', title: '原材料', width: 200, align: 'left' },
                    { field: 'Quantity', title: '数量', width: 80, align: 'left', editor: { type: 'numberbox', options: { required: true, precision: 2 } } },
                    { field: 'UnitName', title: '单位', width: 80, align: 'left' },
                    { field: 'ItemName', title: '所属类别', width: 300, align: 'left' }
                ]],
                onLoadSuccess: function (data) {
                    for (var i = 0; i < data.rows.length; ++i) {
                        if (data.rows[i]['IsSelect'] == 1) $(this).datagrid('checkRow', i);
                    }
                }

            });//end grid init
            com.mask($('#east_panel'), true);
            $("#BomId").combobox('loadData', km.bomList);
            $("#ElementId").combobox('loadData', km.eleData);
            $("#HalfProductId").combobox('loadData', km.halfProductList);
        },

        reload: function (params) {
            var defaults = { _t: com.settings.timestamp() };
            if (params) {
                defaults = $.extend(defaults, params);
            }
            $grid.datagrid('reload', defaults);

        },
        clear: function () {
            km.pid = 0;
            this.reload({ pid: 0 });
        },
        do_addbom: function () {
            var bomid = $("#BomId").combobox("getValue");

            if (bomid) {
                do_accept();
                var isExist = CheckExistByBom(bomid);
                if (isExist) {
                    com.message('e', "请勿重复添加Bom");
                    return false;
                }
                $.getJSON(km.model.urls["getListByBomId"], {
                    bomid: bomid
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
        },
        do_addelement: function () {
            var eleid = $("#ElementId").combobox("getValue");
            if (eleid) {
                do_accept();
                var isExist = CheckExistByEle(eleid);
                if (isExist) {
                    com.message('e', "请勿重复添加原材料");
                    return false;
                }
                $.getJSON(km.model.urls["getListByEleId"], {
                    eleid: eleid
                }, function (row) {
                    //  alert(JSON.stringify(data));
                    var indexMax = $grid.datagrid('getRows').length;
                    if (row) {
                        $grid.datagrid('appendRow', row);
                        if (row['IsSelect'] == 1) $grid.datagrid('checkRow', indexMax);
                    }
                });
            }
        },
        do_addhalfproduct: function () {
            var id = $("#HalfProductId").combobox("getValue");
            if (id) {
                do_accept();
                var isExist = CheckExistByProduct(id);
                if (isExist) {
                    com.message('e', "请勿重复添加半成品");
                    return false;
                }
                $.getJSON(km.model.urls["getListByProductId"], {
                    pid: id
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
        },
        do_saveProductDetail: function () {
            var addDatas = $grid.datagrid('getChecked');
            if (!addDatas || addDatas.length <= 0) {
                com.message('e', "请先添加的原材料或产品或半成品");
                return false;
            }
            com.message('c', ' <b style="color:red">是否确定保存？ </b>', function (b) {
                if (b) {
                    do_accept();
                    if (addDatas && addDatas.length > 0) {
                        com.SaveAjaxInfos(addDatas, km.model.urls["saveProductDetail"] + "?pid=" + km.pid, "", do_aftersave);
                    }
                }
            });

        }

    };
}();

/*工具栏按钮事件*/
km.toolbar = {
    do_add: function () {
        //  var jq_add = $("#ele_add");
        km.template.jq_add.dialog_ext({
            title: '新增产品种类', iconCls: 'icon-standard-add',
            onOpenEx: function (win) {
                win.find('#ModelId').combobox('loadData', km.modelList);
                win.find('#UnitTypeCode').combobox('loadData', km.unitList);
                //  win.find('#ShelfId').combobox('reload', '/Common/GetShelfSelectList');
            },
            onClickButton: function (win) { //保存操作
                if (com.CheckError(win)) {
                    return false;
                }
                com.jqFormOption.success = function (result) {
                    if (result == "") {
                        com.message('s', "新增产品成功");
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
            title: '编辑产品【' + sRow.Aliases + '】', iconCls: 'icon-standard-edit',
            onOpenEx: function (win) {
                win.find('#ModelId').combobox('loadData', km.modelList);
                win.find('#UnitTypeCode').combobox('loadData', km.unitList);
                win.find('#formadd').form('load', sRow);
            },
            onClickButton: function (win) { //保存操作
                if (com.CheckError(win)) {
                    return false;
                }
                com.jqFormOption.success = function (result) {
                    if (result == "") {
                        com.message('s', "编辑产品成功");
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
        com.message('c', ' <b style="color:red">确定要删除产品【' + sRow.Code + '】吗？ </b>', function (b) {
            if (b) {
                com.ajax({
                    url: km.model.urls["delete"], data: { Id: sRow.Id }, success: function (result) {
                        //layer.msg(result.emsg); 
                        if (result == "") {
                            com.message('s', "删除产品【" + sRow.Code + "】成功");
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
