
$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/api/account/GetAcc",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            for (var i in data) {
                if (data[i].f_id == 0) {//預設帳號不可刪除
                    var rows = rows + "<tr>" +
                        "<td name='fid' id='" + data[i].f_id + "'>" + data[i].f_id + "</td>" +
                        "<td name='facc' id='" + data[i].f_id + "'>" + data[i].f_acc + "</td>" +
                        "<td name='faccPosition' id='" + data[i].f_id + "'>" + data[i].f_accPosition + "</td>" +
                        "<td name='fcreateDate' id='" + data[i].f_id + "'>" + data[i].f_createDate + "</td>" +
                        "<td align='center'></td>" +
                        "<td align='center'></td>" +
                        "</tr>";
                }
                else {
                    var rows = rows + "<tr>" +
                        "<td name='fid' id='" + data[i].f_id + "'>" + data[i].f_id + "</td>" +
                        "<td name='facc' id='" + data[i].f_id + "'>" + data[i].f_acc + "</td>" +
                        "<td name='faccPosition' id='" + data[i].f_id + "'>" + data[i].f_accPosition + "</td>" +
                        "<td name='fcreateDate' id='" + data[i].f_id + "'>" + data[i].f_createDate + "</td>" +
                        "<td align='center'> <input type='button' class='EditBtn'  name='EditBtn'   id = '" + data[i].f_id + "'  value='編輯'/ ></td>" +
                        "<td align='center'> <input type='button' class='DeleteBtn'  name='DeleteBtn' id = '" + data[i].f_id + "'  value='刪除'/ ></td>" +
                        "</tr>";
                }
            }
            $('#TableBody').append(rows);
        },

        failure: function (data) {
        },
        error: function (data) {
        }
    });

    //前往新增帳號
    $("#AddAccount").click(function () {
        location.href = "/Account/AddAccount"
    });
    //前往修改密碼
    $("#EditPwd").click(function () {
        location.href = "/Account/PutPwd"
    });
    //前往新增權限
    $("#AddAccountLevel").click(function () {
        location.href = "/Account/AddAccountLevel"
    });
    //前往首頁
    $("#GoIndex").click(function () {
        location.href = "/index"
    });

    //點擊編輯按鈕
    $("#TableBody").on('click', '.EditBtn', function () {
        var currentRow = $(this).closest("tr");

        var col1 = currentRow.find("td:eq(0)").text();
        var col2 = currentRow.find("td:eq(1)").text();
        var col3 = currentRow.find("td:eq(2)").text();
        //var data = col1 + "\n" + col2 + "\n" + col3;
        //alert(data);

        for (var i = 0; i < accLevel.length - 1; i++) {
            if (accPosition[i] === col3) {
                var LvRows = LvRows + "<option selected value='" + accLevel[i] + "'>" + accPosition[i] + "</option>"
            } else {
                var LvRows = LvRows + "<option value='" + accLevel[i] + "'>" + accPosition[i] + "</option>"
            }

        }
        if ($("#EditBox").css("display") == "none") {
            var EditData =
                "<div><label id='Editfacc'> 帳號:" + col2 + "</label></div>" +
                "<div><label for='Level'>Level:</label><select id='Editlevel'>" + LvRows + "</select></div>" +
                "<div id='Editbutton'><input name='EditOK' id='" + col1 + "' onclick ='EditOK_Click(this.id)' type='Button' value='確認編輯' />" +
                "<input name='EditCancel' id = 'EditCancel' type = 'Button'  onclick = 'EditCancel_Click()' value = '取消編輯' /></div > "
            $('#Editform').append(EditData);
            $("#EditBox").show();
        }
    });

    //刪除
    $("#TableBody").on('click', '.DeleteBtn', function () {
        var currentRow = $(this).closest("tr");

        var col1 = currentRow.find("td:eq(0)").text();

        if (window.confirm("確定要刪除此帳號嗎?")) {
            $.ajax({
                url: "/api/Account/DelAcc?id=" + col1,
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
    });

    $("#logOut").click(function () {
        $.ajax({
            url: "~/api/Login/Logout",
            type: "Delete",
            contentType: "application/json",
            dataType: "text",
            data: JSON.stringify({
                "account": $("#Account").val(),
                "Pwd": $("#PassWord").val()
            }),
            success: function (result) {
                location.href = "/Index"
            },
            error: function (error) {
            }
        })
    });

})


//確認編輯
function EditOK_Click(Id) {
    $.ajax({
        url: "/api/account/PutAcc?id=" + Id,
        type: "put",
        contentType: "application/json",
        dataType: "text",
        data: JSON.stringify({
            "Level": parseInt($("#Editlevel").val())
        }),
        success: function (result) {
            alert(result)

            if (result == "帳號更新成功") {
                location.reload(); //新增成功才更新頁面
            }
        },
        error: function (error) {
            alert(error);
        }
    })
}
//取消編輯
function EditCancel_Click() {
    if ($("#EditBox").css("display") !== "none") {
        $("#EditBox").hide();
        $("#Editform > div").remove();
    }
}
