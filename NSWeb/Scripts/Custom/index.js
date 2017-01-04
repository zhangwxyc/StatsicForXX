$(function () {
    initFileInput("file_up_1", "/home/upload");
    $("#file_up_1").on("fileuploaded", function (event, data, previewId, index) {
        if (data == undefined) {
            alert('文件格式类型不正确');
            return;
        }
        else if (data.IsSuccess == false) {
            alert('上传失败');
        }
        else {
            alert('上传成功');
        }
        LoadFileList();
    });
    LoadFileList();
});
//初始化fileinput控件（第一次初始化）
function initFileInput(ctrlName, uploadUrl) {
    var control = $('#' + ctrlName);

    control.fileinput({
        language: 'zh', //设置语言
        uploadUrl: uploadUrl, //上传的地址
        allowedFileExtensions: ['xls', 'txt', 'gif'],//接收的文件后缀
        showUpload: true, //是否显示上传按钮
        showCaption: true,//是否显示标题
        browseClass: "btn btn-primary", //按钮样式             
        showPreview:false
        // previewFileIcon: "<i class='glyphicon glyphicon-king'></i>",
    });
}

function showFileList(cid, obj) {
    if (obj.length == 0) {
        $("#" + cid).html("<p>no files !</p>");
        return;
    }
    var tab = "<table class='table table-bordered'  id='f_list'>";
    tab += "<tr><th>Id</th><th>文件名</th><th>创建时间</th><th>...</th></tr>";
    $.each(obj, function (i, n) {
        tab += "<tr><td>" + n.Id + "</td><td>" + n.Name + "</td><td class='td_date'>" + n.CreateTime + "</td><td class='td_op'>";
        tab += "<a href='/home/down_src?id=" + n.Id + "' title='下载源数据'><span class='glyphicon glyphicon-cloud'></span> </a>";
        tab += "<a href='javascript:DelFile(&#39;" + n.Id + "&#39;)'  title='删除'><span class='glyphicon glyphicon-trash'></span> </a>";
        tab += "<a href='/home/down_Anaysle?id=" + n.Id + "' title='生成报表'><span class='glyphicon glyphicon-cloud-download'></span> </a>";
        tab += "<a href='/home/email?id=" + n.Id + "' title='发送'>e </a>";

        tab += "</td></tr>";
    });
    tab += "</table>";
    // alert(tab);
    $("#" + cid).html(tab);
}
function DelFile(obj) {
    if (confirm("确定删除吗")) {
        if (obj != "0") {
            $.ajax({
                async: false,
                type: "Post",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                url: '/home/d?id=' + obj,
                data: '',
                success: function (data) {

                    var responseData = data;
                    // alert(responseData.length);
                    if (data.IsSuccess = "true") {
                        //alert("成功");
                        LoadFileList();
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

function LoadFileList() {
    $.ajax({
        async: false,
        type: "Get",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        url: '/home/fileList',
        data: '',
        success: function (data) {

            var responseData = data;
            // alert(responseData.length);
            showFileList("m_panel", responseData);
        },
        error: function (data) {
            alert("errorText:" + data.message);
        }
    });
}