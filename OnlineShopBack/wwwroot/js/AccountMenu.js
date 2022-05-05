$(document).ready(function () {

    //取得帳號列表
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
                        "<td align='center'></td>" +
                        "</tr>";
                }
                else {
                    var rows = rows + "<tr>" +
                        "<td name='fid'>" + data[i].f_id + "</td>" +
                        "<td name='facc'>" + data[i].f_acc + "</td>" +
                        "<td name='faccPosition' >" + data[i].f_accPosition + "</td>" +
                        "<td name='fcreateDate'>" + data[i].f_createDate + "</td>" +
                        "<td align='center'> <input type='button' class='EditAccBtn'  name='EditAccBtn'  value='編輯帳號'/ ></td>" +
                        "<td align='center'> <input type='button' class='EditPwdBtn'  name='EditPwdBtn' value='修改密碼'/ ></td>" +
                        "<td align='center'> <input type='button' class='DeleteBtn'  name='DeleteBtn' value='刪除'/ ></td>" +
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




    //點擊編輯帳號按鈕
    $("#TableBody").on('click', '.EditAccBtn', function () {
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
                "<h5>帳號編輯</h5>" +
                "<div><label> 帳號:</label><label id='Editfacc'>" + col2 + "</label></div>" +
                "<div><label for='Level'>Level:</label><select id='Editlevel'>" + LvRows + "</select></div>" +
                "<div id='Editbutton'><input name='EditAcc' onclick ='EditAcc_Click(" + col1 + ")' type='Button' value='確認編輯' />" +
                "<input name='EditCancel' id = 'EditCancel' type = 'Button'  onclick = 'EditCancel_Click()' value = '取消編輯' /></div > "
            $('#Editform').append(EditData);
            $("#EditBox").show();
        }
    });
    //點擊修改密碼按鈕
    $("#TableBody").on('click', '.EditPwdBtn', function () {
        var currentRow = $(this).closest("tr");

        var col1 = currentRow.find("td:eq(0)").text(); //取該列id
        var col2 = currentRow.find("td:eq(1)").text(); //取該列帳號

        if ($("#EditBox").css("display") == "none") {
            var EditData =
                "<h5>密碼修改</h5>" +
                "<div><label> 帳號:</label><label id='Editfacc'>" + col2 + "</label></div>" +
                "<div><label>新密碼:</label><input type='password' id='newPwd' name='newPwd'maxlength='16' /></div>" +
                "<div><label>確認新密碼:</label><input id='cfmNewPwd' name='cfmNewPwd' maxlength='16' /></div>" +
                "<div id='Editbutton'><input name='EditPwd' onclick ='EditPwd_Click(" + col1 + ")' type='Button' value='確認修改' />" +
                "<input name='EditCancel' id = 'EditCancel' type = 'Button'  onclick = 'EditCancel_Click()' value = '取消修改' /></div > "
            $('#Editform').append(EditData);
            $("#EditBox").show();
        }
    });
    //刪除按鈕
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

    //前往新增帳號
    $("#AddAccount").click(function () {
        location.href = "/Account/AddAccount"
    });
    //前往新增權限
    $("#AddAccountLevel").click(function () {
        location.href = "/Account/AddAccountLevel"
    });
    //前往首頁
    $("#GoIndex").click(function () {
        location.href = "/index"
    });

})

//$(document.body).on('blur', '#newPwd', function () { })

//確認修改密碼
function EditPwd_Click(Id) {
    var errorCode = "";

    //新密碼檢測
    if ($("#newPwd").val().length < 8) {
        errorCode += "[新密碼] 請大於8個字。\n"
    }
    if (/^[a-zA-Z0-9]*$/.test($("#newPwd").val()) == false) {
        errorCode += "[新密碼] 只允許輸入英文及數字。\n"
    }
    if ($("#cfmNewPwd").val() !== $("#newPwd").val()) {
        errorCode += "[新密碼] 與 [確認新密碼] 需輸入一致。\n"
    }
    //errorCode若不為空則,不進行修改
    if (errorCode !== "") {
        alert(errorCode);
    }
    else {
        if (window.confirm("確定要修改密碼嗎?")) {
            $.ajax({
                url: "/api/account/PutPwd",
                type: "put",
                contentType: "application/json",
                dataType: "text",
                data: JSON.stringify({
                    "id": Id,
                    "newPwd": $("#newPwd").val(),
                    "cfmNewPwd": $("#cfmNewPwd").val()
                }),
                success: function (result) {
                    alert(result)

                    if (result == "密碼修改成功") {
                        location.reload(); //新增成功才更新頁面
                    }
                },
                error: function (error) {
                    alert(error);
                }
            })
        }
    }
}
//確認編輯帳號
function EditAcc_Click(Id) {
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
        $("#Editform > div,#Editform >h5").remove();
    }
}
