"use strict";
km.init = function () {
    //   com.initbuttons($('#km_toolbar'), km.model.buttons);
    km.init_parent_model();
    km.maingrid.init();

    com.CheckPer();
}



km.maingrid = {
    jq: null,
    init: function () {
        var warnStyle = function (value, row, index) {
            if (row.Quantity < row.WarnQuantity) {
                return 'background-color:#f24b21;color:#fff;';
            }
        }
        var FormatOper = function (val, row, index) {
            return "<button  style=\"color:red\"  onclick=\"km.maingrid.deleteStock(" + row.Id + ");\">删除</button>";
        }
        var checkCanDel = function () {
            for (var i = 0; i < km.pers.length; i++) {
                if ("delete" == km.pers[i]) {
                    return true;
                }
            }
            return false;
        }
        this.jq = $("#maingrid").datagrid(km.gridOption({
            fitColumns: true,
            queryParams: { },
            url: km.model.urls["pagelist"],
            columns: [[
	            { field: 'ShelfName', title: '库位', width: 60, align: 'left', sortable: true },
                { field: 'ElementName', title: '存放物品', width: 200, align: 'left', sortable: true },
                { field: 'ItemTypeText', title: '物品类型', width: 70, align: 'left', sortable: true },
                { field: 'Quantity', title: '库存数量', width: 70, align: 'left', sortable: true, styler: warnStyle },
                { field: 'WarnQuantity', title: '预警数量', width: 70, align: 'left', sortable: true },
                { field: 'UnitType', title: '单位', width: 80, align: 'left', sortable: true },
                { field: 'FirstInTime', title: '首次入库时间', width: 150, align: 'left', sortable: true },
                { field: 'LastInTime', title: '最近入库时间', width: 150, align: 'left', sortable: true },
                { field: 'LastOutTime', title: '最近出库时间', width: 150, align: 'left', sortable: true },
                { field: 'delete', title: '操作', width: 150, align: 'left', formatter: FormatOper, hidden: !checkCanDel() }
            ]],
            toolbar: '#toolbar1',
            onClickRow: function (index, row) {
                km.maingrid.ShowStockDetail(row.Id);
                // alert(JSON.stringify(row));
                //km.maingrid.selectedIndex = index;
                //km.maingrid.selectedRow = row;
                //km.rolegrid.setUserRoles(row);
                //if (km.maingrid.selectedRow)
                //    km.set_mode('show');
            },
        }));//end grid init
    },

    search_data: function () {
        var s_element = com.trim($("#s_element").val());
        var s_shelf = com.trim($("#s_shelf").val());
        var s_product = com.trim($("#s_product").val());
        this.reload({ ElementNameOrCode: s_element, ShelfCode: s_shelf, ProductCode: s_product });
    },
    reload: function (params) {
        var defaults = { _t: com.settings.timestamp() };
        if (params) {
            defaults = $.extend(defaults, params);
        }
        this.jq.datagrid('reload', defaults);

    },
    ShowStockDetail: function (id) {
        $.get(km.model.urls["stockdetail"], { id: id }, function (html) {
            document.getElementById("east_panel").innerHTML = html;
        })
    },
    do_export: function (exportPageData) {
        km.GetExportParams(this.jq, "库存信息", exportPageData);
        km.export.ElementNameOrCode = com.trim($("#s_element").val());
        km.export.ShelfCode = com.trim($("#s_shelf").val());
        km.export.ProductCode = com.trim($("#s_product").val());
     
        com.ExportToExcel("/Stock/ExportExcelForStock", km.export);
    },
    deleteStock: function (id) {
        //  var index = this.jq.datagrid('getRowIndex', row);
        if (confirm("是否删除此条库存记录？")) {
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
};



$(km.init);
