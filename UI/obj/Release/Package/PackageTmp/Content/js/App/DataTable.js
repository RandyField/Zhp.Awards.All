$(function () {
    loadData(param);
    //$.ajax({
    //    type: 'post',//可选get  
    //    url: url,
    //    data: JSON.stringify(param),
    //    //data:{"channelType":$('#channelType').val(),"channel":$('#channel').val(),"day":$('#day').val(),"startTime":$('#startTime').val(),"endTime":$('#endTime').val(),"database":$('#database').val()},  
    //    contentType: "application/json", //必须有
    //    dataType: 'json',//服务器返回的数据类型 可选XML ,Json jsonp script htmltext等  
    //    success: function (result) {
    //        templateRender(result);
    //    },
    //    error: function () {
    //        alert('数据表加载出错！');
    //    }
    //}).done(function () {
    //    debugger;
    //    CheckAll();
    //    bindBtn();
    //});   
});

//加载数据
function loadData(param) {
    debugger;
    //if ($("#showRecords").val()=="") {
    //    param["pageSize"] = "10";//默认分页10 ，可选项25 50 100
    //}
    //else {
    //    param["pageSize"] = $("#showRecords").val();
    //}

    //$("#showRecords").val() == "" ? param["pageSize"] = "10" : param["pageSize"] = $("#showRecords").val();
    //$("#cityList").val() == "" ? param["cityName"] = "" : param["cityName"] = $("#cityList").val();
    //$("#lineList").val() == "" ? param["lineName"] = "" : param["lineName"] = $("#lineList").val();
    //$("#stateList").val() == "" ? param["stationName"] = "" : param["stationName"] = $("#stateList").val();
    //$("#postionList").val() == "" ? param["position"] = "" : param["position"] = $("#postionList").val();

    $.ajax({
        type: 'post',//可选get  
        url: url,
        data: JSON.stringify(param),
        //data:{"channelType":$('#channelType').val(),"channel":$('#channel').val(),"day":$('#day').val(),"startTime":$('#startTime').val(),"endTime":$('#endTime').val(),"database":$('#database').val()},  
        contentType: "application/json", //必须有
        dataType: 'json',//服务器返回的数据类型 可选XML ,Json jsonp script htmltext等  
        success: function (result) {
            templateRender(result);
        },
        error: function (a, b, c) {
            //alert(a);
            //alert(b);
            //alert(c);
            //alert('数据表加载出错！');
            $("#result").html("").html("无数据");
        }
    }).done(function () {
        debugger;
        CheckAll();
        bindBtn();
        if ($("[data-toggle='popover']").size() > 0) {
            $("[data-toggle='popover']").popover();

            //$('body').on('click', function (event) {
            //    var target = $(event.target);
            //    if (!target.hasClass('popover') //弹窗内部点击不关闭
            //            && target.parent('.popover-content').length === 0
            //            && target.parent('.popover-title').length === 0
            //            && target.parent('.popover').length === 0
            //            && target.data("toggle") !== "popover") {
            //        //弹窗触发列不关闭，否则显示后隐藏
            //        $('[data-toggle="popover"]').popover('hide');
            //    }
            //});


            //$("[data-toggle='popover']").on('shown.bs.popover', function () {
            //    debugger;
            //    //$('[data-toggle="popover"]').popover('hide');
            //    //setTimeout("popoverHide()", 3000);
            //})
        }

    });
}

function popoverHide() {
    debugger;
    $('[data-toggle="popover"]').popover('toggle');
}

//渲染行 修改分页控件的值
function templateRender(result) {
    debugger;
    var resultJson = $.parseJSON(result);
    var pageSize = resultJson.pageSize;
    var recordTotal = resultJson.recordTotal;
    var pageCount = resultJson.pageCount;
    var pageIndex = resultJson.pageIndex;
    var startRecord = 0;
    var endRecord = 0;
    //var pages = 5.0;
    var pages = 15.0;
    var html = template('rowData', resultJson);
    document.getElementById('result').innerHTML = html;

    //for (var i = 0; i < resultJson.data.length; i++) {
    //    debugger;
    //    var rowHtml;
    //    if ((i + 1) % 2 == 1) {
    //        rowHtml = "<tr class='odd'>";
    //    }
    //    if ((i + 1) % 2 == 0) {
    //        rowHtml = "<tr class='even'>";
    //    }
    //    rowHtml += cbboxHtml;
    //    for (key in resultJson.data[i]) {
    //        rowHtml += "<td class=' '>" + resultJson.data[i][key] + "</td>";
    //    }
    //    rowHtml += btnHtml_lg;
    //    rowHtml += btnHtml_sm;
    //    rowHtml += "</tr>";
    //    //$("#result").append(btnHtml_lg);
    //    //$("#result").append(btnHtml_sm);
    //    $("#result").append(rowHtml)
    //}
    //$("#showRecords").val(pageSize);

    //页面大小pagesize
    $("#showRecords").find("option").each(function () {
        if ($(this).val() == pageSize) {
            $(this).attr('selected', 'selected');
        }
    });

    debugger;
    //分页显示起止
    if (pageIndex * pageSize > recordTotal) {
        startRecord = (pageIndex - 1) * pageSize + 1;
        endRecord = recordTotal;
    }
    else if (pageIndex * pageSize <= recordTotal) {
        startRecord = (pageIndex - 1) * pageSize + 1;
        endRecord = pageSize * pageIndex;
    }

    //分页显示起止
    $("#sample-table-2_info").html("").html("显示 " + startRecord + " - " + endRecord + " 共计 " + recordTotal + " 条记录");


    showPager(pageIndex, pageCount, pages);
    ////页码
    //for (var i = 0; i < pageCount; i++) {
    //    if (i + 1 == pageIndex) {
    //        $(".pagination").append("<li class='active'><a href='#'>" + (i + 1) + "</a></li>")
    //    }
    //    else {
    //        $(".pagination").append("<li><a href='#'>" + (i + 1) + "</a></li>")
    //    }
    //    if (i == 9) {
    //        break;
    //    }
    //}


}


function showPager(pageIndex, pageCount, pages) {
    debugger;
    var critical = Math.ceil(pages / 2);
    //分页显示上一页
    $(".pagination").html("");
    $(".pagination").append("<li class='prev disabled'><a href='#' data-page=''><i class='icon-double-angle-left'></i></a></li>");

    //小于等于分页设置的总页数
    debugger;
    if (pageCount <= pages) {
        for (var i = 0; i < pageCount; i++) {
            if (i + 1 == pageIndex) {
                $(".pagination").append("<li class='active'><a href='#' data-page='" + (i + 1) + "'>" + (i + 1) + "</a></li>")
            }
            else {
                $(".pagination").append("<li><a href='#' data-page='" + (i + 1) + "'>" + (i + 1) + "</a></li>")
            }
        }
    }

    //超过分页设置的总页数
    if (pageCount > pages) {

        //索引小于分页设置的总页数一半+1
        if (pageIndex <= critical) {
            for (var i = 0; i <= pageCount; i++) {
                if (i + 1 == pageIndex) {
                    $(".pagination").append("<li class='active'><a href='#' data-page='" + (i + 1) + "'>" + (i + 1) + "</a></li>")
                }
                else {
                    $(".pagination").append("<li><a href='#' data-page='" + (i + 1) + "'>" + (i + 1) + "</a></li>")
                }
                if (i+1 == pages) {
                    break;
                }
            }
        }

            //索引大于索引小于分页设置的总页数一半+1且 索引+2>=索引小于分页设置的总页数
        else if (pageIndex > critical && pageCount <= (pageIndex + critical - 1)) {
            for (var i = pageIndex - (critical - 1) ; i <= pageCount; i++) {
                if (i == pageIndex) {
                    $(".pagination").append("<li class='active' ><a href='#' data-page='" + i + "'>" + i + "</a></li>")
                }
                else {
                    $(".pagination").append("<li><a href='#' data-page='" + i + "'>" + i + "</a></li>")
                }
            }
        }
            //索引大于索引小于分页设置的总页数一半+1且 索引+2<索引小于分页设置的总页数
        else if (pageIndex > critical && pageCount > (pageIndex + critical - 1)) {
            for (var i = pageIndex - (critical - 1) ; i <= pageIndex + critical - 1; i++) {
                if (i == pageIndex) {
                    $(".pagination").append("<li class='active'><a href='#' data-page='" + i + "'>" + i + "</a></li>")
                }
                else {
                    $(".pagination").append("<li><a href='#' data-page='" + i + "'>" + i + "</a></li>")
                }
            }
        }
    }

    //分页显示下一页
    $(".pagination").append("<li class='next disabled'><a href='#' data-page=''><i class='icon-double-angle-right'></i></a></li>");

    //控制上一页样式
    debugger;
    if (pageIndex == 1) {
        $('.pagination li:first').addClass('disabled');
    }
    else {
        $('.pagination li:first').removeClass('disabled');
    }

    //控制下一页样式
    debugger;
    if (pageIndex == pageCount) {
        $('.pagination li:last').addClass('disabled');
    }
    else {
        $('.pagination li:last').removeClass('disabled');
    }

    $(".pagination").find("a").each(function () {
        $(this).click(function (event) {
            //屏蔽默认事件
            event.preventDefault();

            //点击的非上页或者下页的禁用按钮
            if ($(this).parent().attr("class") != "prev disabled"
                && $(this).parent().attr("class") != "next disabled") {

                //上一页
                if ($(this).parent().attr("class") == "prev") {
                    //当前页码
                    var cruuentPage = $(this).parent().siblings(".active");

                    //移除当前页码激活样式
                    $(".pagination").find(".active").removeClass("active");

                    //当前页码的下一个新增激活样式
                    cruuentPage.prev().addClass("active");

                    //赋值参数pageindex
                    param["pageIndex"] = cruuentPage.prev().find("a").attr("data-page");
                }
                if ($(this).parent().attr("class") == "next") {
                    //下一页
                    var cruuentPage = $(this).parent().siblings(".active");

                    //当前页码
                    $(".pagination").find(".active").removeClass("active");

                    //移除当前页码激活样式
                    cruuentPage.next().addClass("active");

                    //赋值参数pageindex
                    param["pageIndex"] = cruuentPage.next().find("a").attr("data-page");
                }

                //点击的页码
                if ($(this).attr("data-page") != "") {

                    //移除所有激活样式
                    $(".pagination").find(".active").removeClass("active");

                    //a标签的父亲 li标签新增激活样式
                    $(this).parent().addClass("active");

                    //赋值参数pageindex
                    param["pageIndex"] = $(this).attr("data-page");
                }
                //加载数据
                loadData(param);
            }
        });
    });
}

//实现表格全选
function CheckAll() {
    $('table th input:checkbox').on('click', function () {
        debugger;
        var that = this;
        $("#result").find('input:checkbox').each(function () {
            this.checked = that.checked;
            $(this).closest('tr').toggleClass('selected');
        });
    });
}

//绑定所有操作按钮的点击事件
function bindBtn() {
    debugger;
    $('#delete-Tips').on('shown.bs.modal', function (e) {
        debugger;
        var btn = $(e.relatedTarget)
        id = btn.data("id");
        debugger;
        $("#btn-delete-confirm").click(function () {
            delet(id);
        });
    });

    $(".btn-deal a,.btn-deal span").each(function () {
        if ($(this).attr("type") == "view") {
            $(this).click(function (event) {
                event.preventDefault();
                view();
            });
        }
        if ($(this).attr("type") == "edit") {
            $(this).click(function (event) {
                event.preventDefault();
                showEdit();
            });
        }
    });
}


//删除
function delet(id) {
    debugger;
    var param = { "Id": id };
    $.ajax({
        type: 'post',//可选get  
        url: deleteurl,
        data: JSON.stringify(param),//默认pageSize为10
        contentType: "application/json", //必须有
        //data:{"channelType":$('#channelType').val(),"channel":$('#channel').val(),"day":$('#day').val(),"startTime":$('#startTime').val(),"endTime":$('#endTime').val(),"database":$('#database').val()},  
        dataType: 'json',//服务器返回的数据类型 可选XML ,Json jsonp script htmltext等  
        success: function (result) {
            if (result.success) {
                //删除modal隐藏
                $('#delete-Tips').modal('hide');

                //删除成功modal显示
                $('#delete-success-Tips').modal('show');

                //重新加载
                reLoadData();

            }
            if (result.failure) {
                //删除modal隐藏
                $('#delete-Tips').modal('hide');

                //删除成功modal显示
                $('#deletefailure-Tips').modal('show');

                //加载失败原因 待补充
                //reLoadData();
            }
        },
        error: function () {
            alert('数据表加载出错！');
        }
    });
}

//重新加载数据
function reLoadData() {
    $('#delete-success-Tips').on('hidden.bs.modal', function (e) {
        loadData();
        alert("删除成功，重新加载数据");
    })
}

//编辑
function showEdit() {
    $(".main-content").html("");
    $(".main-content").load(addurl);
}

//查看
function view() {
    $(".main-content").html("");
    $(".main-content").load(addurl);
}

//新增
function add() {
    debugger;
    $(".main-content").html("");
    $(".main-content").load(addurl);
}

