$(function () {
    // $("input[type='checkbox']").bootstrapSwitch();
});
function selectAll(id) {
    $("#" + id + " input:checkbox").each(function () {
        $(this).prop('checked', true);//  
    });
};
function unSelect(id) {
    $("#" + id + " input:checkbox").removeAttr("checked");
};
function reverse(id) {
    $("#" + id + " input:checkbox").each(function () {
        this.checked = !this.checked;
    });
};
function OpStatsicLine(statsicName, op) {
    if (op == 0) {
        if (!confirm("确实要删除吗")) {
            return;
        }
    }
    else {
        statsicName = $("#tb_statsic").val();
        if (statsicName=='') {
            alert("行名不能为空");
            return;
        }
    }
    var datas = new Object();
    datas.statsicName = statsicName;
    datas.op = op;

    datas = window.JSON.stringify(datas);

    $.ajax({
        async: false,
        type: "Post",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        url: '/statsic/op',
        data: datas,
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
function SaveData(statsicName, index) {
    // index=1
    var list = new Array();
    //var cbList = $("#collapse_" + (index - 1)).find("input[checked='checked']");
    var cbList = $("#collapse_" + (index) + " input:checked");
    // alert(cbList.length);


    $.each(cbList, function (i, n) {
        list[i] = $(this).val();
    });

    var datas = new Object();
    datas.statsicName = statsicName;
    datas.infos = list;

    datas = window.JSON.stringify(datas);

    $.ajax({
        async: false,
        type: "Post",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        url: '/statsic/save',
        data: datas,
        success: function (data) {

            var responseData = data;
            // alert(responseData.length);
            if (data.IsSuccess = "true") {
                alert("保存成功");
            }
        },
        error: function (data) {
            alert("errorText:" + data.message);
        }
    });
}