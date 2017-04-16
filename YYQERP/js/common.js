/*
==================================================================
程序：框架基础函数包
创建：键盘男
说明：通用方法。
最近更新：16:46 2015-06-10
==================================================================
*/


var isArray = function (obj) {
    return Object.prototype.toString.call(obj) === '[object Array]';
}
var com = {};

//弹框提示messager
com.show = function (msg) {
    if (parent == window) {
        $.messager.show({ title: '提示', msg: msg, showType: 'show' });
    } else {
        parent.$.messager.show({ title: '提示', msg: msg, showType: 'show' });
    }
}

//提示messager-页面中间显示
com.showcenter = function (title, msg) {
    if (parent == window) {
        $.messager.show({
            title: title,
            msg: msg,
            showType: 'show',
            style: {
                right: '',
                top: document.body.scrollTop + document.documentElement.scrollTop,
                bottom: ''
            }
        });
    } else {
        parent.$.messager.show({
            title: title,
            msg: msg,
            showType: 'show',
            style: {
                right: '',
                top: document.body.scrollTop + document.documentElement.scrollTop,
                bottom: ''
            }
        });
    }
}

//警告alert
com.alert = function (msg) {
    var htmlHS = '<span style="font-size:15px;color:red;font-weight:bold">' + msg + '</span>';
    if (parent == window) {
        $.messager.alert('警告', htmlHS, 'warning');
    } else {
        parent.$.messager.alert('警告', htmlHS, 'warning');
    }
}


//弹messagee
com.message = function (type, message, callback) {
    var html_success = '<span style="font-size:13px;color:green;font-weight:bold"> ' + message + '</span>';
    var html_alert = '<span style="font-size:13px;color:red;font-weight:bold">' + message + '</span>';
    var html_confirm = '<span style="font-size:13px;color:#E2392D;font-weight:bold">' + message + '</span>';
    switch (type) {
        case "s":
        case "success":
            if (parent == window) {
                $.messager.show({
                    title: '成功提示', msg: html_success, showType: 'slide', style: {
                        right: '',
                        top: document.body.scrollTop + document.documentElement.scrollTop,
                        bottom: ''
                    }
                });
            }
            else {
                parent.$('#notity').jnotifyAddMessage({ text: message, permanent: false });
            }
            break;
        case "error":
        case "e":
            if (parent == window) {
                //$.messager.show({ title: '错误', msg: html_success });
                $.messager.alert('错误提示', html_alert, 'warning');
                console.info(html_alert);
            }
            else {
                parent.$('#notity').jnotifyAddMessage({ text: message, permanent: false, type: 'error' });
            }
            break;
        case "warning":
        case "w":
            if (parent == window) {
                $.messager.alert('警告', html_alert, 'warning');
            }
            else {
                parent.$('#notity').jnotifyAddMessage({ text: message, permanent: false, type: 'warning' });
            }
            break;
        case "information":
        case "i":
        case "info":
            if (parent == window) {
                $.messager.show({ title: '消息', msg: message });
            }
            else {
                parent.$.messager.show({ title: '消息', msg: message });
            }
            break;
        case "confirm":
        case "c":
            if (parent == window) {
                return $.messager.confirm('确认', html_confirm, callback);
            }
            else {
                return parent.$.messager.confirm('确认', html_confirm, callback);
            }
    }

    if (callback) callback();
    return null;
};

//显示模态dialog-顶层模态。
com.dialog = function (opts, onBeforeOpen, onSave) {
    var query = parent.$;
    var win = query('<div></div>').appendTo('body').html(opts.html);
    var btntext = '保存';
    if (opts.btntext) btntext = opts.btntext;
    opts = query.extend({
        title: 'My Dialog', cache: false, modal: true, html: '', url: '',
        buttons: [{
            text: '&nbsp;&nbsp;&nbsp;&nbsp;<b>' + btntext + '<b>&nbsp;&nbsp;&nbsp;&nbsp;', handler: function () {
                if (onSave instanceof Function) {
                    onSave(win); //win.dialog('destroy');
                }
            }
        }],
        onBeforeOpen: function () {
            if (onBeforeOpen instanceof Function) {
                onBeforeOpen(win);
            }
        },
        onClose: function () {
            query(this).dialog('destroy');
        }
    }, opts);

    win.dialog(opts);
    query.parser.parse(win);
    return win;
}

//初始化模态dialog--iframe内模态。
com.initdialog = function (target, title, callback) {
    var d = target.dialog({
        title: title, top: 10, cache: false, modal: true, inline: true,
        buttons: [{
            text: '保存', iconCls: 'icon-save', handler: function () {
                if (callback instanceof Function) {
                    callback();
                }
            }
        }]
    }).dialog('close');
}

//局部遮罩。
com.mask = function (locale, show) {
    var zindex = 1;
    if (show == true) {
        locale.addClass("mask-container");
        //var mask = $("<div style='background-color:#000 '><div style='position:absolute;background-color:yellow;color:blue;height: 22px; width: 100px;margin: 0 auto ;top:10px; left:10px;border-radius:5px;z-index:0'><img src='../style/images/ajax-loader.gif' />正在加载...</div></div>").addClass("datagrid-mask").css({ display: "block", "z-index": +zindex }).appendTo(locale);
        var mask = $("<div style='background-color:#ccc '></div>").addClass("datagrid-mask").css({ display: "block", "z-index": +zindex }).appendTo(locale);
    } else {
        locale.removeClass("mask-container");
        locale.children("div.datagrid-mask-msg,div.datagrid-mask").remove();
    }
}

//ajax请求
com.ajax = function (options) {
    options = $.extend({
        showLoading: true//新加属性，是否显示loading效果
    }, options);

    var ajaxDefaults = {
        type: 'POST',////数据的提交方式：get和post
        //   dataType: 'json',////服务器返回数据的类型，例如xml,String,Json等
        //contentType: 'application/json',
        error: function (e) {
            var msg = e.responseText;
            console.info(e);
            alert('ajax出现错误：' + msg);
        }
    };

    if (options.showLoading) {
        ajaxDefaults.beforeSend = $.showLoading;
        ajaxDefaults.complete = $.hideLoading;
    }

    $.ajax($.extend(ajaxDefaults, options));
};


/*关于页面html公共格式化方式*/
com.html_formatter = {
    get_color_html: function (text, color) {
        return '<span style="font-weight:bold; color:' + color + ';">' + text + '</span>';
    },
    yesno: function (value, row, index) {
        var text = value == 1 ? '是' : '否';
        var color = value == 1 ? 'green' : 'red';
        var result = com.html_formatter.get_color_html(text, color);
        return result;
    }
};

/*公共设置*/
com.settings = {
    timestamp: function () {
        var d = new Date();
        var result = d.getYear() + '' + d.getMonth() + '' + d.getDay() + '' + d.getMinutes() + '' + d.getSeconds() + '' + d.getMilliseconds();
        return result;
    },
    ajax_timestamp: function () {
        return '?timestamp=' + this.timestamp();
    }
};

/*初始化页面权限按钮*/
com.initbuttons = function (target, listbuttons) {
    //alert($(target).html());
    $(target).html('');
    var htmlButtons = '';
    if (km.model.buttons.length > 0) {
        for (var i = 0; i < listbuttons.length; i++) {
            var jsEvent = listbuttons[i].JsEvent.indexOf("km.") >= 0 ? listbuttons[i].JsEvent : ("km.toolbar." + listbuttons[i].JsEvent);
            var checkper = listbuttons[i].data_checkper ? 'data-checkper="' + listbuttons[i].data_checkper + '"' : '';
            htmlButtons += '<a id="toolbar_' + listbuttons[i].ButtonCode + '" href="javascript:void(0)" class="easyui-linkbutton" data-options="plain:true,iconCls:\'' + listbuttons[i].IconClass + '\' "  title="' + listbuttons[i].ButtonName + '" onclick="' + jsEvent + '"  ' + checkper + '>' + listbuttons[i].ButtonName + '</a>';
        }
        $(target).append(htmlButtons);
        $(target).find(".easyui-linkbutton").linkbutton();
    }
    //else {
    //    //alert($(target).parent()[0].id); 
    //    $(target).parent().height(0);
    //    $(target).remove();
    //}
}

//terius start
com.trim = function (val) {
    if (val != null && val != undefined && typeof val == "string") {
        return $.trim(val.replace(/<[^>]*>/g, ""));
    }
    return val;
}

var loading = $("<div id=\"myload\" style=\"position:absolute;left:20px;top:20px;background-color:#fff;border:1px solid green;padding:5px;z-index:99999\">执行中，请稍候。。。</div>");
com.jqFormOption = {
    data: {},
    error: function (result, textStatus, errorThrown) {
        alert('查询出错');
    },
    beforeSerialize: function ($form, options) {
        $form.find(":text,select,:hidden").each(function (index, ele) {
            ele.value = com.trim(ele.value);
        })

    }
    //beforeSubmit: function (arr, $form, options) {
    //    var $body = $("body");
    //    if ($body.find("#myload").length <= 0) {
    //        loading.css({ top: $body.outerHeight() / 2, left: $body.outerWidth() / 2 });
    //        $body.append(loading);
    //    }
    //    loading.fadeIn();

    //},
    //success: function (result) {
    //    loading.fadeOut();

    //}


};

com.CheckError = function (win) {
    var list = win.find(".required");
    for (var i = 0; i < list.length; i++) {
        var em = list[i];
        var val = em.value;
        if (em.className.indexOf("easyui-combobox") >= 0) {
            var name = $(em).attr("textboxname");
            val = win.find(":hidden[name='" + name + "']").val();
        }
        if (!val) {
            var errmsg = $(em).data("errmsg");
            if (!errmsg) {
                errmsg = $(em).parent().prev("td").text();

                if (!errmsg) {
                    errmsg = $(em).prev(".caption").text();
                    if (errmsg) {
                        errmsg += "不能为空";
                    }
                    else {
                        errmsg = "必填项不能为空";
                    }

                }
                else {
                    errmsg += "不能为空";
                }
            }
            com.message('e', errmsg);
            return true;


            //  em.focus;

        }

    }
    return false;
}

com.SaveAjaxInfos = function (params, url, success_msg, callback) {
    if (params instanceof Array) {
        var len = params.length;
        while (len--) {
            $.each(params[len], function (name, value) {
                (params[len])[name] = com.trim(value);
            });
        }
    }
    else {
        $.each(params, function (name, value) {
            params[name] = com.trim(value);
        });
    }


    $.ajax({
        url: url,
        type: "POST",
        data: JSON.stringify(params),
        // dataType: "json",
        //  async: false

    }).then(function (result) {
        if (result == "") {
            if (typeof callback == "function") {
                callback.call();
            }
            else {
                com.message('s', success_msg);
            }
        }
        else {
            com.message('e', result);
        }
    }, function (error) {
        com.message('e', "AJAX执行出错！" + error);
    });
}

com.CheckPer = function () {
  //  com.showLog(km.pers);
    var list = $("[data-checkper]");
    list.each(function () {
        var cper = $(this).data("checkper");
        var isAllow = false;
        for (var i = 0; i < km.pers.length; i++) {
            if (cper == km.pers[i]) {
                isAllow = true;
                break;
            }
        }
        if (!isAllow) {
            $(this).hide();
        }
    })
}

com.ExportToExcel = function (url, params) {
    url += "?" + $.param(params);
    //如果页面中没有用于下载iframe，增加iframe到页面中
    if ($('#downloadcsv').length <= 0)
        $('body').append("<iframe id=\"downloadcsv\" style=\"display:none\"></iframe>");
    $('#downloadcsv').attr('src', url);
}

com.ExportToExcelByForm = function (url, params) {
    if ($('#formbyexp').length <= 0) {
        $('body').append("<form id=\"formbyexp\" style=\"display:none\" action=\"" + url + "\" method=\"post\"></form>");
    }
    var form = $("#formbyexp");
    var html = "";
    for (var key in params) {
        html += '<input type="hidden" name="' + key + '" value="' + params[key] + '" />'
    }
    form.append(html);
    form[0].submit();
    form.html("");
}

com.MoneyStringToInt = function (str) {
    if (str) {
        var newStr = str.substr(1);
        return parseInt(newStr);
    }
    return 0;

}


com.showLog = function (msg) {
    var now = com.getNowDate();
    if (msg instanceof Object) {
        console.log(now + " ---- " + JSON.stringify(msg, null, 4))
    }
    else {
        console.log(now + " ---- " + msg);
    }
}

com.getNowDate = function () {
    var now = new Date();
    return com.getDateTime(now);
}

com.getDate = function (d) {
    var year = "" + d.getFullYear();
    var month = "" + (d.getMonth() + 1); if (month.length == 1) { month = "0" + month; }
    var day = "" + d.getDate(); if (day.length == 1) { day = "0" + day; }
    return year + "-" + month + "-" + day;
}

com.getDateTime = function (d) {
    var hour = "" + d.getHours(); if (hour.length == 1) { hour = "0" + hour; }
    var minute = "" + d.getMinutes(); if (minute.length == 1) { minute = "0" + minute; }
    var second = "" + d.getSeconds(); if (second.length == 1) { second = "0" + second; }
    return com.getDate(d) + " " + hour + ":" + minute + ":" + second;
}





































