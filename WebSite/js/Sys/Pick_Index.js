

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
            var FormatOper = function (val, row, index) {
                var isFeedback = row.IsFeedback;
                if (isFeedback === "否") {
                    return "<button  style=\"color:red\"  onclick=\"km.maingrid.deleteDetailRow(" + row.Id + ");\">删除</button>";
                }
                return "";
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



            var ShowDetail = function (id) {
                $.get(km.model.urls["GetPickDetail"], { id: id }, function (html) {
                    document.getElementById("east_panel").innerHTML = html;
                })
            }

            $grid.datagrid(km.gridOption({
                fitColumns: true,
                queryParams: { STime: "", ETime: "" },
                url: km.model.urls["pagelist"],
                columns: [[
                    { field: 'Addtime', title: '申请时间', width: 150, align: 'left', sortable: true },
                    { field: 'AddUserName', title: '申请人', width: 100, align: 'left', sortable: true },
                    { field: 'ElementName', title: '零件名称', width: 200, align: 'left', sortable: true },
                    { field: 'BomName', title: '所属Bom', width: 100, align: 'left', sortable: true },
                    { field: 'PartName', title: '所属部件', width: 100, align: 'left', sortable: true },
                    { field: 'Quantity', title: '申请数量', width: 100, align: 'left', sortable: true },
                    { field: 'IsFeedback', title: '是否已发料', width: 100, align: 'left', sortable: true },
                    { field: 'StockOutQuantity', title: '发料数量', width: 100, align: 'left', sortable: true },
                    { field: 'delete', title: '操作', width: 100, align: 'left', formatter: FormatOper, hidden: !checkCanDel() }
                ]],
                toolbar: '#toolbar1',
                view: groupview,
                groupField: 'ParentId',
                groupFormatter: function (value, rows) {
                    return '<div style="background:yellow;padding:0 10px">申请时间：' + rows[0].Addtime + '          申请目的：' + (rows[0].Purpose || "") + '</div>';
                },
                onClickRow: function (index, row) {
                    ShowDetail(row.Id);
                }

            }));


        },

        search_data: function () {
            var stime = com.trim($("#STime").datebox("getValue"));
            var etime = com.trim($("#ETime").datebox("getValue"));

            reload({ STime: stime, ETime: etime });
        },

        deleteDetailRow: function (id) {
            //  var index = this.jq.datagrid('getRowIndex', row);
            if (confirm("是否删除此条申请记录？")) {
                $.get(km.model.urls["DeletePick"], { id: id }, function (msg) {

                    if (msg == "") {
                        com.message('s', "删除成功");
                        reload();
                    }
                    else {
                        com.message('e', msg);
                    }

                })
            }

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
                columns: [[
                        { field: 'IsSelect', title: '选择', width: 20, align: 'left', checkbox: true },
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

            $("#BomId").combobox('loadData', km.bomList);
            $("#ElementId").combobox('loadData', km.eleList);
            $("#PartId").combobox('loadData', km.partList);
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
                $.getJSON(km.model.urls["GetListByBomId_For_Add"], {
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
        do_addpart: function () {
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
           // $grid.data = [];
            $grid.datagrid("loadData", []);
        }

    }

}();
$(km.init);
