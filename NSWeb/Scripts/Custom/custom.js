$(function () {
    //var links = $("#list_group").find("a");
    //$.each(links, function (i, n) {
    //    $("")
    //});

    $("#tb_IsSheild").bootstrapSwitch({
        onText: "自动",
        offText: "手动",
        onColor: "success",
        offColor: "info",
        size: "small",
        onSwitchChange: function (event, state) {
            if (state == true) {
                setAuto();
            } else {
                DelAuto();
            }
        }
    });
    //$('#tb_IsSheild').on('switch-change', function (e, data) {
    //    //var $el = $(data.el)
    //    //  , value = data.value;
    //    alert(data);
    //    //console.log(e, $el, value);
    //});



    $("#s_fileList").change(function () {
        var id = $(this).val();
        if (id == 0) {
            return;
        }
        GetDataShowTable(id);

    });
});













var cols = new Array("技能组", "录音抽检数", "平均得分", "通过量", "中度服务瑕疵量", "重大服务失误量", "有效投诉量", "通过率", "通过率系数", "净满意度", "客户评价率");

function filterGroups() {
    var filterData = $("#tb_filter").val();
    var filterData = trimStr(filterData).toLowerCase();
    $("#list_group a").each(function () {
        if (filterData == '') {
            $(this).removeClass("hide_Item");
        }
        else {
            if ($(this).html().toLowerCase().indexOf(filterData) == -1) {
                $(this).addClass("hide_Item");
            }
        }
    });
}
function trimStr(str) {
    return str.replace(/(^\s*)|(\s*$)/g, "");
}

function GetDataShowTable(id) {

    $.ajax({
        async: false,
        type: "Post",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        url: '/custom/f?id=' + id,
        data: '',
        cache: false,
        success: function (data) {

            var responseData = data;
            // alert(responseData.length);
            ShowTable("d_bdt", responseData);
        },
        error: function (data) {
            alert("errorText:" + data.responseText);
        }
    });
}
function GetReadTB(data) {
    return "<td>" + data + "</td>";
}
function GetWriteTB(gName, colName, data) {
    return "<td><input type='number' class='gUserList_input' gdata='" + gName + "' colName='" + colName + "'  value='" + data + "'/></td>";
}
var canEditArray = new Array(3, 4, 5, 6);
function ShowTable(cid, obj) {
    if (obj.length == 0) {
        $("#" + cid).html("<p>no users !</p>");
        return;
    }
    var tab = "<table class='table table-bordered'  id='gUserList'>";
    tab += "<tr>";
    for (var colIndex = 0; colIndex < cols.length; colIndex++) {
        //tab += "<th>" + cols[colIndex] + "</th>";
    }
    tab += "</tr>";
    var index = 0;
    $.each(obj, function (i, n) {
        tab += "<tr title='.'>";

        for (var colIndex = 0; colIndex < cols.length; colIndex++) {
            if (canEditArray.indexOf(colIndex) != -1) {
                tab += GetWriteTB(n.技能组, cols[colIndex], n[cols[colIndex]]);
            }
            else {
                tab += GetReadTB(n[cols[colIndex]]);
            }
        }

        //tab += GetReadTB(n.技能组);
        //tab += GetWriteTB(n.技能组, 2, n.录音抽检数);
        //tab += GetReadTB(n.平均得分);
        //tab += GetWriteTB(n.技能组, 2, n.通过量);
        //tab += GetWriteTB(n.技能组, 2, n.中度服务瑕疵量);
        //tab += GetWriteTB(n.技能组, 2, n.重大服务失误量);
        //tab += GetWriteTB(n.技能组, 2, n.有效投诉量);
        //tab += GetReadTB(n.通过率);
        //tab += GetReadTB(n.通过率系数);
        //tab += GetReadTB(n.客户评价率);
        tab += "</tr>";
        index++;
    });
    tab += "</table>";
    // alert(tab);平均得分	通过量	中度服务瑕疵量	重大服务失误量	有效投诉量	通过率	通过率系数	净满意度	客户评价率

    $("#" + cid).html(tab);

    if ($("#tb_IsSheild").bootstrapSwitch('state') == true) {
        setAuto();
    }
}

function Refresh() {
    var id = $("#s_fileList").val();
    if (id == 0) {
        return;
    }
    var info = new Object();
    var datas = "";
    $("#gUserList input[type='number']").each(function (i, n) {
        datas += $(this).attr("gdata") + "|" + $(this).attr("colName") + "|" + $(this).val() + "\n";
    });
    info.Datas = datas;
    info.Id = id;

    $.ajax({
        async: false,
        type: "Post",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        url: '/custom/Refresh',
        data: window.JSON.stringify(info),
        cache: false,
        success: function (data) {

            var id = $("#s_fileList").val();
            if (id != 0) {
                GetDataShowTable(id);
            }
        },
        error: function (data) {
            alert("errorText:" + data.responseText);
        }
    });
}
function ReSet() {
    var id = $("#s_fileList").val();
    if (id == 0) {
        return;
    }
    $.ajax({
        async: false,
        type: "Post",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        url: '/custom/f?isReset=true&&id=' + id,
        data: '',
        cache: false,
        success: function (data) {

            var responseData = data;
            // alert(responseData.length);
            ShowTable("d_bdt", responseData);
        },
        error: function (data) {
            alert("errorText:" + data.responseText);
        }
    });
}

function setAuto() {
    $("#gUserList input[type='number']").each(function (i, n) {
        $(this).attr("onpropertychange", " Refresh()");
        $(this).attr("oninput", " Refresh()");
    });
}
function DelAuto() {
    $("#gUserList input[type='number']").each(function (i, n) {
        $(this).removeAttr("onpropertychange");
        $(this).removeAttr("oninput");
    });
}

function ChangeGroup(obj) {
    $("#h_curUser").val(obj);
    $("#div_ChangeGroup").modal();
}
function UserDig(obj) {
    $("#h_curUser").val(obj);
    if (obj != "0") {
        $("#div_opTitle").html("修改用户");
        $.ajax({
            async: false,
            type: "Get",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            url: '/user/u?uid=' + obj,
            data: '',
            cache: false,
            success: function (data) {

                var responseData = data;
                //alert(data.Name);
                $("#tb_Num").val(data.Id);
                $("#tb_Name").val(data.Name);
                $("#tb_Time").val(data.InTime);

                $("#tb_Remark").val(data.Remark);

                // alert(data.IsShield);
                $('#tb_IsSheild').bootstrapSwitch('state', data.IsShield == 0);
                //$("#s_groupList").val(data.GroupName);
                //var lastselected= $("#s_groupList").find("attr='selected'");
                //$.each(obj, function (i, n) {
                //    $(this).removeAttr("selected");
                //});
                $("#s_groupList option[selected='selected']").removeAttr("selected");
                $("#s_groupList option[value='" + data.GroupName + "']").attr("selected", true);
                $("#div_OpUser").modal();
            },
            error: function (data) {
                alert("errorText:" + data.message);
            }
        });
    }
    else {
        $("#div_opTitle").html("添加用户");
        $("#tb_Num").val("");
        $("#tb_Name").val("");
        $("#tb_Time").val("");
        $("#s_groupList").val("请选择");
        $("#div_OpUser").modal();
    }

}
function DelUser(obj) {
    $("#h_curUser").val(obj);
    var group = $("#h_curentGroup").val();
    if (confirm("确定删除吗")) {
        if (obj != "0") {
            $.ajax({
                async: false,
                type: "Post",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                url: '/user/del?uid=' + obj,
                data: '',
                success: function (data) {

                    var responseData = data;
                    // alert(responseData.length);
                    if (data.IsSuccess = "true") {
                        //alert("成功");
                        GetDataShowTable(encodeURI(group));
                    }
                },
                error: function (data) {
                    alert("errorText:" + data.message);
                }
            });
        }
        else
            alert("用户ID不正确");
    }
}
function SaveUser() {
    var obj = $("#h_curUser").val();
    var datas = new Object();
    datas.Id = $("#tb_Num").val();
    datas.Name = $("#tb_Name").val();
    datas.InTime = $("#tb_Time").val();
    datas.Remark = $("#tb_Remark").val();
    datas.IsShield = $("#tb_IsSheild").bootstrapSwitch('state') == true ? 0 : 1;
    var group = $("#s_groupList").val();
    if (datas.Id == '' || datas.Name == '' || datas.group == '') {
        alert("信息填写不完整");
        return;
    }
    datas.GroupName = group;

    datas = window.JSON.stringify(datas);
    if (obj != "0") {
        $.ajax({
            async: false,
            type: "Post",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            url: '/user/save',
            data: datas,
            cache: false,
            success: function (data) {

                var responseData = data;
                // alert(responseData.length);
                if (data.IsSuccess = "true") {
                    alert("保存成功");
                    GetDataShowTable(encodeURI(group));
                    $('#div_OpUser').modal('hide');
                }
            },
            error: function (data) {
                alert("errorText:" + data.message);
            }
        });
    }
    else
        alert("用户ID不正确");
}

function OpGroup(op) {
    var name = "";
    if (op == 0) {
        if (!confirm("确实要删除吗")) {
            return;
        }
        name = $("#h_curentGroup").val();
    }
    else {
        name = prompt("输入组名");
        if (name == '') {
            alert("组名不能为空");
            return;
        }
    }
    var datas = new Object();
    datas.Name = name;
    datas.op = op;

    datas = window.JSON.stringify(datas);

    $.ajax({
        async: false,
        type: "Post",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        url: '/user/op',
        data: datas,
        cache: false,
        success: function (data) {

            var responseData = data;
            // alert(responseData.length);
            if (data.IsSuccess = "true") {
                alert("操作成功");
                location.reload();
            }
        },
        error: function (data) {
            alert("errorText:" + data.message);
        }
    });
}

function ChangeP(name, op) {
    var datas = new Object();
    datas.id = name;
    datas.op = op;

    datas = window.JSON.stringify(datas);
    var group = $("#h_curentGroup").val();
    $.ajax({
        async: false,
        type: "Post",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        url: '/user/changePostion',
        data: datas,
        cache: false,
        success: function (data) {

            var responseData = data;
            // alert(responseData.length);
            if (data.IsSuccess = "true") {
                // alert("操作成功");
                GetDataShowTable(encodeURI(group));
            }
        },
        error: function (data) {
            alert("errorText:" + data.message);
        }
    });
}
