$(function () {
    //var links = $("#list_group").find("a");
    //$.each(links, function (i, n) {
    //    $("")
    //});
});
function GetDataShowTable(data) {
    //alert(data);
    $("#h_curentGroup").val(decodeURI(data));
    $.ajax({
        async: false,
        type: "Get",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        url: '/user/s?groupName=' + data,
        data: '',
        success: function (data) {

            var responseData = data;
            // alert(responseData.length);
            ShowTable("tb_Users", responseData);
        },
        error: function (data) {
            alert("errorText:" + data.message);
        }
    });
}
function ShowTable(cid, obj) {
    if (obj.length == 0) {
        $("#" + cid).html("<p>no users !</p>");
        return;
    }
    var tab = "<table class='table table-bordered'  id='UserList'>";
    tab += "<tr><th>Num</th><th>姓名</th><th>Date</th><th>...</th></tr>";
    $.each(obj, function (i, n) {
        tab += "<tr><td>" + n.Id + "</td><td>" + n.Name + "</td><td>" + n.InTime + "</td><td>";
        tab += "<a href='javascript:UserDig(&#39;" + n.Id + "&#39;)'><span class='glyphicon glyphicon-edit'></span> </a>";
        tab += "<a href='javascript:DelUser(&#39;" + n.Id + "&#39;)'><span class='glyphicon glyphicon-trash'></span> </a>";
        tab += "</td></tr>";
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
            success: function (data) {

                var responseData = data;
                //alert(data.Name);
                $("#tb_Num").val(data.Id);
                $("#tb_Name").val(data.Name);
                $("#tb_Time").val(data.InTime);
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
    var group = $("#s_groupList").val();
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