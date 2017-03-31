/*
基于jquery的扩展组件 yxz 14:42 2015-11-07

jquery插件的开发包括两种：一种是类级别的插件开发，即给jquery添加新的全局函数，相当于给jquery类本身添加方法。
jquery的全局函数就是属于jquery命名空间的函数，另一种是对象级别的插件开发，即给jQuery对象添加方法。
也可以理解为另外两种方式，一种是jQuery本身的扩展方法，另一种是jQuery所选对象的扩展方法。
1.jQuery.extend(Object);　　　// jQuery 本身的扩展方法 
2.jQuery.fn.extent(Object);　   // jQuery 所选对象扩展方法
下面就两种函数的开发做详细的说明。
*/
;(function ($) {

    /************************************************************************************/
    /**************************** jQuery类级别扩展方法******************************/
    /************************************************************************************/
    $.yxz = {
        getUrlParams: function () {
            /*
            作用：jQuery扩展方法 [获取URL的GET参数值] yxz 12:12 2015-07-08
            返回：return array() 
            使用：
                 var GET = $.getUrlParams(); //获取URL的Get参数
                 var id = GET['id']; //取得id的值
            */
            var aQuery = window.location.href.split("?");  //取得Get参数
            var aGET = new Array();
            if (aQuery.length > 1) {
                var aBuf = aQuery[1].split("&");
                for (var i = 0, iLoop = aBuf.length; i < iLoop; i++) {
                    var aTmp = aBuf[i].split("=");  //分离key与Value
                    aGET[aTmp[0]] = aTmp[1];
                }
            }
            return aGET;
        }
    }














    /************************************************************************************/
    /**************************** jQuery对象级别扩展方法******************************/
    /************************************************************************************/

    /*
    作用：jQuery对象级别扩展方法，序列化表单为json对象
    返回：return array() 
    使用：
         var GET = $.getUrlParams(); //获取URL的Get参数
         var id = GET['id']; //取得id的值
    */
    $.fn.serializeJson = function () {
        var serializeObj = {};
        var array = this.serializeArray();
        var str = this.serialize();
        $(array).each(function () {
            if (serializeObj[this.name]) {
                if ($.isArray(serializeObj[this.name])) {
                    serializeObj[this.name].push(this.value);
                } else {
                    serializeObj[this.name] = [serializeObj[this.name], this.value];
                }
            } else {
                serializeObj[this.name] = this.value;
            }
        });
        return serializeObj;
    };


})(jQuery);