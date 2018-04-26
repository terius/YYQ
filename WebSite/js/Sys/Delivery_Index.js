

"use strict";
km.init = function () {
    com.initbuttons($('#km_toolbar'), km.model.buttons);
    // km.init_parent_model();
    km.maingrid.init();
    km.addgrid.init();
    com.CheckPer();
    $("#SerialNo").textbox('setValue', km.maxNo);
}

com.addCheckSpan();

km.maingrid = function () {
    var $grid = $("#dgList");
    var reload = function (params) {
        var defaults = { _t: com.settings.timestamp() };
        if (params) {
            defaults = $.extend(defaults, params);
        }
        $grid.datagrid('reload', defaults);

    }

    var FormatOper = function (val, row, index) {
        return "<button   style=\"color:blue\"  onclick=\"km.maingrid.do_print(" + row.Id + ")\">打印</button>";

    }
    return {
        init: function () {
            $grid.datagrid(km.gridOption({
                fitColumns: true,
                queryParams: { STime: "", ETime: "", SaleName: km.saleName, Customer:"" },
                url: km.model.urls["pagelist"],
                columns: [[
                    { field: 'Addtime', title: '打印时间', width: 100, align: 'left', sortable: true },
                    { field: 'Customer', title: '客户', width: 100, align: 'left', sortable: true },
                    { field: 'OrderNo', title: '订单号', width: 100, align: 'left', sortable: true },
                    { field: 'SerialNo', title: '流水号', width: 100, align: 'left', sortable: true },
                    { field: 'OrderDate', title: '订单日期', width: 100, align: 'left', sortable: true },
                    { field: 'TotalAmount', title: '总价', width: 100, align: 'left', sortable: true },
                    { field: 'Sender', title: '发货人', width: 100, align: 'left', sortable: true },
                    { field: 'Manager', title: '收货人', width: 100, align: 'left', sortable: true },
                    { field: 'AddUserName', title: '添加人', width: 100, align: 'left', sortable: true },
                    { field: 'IsOut', title: '是否已出货', width: 100, align: 'left', sortable: false },
                    { field: 'Remark', title: '产品描述', width: 100, align: 'left', sortable: true },
                    { field: 'print', title: '打印', width: 50, align: 'left', formatter: FormatOper }

                ]],
                toolbar: '#toolbar1',
                view: detailview,
                detailFormatter: function (index, row) {
                    return '<div style="padding:10px"><table class="ddv"></table></div>';
                },
                onExpandRow: function (index, row) {
                    var ddv = $(this).datagrid('getRowDetail', index).find('table.ddv');
                    ddv.datagrid({
                        data: row.Details,
                        fitColumns: true,
                        singleSelect: true,
                        rownumbers: true,
                        loadMsg: '',
                        height: 'auto',
                        columns: [[
                            { field: 'Type', title: '类型', width: 100, align: 'left' },
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
            var customer = com.trim($("#Skey").val());
            reload({ STime: stime, ETime: etime, SaleName: km.saleName, Customer: customer });
        },
        do_print: function (id) {
            var isout = false;
            if (confirm("此送货单是否出货？")) {

                isout = true;
            }
            com.ExportToExcel(km.model.urls["exportExcel"], { id: id, isOut: isout });
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
        com.message('s', "新增送货单成功");
        window.location.reload();
    }

    var CheckProductIsDup = function (pid) {

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



    var onClickRow = function (index) {
        if (editIndex != index) {
            if (endEditing()) {
                $grid.datagrid('selectRow', index)
                    .datagrid('beginEdit', index);
                var ed = $grid.datagrid('getEditor', { index: index, field: "Quantity" });
                // $(ed.target).next().find(":text").select();
                editIndex = index;
            } else {
                $grid.datagrid('selectRow', editIndex);
            }
        }
    }

    var checkSave = function (data) {
        if (!data || data.rows.length <= 0) {
            return "送货单列表不能为空";
        }
        var row;
        //com.showLog(data);
        for (var i = 0; i < data.rows.length; i++) {
            row = data.rows[i];
            if (row.Quantity <= 0) {
                return row.Type + row.Model + "数量不能为0";
            }

            if (row.Price <= 0) {
                return row.Type + row.Model + "单价不能为0";
            }
        }
        return "";
    }
    var FormatOper = function (val, row, index) {
        return "<button   style=\"color:red\"  onclick=\"km.addgrid.do_deleteRow()\">删除</button>";

    }


    var onClickCell = function (index, field, value) {

        if (field == "delete") {
            if (deleteButtonClick) {
                deleteButtonClick = false;
                $grid.datagrid("deleteRow", index).datagrid("unselectAll");
            }
            //    com.showLog("index:" + index + "  field:" + field + "   value:" + value);

        }

    }
    var deleteButtonClick = false;
    var onEndEdit = function (index, row, changes) {
        var num = parseInt(row.Quantity) || 0;
        var price = parseInt(row.Price) || 0;
        row.TotalPrice = num * price;

        var totalPrice = 0;
        var data = $grid.datagrid('getData');

        for (var i = 0; i < data.rows.length; i++) {
            num = parseInt(data.rows[i].Quantity) || 0;
            price = parseInt(data.rows[i].Price) || 0;
            totalPrice += num * price;
        }
        $("#TotalAmount").text(totalPrice);
    }

    var validateForm = function () {
        return $("#formadd").form('enableValidation').form('validate');
    }



    return {
        init: function () {

            //       public Nullable<int> ElementId { get; set; }
            //public Nullable<int> ProductId { get; set; }
            //public double Quantity { get; set; }

            //public string Remark { get; set; }
            //public string UnitTypeCode { get; set; }
            //public decimal Price { get; set; }
            //public decimal TotalPrice { get; set; }
            $grid.datagrid({
                iconCls: 'icon-edit',
                singleSelect: true,
                method: 'get',
                onClickRow: onClickRow,
                onClickCell: onClickCell,
                onEndEdit: onEndEdit,
                //      checkOnSelect: false,
                //url: km.model.urls["getAddTemp"],
                columns: [[
                    { field: 'Type', title: '类型', width: 100, align: 'left' },
                    { field: 'Model', title: '品名及规格', width: 200, align: 'left' },
                    {
                        field: 'Quantity', title: '数量', width: 100, align: 'left',
                        editor: { type: 'numberbox', options: { required: true, precision: 1 } }

                    },
                    { field: 'Unit', title: '单位', width: 100, align: 'left' },
                    { field: 'Price', title: '单价', width: 100, align: 'left', editor: { type: 'numberbox', options: { required: true } } },
                    { field: 'TotalPrice', title: '金额', width: 100, align: 'left' },
                    { field: 'Remark', title: '备注', width: 300, align: 'left', editor: { type: 'textbox' } },
                    { field: 'delete', title: '操作', width: 300, align: 'left', formatter: FormatOper },

                ]]

            });//end grid init


            $("#ElementId").combobox('loadData', km.eleList);
            $("#ProductId").combobox('loadData', km.prodList);
        },

        do_addprod: function () {
            var productId = $("#ProductId").combobox("getValue");

            if (productId) {
                do_accept();
                var isExist = CheckProductIsDup(productId);
                if (isExist) {
                    com.message('e', "请勿重复添加成品");
                    return false;
                }

                $.getJSON(km.model.urls["addProductItem"], {
                    pid: productId
                }, function (data) {
                    if (data) {
                        // com.showLog(data);

                        $grid.datagrid('appendRow', data);
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
                $.getJSON(km.model.urls["addElementItem"], {
                    eleid: eleid
                }, function (row) {
                    if (row) {
                        $grid.datagrid('appendRow', row);
                    }
                });
            }
        },
        do_saveadd: function () {
            do_accept();

            var addDatas = $grid.datagrid('getData');
            var msg = checkSave(addDatas);
            if (msg != "") {
                com.message('e', msg);
                return false;
            }
            var rs = validateForm();
            if (!rs) {
                return false;
            }

            //if (com.CheckError($("#north_panel")))
            //{
            //    return false;
            //}

            com.message('c', ' <span style="color:red">是否确定要添加送货单？ </span>', function (yes) {
                if (yes) {
                    km.addView.Remark = $("#Remark").textbox("getValue");
                    km.addView.Customer = $("#Customer").textbox("getValue");
                    km.addView.OrderNo = $("#OrderNo").textbox("getValue");
                    km.addView.OrderDate = $("#OrderDate").datebox("getValue");
                    //     km.addView.TotalAmount = $("#OrderDate").datebox("getValue");
                    km.addView.Sender = $("#Sender").textbox("getValue");
                    km.addView.Manager = $("#Manager").textbox("getValue");
                    km.addView.SerialNo = $("#SerialNo").textbox("getValue");
                    km.addView.Details = addDatas.rows;
                    //  com.showLog(km.addView);

                    com.SaveAjaxInfos(km.addView, km.model.urls["saveAdd"], "", do_aftersave);

                }
            });

        },
        do_clear: function () {
            $grid.datagrid("loadData", []);
            //$grid.datagrid("reload");
        },
        do_deleteRow: function () {
            if (confirm("是否删除此条记录？")) {
                deleteButtonClick = true;
            }
        }

    }

}();
$(km.init);
