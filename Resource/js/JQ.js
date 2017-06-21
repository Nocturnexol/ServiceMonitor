/**
*
* by:tanh
* 
*/

var JQ = {
    InitTable: function (n) {
        var t = n.id ? $("#" + n.id) : $("#jqGridList");
        t.jqGrid({
            caption: n.title,
            url: n.url,
            mtype: "POST",
            styleUI: "Bootstrap",
            loadtext: "加载中...",
            postData: n.postData,
            datatype: "json",
            colNames: n.colNames,
            colModel: n.colModel,
            viewrecords: true,
            rownumbers: n.rownumbers ? !1 : !0,
            multiselect: n.multiselect == undefined ? !0 : n.multiselect,
            autowidth: n.autowidth == undefined ? !0 : n.autowidth,
            height: n.height ? n.height :'100%',
            rowNum: n.rowNum ? n.rowNum : 50,
            rowList: [50, 100],
            dataheight: 20,
            pager: n.pagerId ? "#" + n.pagerId : "#jqGridPager",
            subGrid: n.subGrid ? !0 : !1,
            condition: n.condition,
            gridComplete:n.gridComplete,
            resizeStop: function () {
               
                if (n.autowidth == undefined || n.autowidth) {
                    var jqWidth = t.width();
                    var jqparWidth = t.parent().parent().width();
                    if (jqWidth == jqparWidth) {
                        t.parent().parent().css("overflow-x", "hidden");
                    } else {
                        t.parent().parent().css("overflow-x", "auto");
                    }
                }
            },
            subGridRowExpanded: n.subGridRowExpanded ? n.subGridRowExpanded : null
        }).jqGrid('setFrozenColumns');
        if (n.autowidth == undefined || n.autowidth) {
            var jqWidth = t.width();
            var jqparWidth = t.parent().width();
            if (jqWidth == jqparWidth) {
                t.parent().parent().css("overflow-x", "hidden");
            }
        }
       
        t.setGridWidth(t.closest(".jqGrid_wrapper").width());

        $(window).bind("resize", function () {
            var n = t.closest(".jqGrid_wrapper").width();
            t.setGridWidth(n);
            var jqWidth = t.width();
            var jqparWidth = t.parent().parent().width();
            if (jqWidth == jqparWidth) {
                t.parent().parent().css("overflow-x", "hidden");
            }
        });
        t.navGrid(n.pagerId ? "#" + n.pagerId : "#jqGridPager", { edit: false, add: false, del: false, search: false });
    },

    GetDataTableDeleteData: function (id) {
        return JQ.GetAllSelected(id)
    },
    GetAllSelected: function (n) {
        var t = {
            Len: 0,
            Data: []
        },
			u = n ? $("#" + n) : $("#jqGridList"),
			f = u.getGridParam("selrow"),
			r, e, i;
        if (f) for (r = u.getGridParam("selarrrow"), e = "", i = 0; i < r.length; i++) t.Data.push(r[i]);
        return t.Len = t.Data.length, t;
    },
    InitGrid: function (n) {
        var col = n.col;

        var colNames = [];
        var colModel = [];
        for (var i = 0; i < col.length; i++) {
            var item = col[i];
            colNames.push(item.name);
            var Model = {};
            Model.name = item.index;
            Model.index = item.index;
            if (item.sortable != undefined) {
                Model.sortable = item.sortable;
            }
            if (item.fixed != undefined) {
                Model.fixed = item.fixed;
            }
            if (item.frozen != undefined) {
                Model.frozen = item.frozen;
            }
            if (item.key != undefined && item.key) {
                Model.key = true;
            }
            if (item.formatter != undefined) {
                if (item.formatter == "date") {
                    Model.formatter = "date";
                    Model.formatoptions = { srcformat: 'Y-m-d', newformat: 'Y-m-d' };
                } else if (item.formatter == "bool") {
                    Model.formatter = function (data) { if (data) return "<input type='checkbox' checked=true disabled/>"; else return "<input type='checkbox' disabled/>"; };
                } else {
                    Model.formatter = item.formatter;
                }
            }
            if (item.width != undefined) {
                Model.width = item.width;
            } else {
                Model.width = 60;
            }
            if (item.align != undefined) {
                Model.align = item.align;
            } else {
                Model.align = "center";
            }
            if (item.hidden != undefined && item.hidden) {
                Model.hidden = true;
            }
            colModel.push(Model);
        }

        var config = {
            id: n.id,
            pagerId: n.pagerId,
            title: n.title,
            url: n.url,
            postData: n.postData,
            colNames: colNames,
            colModel: colModel,
            condition: n.condition,
            multiselect: n.multiselect
        };
        JQ.InitTable(config);
    }
},
	XPage = {
	    DelData: function (n, id) {
	        id = id ? id : "jqGridList";
	        var t = JQ.GetDataTableDeleteData(id),
				i;
	        localStorage.setItem($("#WindowId").val(), id);
	        t.Len > 0 && t.Data.length > 0 ? layer.confirm("确认要删除这" + t.Len + "条数据？", { icon: 3, title: '提示' }, function () {
	            var paramData = "";
	            for (var ii = 0; ii < t.Data.length; ii++) {
	                paramData += "*" + t.Data[ii];
	            }
	            $.ajax({
	                url: n,
	                type: "POST",
	                data: { paramData: paramData },
	                success: function (n) {
	                    n.IsSuccess ? (layer.alert("删除成功", { icon: 1 }), $("#" + id).trigger("reloadGrid")) : layer.alert("删除失败：" + n.Message, { icon: 2 });
	                }
	            }
                );
	        }
            ) : layer.alert("请选择要删除的数据！", { icon: 0 });
	    },
	    DelSingleData: function (n, id) {
	        id = id ? id : "jqGridList";
	        localStorage.setItem($("#WindowId").val(), id);
	        layer.confirm("确认要删除么？", { icon: 3, title: '提示' },
            function (index) {
                $.ajax({
                    url: n.url,
                    type: "POST",

                    data: { "paramData": n.Data },

                    success: function (n) {
                        n.IsSuccess ? (layer.alert("删除成功"), $("#" + id).trigger("reloadGrid")) : layer.alert("删除失败：" + n.Message);
                    }
                });
            });
	    },
	    Refresh: function () {
	        var id = localStorage.getItem($("#WindowId").val());
	        if (id == null || id == undefined) {
	            XPage.Search();
	        }
	        else {
	            $("#" + id).trigger("reloadGrid", [{
	                page: 1
	            }]);

	        }
	    },
        ReLoad: function () {
            if ($("#jqGridList")[0]!=undefined) {
                $("#jqGridList").trigger("reloadGrid", []);
	        }
            else {
                XPage.Search(); 
	        }
	    },
	    Search: function (n) {
	        $("table[tabindex='0']").each(function () {
	            var t = $(this).jqGrid("getGridParam", "postData");
	            $.extend(t, n), $(this).setGridParam({
	                search: !0
	            }).trigger("reloadGrid", [{
	                page: 1
	            }])
	        });
	    }
	}
