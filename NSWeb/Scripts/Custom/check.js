$(function () {
    initFileInput("file_up_2", "/user/upload");
    alert("CC");
    $("#file_up_2").on("fileuploaded", function (event, data, previewId, index) {
        if (data == undefined) {
            alert('文件格式类型不正确');
            return;
        }
        else if (data.IsSuccess == false) {
            alert('上传失败');
        }
        else {
            showFileList("check_m_panel", data.Message);
            alert('校正成功');
        }
    });

});
//初始化fileinput控件（第一次初始化）
function initFileInput(ctrlName, uploadUrl) {
    var control = $('#' + ctrlName);

    control.fileinput({
        language: 'zh', //设置语言
        uploadUrl: uploadUrl, //上传的地址
        allowedFileExtensions: ['xlsx'],//接收的文件后缀
        showUpload: true, //是否显示上传按钮
        showCaption: true,//是否显示标题
        browseClass: "btn btn-primary", //按钮样式             
        showPreview:false
        // previewFileIcon: "<i class='glyphicon glyphicon-king'></i>",
    });
}

function showFileList(cid, tab) {
    $("#" + cid).html(tab);
}
