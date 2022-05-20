﻿$(document).ready(function () {


    //取得帳號列表
    $.ajax({
        type: "GET",
        url: "/api/account/GetAcc",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            AccJson = data;

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
                "<div id='Editbutton'><input name='EditAcc' onclick ='AccountMenufun.EditAcc_Click(" + col1 + ")' type='Button' value='確認編輯' />" +
                "<input name='EditCancel' id = 'EditCancel' type = 'Button'  onclick = 'AccountMenufun.EditCancel_Click()' value = '取消編輯' /></div > "
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
                "<div id='Editbutton'><input name='EditPwd' onclick ='AccountMenufun.EditPwd_Click(" + col1 + ")' type='Button' value='確認修改' />" +
                "<input name='EditCancel' id = 'EditCancel' type = 'Button'  onclick = 'AccountMenufun.EditCancel_Click()' value = '取消修改' /></div > "
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
                    } else if (result === "已從另一地點登入,轉跳至登入頁面") {
                        location.reload();
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

//點擊表頭排列
$("#Tbheader > th").click(function () {//modi@  documant -> "#Tbheader > th"  要精準定位

    //extend  深複製暫存檔來操作;
    var tempTable = $.extend(true, [], AccJson);

    var sortKey = event.target.id; //記錄所點選 th的 Id
    this.Reverse = !this.Reverse


    if (sortKey === "f_id") {  //判斷如果為 f_id 則用數字排序
        if (!this.Reverse) {
            tempTable.sort(function (a, b) {
                return a[sortKey] - b[sortKey]  //升序
            })
        } else {
            tempTable.sort(function (a, b) {
                return b[sortKey] - a[sortKey]  //降序
            })
        }
    } else if (sortKey === "f_acc" || sortKey === "f_accPosition") {  //字串排序

        if (!this.Reverse) {
            tempTable.sort(function (a, b) {
                return a[sortKey].localeCompare(b[sortKey], "zh-hant"); //升序
            })
        }
        else {
            tempTable.sort(function (a, b) {
                return a[sortKey].localeCompare(b[sortKey], "zh-hant"); //降序
            })
            tempTable.reverse();
        }
    } else if (sortKey === "f_createDate") {  //日期排序
        if (!this.Reverse) {
            tempTable.sort(function (a, b) {
                return a[sortKey].localeCompare(b[sortKey], "zh-hant"); //升序
            });
        } else {
            tempTable.sort(function (a, b) {
                return b[sortKey].localeCompare(a[sortKey], "zh-hant"); //降序
            });
        }
    }
    //組HTML,覆蓋
    AccountMenufun.DrawAccountList(tempTable);
});

var AccountMenufun = {
    //組html標籤
    DrawAccountList: function (accArray) {
        var htmlText = '';

        for (var i = 0; i < accArray.length; i++) {
            var accEdit = accArray[i].f_id == 0 ? '' : "<input type='button' class='EditAccBtn'  name='EditAccBtn'  value='編輯帳號'/ >";
            var pwdEdit = accArray[i].f_id == 0 ? '' : "<input type='button' class='EditPwdBtn'  name='EditPwdBtn' value='修改密碼'/ ></td>";
            var accDel = accArray[i].f_id == 0 ? '' : "<input type='button' class='DeleteBtn'  name='DeleteBtn' value='刪除'/ >";

            htmlText += "<tr>" +
                "<td name='fid' id='" + accArray[i].f_id + "'>" + accArray[i].f_id + "</td>" +
                "<td name='facc' id='" + accArray[i].f_id + "'>" + accArray[i].f_acc + "</td>" +
                "<td name='faccPosition' id='" + accArray[i].f_id + "'>" + accArray[i].f_accPosition + "</td>" +
                "<td name='fcreateDate' id='" + accArray[i].f_id + "'>" + accArray[i].f_createDate + "</td>" +
                "<td align='center'>" + accEdit + "</td>" +
                "<td align='center'>" + pwdEdit + "</td>" +
                "<td align='center'>" + accDel + "</td>";
        }

        htmlText += "</tr>";

        $("#TableBody").html(htmlText);
    },
    //確認修改密碼
    EditPwd_Click: function (Id) {
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

                        if (result === "密碼修改成功") {
                            location.reload(); //新增成功才更新頁面
                        } else if (result === "已從另一地點登入,轉跳至登入頁面") {
                            location.reload();
                        }
                    },
                    error: function (error) {
                        alert(error);
                    }
                })
            }
        }
    },
    //確認編輯帳號
    EditAcc_Click: function (Id) {
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

                if (result === "帳號更新成功") {
                    location.reload(); //新增成功才更新頁面
                } else if (result === "已從另一地點登入,轉跳至登入頁面") {
                    location.reload();
                }
            },
            error: function (error) {
                alert(error);
            }
        })
    },
    //取消編輯
    EditCancel_Click: function () {
        if ($("#EditBox").css("display") !== "none") {
            $("#EditBox").hide();
            $("#Editform > div,#Editform >h5").remove();
        }
    },
    //排序下拉選單
    AccListOrder: function AccListOrder(OrderClass) {
        //extend  深複製暫存檔來操作;
        var tempTable = $.extend(true, [], AccJson);

        //id
        if (OrderClass == "0" || OrderClass == "1") {
            tempTable.sort(function (a, b) {
                return a["f_id"] - b["f_id"]  //升序
            })
            if (OrderClass == "1") {
                tempTable.reverse();  //反序
            }

        }
        //帳號
        else if (OrderClass == "2" || OrderClass == "3") {
            tempTable.sort(function (a, b) {
                return a["f_acc"].localeCompare(b["f_acc"], "zh-hant"); //升序
            })
            if (OrderClass == "3") {
                tempTable.reverse();//降序
            }

        }
        //權限
        else if (OrderClass == "4" || OrderClass == "5") {
            tempTable.sort(function (a, b) {
                return a["f_accPosition"].localeCompare(b["f_accPosition"], "zh-hant"); //升序
            })
            if (OrderClass == "5") {
                tempTable.reverse();//降序
            }

        }
        //日期
        else if (OrderClass == "6" || OrderClass == "7") {
            tempTable.sort(function (a, b) {
                return a["f_createDate"].localeCompare(b["f_createDate"], "zh-hant"); //正序
            })
            if (OrderClass == "7") {
                tempTable.reverse();//反序
            }
        }
        //組HTML標籤
        AccountMenufun.DrawAccountList(tempTable);

    },
    //搜尋
    AccListSerch: function () {
        //extend  深複製暫存檔來操作;
        var tempTable = $.extend(true, [], AccJson);
        var StrClassArr = ["f_id", "f_acc", "f_accPosition", "f_createDate"];
        var serchvalue = $("#Search").val();

        if (serchvalue === "") {

            //組HTML,覆蓋
            AccountMenufun.DrawAccountList(AccJson);

            AccountMenufun.AccListOrder($("#OrderClass").val)

        } else {

            //字串搜尋
            var searchStr = function (searchClass) {
                tempTable = tempTable.filter(function (item) {//filter搜尋json
                    if (item[searchClass].indexOf(serchvalue) >= 0) {//indexOf -> 有找到所鍵入文字則回傳 >=0
                        return item //大於等於0則 return item
                    }
                })
            }  

            //字串搜尋
            if (StrClassArr.indexOf($("#SearchClass").val()) >= 0) {
                searchStr($("#SearchClass").val());
            }

            //組HTML,覆蓋
            AccountMenufun.DrawAccountList(tempTable);

        }
    }
}








