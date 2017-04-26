"use strict";
km.init = function () {
    com.initbuttons($('#km_toolbar'), km.model.buttons);
    km.init_parent_model();
    km.maingrid.init();
    km.addgrid.init();
    km.addgridByProduct.init();
    //  km.toolbar.do_appendrow();

    com.CheckPer();
}




km.maingrid = function () {
    var $grid = $("#dgList");

    return {

        init: function () {
            var FormatOper = function (val, row, index) {
                return "<button  style=\"color:red\"  onclick=\"km.maingrid.deleteDetailRow(" + row.Id + ");\">删除</button>";
            }
            var QuantityWarning = function (value, row, index) {
                if (row.ShowWarn == 1) {
                    return 'background-color:#f24b21;color:#fff;';
                }
            }
            var checkCanDel = function () {
                for (var i = 0; i < km.pers.length; i++) {
                    if ("delete" == km.pers[i]) {
                        return true;
                    }
                }
                return false;
            }
            $grid.datagrid(km.gridOption({
                fitColumns: true,
                queryParams: { ElementCode: "", ShelfCode: "" },
                url: km.model.urls["pagelist"],
                columns: [[
                    { field: 'ShelfName', title: '库位', width: 60, align: 'left', sortable: true },
                    { field: 'ItemName', title: '物品名称', width: 200, align: 'left', sortable: true },
                     { field: 'ItemTypeText', title: '物品属性', width: 80, align: 'left', sortable: true },
                    { field: 'Quantity', title: '入库数量', width: 80, align: 'left', sortable: true },
                     { field: 'StockQuantity', title: '库存', width: 80, align: 'left', sortable: true, styler: QuantityWarning },
                     { field: 'UnitName', title: '单位', width: 80, align: 'left', sortable: true },
                    { field: 'Addtime', title: '入库时间', width: 150, align: 'left', sortable: true },
                    { field: 'delete', title: '操作', width: 150, align: 'left', formatter: FormatOper, hidden: !checkCanDel() }
                ]],
                toolbar: '#toolbar1',
                view: groupview,
                groupField: 'GroupGuid',
                groupFormatter: function (value, rows) {
                    return '<div style="background:yellow;padding:0 10px">入库时间：' + rows[0].Addtime + '       入库目的：' + rows[0].Reason + '</div>';
                }

            }));//end grid init
        },

        search_data: function () {
            var elementCode = com.trim($("#elementCode").val());
            var shelfCode = com.trim($("#shelfCode").val());
            this.reload({ ElementCode: elementCode, ShelfCode: shelfCode });
        },
        reload: function (params) {
            var defaults = { _t: com.settings.timestamp() };
            if (params) {
                defaults = $.extend(defaults, params);
            }
            $grid.datagrid('reload', defaults);

        },
        deleteDetailRow: function (id) {
            //  var index = this.jq.datagrid('getRowIndex', row);
            if (confirm("是否删除此条入库记录？")) {
                $.get(km.model.urls["delete"], { id: id }, function (msg) {

                    if (msg == "") {
                        com.message('s', "删除成功");
                        km.maingrid.reload();
                    }
                    else {
                        com.message('e', msg);
                    }

                })
            }

        },
        do_export: function (exportPageData) {
            km.GetExportParams($grid, "入库信息", exportPageData);
            km.export.ElementCode = com.trim($("#elementCode").val());
            km.export.ShelfCode = com.trim($("#shelfCode").val());

            com.ExportToExcel("/Stock/ExportExcelForStockIn", km.export);
        }

        //getSelectedRow: function () {
        //    //获取当前选中的行
        //    return this.jq.datagrid('getSelected');
        //},


    }
}();


km.addgrid = function () {
    var $grid = $("#dg");
    var editIndex = undefined;

    function endEditing() {
        if (editIndex == undefined) { return true }
        if ($grid.datagrid('validateRow', editIndex)) {
            var ed = $grid.datagrid('getEditor', { index: editIndex, field: 'ElementId' });
            var name = $(ed.target).combobox('getText');
            $grid.datagrid('getRows')[editIndex]['elementtext'] = name;


            ed = $grid.datagrid('getEditor', { index: editIndex, field: 'ShelfId' });
            name = $(ed.target).combobox('getText');
            $grid.datagrid('getRows')[editIndex]['shelftext'] = name;

            ed = $grid.datagrid('getEditor', { index: editIndex, field: 'UnitTypeCode' });
            name = $(ed.target).combobox('getText');
            $grid.datagrid('getRows')[editIndex]['unitname'] = name;



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
        com.message('s', "入库成功");
        window.location.reload();
        // $grid.datagrid('reload');
    }

    return {
        init: function () {
            var SetRowData = function (record, rowIndex) {
                var ed = $grid.datagrid('getEditor', { index: rowIndex, field: 'ShelfId' });
                $(ed.target).combobox('setValue', record.shelfid);

                ed = $grid.datagrid('getEditor', { index: rowIndex, field: 'UnitTypeCode' });
                $(ed.target).combobox('setValue', record.unitcode);

                //   $grid.datagrid('beginEdit', rowIndex);
                var ed = $grid.datagrid('getEditor', { index: rowIndex, field: "Quantity" });
                $(ed.target).next().children(":text").focus();
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


            $grid.datagrid({
                fitColumns: true,
                iconCls: 'icon-edit',
                singleSelect: true,
                method: 'get',
                onClickRow: onClickRow,
                url: "/Stock/GetStockInTempleteList",
                columns: [[
                    {
                        field: 'ElementId', title: '原材料', width: 200, align: 'left',
                        formatter: function (value, row) {
                            return row.elementtext;
                        },
                        editor: {
                            type: 'combobox',
                            options: {
                                valueField: 'elementid',
                                textField: 'elementtext',
                                method: 'get',
                                url: '/Element/GetElementSelectList',
                                required: true,
                                onSelect: function (record) {
                                    var selectedrow = $grid.datagrid("getSelected");
                                    var rowIndex = $grid.datagrid("getRowIndex", selectedrow);
                                    SetRowData(record, rowIndex);
                                }
                            }
                        }
                    },
                    {
                        field: 'ShelfId', title: '库位', width: 120, align: 'left',
                        formatter: function (value, row) {
                            return row.shelftext;
                        },
                        editor: {
                            type: 'combobox',
                            options: {
                                valueField: 'shelfid',
                                textField: 'shelftext',
                                method: 'get',
                                url: '/Common/GetShelfSelectList',
                                required: true,
                                readonly: true
                            }
                        }
                    },
                    { field: 'Quantity', title: '数量', width: 120, align: 'left', editor: { type: 'numberbox', options: { required: true, precision: 2 } } },
                    {
                        field: 'UnitTypeCode', title: '单位', width: 120, align: 'left',
                        formatter: function (value, row) {
                            return row.unitname;
                        },
                        editor: {
                            type: 'combobox',
                            options: {
                                valueField: 'unitcode',
                                textField: 'unitname',
                                method: 'get',
                                url: '/Common/GetUnitSelectList',
                                required: true,
                                readonly: true
                            }
                        }

                    }
                ]]

            });//end grid init
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

        do_save: function () {
            do_accept();
            var data = $grid.datagrid('getData');
            //if (!data || data.total <= 0) {
            //    com.message('e', "请先添加要入库的原材料");
            //    return false;
            //}
            com.message('c', ' <span style="color:red">是否确定要入库？ </span>', function (b, msg, a) {
                if (b) {
                    if (km.CheckNullAndQuantity(data, "Quantity")) {
                        var reason = encodeURI(com.trim($("#Reason").val()));
                        com.SaveAjaxInfos(data, km.model.urls["save"] + "?reason=" + reason, "", do_aftersave);
                    }
                }
            });
        },





    }
}();

km.addgridByProduct = function () {
    var $this = $(this), $grid = $("#dg_product"), that = this, editIndex = undefined, jq = null;
    var endEditing = function () {
        if (editIndex == undefined) {
            return true
        }
        if ($grid.datagrid('validateRow', editIndex)) {
            var ed = $grid.datagrid('getEditor', { index: editIndex, field: 'ShelfId' });
            name = $(ed.target).combobox('getText');
            $grid.datagrid('getRows')[editIndex]['shelftext'] = name;

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
        com.message('s', "入库保存成功");
        window.location.reload();
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
    var CheckExistByProduct = function (pid) {
        var gridData = $grid.datagrid("getData");
        if (gridData.total > 0) {
            for (var i = 0; i < gridData.rows.length; i++) {
                if (pid == gridData.rows[i].ProductId) {
                    return true;
                }
            }
        }
        return false;
    }
    return {
        init: function () {
            jq = $grid.datagrid({
                iconCls: 'icon-edit',
                singleSelect: false,
                method: 'get',
                onClickRow: onClickRow,
                url: "/Stock/GetStockInTempleteListByProduct",
                columns: [[
                        { field: 'IsSelect', title: '选择', width: 120, align: 'left', checkbox: true },
                        { field: 'ProductName', title: '产品序列号', width: 160, align: 'left' },
                        { field: 'ItemTypeText', title: '产品类别', width: 100, align: 'left' },
                        {
                            field: 'ShelfId', title: '库位', width: 80, align: 'left',
                            formatter: function (value, row) {
                                return row.shelftext;
                            },
                            editor: {
                                type: 'combobox',
                                options: {
                                    valueField: 'shelfid',
                                    textField: 'shelftext',
                                    data: km.shelfList,
                                    required: true
                                }
                            }
                        },
                        { field: 'Quantity', title: '数量', width: 80, align: 'left', editor: { type: 'numberbox', options: { required: true } } },
                        { field: 'UnitName', title: '单位', width: 40, align: 'left' },
                        { field: 'Remark', title: '备注', width: 120, align: 'left', editor: { type: 'textbox' } }
                ]]

            });//end grid init
            $("#ProductId").combobox('loadData', km.productList);
        },
        do_addproduct: function () {
            var id = $("#ProductId").combobox("getValue");
            if (id) {
                do_accept();
                var isExist = CheckExistByProduct(id);
                if (isExist) {
                    com.message('e', "请勿重复添加产品");
                    return false;
                }
                $.getJSON(km.model.urls["getListByProductId"], {
                    pid: id
                }, function (row) {
                    var addIndex = $grid.datagrid('getRows').length;
                    if (row) {
                        $grid.datagrid('appendRow', row);
                        if (row['IsSelect'] == 1) $grid.datagrid('checkRow', addIndex);
                        editIndex = $grid.datagrid('getRows').length - 1;
                        $grid.datagrid('selectRow', editIndex)
                                .datagrid('beginEdit', editIndex);
                    }
                });
            }
        },
        do_SaveByProduct: function () {
            var addDatas = $grid.datagrid('getChecked');
            if (!addDatas || addDatas.length <= 0) {
                com.message('e', "请先添加要入库的产品或半成品");
                return false;
            }
            com.message('c', ' <b style="color:red">是否确定要入库？ </b>', function (b) {
                if (b) {
                    do_accept();
                    if (addDatas && addDatas.length > 0) {
                        var reason = encodeURI(com.trim($("#ReasonP").val()));
                        com.SaveAjaxInfos(addDatas, km.model.urls["saveStockInByProduct"] + "?reason=" + reason, "", do_aftersave);
                    }
                }
            });
        }




    };
}();



$(km.init);
