var wrapper = {};

//首页设置对象
wrapper.settings = {
    homeTabTitle: '主页',
    homeTabUrl: '/Main/Index',
    navigation: $('#hd_navigation').val(),
    maxTabCount: 10
};

wrapper.model = null;
wrapper.maintabs = $('#tabs');

//初始化
wrapper.init = function () {
    //wrapper.checklogin();

    ////每间隔10秒 检测登录信息
    //setInterval(function () {
    //    $('#span_checklogin').html('[正在检测登录状态......]');
    //    wrapper.checklogin();
    //}, 10000);

    //加载当前用户拥有权限的模块菜单数据
    //com.ajax({ type: 'GET', url: '/Home/UserMenu' + com.settings.ajax_timestamp(), showLoading: false, success: wrapper.initMenu });
    wrapper.initMenu(wrapper.model);

    //设置信息
    wrapper.settings.navigation = $('#hd_navigation').val();
    $("#div-password-edit").dialog_page({}).dialog('close');
    $('.loginOut').click(wrapper.logout);
    $('.changepwd').click(wrapper.changePassword);
    $('.myconfig').click(wrapper.mysettings);

    //初始化jnotify组件显示
    $('#notity').jnotifyInizialize({ oneAtTime: true, appendType: 'append' }).css({ 'position': 'absolute', '*top': '5px', 'left': '30%', 'right': '30%', 'margin': '5px 0px 0px 0px', '*margin': '0px 0px 0px 0px', 'word-wrap': 'break-word', 'width': 'auto', 'height': 'auto', 'z-index': '99999', 'border-radius': '5px' });
    //初始化Tab标签选项卡标题右键事件处理
    $('#closeMenu').menu({ onClick: wrapper.rightMenuClick });
    //初始化main_panel中的tabs
    wrapper.maintabs.tabs({
        border: false,
        onContextMenu: wrapper.tabContextMenu
    });

    //添加我的桌面tab页 【使用jeasyui.tabs.extend.js扩展】
    wrapper.addIframeTab(wrapper.settings.homeTabTitle, 'portal', wrapper.settings.homeTabUrl, 'icon-standard-application-home', false);

    //初始化Tab标签选项卡双击关闭tab事件 【使用jeasyui.tabs.extend.js扩展】
    wrapper.maintabs.tabs('bindDblclick', function (index, title) {
        if (title != wrapper.settings.homeTabTitle)
            wrapper.maintabs.tabs('close', title);
    });
};

//检测登录状态
wrapper.checklogin = function () {
    com.ajax({
        type: 'GET', url: '/Login/CheckLogin' + com.settings.ajax_timestamp(), showLoading: false, success: function (result) {
            if (result.s == false) {
                com.alert("用户未登录或登录已超时");
                window.location.href = '/Login';
            } else {
                $('#span_checklogin').html('[ok]');
            }
        }
    });
}

//初始化配置对象
wrapper.initSettings = function (settings) {
    wrapper.settings = $.extend(wrapper.settings, settings);
};

//新增tab选项卡页面 【使用jeasyui.tabs.extend.js扩展】
/*
参数说明：title:tab标题   menucode：自定义值(这里用来存储菜单代码) icon：图标  closable：是否显示关闭按钮
*/
wrapper.addIframeTab = function (title, menucode, url, icon, closable) {
    if (!closable) closable = false;//默认值是false，不显示关闭按钮
    if (!menucode) menucode = '';
    wrapper.maintabs.tabs('addIframeTab', {
        //tab参数为一对象，其属性同于原生add方法参数
        opts_ext: {
            title: title, menucode: menucode, closable: closable, icon: icon,
            tools: [{
                iconCls: 'icon-mini-refresh',
                handler: function (e) {
                    var temp_title = $(e.target).parent().parent().text();
                    wrapper.maintabs.tabs('updateIframeTab', { 'which': temp_title });
                }
            }]
        },
        /*iframe参数用于设置iframe信息，包含属性有：
        src：iframe地址
        frameBorder：iframe边框,，默认值为0
        delay：淡入淡出效果时间
        height：iframe高度，默认值为100%
        width：iframe宽度，默认值为100%
        */
        iframe: { src: url, delay: 10 }
    });
    wrapper.maintabs.tabs('addEventParam');
}

//初始化权限菜单
wrapper.initMenu = function (menuData) {
    if (!menuData || !menuData.length) {
        $.messager.alert("系统提示", "<font color=red><b>您没有任何权限！请联系管理员。</b></font>", "warning", function () {
            // location.href = '/login';
            wrapper.logout();
        });
        return;
    }

    //.data() 方法允许我们安全地将任何类型的数据附加到DOM元素上而不用担心循环引用和内存泄露之类的问题
    //$('body').data('menulist', menuData);

    //将菜单数据转换成treedata格式。
    //var visibleMenu = $.grep(menuData, function (row) { return row.Enabled; });
    var menus = utils.toTreeData(menuData, 'MenuCode', 'ParentCode', 'children');

    //根据当前用户菜单风格配置，来初始化不同的菜单风格
    switch (wrapper.settings.navigation) {
        case "tree"://树状菜单，使用zTree初始化
            wrapper.menuTree(menus);
            //wrapper.menuButton(menus);
            break;
        case "menubutton":
            wrapper.menuButton(menus);
            break;
        case "accordion"://手风琴菜单，子菜单使用树状呈现
            wrapper.menuAccordion(menus);
            break;
        default:
            wrapper.menuAccordion(menus);
            break;
    }
    //wrapper.initLocationHash(menuData);
};

wrapper.initLocationHash = function (data) {
    var subUrl = location.hash.replace('#!', '');
    $.each(data, function () {
        var s = this.Url.replace('.aspx', '');
        if (this.Url && this.Url != '#' && (subUrl == s || subUrl.indexOf(s + "/") > -1))
            wrapper.addTab(this.MenuName, this.MenuCode, subUrl, this.IconClass);
    });
};

//tabContextMenu事件
wrapper.tabContextMenu = function (e, title) {
    $('#closeMenu').menu('show', { left: e.pageX, top: e.pageY });
    wrapper.maintabs.tabs('select', title);
    e.preventDefault();
};

wrapper.changePassword = function () {
    $("#div-password-edit").dialog_page({
        title: '个人密码修改', iconCls: 'icon-standard-key', resizable: true, maximizable: true, inline: true, width: 400, height: 200, top: 200,
        onClickButton: function (self) { //保存操作
            //  com.message('warning', '演示版修改密码功能已禁用！'); return;
            var confirmPassword = com.trim($("#confirmpassword").val());
            var newPassword = com.trim($("#newpassword").val());
            if (newPassword == "") { return com.message('warning', '新密码不能为空！'); return; }
            if (confirmPassword == "") { return com.message('warning', '确认密码不能为空！'); return; }
            if (newPassword != confirmPassword) { return com.message('warning', '确认密码不正确！'); return; }
            $.get("/Home/ChangePassword", { newPassword: newPassword }, function (result) {
                if (result == "") {
                    com.message('s', "修改密码成功");
                    self.dialog('close');
                } else {
                    com.message('e', result);
                }
            })
        }
    });// end $("#div-password-edit").dialog_page
};

wrapper.mysettings = function () {
    wrapper.addTab("个性化设置", '', "/Sys/Config/", "icon-standard-cog");
};

wrapper.logout = function () {
    $.messager.confirm('系统提示', '<b style="color:red">您确定要退出本次登录吗?</b>', function (r) {
        if (r) location.href = '/Account/LogOut' + com.settings.ajax_timestamp();
    });
};

//全屏事件
wrapper.setFullScreen = function (target) {
    var that = $(target);
    if (that.find('.icon-standard-arrow-out').length) {
        that.find('.icon-standard-arrow-out').removeClass('icon-standard-arrow-out').addClass('icon-standard-arrow-in');
        $('#homebody').layout('panel', 'north').panel('close');
        $('#homebody').layout('panel', 'south').panel('close');
        $('#homebody').layout('panel', 'west').panel('close');
        $('#homebody').layout('resize');
    } else if ($(target).find('.icon-standard-arrow-in').length) {
        that.find('.icon-standard-arrow-in').removeClass('icon-standard-arrow-in').addClass('icon-standard-arrow-out');
        $('#homebody').layout('panel', 'north').panel('open');
        $('#homebody').layout('panel', 'south').panel('open');
        $('#homebody').layout('panel', 'west').panel('open');
        $('#homebody').layout('resize');
    }
};

//在新页面打开选项卡
wrapper.jumpOpen = function () {
    var currentTab = wrapper.maintabs.tabs('getSelected');
    var currtab_title = currentTab.panel('options').title;
    if (currtab_title == wrapper.settings.homeTabTitle) {
        com.message('w', '此选项卡不能再新页面中打开');
        return;
    }
    var url = wrapper.maintabs.tabs('getIframeTabUrl', { 'which': currtab_title });
    if (url) {
        window.open(url, "_blank");
    } else {
        var msgText = "\"" + opts.title + "\" 选项卡不可在新页面中打开。";
        $.messager.show({ title: '提示', msg: msgText, showType: 'show' });
    }
}

//Tab标签选项卡最右侧菜单按钮点击事件
wrapper.rightMenuClick = function (item) {
    var $tab = wrapper.maintabs;
    var currentTab = $tab.tabs('getSelected');
    var titles = wrapper.getTabTitles($tab);

    switch (item.id) {
        case "refresh":
            var currtab_title = currentTab.panel('options').title;
            $tab.tabs('updateIframeTab', { 'which': currtab_title });
            break;
        case "close":
            var currtab_title = currentTab.panel('options').title;
            $tab.tabs('close', currtab_title);
            break;
        case "closeall":
            $.each(titles, function () {
                if (this != wrapper.settings.homeTabTitle)
                    $tab.tabs('close', this);
            });
            break;
        case "closeother":
            var currtab_title = currentTab.panel('options').title;
            $.each(titles, function () {
                if (this != currtab_title && this != wrapper.settings.homeTabTitle)
                    $tab.tabs('close', this);
            });
            break;
        case "closeright":
            var tabIndex = $tab.tabs('getTabIndex', currentTab);
            if (tabIndex == titles.length - 1) {
                alert('亲，后边没有啦 ^@^!!');
                return false;
            }
            $.each(titles, function (i) {
                if (i > tabIndex && this != wrapper.settings.homeTabTitle)
                    $tab.tabs('close', this);
            });

            break;
        case "closeleft":
            var tabIndex = $tab.tabs('getTabIndex', currentTab);
            if (tabIndex == 1) {
                alert('亲，前边那个上头有人，咱惹不起哦。 ^@^!!');
                return false;
            }
            $.each(titles, function (i) {
                if (i < tabIndex && this != wrapper.settings.homeTabTitle)
                    $tab.tabs('close', this);
            });
            break;
        case "exit":
            $('#closeMenu').menu('hide');
            break;
    }

};



//刷新当前打开的Tab标签选项卡
wrapper.tabRefresh = function () {
    var currentTab = wrapper.maintabs.tabs('getSelected');
    var currtab_title = currentTab.panel('options').title;
    wrapper.maintabs.tabs('updateIframeTab', { 'which': currtab_title });
};
//关闭当前打开的Tab标签选项卡
wrapper.tabClose = function () { wrapper.rightMenuClick({ id: 'close' }); };
//关闭所有Tab标签选项卡
wrapper.tabCloseAll = function () { com.message('c', '确认要关闭所有窗口吗？', function (ok) { if (ok) { wrapper.rightMenuClick({ id: 'closeall' }); } }); };

//创建iframe页面
wrapper.createFrame = function (url) {
    return '<iframe scrolling="no" frameborder="0" style="width:100%;height:100%; padding:0;" src="' + url + '" ></iframe>';
}

//打开Tab标签页面
wrapper.openTabHandler = function ($tab, hasTab, subtitle, menucode, url, icon) {
    if (!hasTab) {
        wrapper.addIframeTab(subtitle, menucode, url, icon, true);//使用tabs扩展方法新增tab
    } else {
        $tab.tabs('select', subtitle); //选中Tab页面
        $tab.tabs('updateIframeTab', { 'which': subtitle });//选择TAB时刷新页面
    }
    //$('#homebody').layout('collapse', 'west');
};

wrapper.getTabTitles = function ($tab) {
    var titles = [];
    var tabs = $tab.tabs('tabs');
    $.each(tabs, function () { titles.push($(this).panel('options').title); });
    return titles;
};

//新增Tab标签页面
wrapper.addTab = function (subtitle, menucode, url, icon) {
    if (!url || url == '#') return false;
    var $tab = wrapper.maintabs;
    var tabCount = $tab.tabs('tabs').length;
    var hasTab = $tab.tabs('exists', subtitle);
    if ((tabCount <= wrapper.settings.maxTabCount) || hasTab)
        wrapper.openTabHandler($tab, hasTab, subtitle, menucode, url, icon);
    else
        com.message("confirm", '<b style="color:red">您当前打开了太多的页面，如果继续打开，会造成程序运行缓慢，无法流畅操作！</b>', function (b) {
            if (b)
                wrapper.openTabHandler($tab, hasTab, subtitle, url, icon);
        });
};

//导航菜单样式：生成Accordion样式的导航菜单，里面是树状菜单
wrapper.menuAccordion = function (menus) {
    var wnav = $('#wnav');
    wnav.accordion({ animate: true, fit: true, border: false });
    $.each(menus, function (i, item) {
        var c_id = 'tree_' + item.MenuCode
        wnav.accordion('add', {
            title: item.MenuName,
            content: '<ul id="' + c_id + '"></ul>',
            iconCls: 'icon ' + item.IconClass,
            border: false
        });
        //$.parser.parse(); //其中$.parser.parse(); 是再次加载Easyui
        var ztree = $('#' + c_id).addClass("ztree");
        var settings = {
            data: { key: { name: "MenuName", url: "javascript:void(0);" } }, callback: {
                onClick: function (event, treeId, node) { wrapper.addTab(node.MenuName, node.MenuCode, node.Url, node.IconClass); }
            }
        }
        if (item.children.length > 0) item.open = true;
        $.fn.zTree.init(ztree, settings, item.children);
    });//end $.each
};

//导航菜单样式：生成ZTree样式的导航菜单
wrapper.menuTree = function (menus) {
    var settings = {
        data: { key: { name: "MenuName", url: "javascript:void(0);" } }, callback: {
            onClick: function (event, treeId, node) { wrapper.addTab(node.MenuName, node.MenuCode, node.Url, node.IconClass); }
        }
    };
    var ztree = $('#wnav').addClass("ztree");
    if (menus.length > 0) {
        for (var i = 0; i < menus.length; i++) {
            menus[i].open = true;
        }
    }
    $.fn.zTree.init(ztree, settings, menus);
};

//导航菜单样式：生成横向下拉样式的导航菜单
wrapper.menuButtonChild = function (s, n) {
    var str = '';
    $.each(n.children, function (j, o) {
        if (o.children) {
            str += '<div>';
            str += '<span iconCls="' + o.IconClass + '" menucode="' + o.MenuCode + '">' + o.MenuName + '</span><div style="width:120px;">';
            str = wrapper.menuButtonChild(str, o);
            str += '</div></div>';
        } else
            str += '<div iconCls="' + o.IconClass + '" url="' + o.Url + '"  menucode="' + o.MenuCode + '">' + o.MenuName + '</div>';
    });
    return s + str;
}
wrapper.menuButton = function (menus) {
    var menulist = "";
    var childMenu = '';
    //遍历一级菜单
    $.each(menus, function (i, n) {
        menulist += utils.formatString('<a href="javascript:void(0)" id="mb{0}" class="easyui-menubutton" menu="#mm{0}" iconCls="{1}">{2}</a>',
            (i + 1), n.IconClass, n.MenuName);

        if ((n.children || []).length > 0) {
            childMenu += '<div id="mm' + (i + 1) + '" style="min-width:120px;">';
            childMenu += wrapper.menuButtonChild('', n);//针对每一个一级菜单 再进行递归遍历所有子菜单
            childMenu += '</div>';
        }
    });
    //将生成的menubutton html字符串添加到导航元素wnav中
    $('#wnav').append(menulist).append(childMenu);
    //初始化menubutton按钮事件
    var mb = $('#wnav .easyui-menubutton').menubutton();
    $.each(mb, function (i, n) {
        $($(n).menubutton('options').menu).menu({
            onClick: function (item) {
                var tabTitle = item.text;
                var menucode = $(item.target).attr("menucode");// item.menucode; edit by yxz 10:27 2016-04-05
                var url = $(item.target).attr("url");// item.url;edit by yxz 10:27 2016-04-05
                var icon = item.iconCls;
                wrapper.addTab(tabTitle, menucode, url, icon);
                return false;
            }
        });
    });
};


$.getJSON("/Home/GetMenuTree?t=" + com.settings.timestamp(), function (data) {
   
    //  var s = JSON.stringify(data);
    wrapper.model = data;// JSON.parse(data);
    $(wrapper.init);
});
