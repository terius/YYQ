"use strict";
km.init = function () {
    km.init_parent_model();
    km.maingrid.init();
    km.detailgrid.init();
    com.CheckPer();
}


km.maingrid = function () {
    var $grid = $("#maingrid");
    var reload = function (params) {
        var defaults = { _t: com.settings.timestamp() };
        if (params) {
            defaults = $.extend(defaults, params);
        }
        $grid.datagrid('reload', defaults);
        km.detailgrid.clear();
    }
    return {
        init: function () {
            var ShowDetail = function (index, row) {
                com.mask($('#east_panel'), false);
                km.parentid = row.Id;
                km.detailgrid.reload({ parentid: km.parentid });
            }
            var flStyle = function (value, row, index) {
                if (value === "是") {
                    return 'background-color:green;color:#fff;';
                }
            }
            $grid.datagrid(km.gridOption({
                fitColumns: true,
                queryParams: { _t: com.settings.timestamp(), UserName: "" },
                url: km.model.urls["pagelist"],
                columns: [[
                    { field: 'Addtime', title: '申请时间', width: 200, align: 'left', sortable: true },
                    { field: 'AddUserName', title: '申请人', width: 120, align: 'left', sortable: true },
                    { field: 'Purpose', title: '申请目的', width: 120, align: 'left', sortable: true },
                    { field: 'IsFeedback', title: '是否发料', width: 180, align: 'left', sortable: true, styler: flStyle },
                    { field: 'StockOutTime', title: '发料时间', width: 180, align: 'left', sortable: true }

                ]],

                onClickRow: ShowDetail,
                toolbar: '#toolbar1',
                //rowStyler: function (index, row) {
                //    if (row.IsFeedback == "是") {
                //        return 'background-color:green;color:#fff;';
                //    }
                //}
            }));

            $("#UserName").combobox('loadData', km.userList);
        },

        search_data: function () {
            var username = com.trim($("#UserName").combobox("getValue"));
            reload({ UserName: username });
        }



    }
}();

km.detailgrid = function () {
    var $grid = $("#DetailGrid");
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
    var checkSave = function (data) {
        if (!data || data.length <= 0) {
            return "申请列表不能为空";
        }
        for (var i = 0; i < data.length; i++) {

            if (data[i].StockOutQuantity <= 0) {
                return "原材料：" + data[i].ElementName + "发料数量不能为0";
            }
            if (data[i].StockQuantity == "未入库" || data[i].StockQuantity <= 0) {
                return "原材料：" + data[i].ElementName + "库存数量不够";
            }
            var QuantityNum = parseInt(data[i].Quantity);
            var StockOutQuantityNum = parseInt(data[i].StockOutQuantity);
            var StockQuantityNum = parseInt(data[i].StockQuantity) || 0;
            var ALStockOutQuantityNum = parseInt(data[i].ALStockOutQuantity);

            if (StockOutQuantityNum > StockQuantityNum) {
                return "原材料：" + data[i].ElementName + "发料数量超过库存数量";
            }

            if (StockOutQuantityNum + ALStockOutQuantityNum > QuantityNum) {
                return "原材料：" + data[i].ElementName + "发料数量超过申请数量";
            }
        }
        return "";
    }

    var do_aftersave = function () {
        com.message('s', "发料成功");
        window.location.reload();
    }
    return {
        init: function () {

            var onClickRow = function (index) {


                if (editIndex != index) {
                    if (endEditing()) {
                        $grid.prev().find("tr.datagrid-row").removeClass("datagrid-row-selected");
                        $grid.datagrid('selectRow', index)
                                .datagrid('beginEdit', index);
                        var ed = $grid.datagrid('getEditor', { index: index, field: "StockOutQuantity" });
                        $(ed.target).next().find(":text").select();
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

            var OutStyle = function (value, row, index) {

                return 'background-color:green;color:#fff;';

            }
            $grid.datagrid({
                remoteSort: false,
                fitColumns: true,
                iconCls: 'icon-edit',
                singleSelect: false,
                method: 'get',
                onClickRow: onClickRow,
                rownumbers: true,
                checkOnSelect: false,
                //  selectOnCheck: false,
                url: km.model.urls["GetPickDetail"],
                queryParams: { _t: com.settings.timestamp(), parentid: 0 },

                columns: [[
                        { field: 'IsSelect', title: '选择', width: 20, align: 'left', checkbox: true },
                        { field: 'ElementName', title: '原材料', width: 200, align: 'left', sortable: true },
                        { field: 'BomName', title: 'Bom', width: 100, align: 'left', sortable: true },
                        { field: 'PartName', title: '部件', width: 100, align: 'left', sortable: true },
                        { field: 'Quantity', title: '申请数量', width: 100, align: 'left', sortable: true },
                        { field: 'ALStockOutQuantity', title: '已发料数量', width: 100, align: 'left', sortable: true },
                        { field: 'StockOutQuantity', title: '需发料数量', width: 100, align: 'left', editor: { type: 'numberbox', options: { required: true, precision: 2 } }, styler: OutStyle, sortable: true },
                        { field: 'UnitName', title: '数量单位', width: 100, align: 'left', sortable: true },
                        { field: 'StockQuantity', title: '库存数量', width: 100, align: 'left', styler: QuantityError, sortable: true },
                        { field: 'ShelfName', title: '库位', width: 100, align: 'left', sortable: true }
                ]],
                onLoadSuccess: function (data) {
                    for (var i = 0; i < data.rows.length; ++i) {
                        if (data.rows[i]['IsSelect'] == 1) $(this).datagrid('checkRow', i);
                    }
                }

            });
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
            km.parentid = 0;
            this.reload({ parentid: 0 });
        },
        do_save: function () {
            do_accept();
            var data = $grid.datagrid('getChecked');
            var msg = checkSave(data);
            if (msg != "") {
                com.message('e', msg);
                return false;
            }

            if (data && data.length > 0) {
                com.message('c', ' <b style="color:red">是否确定要发料？ </b>', function (b) {
                    if (b) {
                        com.SaveAjaxInfos(data, km.model.urls["SavePickDetail"] + "?parentid=" + km.parentid, "", do_aftersave);
                    }
                });
            }

        },
        do_export: function () {
            do_accept();
            km.GetExportParams($grid, "发料清单", 0);
            var data = $grid.datagrid('getChecked');
            km.export.ExpData = encodeURI(JSON.stringify(data));
            com.ExportToExcelByForm("/Pick/ExportExcel", km.export);

        }


    }
}();
$(km.init);
