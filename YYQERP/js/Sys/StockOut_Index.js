

"use strict";
km.init = function () {
    com.initbuttons($('#km_toolbar'), km.model.buttons);
    km.init_parent_model();
    //   km.template.init();
    km.maingrid.init();
    km.addgrid.init();
    km.addgridByProduct.init();
    com.CheckPer();
    //  km.toolbar.do_appendrow();
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
                    { field: 'ElementName', title: '物品名称', width: 200, align: 'left', sortable: true },
                    { field: 'ItemTypeText', title: '物品属性', width: 70, align: 'left', sortable: true },
                    { field: 'Quantity', title: '出库数量', width: 70, align: 'left', sortable: true },
                    { field: 'StockQuantity', title: '当前库存', width: 70, align: 'left', sortable: true, styler: QuantityWarning },
                    { field: 'UnitName', title: '单位', width: 50, align: 'left', sortable: true },
                    { field: 'Addtime', title: '出库时间', width: 150, align: 'left', sortable: true },
                    { field: 'Reason', title: '出库原因', width: 150, align: 'left', sortable: true, hidden: true },
                    { field: 'delete', title: '操作', width: 150, align: 'left', formatter: FormatOper, hidden: !checkCanDel() }
                ]],
                toolbar: '#toolbar1',
                view: groupview,
                groupField: 'GroupGuid',
                groupFormatter: function (value, rows) {
                    return '<div style="background:yellow;padding:0 10px">出库时间：' + rows[0].Addtime + ' 出库目的：' + rows[0].Reason + '</div>';
                }

            }));


        },

        search_data: function () {
            var elementCode = com.trim($("#elementCode").val());
            var shelfCode = com.trim($("#shelfCode").val());

            km.maingrid.reload({ ElementCode: elementCode, ShelfCode: shelfCode });
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
            if (confirm("是否删除此条出库记录？")) {
                $.get(km.model.urls["deletestockout"], { id: id }, function (msg) {

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
            km.GetExportParams($grid, "出库信息", exportPageData);
            km.export.ElementCode = com.trim($("#elementCode").val());
            km.export.ShelfCode = com.trim($("#shelfCode").val());

            com.ExportToExcel("/Stock/ExportExcelForStockOut", km.export);
        }



    }
}();


//km.maingrid = function () {
//    var $grid = $("#dgList");

//    return {
//        init: function () {
//            var mainFormatOper = function (val, row, index) {
//                return "<button  style=\"color:red\"  onclick=\"km.maingrid.deleteMainRow(" + row.Id + ");\">删除</button>";
//            }
//            $grid.datagrid(km.gridOption({
//                fitColumns: true,
//                queryParams: { ElementCode: "", ShelfCode: "" },
//                url: km.model.urls["pagelist"],
//                columns: [[
//                     { field: 'Addtime', title: '出库时间', width: 100, align: 'left', sortable: true },
//                    { field: 'Reason', title: '出库目的', width: 300, align: 'left', sortable: true },
//                     { field: 'delete', title: '操作', width: 100, align: 'left', formatter: mainFormatOper }
//                ]],
//                toolbar: '#toolbar1',
//                view: detailview,
//                detailFormatter: function (index, row) {
//                    return '<div style="padding:5px"><table class="ddv"></table></div>';
//                },
//                onExpandRow: function (index, row) {



//                    var formatOper = function (val, row, index) {
//                        return "<button  style=\"color:red\"  onclick=\"km.maingrid.deleteDetailRow(" + row.Id + ");\">删除</button>";
//                    }
//                    var QuantityWarning = function (value, row, index) {
//                        if (row.ShowWarn == 1) {
//                            return 'background-color:#f24b21;color:#fff;';
//                        }
//                    }

//                    var ddv = $(this).datagrid('getRowDetail', index).find('table.ddv');
//                    ddv.datagrid({
//                        queryParams: { _t: com.settings.timestamp(), bomId: row.Id },
//                        data: row.Details,
//                        fitColumns: true,
//                        singleSelect: true,
//                        rownumbers: true,
//                        border: false,
//                        loadMsg: '',
//                        height: 'auto',
//                        columns: [[
//                            { field: 'ShelfName', title: '库位', width: 60, align: 'left', sortable: true },
//                            { field: 'ElementName', title: '物品名称', width: 200, align: 'left', sortable: true },
//                            { field: 'ItemTypeText', title: '物品属性', width: 70, align: 'left', sortable: true },
//                            { field: 'Quantity', title: '出库数量', width: 70, align: 'left', sortable: true },
//                            { field: 'StockQuantity', title: '当前库存', width: 70, align: 'left', sortable: true, styler: QuantityWarning },
//                            { field: 'UnitName', title: '单位', width: 50, align: 'left', sortable: true },
//                            { field: 'Addtime', title: '出库时间', width: 150, align: 'left', sortable: true },
//                            { field: 'delete', title: '操作', width: 150, align: 'left', formatter: formatOper }
//                        ]],
//                        onResize: function () {
//                            $grid.datagrid('fixDetailRowHeight', index);
//                        },
//                        onLoadSuccess: function () {
//                            setTimeout(function () {
//                                $grid.datagrid('fixDetailRowHeight', index);
//                            }, 0);
//                        }
//                    });
//                    $grid.datagrid('fixDetailRowHeight', index);
//                }
//            }));


//        },

//        search_data: function () {
//            var elementCode = com.trim($("#elementCode").val());
//            var shelfCode = com.trim($("#shelfCode").val());

//            km.maingrid.reload({ ElementCode: elementCode, ShelfCode: shelfCode });
//        },
//        reload: function (params) {
//            var defaults = { _t: com.settings.timestamp() };
//            if (params) {
//                defaults = $.extend(defaults, params);
//            }
//            $grid.datagrid('reload', defaults);

//        },
//        deleteDetailRow: function (id) {
//            //  var index = this.jq.datagrid('getRowIndex', row);
//            if (confirm("是否删除此条出库记录？")) {
//                $.get(km.model.urls["deletestockout"], { id: id }, function (msg) {

//                    if (msg == "") {
//                        com.message('s', "删除成功");
//                        km.maingrid.reload();
//                    }
//                    else {
//                        com.message('e', msg);
//                    }

//                })
//            }

//        },
//        deleteMainRow: function (id) {
//            //  var index = this.jq.datagrid('getRowIndex', row);
//            if (confirm("是否删除此条出库记录？")) {
//                $.get(km.model.urls["deletestockoutByMain"], { id: id }, function (msg) {

//                    if (msg == "") {
//                        com.message('s', "删除成功");
//                        km.maingrid.reload();
//                    }
//                    else {
//                        com.message('e', msg);
//                    }

//                })
//            }

//        }


//    }
//}();


km.addgrid = function () {
    var $this = $(this), $grid = $("#dg"), that = this, editIndex = undefined, jq = null;

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
        com.message('s', "出库保存成功");
        window.location.reload();
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

    var QuantityError = function (value, row, index) {
        if (value === "未入库" || row.Quantity > value) {
            return 'background-color:#ffee00;color:red;';
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

    return {
        init: function () {
            this.jq = $grid.datagrid({
                iconCls: 'icon-edit',
                singleSelect: false,
                method: 'get',
                onClickRow: onClickRow,
                url: "/Stock/GetStockOutTempleteList",
                columns: [[
                        { field: 'IsSelect', title: '选择', width: 20, align: 'left', checkbox: true },
                        { field: 'BomName', title: 'Bom', width: 220, align: 'left' },
                        { field: 'PartName', title: '部件', width: 120, align: 'left' },
                        { field: 'ElementName', title: '原材料', width: 200, align: 'left' },
                        { field: 'ShelfName', title: '库位', width: 60, align: 'left' },
                        { field: 'Quantity', title: '数量', width: 80, align: 'left', editor: { type: 'numberbox', options: { required: true, precision: 2 } } },
                        { field: 'StockQuantity', title: '库存数量', width: 80, align: 'left', styler: QuantityError },
                        { field: 'UnitName', title: '单位', width: 40, align: 'left' },
                        { field: 'Remark', title: '备注', width: 300, align: 'left', editor: { type: 'textbox' } },
                        { field: 'ElementId', title: '原材料Id', width: 1, align: 'left', hidden: true },
                        { field: 'BomId', title: 'BomId', width: 1, align: 'left', hidden: true },
                        { field: 'ShelfId', title: '库位Id', width: 1, align: 'left', hidden: true }
                ]]

            });//end grid init

            $("#BomId").combobox('loadData', km.bomList);
            $("#ElementId").combobox('loadData', km.eleData);
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
                var num = $("#BomNum").val() || 1;
                $.getJSON(km.model.urls["getListByBomId"], {
                    bomid: bomid, num: num
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
        do_savestockout: function () {
            var addDatas = $grid.datagrid('getChecked');
            if (!addDatas || addDatas.length <= 0) {
                com.message('e', "请先添加要出库的原材料或产品");
                return false;
            }
            com.message('c', ' <span style="color:red">是否确定要出库？ </span>', function (b, msg, a) {
                if (b) {

                    do_accept();
                    if (addDatas && addDatas.length > 0) {
                        var reason = encodeURI(com.trim($("#Reason").val()));
                        com.SaveAjaxInfos(addDatas, km.model.urls["savestockout"] + "?reason=" + reason, "Bom明细保存成功", do_aftersave);
                    }
                }
            });

        },
        do_clear: function () {
            $grid.datagrid("reload");
        }

    }

}();


km.addgridByProduct = function () {
    var $this = $(this), $grid = $("#dg_product"), that = this, editIndex = undefined, jq = null;
    var endEditing = function () {
        if (editIndex == undefined) {
            return true
        }
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
        com.message('s', "出库保存成功");
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

    var QuantityError = function (value, row, index) {
        if (value === "未入库" || row.Quantity > value) {
            return 'background-color:#ffee00;color:red;';
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
                url: "/Stock/GetStockOutTempleteListByProduct",
                columns: [[
                        { field: 'IsSelect', title: '选择', width: 120, align: 'left', checkbox: true },
                        { field: 'ProductName', title: '产品序列号', width: 160, align: 'left' },
                        { field: 'ShelfName', title: '库位', width: 60, align: 'left' },
                        { field: 'Quantity', title: '数量', width: 80, align: 'left' },
                        { field: 'StockQuantity', title: '库存数量', width: 80, align: 'left', styler: QuantityError },
                        { field: 'UnitName', title: '单位', width: 40, align: 'left' },
                        { field: 'Remark', title: '备注', width: 120, align: 'left', editor: { type: 'textbox' } },
                        { field: 'ProductId', title: '产品Id', width: 1, align: 'left', hidden: true },
                        { field: 'ShelfId', title: '库位Id', width: 1, align: 'left', hidden: true }
                ]]

            });//end grid init
            $("#ProductId").combobox('loadData', km.productList);
        },
        do_addproduct: function () {
            var id = $("#ProductId").combobox("getValue");
            var dd = $("#ProductId").combobox("getData");
            var stockId = "0";
            for (var i = 0; i < dd.length; i++) {
                if (dd[i].id == id) {
                    stockId = dd[i].extdata;
                    break;
                }
            }
            if (id) {
                do_accept();
                var isExist = CheckExistByProduct(id);
                if (isExist) {
                    com.message('e', "请勿重复添加产品");
                    return false;
                }
                $.getJSON(km.model.urls["getListByProductId"], {
                    stockid: stockId
                }, function (row) {
                    var addIndex = $grid.datagrid('getRows').length;
                    if (row) {
                        $grid.datagrid('appendRow', row);
                        if (row['IsSelect'] == 1) $grid.datagrid('checkRow', addIndex);
                    }
                });
            }
        },
        do_SaveByProduct: function () {
            var addDatas = $grid.datagrid('getChecked');
            if (!addDatas || addDatas.length <= 0) {
                com.message('e', "请先添加要出库的产品或半成品");
                return false;
            }
            com.message('c', ' <b style="color:red">是否确定要出库？ </b>', function (b) {
                if (b) {
                    do_accept();
                    if (addDatas && addDatas.length > 0) {
                        var reason = encodeURI(com.trim($("#ReasonP").val()));
                        com.SaveAjaxInfos(addDatas, km.model.urls["savestockoutByProduct"] + "?reason=" + reason, "", do_aftersave);
                    }
                }
            });
        },
        do_clear: function () {
            $grid.datagrid("reload");
        }




    };
}();



/*工具栏按钮事件*/
//km.toolbar = {
//    $grid: $("#dg"),
//    do_accept: function () {
//        if (endEditing()) {
//            $('#dg').datagrid('acceptChanges');
//        }
//    },
//    do_addbom: function () {
//        var bomid = $("#BomId").combobox("getValue");

//        if (bomid) {
//            $.getJSON(km.model.urls["getListByBomId"], {
//                bomid: bomid
//            }, function (data) {
//                //  alert(JSON.stringify(data));
//                var indexMax = $("#dg").datagrid('getRows').length - 1;
//                for (var i = 0; i < data.length; i++) {
//                    $("#dg").datagrid('appendRow', data[i]);
//                    indexMax++;
//                    if (data[i]['IsSelect'] == 1) $("#dg").datagrid('checkRow', indexMax);
//                }

//            });
//        }
//    },
//    do_addelement: function () {
//        var eleid = $("#ElementId").combobox("getValue");
//        if (eleid) {
//            $.getJSON(km.model.urls["getListByEleId"], {
//                eleid: eleid
//            }, function (row) {
//                //  alert(JSON.stringify(data));
//                var indexMax = $("#dg").datagrid('getRows').length;
//                if (row) {
//                    $("#dg").datagrid('appendRow', row);
//                    if (row['IsSelect'] == 1) $("#dg").datagrid('checkRow', indexMax);
//                }
//            });
//        }
//    },
//    do_savestockout: function () {
//        this.do_add();

//    },
//    do_aftersave: function () {
//        com.message('s', "出库保存成功");
//        window.location.reload();
//    },
//    do_add: function () {
//        // var selectValue = "0";
//        var addDatas = $('#dg').datagrid('getChecked');
//        if (!addDatas || addDatas.length <= 0) {
//            com.message('e', "请先添加要出库的原材料或产品");
//            return false;
//        }
//        com.message('c', ' <b style="color:red">是否确定要出库？ </b>', function (b) {
//            if (b) {
//                km.toolbar.do_accept();
//                // var data = $('#dg').datagrid('getChecked');
//                if (addDatas && addDatas.length > 0) {
//                    com.SaveAjaxInfos(addDatas, km.model.urls["savestockout"], "Bom明细保存成功", km.toolbar.do_aftersave);
//                }
//            }
//        });

//        //km.template.jq_add.dialog_ext({
//        //    title: '出库用途', iconCls: 'icon-standard-add',
//        //    onOpenEx: function (win) {
//        //        var userForChange = function (newValue, oldValue) {
//        //            //   alert(newValue);
//        //            if (newValue == "0") {
//        //                win.find(".cla").hide();
//        //            }
//        //            else if (newValue == "1") {
//        //                win.find("#tr_Product").hide();
//        //                win.find("#tr_Aliases").show();
//        //                win.find("#tr_ModelId").show();
//        //            }
//        //            else {
//        //                win.find("#tr_Product").show();
//        //                win.find("#tr_Aliases").hide();
//        //                win.find("#tr_ModelId").hide();
//        //            }
//        //            selectValue = newValue;
//        //        }
//        //        win.find('#SelectModel').combobox('loadData', km.modelList);
//        //        win.find('#SelectProduct').combobox('loadData', km.productList);
//        //        win.find("#UserFor").combobox({ 'onChange': userForChange });
//        //    },
//        //    onClickButton: function (win) { //保存操作
//        //        var CheckError = function (userForValue) {
//        //            if (userForValue == "1") {
//        //                var name = win.find("#Aliases").val();
//        //                if (!name) {
//        //                    com.message('e', "产品序列号不能为空");
//        //                    return true;
//        //                }

//        //                var modelId = win.find("#SelectModel").combobox("getValue");

//        //                if (!modelId) {
//        //                    com.message('e', "产品型号不能为空");
//        //                    return true;
//        //                }

//        //            }
//        //            else if (userForValue == "2")
//        //            {
//        //                var productId = win.find("#SelectProduct").combobox("getValue");
//        //                if (!productId) {
//        //                    com.message('e', "请选择要生产的产品");
//        //                    return true;
//        //                }
//        //            }
//        //            return false;
//        //        }
//        //        if (CheckError(selectValue)) {
//        //            return false;
//        //        }
//        //        com.message('c', ' <b style="color:red">是否确定要出库？ </b>', function (b) {
//        //            if (b) {
//        //                km.toolbar.do_accept();
//        //               // var data = $('#dg').datagrid('getChecked');
//        //                if (addDatas && addDatas.length > 0) {
//        //                    com.SaveAjaxInfos(addDatas, km.model.urls["savestockout"], "Bom明细保存成功", km.toolbar.do_aftersave);
//        //                }
//        //            }
//        //        });



//        //    }
//        //});
//    },

//};

$(km.init);
