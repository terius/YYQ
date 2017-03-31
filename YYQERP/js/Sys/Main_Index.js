"use strict";
km.init = function () {
    //   com.initbuttons($('#km_toolbar'), km.model.buttons);
    km.init_parent_model();
    km.maingrid.init();
}

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
            queryParams: { _t: com.settings.timestamp(), Name: "" },
            url: km.model.urls["pagelist"],
            pagination: true,
            pageList: [5, 10, 15, 20, 30, 50, 100],
            pageSize: 15,
            rowStyler: function (index, row) {
                if (row.WarnQuantity > row.Quantity) {
                    return 'background-color:#ebde33;color:#eb3358;';
                }
                else if (row.PickQuantity > row.Quantity)
                {
                    return 'background-color:#b8c324;color:#eb3358;';
                }
            },
            columns: [[
	            { field: 'ShelfName', title: '库位', width: 120, align: 'left', sortable: true },
                { field: 'ElementName', title: '存放物品', width: 180, align: 'left', sortable: true },
                { field: 'ItemTypeText', title: '物品类型', width: 120, align: 'left', sortable: true },
                { field: 'Quantity', title: '库存数量', width: 120, align: 'left', sortable: true },
                { field: 'WarnQuantity', title: '预警数量', width: 120, align: 'left', sortable: true },
                { field: 'PickQuantity', title: '申请领料数量', width: 120, align: 'left', sortable: true },
                { field: 'LastOutQuantity', title: '最近出库数量', width: 120, align: 'left', sortable: true },
                { field: 'UnitType', title: '单位', width: 120, align: 'left', sortable: true },
                { field: 'LastOutTime', title: '最近出库时间', width: 150, align: 'left', sortable: true }
            ]],
            onLoadSuccess: function () {
                //     alert('load data successfully!');
            },
            onLoadError: function (a, b, c) {
                alert('ajax执行出错');

            },
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
        });//end grid init
    },

    search_data: function () {
        var name = com.trim($("#keyword").val());
        this.reload({ Name: name });
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
    }
};



$(km.init);
