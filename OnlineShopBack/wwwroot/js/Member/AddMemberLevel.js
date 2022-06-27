$(document).ready(function (data) {

    //取得等級資料
    $.ajax({
        type: "GET",
        url: "/api/member/GetMemLvList",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            for (var i in data) {

                if (data[i].memLv === 0) {//編號0 不可刪除
                    var rows = rows + "<tr>" +
                        "<td name='faccLevel'>" + data[i].memLv + "</td>" +
                        "<td name='faccPosition'>" + data[i].LvName + "</td>" +
                        "<td align='center'> <input type='button'   class='EditBtn' value='編輯'/ ></td>" +
                        "<td></td >" +
                        "</tr>";
                }
                else {
                    var rows = rows + "<tr>" +
                        "<td name='faccLevel'>" + data[i].memLv + "</td>" +
                        "<td name='faccPosition'>" + data[i].LvName + "</td>" +
                        "<td align='center'> <input type='button'   class='EditBtn' value='編輯'/ ></td>" +
                        "<td align='center'> <input type='button'   class='DeleteBtn'  value='刪除'/ ></td>" +
                        "</tr>";
                }
            }
            $('#TableBody').html(rows);
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
        if ($("#LevelName").val() === "" || $("#memberLevel").val() === "") {
            ErrorCode += "[會員等級名稱] 或 [會員等級編號] 不可空白\n"
        } else {
            if (parseInt($("#memberLevel").val()) < 0 || parseInt($("#memberLevel").val()) > 255) {
                ErrorCode += "[會員等級編號]請輸入0～255之間。\n"
            }
            if (/^[0-9]*$/.test($("#memberLevel").val()) == false) {
                ErrorCode += "[會員等級編號] 只允許輸入數字。\n"
            }
            if (/^[a-zA-Z0-9\u4e00-\u9fa5]+$/.test($("#LevelName").val()) == false) {
                ErrorCode += "[會員等級名稱] 不允許中英數以外字符。\n"
            }
            if ($("#LevelName").val().length > 10) {
                ErrorCode += "[會員等級名稱]請小於10個字\n"
            }
        }

        if (ErrorCode !== "") {
            alert(ErrorCode)
        }
        else {
            $.ajax({
                type: "post",
                url: "/api/member/AddMemLv",
                contentType: "application/json",
                dataType: "text",
                data: JSON.stringify({
                    "memLv": parseInt($("#memberLevel").val()),
                    "LvName": $("#LevelName").val()
                }),
                success: function (result) {
                    var JsonResult = JSON.parse(result);//JSON字串轉物件

                    switch (JsonResult[0].st) {
                        case 0: {
                            alert('會員等級新增成功');
                            location.reload();
                            break;
                        };
                        case 100: {
                            alert('權限重複');
                            break;
                        };
                        case 200: {
                            alert('後端驗證失敗,請查詢LOG');
                            break;
                        };
                        case 201: {
                            alert('例外錯誤,請查詢LOG');
                            location.reload();
                            break;
                        }
                        default: {
                            alert(result);
                        }
                    }

                    if (result === "已從另一地點登入,轉跳至登入頁面") {
                        location.reload();
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
                "<div><label>會員等級名稱 ： </label><input type='text' id='memLvName' name='memLvName'maxlength='10'value='" + MemName + "' /></div>" +
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

        if (window.confirm("確定要刪除此等級嗎?")) {
            $.ajax({
                url: "/api/member/DelMemLv?memLv=" + col1,
                type: "DELETE",
                data: {},
                success: function (result) {
                    var JsonResult = JSON.parse(result);//JSON字串轉物件

                    switch (JsonResult[0].st) {
                        case 0: {
                            alert('會員等級刪除成功');
                            location.reload();
                            break;
                        };
                        case 100: {
                            alert('此等級不可刪除');
                            break;
                        };
                        case 101: {
                            alert('此等級尚未建立');
                            break;
                        };
                        case 102: {
                            alert('有使用者正在套用此等級');
                            break;
                        };
                        case 200: {
                            alert('後端驗證失敗,請查詢LOG');
                            break;
                        };
                        case 201: {
                            alert('例外錯誤,請查詢LOG');
                            location.reload();
                            break;
                        }
                        default: {
                            alert(result);
                        }
                    };
                    if (result === "已從另一地點登入,轉跳至登入頁面") {
                        location.reload();
                    };
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
            url: "/api/Member/EditMemLv?MemLv=" + MemLv,
            type: "put",
            contentType: "application/json",
            dataType: "text",
            data: JSON.stringify({
                "LvName": $("#memLvName").val()
            }),
            success: function (result) {
                var JsonResult = JSON.parse(result);//JSON字串轉物件

                switch (JsonResult[0].st) {
                    case 0: {
                        alert('會員等級編輯成功');
                        location.reload();
                        break;
                    };
                    case 100: {
                        alert('尚未建立此權限');
                        break;
                    };
                    case 200: {
                        alert('後端驗證失敗,請查詢LOG');
                        break;
                    };
                    case 201: {
                        alert('例外錯誤,請查詢LOG');
                        location.reload();
                        break;
                    }
                    default: {
                        alert(result);
                    }
                }
                if (result === "已從另一地點登入,轉跳至登入頁面") {
                    location.reload();
                }
            },
            error: function (error) {
                alert(error);
            }
        })
    }
}




