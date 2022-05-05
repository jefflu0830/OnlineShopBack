$(document).ready(function (data) {

    //取得等級資料
    $.ajax({
        type: "GET",
        url: "/api/member/GetSuspensionList",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            for (var i in data) {

                if (data[i].f_suspensionLv === "0" || data[i].f_suspensionLv === "100" ) {//編號0 or 100 不可刪除
                    var rows = rows + "<tr>" +
                        "<td name='suspensionLv'>" + data[i].f_suspensionLv + "</td>" +
                        "<td name='suspensionName'>" + data[i].f_suspensionName + "</td>" +
                        "<td align='center'> <input type='button'   class='EditBtn' value='編輯'/ ></td>" +
                        "<td></td >"+
                        "</tr>";
                }
                else {
                    var rows = rows + "<tr>" +
                        "<td name='suspensionLv'>" + data[i].f_suspensionLv + "</td>" +
                        "<td name='suspensionName'>" + data[i].f_suspensionName + "</td>" +
                        "<td align='center'> <input type='button'   class='EditBtn' value='編輯'/ ></td>" +
                        "<td align='center'> <input type='button'   class='DeleteBtn'  value='刪除'/ ></td>" +
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

    //新增會員等級
    $("#Addform").on('click', '#post', function () {
        var ErrorCode = "";
        //檢測
        if ($("#suspensionLevel").val() === "" || $("#suspensionName").val() === "") {
            ErrorCode += "[名稱] 或 [編號] 不可空白\n"
        } else {
            if (parseInt($("#suspensionLevel").val()) < 0 || parseInt($("#suspensionLevel").val()) > 255) {
                ErrorCode += "[編號]請輸入0～255之間。\n"
            }
            if (/^[0-9]*$/.test($("#suspensionLevel").val()) == false) {
                ErrorCode += "[編號] 只允許輸入數字。\n"
            }
            if (/^[a-zA-Z0-9\u4e00-\u9fa5]+$/.test($("#suspensionName").val()) == false) {
                ErrorCode += "[名稱] 不允許中英數以外字符。\n"
            }
            if ($("#suspensionName").val().length > 10) {
                ErrorCode += "[名稱]請小於10個字\n"
            }
        }

        if (ErrorCode !== "") {
            alert(ErrorCode)
        }
        else {
            $.ajax({
                type: "post",
                url: "/api/member/AddSuspension",
                contentType: "application/json",
                dataType: "text",
                data: JSON.stringify({
                    "suspensionLv": parseInt($("#suspensionLevel").val()),
                    "suspensionName": $("#suspensionName").val()
                }),
                success: function (result) {
                    alert(result)

                    if (result == "狀態新增成功") {
                        location.reload(); //新增成功才更新頁面

                    }
                },
                error: function (error) {
                    alert(error);
                }
            })
        }
    });

    //點擊編輯會員等級按鈕
    $("#TableBody").on('click', '.EditBtn', function () {

        var currentRow = $(this).closest("tr");

        var MemLv = currentRow.find("td:eq(0)").text();
        var MemName = currentRow.find("td:eq(1)").text();
        //var data = col1 ;
        //alert(data);

        if ($("#EditBox").css("display") == "none") {
            var EditData =
                "<h5>會員等級修改</h5>" +
                "<div><label>會員等級編號 ： </label><label >" + MemLv + "</label></div>" +
                "<div><label>會員等級名稱 ： </label><input type='text' id='memLvName' name='memLvName'maxlength='10'value='" + MemName+"' /></div>" +
                //"<div id='Editbutton'><input name='EditMemLv' onclick ='EditMemLv_Click(" + col1 + ")' type='Button' value='確認修改' />" +
                "<div id='Editbutton'><input id='EditMemLv' name='EditMemLv' type='Button' onclick ='EditPwd_Click(" + MemLv + ")' value='確認修改' />" +
                "<input name='EditCancel' id = 'EditCancel' name='EditCancel' type = 'Button'  value = '取消修改' /></div > "
            $('#Editform').append(EditData);
            $("#EditBox").show();
        }
    });

    //刪除會員等級按鈕
    $("#TableBody").on('click', '.DeleteBtn', function () {

        var currentRow = $(this).closest("tr");
        var col1 = currentRow.find("td:eq(0)").text();

        if (window.confirm("確定要刪除此帳號嗎?")) {
            $.ajax({
                url: "/api/member/DelSuspension?id=" + col1,
                type: "DELETE",
                data: {},
                success: function (result) {
                    alert(result)

                    if (result == "刪除成功") {
                        location.reload(); //刪除成功才更新頁面
                    }
                },
                error: function (error) {
                    alert(error);
                }
            })
        }
    });

    //回到上頁會員menu
    $("#GoMemMenu").click(function () {
        location.href = "/Member/MemberMenu"
    });

    $(document.body).on('click', '#EditCancel', function () {
        if ($("#EditBox").css("display") !== "none") {
            $("#EditBox").hide();
            $("#Editform > div,#Editform >h5").remove();
        }
    });

});

//編輯確認
function EditPwd_Click(MemLv) {
    var errorCode = ""

    if ($("#memLvName").val() === "") {
        errorCode += "更改名稱不可為空白\n"
    }
    if (/^[a-zA-Z0-9\u4e00-\u9fa5]*$/.test($("#memLvName").val()) == false) {
        errorCode += "[會員等級名稱] 只允許輸入英文及數字。\n"
    }


    //errorCode若不為空則,不進行修改
    if (errorCode !== "") {
        alert(errorCode);
    }
    else {
        $.ajax({
            url: "/api/Member/PutMemLv?MemLv=" + MemLv,
            type: "put",
            contentType: "application/json",
            dataType: "text",
            data: JSON.stringify({
                "LvName": $("#memLvName").val()
            }),
            success: function (result) {
                alert(result)

                if (result == "會員等級更新成功") {
                    location.reload(); //新增成功才更新頁面
                }
            },
            error: function (error) {
                alert(error);
            }
        })
    }
}




