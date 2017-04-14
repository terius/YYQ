

"use strict";
km.init = function () {
    com.initbuttons($('#km_toolbar'), km.model.buttons);
    km.init_parent_model();
    km.maingrid.init();
    km.addgrid.init();
    com.CheckPer();
}



km.maingrid = function () {
    var $grid = $("#dgList");
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
                fitColumns: true,
                queryParams: { STime: "", ETime: "" },
                url: km.model.urls["pagelist"],
                columns: [[
                    { field: 'Addtime', title: '打印时间', width: 100, align: 'left', sortable: true },
                    { field: 'Customer', title: '客户', width: 100, align: 'left', sortable: true },
                    { field: 'OrderNo', title: '订单号', width: 100, align: 'left', sortable: true },
                    { field: 'OrderDate', title: '订单日期', width: 100, align: 'left', sortable: true },
                    { field: 'TotalAmount', title: '总价', width: 100, align: 'left', sortable: true },
                    { field: 'Sender', title: '发货人', width: 100, align: 'left', sortable: true },
                    { field: 'Manager', title: '收货人', width: 100, align: 'left', sortable: true },
                    { field: 'AddUserName', title: '添加人', width: 100, align: 'left', sortable: true }
                   
                ]],
                toolbar: '#toolbar1',
                view: detailview,
                detailFormatter: function (index, row) {
                    return '<div style="padding:10px"><table class="ddv"></table></div>';
                },
                onExpandRow: function (index, row) {
                    var ddv = $(this).datagrid('getRowDetail', index).find('table.ddv');
                    ddv.datagrid({
                        data:row.Details,
                        fitColumns: true,
                        singleSelect: true,
                        rownumbers: true,
                        loadMsg: '',
                        height: 'auto',
                        columns: [[
                            { field: 'Type', title: '类型', width: 100,align: 'left' },
                            { field: 'Model', title: '品名及规格', width: 200, align: 'left' },
                            { field: 'Quantity', title: '数量', width: 100, align: 'left' },
                            { field: 'Unit', title: '单位', width: 100, align: 'left' },
                            { field: 'Price', title: '单价', width: 100, align: 'left' },
                            { field: 'TotalPrice', title: '金额', width: 100, align: 'left' },
                            { field: 'Remark', title: '备注', width: 100, align: 'left' }
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
                //groupField: 'ParentId',
                //groupFormatter: function (value, rows) {
                //    return '<div style="background:yellow;padding:0 10px">申请时间：' + rows[0].Addtime + '          申请目的：' + (rows[0].Purpose || "") + '</div>';
                //}
              

            }));


        },

        search_data: function () {
            var stime = com.trim($("#STime").datebox("getValue"));
            var etime = com.trim($("#ETime").datebox("getValue"));

            reload({ STime: stime, ETime: etime });
        }


    }
}();

km.addgrid = function () {
    var $this = $(this), $grid = $("#dg"), that = this, editIndex = undefined;

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
        com.message('s', "申请领料保存成功");
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

    var checkSave = function (data) {
        if (!data || data.length <= 0) {
            return "申请列表不能为空";
        }
        for (var i = 0; i < data.length; i++) {
            if (data[i].Quantity <= 0) {
                return "原材料：" + data[i].ElementName + "申请数量不能为0";
            }
        }
        return "";
    }

    return {
        init: function () {
            $grid.datagrid({
                iconCls: 'icon-edit',
                singleSelect: false,
                method: 'get',
                onClickRow: onClickRow,
                checkOnSelect: false,
                url: km.model.urls["getAddTemp"],
                columns: [[
                        { field: 'ElementName', title: '原材料', width: 200, align: 'left' },
                        { field: 'BomName', title: 'Bom', width: 220, align: 'left' },
                        { field: 'PartName', title: '部件', width: 120, align: 'left' },
                        { field: 'Quantity', title: '申请数量', width: 80, align: 'left', editor: { type: 'numberbox', options: { required: true, precision: 2 } } },
                        { field: 'UnitName', title: '数量单位', width: 40, align: 'left' },
                        { field: 'StockQuantity', title: '库存数量', width: 80, align: 'left', styler: QuantityError },
                        //{ field: 'ElementId', title: '原材料Id', width: 1, align: 'left', hidden: true },
                        //{ field: 'BomId', title: 'BomId', width: 1, align: 'left', hidden: true },
                        //{ field: 'ShelfId', title: '库位Id', width: 1, align: 'left', hidden: true }
                ]]

            });//end grid init

          
            $("#ElementId").combobox('loadData', km.eleList);
            $("#ProductId").combobox('loadData', km.prodList);
        },
     
        do_addprod: function () {
            var partid = $("#PartId").combobox("getValue");

            if (partid) {
                do_accept();

                var num = $("#PartNum").val() || 1;
                $.getJSON(km.model.urls["GetListByPartId_For_Add"], {
                    partid: partid, num: num
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
                //var isExist = CheckExistByEle(eleid);
                //if (isExist) {
                //    com.message('e', "请勿重复添加原材料");
                //    return false;
                //}
                $.getJSON(km.model.urls["GetByEleId_For_Add"], {
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
        do_savepick: function () {
            do_accept();
            var addDatas = $grid.datagrid('getChecked');
            var msg = checkSave(addDatas);
            if (msg != "") {
                com.message('e', msg);
                return false;
            }

            com.message('c', ' <span style="color:red">是否确定要申请？ </span>', function (b, msg, a) {
                if (b) {


                    if (addDatas && addDatas.length > 0) {
                        var purpose = encodeURI(com.trim($("#Purpose").val()));
                        com.SaveAjaxInfos(addDatas, km.model.urls["SavePick"] + "?purpose=" + purpose, "", do_aftersave);
                    }
                }
            });

        },
        do_clear: function () {
            $grid.datagrid("reload");
        }

    }

}();
$(km.init);
