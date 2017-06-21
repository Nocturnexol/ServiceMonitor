var popWindow = null;
/*
* 清空字符串前后的空格
*/
String.prototype.Trim = function () { return this.replace(/(^\s*)|(\s*$)/g, ""); }
String.prototype.LTrim = function () { return this.replace(/(^\s*)/g, ""); }
String.prototype.RTrim = function () { return this.replace(/(\s*$)/g, ""); }
/*
* 获取日期对象当前或以后几天的时间
*/
Date.prototype.addDays = function (number) {
    if (!isNaN(number)) {
        var nowTime = this.getTime();
        return new Date(nowTime + (number) * 24 * 60 * 60 * 1000);
    } else {
        return this;
    }
};
/*
* 将日期对象显示成UTC日期格式“yyyy-MM-dd”或者“yyyy/MM/dd”格式，可选分隔符范围["-","/"]
* param：separator，string，分隔符
*/
Date.prototype.toDateFormatString = function (separator) {
    var year = this.getFullYear().toString();
    var month = (this.getMonth() + 1).toString();
    var date = this.getDate().toString();
    var str = year.toString() + "-" + (month.length == 1 ? "0" + month : month) + "-" + (date.length == 1 ? "0" + date : date);
    if (arguments.length == 1 && separator.toString() == "/") {
        return str.replace("-", separator);
    }
    return str;
};
/**
* json格式日期("/Date(1410019200000+0800)/")转换成js日期
* param：value，string，json格式的日志字符串
*/
function ConvertJsonToDate(value) {
    return eval(value.replace(/\/Date\((\d+)\)\//gi, "new Date($1)"));
}
/*
* n秒自动跳转页面函数
* param seccs,int 剩余的秒数
* param timeId,string 显示剩余时间的元素Id
* param url,string 跳转到的页面地址
*/
function autoForwardUrl(secs, timeId, url) {
    document.getElementById(timeId).innerText = secs;
    if (--secs > 0) {
        setTimeout("autoForwardUrl(" + secs + ",'" + timeId + "','" + url + "')", 1000);
    } else {
        location.href = url;
    }
}
//通用Jquery read事件
$(function () {
    $('#btnClose').click(function () {
        var ii = parent.layer.getFrameIndex(window.name);
        parent.layer.close(ii);

    });
    $("#tbl1").addClass("table-striped");


    $("#btnCreate").click(function () {
        //$("#a_add")[0].click();
        popIframe($("#a_add")[0].href);
    });
    $("a[tag='btnPop']").click(function () {
        //$("#a_add")[0].click();
        //        alert(this.href);
        popIframe(this.href);

        return false;
    });
    $("#btnClosePop").click(function () {
        var index = parent.layer.getFrameIndex(window.name);
        parent.layer.close(index);
    });

    //全选/取消
    $("#chkAll").click(function () {
        $("table tr>td:first-child>:checkbox").attr("checked", this.checked);
    })

    //分页时页面跳转按钮
    $("#go").click(function () {
        var txtNum = $("#txtNum").val();
        var pageIndex = parseInt(txtNum, 10);
        pageIndex = Math.max(1, pageIndex);
        $("#go")[0].href = $("#go")[0].href.toString().replace(/(page=)(\d+)/g, "$1" + pageIndex);
    })
    $("#btnReturn").click(function () {
        $("#a_return")[0].click();
    });

    // 初始化搜索框
    initSearchText();
});
//弹出页
function popIframe(url) {
    return layer.open({
        type: 2,
        title: false,
        fix: true,
        closeBtn: false,
        shadeClose: true,
        shade: [0.3, '#000', true],
        border: [1, 0.3, '#666', false],
        offset: ['50px', ''],
        area: ['831px', 'auto'],
        iframe: { src: url }
    });
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
        closeBtn:1,
        maxmin: true,
        // shade: false,
        shade: 0.4,
        title: title,
        content: url
    });
    parent.popWindow = this;
    return layerId;
}



function addCheckAllEvent(containerId, chkId) {
    $("#" + chkId).click(function () {
        $("#" + containerId + " :checkbox").prop("checked", this.checked);
    });
}
//关闭窗口(无提示框)
function CloseWin() {
    window.opener = null;
    //window.opener=top;    
    window.open("", "_self");
    window.close();
}
//打开窗口
function openOneWin(urlObj) {
    window.open(urlObj.href, '1', '');
    return false;
}
function openWin(urlObj) {
    window.open(urlObj.href, '权限管理', 'toolbar=no, menubar=no, resizable=yes,location=no, status=yes,scrollbars=yes');
    return false;
}
//点开 折叠 1.objId 要折叠的控件,2.ImgId 当前点击的图片ID,3.obj 当前点击的图片对象
function fadeToOutTable(objId, ImgId, obj) {
    $("#" + objId).fadeToggle(10);
    if (obj.title.valueOf() == "收起") {
        //$("#" + ImgId).attr("src", "../..//Resource/images/1.png");
        $("#" + ImgId).removeClass("ig_up").addClass("ig_down");
        $("#" + ImgId).attr("title", "展开");
    }
    else {
        //$("#" + ImgId).attr("src", "../..//Resource/images/2.png");
        $("#" + ImgId).removeClass("ig_down").addClass("ig_up");
        $("#" + ImgId).attr("title", "收起");
    }
}

//根据名称获取页面地址参数值
function getQueryString(name) {
    var reg = new RegExp("(^|&|\\?)" + name + "=([^&]*)(&|$)"), r;
    if (r = location.search.match(reg))
        return unescape(r[2]);
    return null;
}
//获取地址传参数值列表
function getQueryStrings() {
    var url = location.search; //获取url中"?"符及后的字串 
    var theRequest = new Object();
    if (url.indexOf("?") != -1) {
        var str = url.substr(1);
        var srchArray = str.split("&");
        var tempArray = new Array();
        for (var i = 0; i < srchArray.length; i++) {
            tempArray = srchArray[i].split("=");
            theRequest[tempArray[0]] = unescape(tempArray[1]);
        }
    }
    return theRequest;
}

///删除记录
///id：记录ID 
///url:删除方法所在Controller(例如：/System/SysUser/DeleteData)
function del(id, url) {
    layer.confirm("确定要删除该记录吗？", function () {
        var data = { "TblRcdId": id };
        if (data != null) {
            data = { "paramData": JSON.stringify(data) };
            $.post(url, data, function (result) {
                if (result.IsSuccess) {
                    layer.msg("删除成功！", 1, function () {
                     window.top.location.reload();
                    });
                } else {
                    layer.msg("删除失败," + result.Message);
                }
            }, "json");
        }
    }, "消息");
}
///删除记录
///id：记录ID 
///url:删除方法所在Controller(例如：/System/SysUser/DeleteData)
function delList(id, url) {
    layer.confirm("确定要删除该记录吗？", function () {
        var data = { "TblRcdId": id };
        if (data != null) {
            $.post(url, data, function (result) {
                if (result.IsSuccess) {
                    layer.msg("删除成功！", 1, function () {
                        window.top.location.reload();
                    });
                } else {
                    layer.msg("删除失败," + result.Message);
                }
            }, "json");
        }
    }, "消息");
}
//合同续约2014-08-21lijing
function Extensiondel(id, url) {
    var data = { "TblRcdId": id };
    if (data != null) {
        data = { "paramData": JSON.stringify(data) };
        $.post(url, data, function (result) {
            if (result.IsSuccess) {
                layer.msg("续约成功请修改发布时间！", 1, function () {
                    window.location.href = "/AdOrder/AdOrder/Details?TitleType=MediaOrder&TblRcdId=" + result.Message;
                });
            } else {
                layer.msg("续约失败!");
            }
        }, "json");
    }
}

//合同改稿2014-08-21lijing
function Correctingdel(id, url) {
    var data = { "TblRcdId": id };
    if (data != null) {
        data = { "paramData": JSON.stringify(data) };
        $.post(url, data, function (result) {
            if (result.IsSuccess) {
                layer.msg("改稿成功！", 1, function () {
                    window.location.href = "/AdOrder/AdOrder/CorrectingAdOrderDetails?TitleType=CorrectingAdOrder&TblRcdId=" + result.Message;
                });
            } else {
                layer.msg("改稿失败!");
            }
        }, "json");
    }
}


///设置(户外媒体/车身媒体)关联订单
///url:提交地址
function setRelationOrder(url) {
    var AdorderId = $("#AdOrderId").val();
    if (AdorderId == null || AdorderId == undefined || AdorderId == "") {
        layer.msg("请选择订单号!");
        return;
    }
    var data = { "paramData": $("#paramData").val(), "AdOrderId": AdorderId };
    $.post(url, data, function (result) {
        if (result.IsSuccess) {
            layer.msg("预定成功！", 1, function () {
                window.location.href = "/AdOrder/AdOrder/Details?TitleType=MediaOrder&TblRcdId=" + result.ResultData["TblRcdId"];
            });
        } else {
            layer.msg("预定失败," + result.Message);
        }
    }, "json");
}
///初始化查询条件
///requestParams：查询条件数组
function initSearch(requestParams) {
    // 初始化过滤条件
    for (var i = 0; i < requestParams.length; i++) {
        var $obj = $("#" + requestParams[i] + "txt");
        if ($obj.size()) {
            var paramVal = $obj.val() // 获取参数值
            if (paramVal != "") {
                $("#" + requestParams[i] + "label").html(paramVal);
                $("dl.hasBeenSelected").show();
                $("#" + requestParams[i] + "div").show();
                $("#" + requestParams[i] + "dl").hide();
            }
        }
    }
}

// 初始化搜索框
function initSearchText() {
    var $searchTxt = $("#searchTxt");
    if ($searchTxt.size()) {
        // 初始化提示消息
        if ($searchTxt.val() == "") {
            $searchTxt.css("color", "#999").val($searchTxt.attr("placeholder"));
        }

        //添加事件
        $searchTxt.blur(function () {
            if ($(this).val() == "") {
                $(this).css("color", "#999").val($(this).attr("placeholder"));
            }
        }).focus(function () {
            if ($(this).val() == $(this).attr("placeholder")) {
                $(this).css("color", "black").val("");
            }
        }).keypress(function (event) {
            switch (event.keyCode) {
                //通过调用stopPropagation()来阻止事件冒泡                                                                                                                                                                                                                                                                                                                                        
                case 13: event.stopPropagation();
                    doSearch();
                    break;
            }
        });
        $("#searchIcon").click(function () {
            doSearch();
        });
    }
}
//搜索框搜索方法
function doSearch() {
    var $obj = $("#searchTxt");
    if ($obj.size()) {
        if ($obj.val() == $obj.attr("placeholder")) {
            $obj.val("");
        }
    }
    // 触发查询按钮事件
    $("#btnSearch").click();

}

///设置查询条件条件的值
///idPrefix：查询项前缀
///val:查询的值
function btnSaveBussinessonclick(idPrefix, val) {
    if (idPrefix == "all") {
        var $obj;
        for (var i = 0; i < requestParams.length; i++) {
            $obj = $("#" + requestParams[i] + "txt");
            if ($obj.size()) {
                $obj.val("");
                $("#" + requestParams[i] + "div").hide();
                $("#" + requestParams[i] + "dl").show();
            }
        }
        if ($(".resizediv1>div:visible").length == 0) {
            $(".hasBeenSelected").hide();
        }
    } else {
        $("#" + idPrefix + "txt").val(val);
    }
    if (val != "") {
        $("#" + idPrefix + "label").html(val);
        $(".hasBeenSelected").show();
        $("#" + idPrefix + "div").show();
        $("#" + idPrefix + "dl").hide();
    } else {
        $("#" + idPrefix + "dl").show();
        $("#" + idPrefix + "div").hide();
        if ($(".resizediv1>div:visible").length == 0) {
            $(".hasBeenSelected").hide();
        }
    }
    doSearch();
}

///添加户外选位
///id：记录ID 
///url:
function btnSavebtnJoinonclick(url) {
    var IssueDate = $("#IssueDatetxt").attr("value");
    var EndDate = $("#EndDatetxt").attr("value");
    if (IssueDate == null || IssueDate == undefined || IssueDate == "" || EndDate == null || EndDate == undefined || EndDate == "") {
        layer.msg("请选择开始时间和结束时间!");
        return false;
    }
    if (IssueDate > EndDate) {

        layer.msg("开始时间必须小于结束时间!");
        return false;
    }
    var date = new Date();
    var sYear = date.getYear();
    var sMonth = date.getMonth() + 1;
    if (sMonth.toString().length < 2) {
        sMonth = "0" + sMonth;
    }
    var sDNo = date.getDate();
    if (sDNo.toString().length < 2) {
        sDNo = "0" + sDNo;
    }
    var sDate = sYear + "-" + sMonth + "-" + sDNo;

    if (sDate > EndDate) {
        layer.msg("结束时间必须大于当前时间!");
        return false;
    }
    if ($(":checkbox[name='check'][checked]").size() <= 0) {
        layer.msg("请选择要加入的数据！");
        return null;
    }
    var dataa = "";
    $(":checkbox[name='check'][checked]").each(function () {
        var RightInfo = this.value;
        if (RightInfo != "" && RightInfo != undefined && RightInfo != null) {
            if (dataa != "" && dataa != undefined && dataa != null) {
                dataa = dataa + "*" + RightInfo;
            }
            else {
                dataa = dataa + RightInfo;
            }
        }
    })

    if (dataa == null || dataa == undefined || dataa == "") {
        layer.msg("请选择媒体数据!");
    }
    if (dataa != null) {
        data = { "paramData": dataa, "IssueDate": IssueDate, "EndDate": EndDate };
        $.post(url, data, function (result) {
            if (result.IsSuccess) {
                layer.msg("添加成功！", 1, function () {
                    window.top.location.reload();
                });
            } else {
                layer.msg("添加失败," + result.Message);
            }
        }, "json");
    }

}

function addCreate(btnId) {
    popIframe($("#" + btnId)[0].href);
}

//单个表数据导出
function GetCheckBoxExport(url) {
    var TblRcdIdlist = "";
    var jqData = JQ.GetAllSelected("jqGridList").Data;
    for (var i = 0; i < jqData.length; i++) {
        var RightInfo = jqData[i];
        TblRcdIdlist += RightInfo + "*";
    }
    /*$(":checkbox[name='check'][checked]").each(function () {
    var RightInfo = this.value;
    TblRcdIdlist += RightInfo + "*";
    });*/
    var pageurl = window.location.search;
    var data = { "paramData": TblRcdIdlist };
    //添加额外的参数
    if (arguments.length >= 2) {
        $.extend(data, arguments[1]);
    }
    exp(data, url + pageurl)
    return false;
}


function GetCheckBoxExportCount(url) {

    var CountDate = $("#CountDateid").val();

    if (CountDate != null) {
        var data = { "paramData": CountDate };
        exp(data, url)

    }
    return false;
}
//明细表派工
function DispatchingList(url, MainKey, id) {
    var TblRcdIdlist = "";
    var data = JQ.GetAllSelected(id).Data;
    for (var i = 0; i < data.length; i++) {
        var RightInfo = data[i];
        TblRcdIdlist += RightInfo + "*";
    }
    var data = { "paramData": TblRcdIdlist, "MainKey": MainKey };
    if (TblRcdIdlist != null) {
        $.post(url, data, function (result) {
            if (result.IsSuccess) {
                layer.alert(result.Message, { icon: 1 }, function (index) {
                    $("#jqGridList").jqGrid('setGridParam', { url: '/Inventory/LcdNew/GetLcdNewListPageDataList?MainKey=' + MainKey, postData: '' }).trigger('reloadGrid'); 
                    layer.close(index);
             });
            } else {
                layer.alert("派工失败," + result.Message, { icon: 1 }, function (index) {
                    layer.close(index);
                });
                //layer.msg("派工失败," + result.Message);
            }
        }, "json");
    }
    return false;
}
//明细表审核
function CheckDataList(url, MainKey, id) {
    var TblRcdIdlist = "";
    var data = JQ.GetAllSelected(id).Data;
    for (var i = 0; i < data.length; i++) {
        var RightInfo = data[i];
        TblRcdIdlist += RightInfo + "*";
    }
    var data = { "paramData": TblRcdIdlist, "MainKey": MainKey };
    if (TblRcdIdlist != null) {
        $.post(url, data, function (result) {
            if (result.IsSuccess) {
                layer.alert(result.Message, { icon: 1 }, function (index) {
                    $("#jqGridList").jqGrid('setGridParam', { url: '/Inventory/LcdNew/GetLcdNewListPageDataList?MainKey=' + MainKey, postData: '' }).trigger('reloadGrid'); 
                    layer.close(index);
                });
            } else {
                layer.alert("审批失败," + result.Message, { icon: 1 }, function (index) {
                    layer.close(index);
                });
               // layer.msg("审批失败," + result.Message);
            }
        }, "json");
    }
    return false;
}
//明细表
function GetCheckBoxExportS(url, MainKey, id) {
    var TblRcdIdlist = "";
    var data = JQ.GetAllSelected(id).Data;


    for (var i = 0; i < data.length; i++) {
        var RightInfo = data[i];
        TblRcdIdlist += RightInfo + "*";
    }
    var data = { "paramData": TblRcdIdlist, "MainKey": MainKey };
    exp(data, url)
    return false;
}

//多个明细
function GetCheckBoxExportList(url, id) {
    var TblRcdIdlist = "";

    if (JQ.GetAllSelected(id).Len > 0) {
        var jqData = JQ.GetAllSelected(id).Data;
        for (var i = 0; i < JQ.GetAllSelected(id).Len; i++) {
            var RightInfo = jqData[i];
            TblRcdIdlist += RightInfo + "*";
        }
    }
    var data = { "paramData": TblRcdIdlist };
    var obj = $('#searchArray').serializeArray();
//    for (var ii = 0; ii < obj.length; ii++) {
//        data[obj[ii].name] = obj[ii].value
//    }

    for (var i = 0; i < obj.length; i++) {
        if (data[obj[i].name] != undefined && data[obj[i].name] != "" && data[obj[i].name] != null) {
            data[obj[i].name] = data[obj[i].name] + ";" + obj[i].value;
        } else {
            data[obj[i].name] = obj[i].value;
        }
    }

    exp(data, url + window.location.search)
    return false;
}
function exp(data, url) {
    if (data != null) {
        $.post(url, data, function (result) {
            if (result.IsSuccess) {
                location.href = "/Home/DownFile?filePath=" + result.Text;
            } else {
                layer.msg("导出失败," + result.Message);
            }
        }, "json");
    }
}

//删除多个明细
function DelCheckBoxList(url, checkName) {
    var TblRcdIdlist = "";
    if ($(":checkbox[name='" + checkName + "'][checked]").size() <= 0) {
        layer.msg("请选择要删除的数据！", 1);
        return null;
    }
    $(":checkbox[name='" + checkName + "'][checked]").each(function () {
        var RightInfo = this.value;
        TblRcdIdlist += RightInfo + "*";
    })
    if (TblRcdIdlist != null) {
        var data = { "paramData": TblRcdIdlist };
        $.post(url, data, function (result) {
            if (result.IsSuccess) {
                layer.msg("删除成功！", 1, 1, function () {
                    window.top.location.reload();
                });
            } else {
                layer.msg("删除失败," + result.Message);
            }
        }, "json");
    }
    return false;
}

//审核多个明细
function SureCheckBoxList(url, checkName) {
    var TblRcdIdlist = "";
    if ($(":checkbox[name='" + checkName + "'][checked]").size() <= 0) {
        layer.msg("请选择要审核的数据！", 1);
        return null;
    }
    var con;
    con = confirm("确定要审核该记录吗?"); //在页面上弹出对话框  
    if (con == true) {
        $(":checkbox[name='" + checkName + "'][checked]").each(function () {
            var RightInfo = this.value;
            TblRcdIdlist += RightInfo + "*";
        })
        if (TblRcdIdlist != null) {
            var data = { "paramData": TblRcdIdlist };
            $.post(url, data, function (result) {
                if (result.IsSuccess) {
                    layer.msg("审核成功！", 1, 1, function () {
                        window.top.location.reload();
                    });
                } else {
                    layer.msg("审核失败," + result.Message);
                }
            }, "json");
        } return false;
    }
    else
        return null;
}
//审核多个明细
function DispatchingCheckBoxList(url, checkName) {
    var TblRcdIdlist = "";
    if ($(":checkbox[name='" + checkName + "'][checked]").size() <= 0) {
        layer.msg("请选择要派工的数据！", 1);
        return null;
    }
    var con;
    con = confirm("确定要派工这些记录吗?"); //在页面上弹出对话框  
    if (con == true) {
        $(":checkbox[name='" + checkName + "'][checked]").each(function () {
            var RightInfo = this.value;
            TblRcdIdlist += RightInfo + "*";
        })
        if (TblRcdIdlist != null) {
            var data = { "paramData": TblRcdIdlist };
            $.post(url, data, function (result) {
                if (result.IsSuccess) {
                    layer.msg("派工成功！", 1, 1, function () {
                        window.top.location.reload();
                    });
                } else {
                    layer.msg("派工失败," + result.Message);
                }
            }, "json");
        } return false;
    }
    else
        return null;
}
function DelRoadLinePrice(url) {
    var RoadLine = $("#RoadLine").val();
    var ToDirection = $("#ToDirection").val();
    if (RoadLine != null) {
        var data = { "paramData": RoadLine + "-" + ToDirection };
        $.post(url, data, function (result) {
            if (result.IsSuccess) {
                layer.alert("删除成功！", { icon: 1 }, function () {
                    window.location.reload();
                });
            } else {
                layer.alert("删除失败," + result.Message);
            }
        }, "json");
    }
    return false;
}

//审批
function CheckData(url, TblRcdId) {
    var Id = TblRcdId;
    if (Id != null) {
        var data = { "paramData": Id };
        $.post(url, data, function (result) {
            if (result.IsSuccess) {
                layer.alert("审批成功！", { icon: 1 }, function (index) {
                    XPage.Search();
                    layer.close(index);
                });
            } else {
                layer.alert("审批失败," + result.Message);
            }
        }, "json");
    }
    return false;
}

/** 
* desc : 合并指定表格（表格id为table_id）指定列（列数为table_colnum）的相同文本的相邻单元格   
* @table_id 表格id : 为需要进行合并单元格的表格的id。如在HTMl中指定表格 id="data" ，此参数应为 #data    
* @table_colnum   : 为需要合并单元格的所在列.参考jQuery中nth-child的参数.若为数字，从最左边第一列为1开始算起;"even" 表示偶数列;"odd" 表示奇数列; "3n+1" 表示的列数为1、4、7、......  
* @table_minrow ? : 可选的,表示要合并列的行数最小的列,省略表示从第0行开始 (闭区间)  
* @table_maxrow ? : 可选的,表示要合并列的行数最大的列,省略表示最大行列数为表格最后一行 (开区间)  
*/
function table_rowspan(table_id, table_colnum) {
    if (table_colnum == "even") {
        table_colnum = "2n";
    }
    else if (table_colnum == "odd") {
        table_colnum = "2n+1";
    }
    else {
        table_colnum = "" + table_colnum;
    }
    var cols = [];
    var all_row_num = $(table_id + " tr td:nth-child(1)").length;
    var all_col_num = $(table_id + " tr:nth-child(1)").children().length;
    if (table_colnum.indexOf("n") == -1) {
        cols[0] = table_colnum;
    }
    else {
        var n = 0;
        var a = table_colnum.substring(0, table_colnum.indexOf("n"));
        var b = table_colnum.substring(table_colnum.indexOf("n") + 1);
        //alert("a="+a+"b="+(b==true));  
        a = a ? parseInt(a) : 1;
        b = b ? parseInt(b) : 0;
        //alert(b);  
        while (a * n + b <= all_col_num) {
            cols[n] = a * n + b;
            n++;
        }
    }
    var table_minrow = arguments[2] ? arguments[2] : 0;
    var table_maxrow = arguments[3] ? arguments[3] : all_row_num + 1;
    var table_firsttd = "";
    var table_currenttd = "";
    var table_SpanNum = 0;
    for (var j = 0; j < cols.length; j++) {
        $(table_id + " tr td:nth-child(" + cols[j] + ")").slice(table_minrow, table_maxrow).each(function (i) {
            var table_col_obj = $(this);
            if (table_col_obj.html() != " ") {
                if (i == 0) {
                    table_firsttd = $(this);
                    table_SpanNum = 1;
                }
                else {
                    table_currenttd = $(this);
                    if (table_firsttd.text() == table_currenttd.text()) {
                        table_SpanNum++;
                        table_currenttd.hide(); //remove();    
                        table_firsttd.attr("rowSpan", table_SpanNum);
                    } else {
                        table_firsttd = $(this);
                        table_SpanNum = 1;
                    }
                }
            }
        });
    }
}

/** 
* desc : 合并指定表格（表格id为table_id）指定行（行数为table_rownum）的相同文本的相邻单元格     
* @table_id 表格id : 为需要进行合并单元格的表格的id。如在HTMl中指定表格 id="data" ，此参数应为 #data    
* @table_rownum   : 为需要合并单元格的所在行.参考jQuery中nth-child的参数.若为数字，从最左边第一列为1开始算起;"even" 表示偶数行;"odd" 表示奇数行; "3n+1" 表示的行数为1、4、7、......  
* @table_mincolnum ? : 可选的,表示要合并行中的最小列,省略表示从第0列开始(闭区间) 
* @table_maxcolnum ? : 可选的,表示要合并行中的最大列,省略表示表格的最大列数(开区间)  
*/
function table_colspan(table_id, table_rownum) {
    //if(table_maxcolnum == void 0){table_maxcolnum=0;}  
    var table_mincolnum = arguments[2] ? arguments[2] : 0;
    var table_maxcolnum;
    var table_firsttd = "";
    var table_currenttd = "";
    var table_SpanNum = 0;
    $(table_id + " tr:nth-child(" + table_rownum + ")").each(function (i) {
        table_row_obj = $(this).children();
        table_maxcolnum = arguments[3] ? arguments[3] : table_row_obj.length;
        table_row_obj.slice(table_mincolnum, table_maxcolnum).each(function (i) {
            if (i == 0) {
                table_firsttd = $(this);
                table_SpanNum = 1;
            } else if ((table_maxcolnum > 0) && (i > table_maxcolnum)) {
                return "";
            } else {
                table_currenttd = $(this);
                if (table_firsttd.text() == table_currenttd.text()) {
                    table_SpanNum++;
                    if (table_currenttd.is(":visible")) {
                        table_firsttd.width(parseInt(table_firsttd.width()) + parseInt(table_currenttd.width()));
                    }
                    table_currenttd.hide(); //remove();    
                    table_firsttd.attr("colSpan", table_SpanNum);
                } else {
                    table_firsttd = $(this);
                    table_SpanNum = 1;
                }
            }
        });
    });
}
//Gird设置title:标题；url:地址；postData:传参；colNames:列名colModel:列绑定数据
function InitGrid(title, url, postData, colNames, colModel) {
    var config = {
        title: title,
        url: url,
        postData: {},
        colNames: colNames,
        colModel: colModel
    };
    JQ.InitTable(config);
    $("#btnSearch").click(function () {
        XPage.Search(postData());
        $('#searchmodfbox_jqGridList').hide();
        $('.ui-widget-overlay').hide()
    });
}
function AdOrderAdjust(AdOrderId, MediaType, ListInfo, gqid) {
    var data = { "AdOrderId": AdOrderId, "MediaType": MediaType, "ListInfo": ListInfo };
    $.post("/AdOrder/AdOrder/AdOrderAdjust", data, function (result) {
        if (result.IsSuccess) {
            layer.alert(result.Message, { icon: 1 }, function (idx) {
                layer.close(idx); //关闭消息
                $("#" + gqid).trigger("reloadGrid", []);
            });
        } else {
            layer.alert(result.Message, { icon: 0 });
        }
    }, "json");
}
function MakeAdjust(ProcessId, MediaType, ListInfo, gqid) {
    var data = { "ProcessId": ProcessId, "MediaType": MediaType, "ListInfo": ListInfo };
    $.post("/AdMake/tblAdMaking/MakeAdjust", data, function (result) {
        if (result.IsSuccess) {
            layer.alert("确认成功!"+result.Message, { icon: 1 }, function (idx) {
                layer.close(idx); //关闭消息
                $("#" + gqid).trigger("reloadGrid", []);
            });
        } else {
            layer.alert("确认失败!"+result.Message, { icon: 0 });
        }
    }, "json");
}
//广告数模块中   字段判断是否为整数
function OnStationNumChange() {
    var StationNum = $("#StationNum").val().Trim();
    if (StationNum != "" && StationNum != null) {
        if (!isInteger(StationNum)) {
            layer.alert("站台数量应为整数!");
            $("#StationNum").val("");
            return false;
        }
    }
}
//广告数模块中   字段判断是否为整数
function OnPassengerFluxChange() {
    var PassengerFlux = $("#PassengerFlux").val().Trim();
    if (PassengerFlux != "" && PassengerFlux != null) {
        if (!isInteger(PassengerFlux)) {
            layer.alert("客流量应为整数!");
            $("#PassengerFlux").val("");
            return false;
        }
    }
}
//行业派单模块中   字段判断必需是整数
function isInteger(obj) {
    return obj % 1 === 0
}