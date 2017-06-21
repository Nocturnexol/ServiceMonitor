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
            height: n.height ? n.height : "100%", // n.height ? n.height : $(window).height() - 160 + 'px', //
            rowNum: n.rowNum ? n.rowNum : 50,
            rowList: [50, 100],
            dataheight: 20,
            pager: n.pagerId ? "#" + n.pagerId : "#jqGridPager",
            subGrid: n.subGrid ? !0 : !1,
            condition: n.condition,
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
        });
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
            if (item.key != undefined && item.key) {
                Model.key = true;
            }
            if (item.formatter != undefined) {
                if (item.formatter == "date") {
                    Model.formatter = "date";
                    Model.formatoptions = { srcformat: 'Y-m-d', newformat: 'Y-m-d' };
                } else if (item.formatter == "datetime") {
                    Model.formatter = "date";
                    Model.formatoptions = { srcformat: 'Y-m-d H:m:s', newformat: 'Y-m-d H:m:s' };
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
            condition: n.condition
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
                        n.IsSuccess ? (layer.alert("删除成功", { icon: 1 }), $("#" + id).trigger("reloadGrid")) : layer.alert("删除失败：" + n.Message);
                    }
                });
            });
        },
        UserPwdSingleData: function (n, id) {
            id = id ? id : "jqGridList";
            localStorage.setItem($("#WindowId").val(), id);
            layer.confirm("重置后密码为123456", { icon: 3, title: '提示' },
            function (index) {
                $.ajax({
                    url: n.url,
                    type: "POST",

                    data: { "paramData": n.Data },

                    success: function (n) {
                        n.IsSuccess ? (layer.alert("重置成功"), $("#" + id).trigger("reloadGrid")) : layer.alert("重置失败：" + n.Message);
                    }
                });
            });
        },
	    Refresh: function () {
	        var id = localStorage.getItem($("#WindowId").val());
	        if (parent.GetShowData != undefined && parent.GetShowData != null && $.isFunction(parent.GetShowData)) {
	            parent.GetShowData(0);
	        }
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
	        if ($("#jqGridList")[0] != undefined) {
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
	function parseDate(a, b, c) {
	    var d, e, f, g, h = /\\.|[dDjlNSwzWFmMntLoYyaABgGhHisueIOPTZcrU]/g, i = /\b(?:[PMCEA][SDP]T|(?:Pacific|Mountain|Central|Eastern|Atlantic) (?:Standard|Daylight|Prevailing) Time|(?:GMT|UTC)(?:[-+]\d{4})?)\b/g, j = /[^-+\dA-Z]/g, k = new 
		RegExp("^/Date\\((([-+])?[0-9]+)(([-+])([0-9]{2})([0-9]{2}))?\\)/$"), l = "string" == typeof 
		b ? b.match(k) : null, m = function (a, b) {
		    for (a = String(a),
			b = parseInt(b, 10) || 2; a.length < b; )
		        a = "0" + a;
		    return a
		}
		, n = {
		    m: 1,
		    d: 1,
		    y: 1970,
		    h: 0,
		    i: 0,
		    s: 0,
		    u: 0
		}, o = 0, p = function (a, b) {
		    return 0 === a ? 12 === b && (b = 0) : 12 !== b && (b += 12),
			b
		}
		, q = 0;
	    if (void 
		0 === d && (d = { "dayNames": ["日", "一", "二", "三", "四", "五", "六", "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"], "monthNames": ["一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二", "一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"], "AmPm": ["am", "pm", "上午", "下午"], "srcformat": "Y-m-d", "newformat": "Y-m-d", "parseRe": {}, "masks": { "ISO8601Long": "Y-m-d H:i:s", "ISO8601Short": "Y-m-d", "ShortDate": "n/j/Y", "LongDate": "l, F d, Y", "FullDateTime": "l, F d, Y g:i:s A", "MonthDay": "F d", "ShortTime": "g:i A", "LongTime": "g:i:s A", "SortableDateTime": "Y-m-d\\TH:i:s", "UniversalSortableDateTime": "Y-m-d H:i:sO", "YearMonth": "F, Y" }, "reformatAfterEdit": false, "userLocalTime": false, S: function (j) { return j < 11 || j > 13 ? ['st', 'nd', 'rd', 'th'][Math.min((j - 1) % 10, 3)] : 'th'; } }),
		void 
		0 === d.parseRe && (d.parseRe = /[#%\\\/:_;.,\t\s-]/),
		d.masks.hasOwnProperty(a) && (a = d.masks[a]),
		b && null != b)
	        if (isNaN(b - 0) || "u" !== String(a).toLowerCase())
	            if (b.constructor === Date)
	                o = b;
	            else
	                if (null !== l)
	                    o = new 
					Date(parseInt(l[1], 10)),
					l[3] && (q = 60 * Number(l[5]) + Number(l[6]),
					q *= "-" === l[4] ? 1 : -1,
					q -= o.getTimezoneOffset(),
					o.setTime(Number(Number(o) + 60 * q * 1e3)));
	                else {
	                    for ("ISO8601Long" === d.srcformat && "Z" === b.charAt(b.length - 1) && (q -= (new 
					Date).getTimezoneOffset()),
					b = String(b).replace(/\T/g, "#").replace(/\t/, "%").split(d.parseRe),
					a = a.replace(/\T/g, "#").replace(/\t/, "%").split(d.parseRe),
					f = 0,
					g = a.length; g > f; f++) {
	                        switch (a[f]) {
	                            case "M":
	                                e = $.inArray(b[f], d.monthNames),
							-1 !== e && 12 > e && (b[f] = e + 1,
							n.m = b[f]);
	                                break;
	                            case "F":
	                                e = $.inArray(b[f], d.monthNames, 12),
							-1 !== e && e > 11 && (b[f] = e + 1 - 12,
							n.m = b[f]);
	                                break;
	                            case "n":
	                                a[f] = "m";
	                                break;
	                            case "j":
	                                a[f] = "d";
	                                break;
	                            case "a":
	                                e = $.inArray(b[f], d.AmPm),
							-1 !== e && 2 > e && b[f] === d.AmPm[e] && (b[f] = e,
							n.h = p(b[f], n.h));
	                                break;
	                            case "A":
	                                e = $.inArray(b[f], d.AmPm),
							-1 !== e && e > 1 && b[f] === d.AmPm[e] && (b[f] = e - 2,
							n.h = p(b[f], n.h));
	                                break;
	                            case "g":
	                                n.h = parseInt(b[f], 10)
	                        }
	                        void 
						0 !== b[f] && (n[a[f].toLowerCase()] = parseInt(b[f], 10))
	                    }
	                    if (n.f && (n.m = n.f),
					0 === n.m && 0 === n.y && 0 === n.d)
	                        return "&#160;";
	                    n.m = parseInt(n.m, 10) - 1;
	                    var 
					r = n.y;
	                    r >= 70 && 99 >= r ? n.y = 1900 + n.y : r >= 0 && 69 >= r && (n.y = 2e3 + n.y),
					o = new 
					Date(n.y, n.m, n.d, n.h, n.i, n.s, n.u),
					q > 0 && o.setTime(Number(Number(o) + 60 * q * 1e3))
	                }
	        else
	            o = new 
				Date(1e3 * parseFloat(b));
	    else
	        o = new 
			Date(n.y, n.m, n.d, n.h, n.i, n.s, n.u);
	    if (d.userLocalTime && 0 === q && (q -= (new 
		Date).getTimezoneOffset(),
		q > 0 && o.setTime(Number(Number(o) + 60 * q * 1e3))),
		void 
		0 === c)
	        return o;
	    d.masks.hasOwnProperty(c) ? c = d.masks[c] : c || (c = "Y-m-d");
	    var 
		s = o.getHours()
		  , t = o.getMinutes()
		  , u = o.getDate()
		  , v = o.getMonth() + 1
		  , w = o.getTimezoneOffset()
		  , x = o.getSeconds()
		  , y = o.getMilliseconds()
		  , z = o.getDay()
		  , A = o.getFullYear()
		  , B = (z + 6) % 7 + 1
		  , C = (new 
		Date(A, v - 1, u) - new 
		Date(A, 0, 1)) / 864e5
		  , D = {
		      d: m(u),
		      D: d.dayNames[z],
		      j: u,
		      l: d.dayNames[z + 7],
		      N: B,
		      S: d.S(u),
		      w: z,
		      z: C,
		      W: 5 > B ? Math.floor((C + B - 1) / 7) + 1 : Math.floor((C + B - 1) / 7) || ((new 
			Date(A - 1, 0, 1).getDay() + 6) % 7 < 4 ? 53 : 52),
		      F: d.monthNames[v - 1 + 12],
		      m: m(v),
		      M: d.monthNames[v - 1],
		      n: v,
		      t: "?",
		      L: "?",
		      o: "?",
		      Y: A,
		      y: String(A).substring(2),
		      a: 12 > s ? d.AmPm[0] : d.AmPm[1],
		      A: 12 > s ? d.AmPm[2] : d.AmPm[3],
		      B: "?",
		      g: s % 12 || 12,
		      G: s,
		      h: m(s % 12 || 12),
		      H: m(s),
		      i: m(t),
		      s: m(x),
		      u: y,
		      e: "?",
		      I: "?",
		      O: (w > 0 ? "-" : "+") + m(100 * Math.floor(Math.abs(w) / 60) + Math.abs(w) % 60, 4),
		      P: "?",
		      T: (String(o).match(i) || [""]).pop().replace(j, ""),
		      Z: "?",
		      c: "?",
		      r: "?",
		      U: Math.floor(o / 1e3)
		  };
	    return c.replace(h, function (a) {
	        return D.hasOwnProperty(a) ? D[a] : a.substring(1)
	    })
	}