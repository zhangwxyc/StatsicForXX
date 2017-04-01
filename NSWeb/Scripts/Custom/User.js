$(function () {
    //var links = $("#list_group").find("a");
    //$.each(links, function (i, n) {
    //    $("")
    //});
    $("#tb_IsSheild").bootstrapSwitch();
    $("#tb_IsTrimFromGroup").bootstrapSwitch();

});
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

function GetDataShowTable(data) {
    //alert(data);
    $("#h_curentGroup").val(decodeURI(data));
    $("#h_curentGroup_Title").html(decodeURI(data));
    //if ($("#btn_AddUser").hasClass("hide_Item")) {
    $("#btn_AddUser").removeClass("hide_Item");
    $("#btn_RemoveGroup").removeClass("hide_Item");
    // }
    $.ajax({
        async: false,
        type: "Get",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        url: '/user/s?groupName=' + data,
        data: '',
        cache: false,
        success: function (data) {

            var responseData = data;
            // alert(responseData.length);
            $("#h_uCount").html("(" + responseData.length + ")");
            ShowTable("tb_Users", responseData);
        },
        error: function (data) {
            alert("errorText:" + data.message);
            $("#h_uCount").html("");
        }
    });
}
function ShowTable(cid, obj) {
    if (obj.length == 0) {
        $("#" + cid).html("<p>no users !</p>");
        return;
    }
    var tab = "<table class='table table-bordered'  id='UserList'>";
    tab += "<tr><th>Num</th><th>姓名</th><th>Date</th><th>是否参与核算</th><th>参与组统计</th><th>...</th></tr>";
    var index = 0;
    $.each(obj, function (i, n) {
        tab += "<tr title='" + n.Remark + "'><td>" + n.Id + "</td><td>" + n.Name + "</td><td>" + n.InTime + "</td><td>" + (n.IsShield == 0 ? '是' : '否') + "</td><td>" + (n.IsTrimFromGroup == 0 ? '是' : '否') + "</td><td class='user_td_op'>";

        tab += "<a href='javascript:UserDig(&#39;" + n.Id + "&#39;)'><span class='glyphicon glyphicon-edit'></span> </a>";
        tab += "<a href='javascript:DelUser(&#39;" + n.Id + "&#39;)'><span class='glyphicon glyphicon-trash'></span> </a>";
        if (index > 0) {
            tab += "<a href='javascript:ChangeP(&#39;" + n.Id + "&#39;,0)'><span class='glyphicon glyphicon-circle-arrow-up'></span> </a>";
        }
        if (index < obj.length - 1) {
            tab += "<a href='javascript:ChangeP(&#39;" + n.Id + "&#39;,1)'><span class='glyphicon glyphicon-circle-arrow-down'></span> </a>";
        }
        tab += "</td></tr>";
        index++;
    });
    tab += "</table>";
    // alert(tab);
    $("#" + cid).html(tab);
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

                $('#tb_IsTrimFromGroup').bootstrapSwitch('state', data.IsTrimFromGroup == 0);

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
    datas.IsTrimFromGroup = $("#tb_IsTrimFromGroup").bootstrapSwitch('state') == true ? 0 : 1;
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