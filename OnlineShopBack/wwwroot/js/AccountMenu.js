
$(document).ready(function () {

    $("#AddAccount").click(function () {
        location.href = "/Account/AddAccount"
    });
    $("#AddAccountLevel").click(function () {
        location.href = "/Account/AddAccountLevel"
    });


    $.ajax({
        type: "GET",
        url: "/api/account/GetAcc",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            //$("#DIV").html('');
            //var DIV = '';
            for (var i in data) {
                var rows = rows + "<tr>" +
                    "<td name='fid' id='" + data[i].f_id+"'>" + data[i].f_id + "</td>" +
                    "<td name='facc' id='" + data[i].f_id+"'>" + data[i].f_acc + "</td>" +
                    "<td name='faccPosition' id='" + data[i].f_id+"'>" + data[i].f_accPosition + "</td>" +
                    "<td name='fcreateDate' id='" + data[i].f_id + "'>" + data[i].f_createDate + "</td>" +
                    "<td align='center'> <input type='button'   name='EditBtn'   id = '" + data[i].f_id + "' onclick = 'Edit_Click(this.id)' value='編輯'/ ></td>" +
                    "<td align='center'> <input type='button'   name='DeleteBtn' id = '" + data[i].f_id + "' onclick = 'Del_Click(this.id)' value='刪除'/ ></td>" +
                    "</tr>";
            }
            $('#Table').append(rows);
        },

        failure: function (data) {
        },
        error: function (data) {
        }

    });

})
//刪除
function Del_Click(DelId) {
    if (window.confirm("確定要刪除此帳號嗎?")) {
        $.ajax({
            url: "/api/Account/DelAcc?id=" + DelId,
            type: "DELETE",
            data: {},
            success: function (result) {
                alert(result)

                if (result == "帳號刪除成功") {
                    location.reload(); //刪除成功才更新頁面
                }
            },
            error: function (error) {
                alert(error);
            }
        })
    }

}
//編輯
function Edit_Click(EditId) {

    //if ($("#EditBox").css("display") == "none") {
    //    var EditData =
    //        "<div id='Editbutton'><input name='EditOK' id='" + Editid + "' onclick ='EditOK_Click(this.id)' type='Button' value='EditOK' />" +
    //        "<input name='EditCancel' id = 'EditCancel' type = 'Button' value = 'Cancel' onclick = 'EditCancel_Click()' /></div > "
    //    $('#Editform').append(EditData);
    //    $("#EditBox").show();
    //}
}
//取消編輯
function EditCancel_Click(EditId) {
    if ($("#EditBox").css("display") !== "none") {
        $("#EditBox").hide();
        $("#Editbutton").remove();
    }
}
