if (typeof (localStorage) == 'undefined') {
    var box = document.body || document.getElementsByTagName("head")[0] || document.documentElement;
    userdataobj = document.createElement('input');
    userdataobj.type = "hidden";
    userdataobj.addBehavior("#default#userData");
    box.appendChild(userdataobj);
    //设定对象
    var localStorage = {
        setItem: function (nam, val) {
            userdataobj.load(nam);
            userdataobj.setAttribute(nam, val);
            var d = new Date();
            d.setDate(d.getDate() + 700);
            userdataobj.expires = d.toUTCString();
            userdataobj.save(nam);
            userdataobj.load("userdata_record");
            var dt = userdataobj.getAttribute("userdata_record");
            if (dt == null) dt = '';
            dt = dt + nam + ",";
            userdataobj.setAttribute("userdata_record", dt);
            userdataobj.save("userdata_record");
        },
        //模拟 setItem
        getItem: function (nam) {
            userdataobj.load(nam);
            return userdataobj.getAttribute(nam);
        },
        //模拟 getItem
        removeItem: function (nam) {
            userdataobj.load(nam);
            clear_userdata(nam);
            userdataobj.load("userdata_record");
            var dt = userdataobj.getAttribute("userdata_record");
            var reg = new RegExp(nam + ",", "g");
            dt = dt.replace(reg, '');
            var d = new Date();
            d.setDate(d.getDate() + 700);
            userdataobj.expires = d.toUTCString();
            userdataobj.setAttribute("userdata_record", dt);
            userdataobj.save("userdata_record");
        },
        //模拟 removeItem
        clear: function () {
            userdataobj.load("userdata_record");
            var dt = userdataobj.getAttribute("userdata_record").split(",");
            for (var i in dt) {
                if (dt[i] != '') clear_userdata(dt[i])
            }
            clear_userdata("userdata_record")
        }
        //模拟 clear();
    }
    function clear_userdata(keyname) //将名字为keyname的变量消除
    {
        var keyname;
        var d = new Date();
        d.setDate(d.getDate() - 1);
        userdataobj.load(keyname);
        userdataobj.expires = d.toUTCString();
        userdataobj.save(keyname);
    }
}
/*弹出层*/
/*
title	标题
url		请求的url
id		需要操作的数据id
w		弹出层宽度（缺省调默认值）
h		弹出层高度（缺省调默认值）
*/
function poplayer(title, url, w, h) {

    if (title == null || title == '') {
        title = false;
    };
    if (url == null || url == '') {
        url = "404.html";
    };
    if (w == null || w == '') {
        w = 800;
    };
    if (h == null || h == '') {
        h = "auto";
    }
    var layerId = parent.layer.open({
        type: 2,
        area: [w + 'px', h],
        offset: '30px',
        fix: false,
        closeBtn: 1,
        maxmin: true,
        // shade: false,
        shade: 0.4,
        title: title,
        content: url
    });
    parent.popWindow = this;
    return layerId;
}

function changePassword() {
    poplayer('修改密码', '/System/SysUser/ChangePassword', '800', "250px");
}
       
       