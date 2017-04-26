"use strict"
//当前页面对象
var km = {};
km.model = null;
km.parent_model = null;



/*初始化iframe父页面的model对象，即：访问app.index.js文件中的客户端对象*/
km.init_parent_model = function () {
    //只有当前页面有父页面时，可以获取到父页面的model对象 parent.wrapper.model
    if (window != parent) {
        if (parent.wrapper) {
            km.parent_model = parent.wrapper.model;
            //    com.message('s', '获取到父页面的model对象：<br>' + JSON.stringify(km.parent_model));
        } else {
            com.showcenter('提示', "存在父页面，但未能获取到parent.wrapper对象");
        }
    } else {
        com.showcenter('提示', "当前页面已经脱离iframe，无法获得parent.wrapper对象！");
    }
}

//页面对象参数设置
km.settings = {};

//格式化数据
km.gridformat = {};

km.export = {};

km.GetExportParams = function ($grid, fileName, exportPageData) {
    var titles = "";
    var cols = "";
    var option = $grid.datagrid("options");
    var columns = option.columns[0];
    for (var i = 0; i < columns.length; i++) {
        if (columns[i].title != "操作" && columns[i].field != "IsSelect") {
            if (titles.length == 0) {
                titles = columns[i].title;
                cols = columns[i].field;
            }
            else {
                titles += "," + columns[i].title;
                cols += "," + columns[i].field;
            }
        }
    }
    km.export.page = option.pageNumber;
    km.export.rows = option.pageSize;
    km.export.ColumnTitles = titles;
    km.export.Columns = cols;
    km.export.FileName = fileName;
    km.export.ExportPageData = exportPageData == 1 ? true : false;
    return km.export;
}



//km.showColumns = {};

//km.showColumns.ShelfName = {
//    field: 'ShelfName',
//    title: '库位',
//    width: 60,
//    align: 'left',
//    sortable: true
//};

km.gridOption = function (option) {

    var defaultOption = {
        fit: true,
        border: false,
        singleSelect: true,
        rownumbers: true,
        remoteSort: false,
        cache: false,
        method: 'get',
        idField: 'Id',
        queryParams: { _t: com.settings.timestamp() },
        url: "",
        pagination: true,
        pageList: [5, 10, 15, 20, 30, 50, 100],
        pageSize: 15,
        rowStyler: function (row) {
            if (row.Enabled == 0) {
                return 'color:red;';
            }
        },
        columns: [],
        onLoadSuccess: function () {
            //     alert('load data successfully!');
        },
        onLoadError: function (a, b, c) {
            com.showLog(a);
            alert('ajax执行出错');
        }
    }

    return $.extend(defaultOption, option);

}

km.checkEleDup = function (data, eName) {
    if (data && (data.length > 0 || data.total > 0)) {
        data = data.rows || data;
        if (!data[0][eName]) {
            return false;
        }
        var isDup = checkArrayRepeat(data, eName);
        return isDup;
    }
    return false;
}

var checkArrayRepeat = function (array, cname) {

    var hash = {};

    for (var i in array) {
        if (hash[array[i][cname]]) {
            return true;
        }
        hash[array[i][cname]] = true;

    }
    return false;

}



km.appendCheckRow = function (data, $grid) {

    var indexMax = $grid.datagrid('getRows').length;
    
    if (isArray(data)) {
        for (var i = 0; i < data.length; i++) {
            $grid.datagrid('appendRow', data[i]);
            if (data[i]['IsSelect'] == 1) $grid.datagrid('checkRow', indexMax);
            indexMax++;
        }
    }
    else {
        if (data) {
            $grid.datagrid('appendRow', data);
            if (data['IsSelect'] == 1) $grid.datagrid('checkRow', indexMax);
        }
    }
}

km.CheckNullAndQuantity = function (data, numColumn, numText) {
    if (!data) {
        com.message('e', "列表不能为空");
        return false;
    }

    if (data.length <= 0 || data.total <= 0) {
        com.message('e', "列表不能为空");
        return false;
    }

    data = data.rows || data;
    for (var i = 0; i < data.length; i++) {
        var num = data[i][numColumn];
        if (!num || num <= 0) {
            var text = data[i][numText] || "";
            com.message('e', text + "数量不能为空");
            return false;
        }
    }
    return true;
}
