$(document).ready(function () {

    //取得會員列表
    $.ajax({
        type: "GET",
        url: "/api/member/getmember",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            memJson = data;

            for (var i in data) {
                var rows = rows + "<tr>" +
                    "<td id='id'>" + data[i].Id + "</td>" +
                    "<td id='acc'>" + data[i].MemAcc + "</td>" +
                    "<td id='name'>" + data[i].Name + "</td>" +
                    "<td id='phone'>" + data[i].Phone + "</td>" +
                    "<td id='mail'>" + data[i].Mail + "</td>" +
                    "<td id='address'>" + data[i].Address + "</td>" +
                    "<td id='shopGold'>" + data[i].ShopGold + "</td>" +
                    "<td id='level'>" + data[i].LevelName + "</td>" +
                    "<td id='suspension'>" + data[i].SuspensionName + "</td>" +
                    "<td id='createDate'>" + data[i].CreateDate + "</td>" +
                    "<td align='center'> <input type='button' class='EditBtn'  name='EditBtn' value='編輯'/ ></td>" +
                    "<td align='center'> <input type='button' class='DeleteBtn'  name='DeleteBtn' value='刪除'/ ></td>" +
                    "</tr>";
            }
            $('#Table').append(rows);
        },
        failure: function (data) {
        },
        error: function (data) {
        }
    });



    //刪除按鈕
    $("#TableBody").on('click', '.DeleteBtn', function () {

        var currentRow = $(this).closest("tr");
        var col1 = currentRow.find("td:eq(0)").text(); //取得該列第一格

        if (window.confirm("確定要刪除此帳號嗎?")) {
            $.ajax({
                url: "/api/Member/DelMember?id=" + col1,
                type: "DELETE",
                data: {},
                success: function (result) {
                    var JsonResult = JSON.parse(result);//JSON字串轉物件

                    switch (JsonResult[0].st) {
                        case 0: {
                            alert('刪除成功');
                            location.reload();
                            break;
                        };
                        case 100: {
                            alert('無此會員');
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
                    }
                },
                error: function (error) {
                    alert(error);
                }
            })
        }
    });



    //點擊編輯帳號按鈕
    $("#TableBody  ").on('click', '.EditBtn', function () {
        var LvRows = "";
        var SuspensionRows = "";

        var currentRow = $(this).closest("tr");
        var col1 = currentRow.find("td:eq(0)").text();
        var col2 = currentRow.find("td:eq(1)").text();
        var col8 = currentRow.find("td:eq(7)").text();
        var col9 = currentRow.find("td:eq(8)").text();


        for (var i = 0; i < memLevel.length - 1; i++) {
            if (memLevelName[i] === col8) {

                LvRows += "<option selected value='" + memLevel[i] + "'>" + memLevelName[i] + "</option>"
            } else {
                LvRows += "<option value='" + memLevel[i] + "'>" + memLevelName[i] + "</option>"
            }
        }
        for (var i = 0; i < Suspension.length - 1; i++) {
            if (SuspensionName[i] === col9) {
                SuspensionRows += "<option selected value='" + Suspension[i] + "'>" + SuspensionName[i] + "</option>"
            } else {
                SuspensionRows += "<option value='" + Suspension[i] + "'>" + SuspensionName[i] + "</option>"
            }
        }
        if ($("#EditBox").css("display") == "none") {
            var EditData =
                "<h5>編輯</h5>" +
                "<div><label> 帳號:</label><label id='Editfacc'>" + col2 + "</label></div>" +
                "<div><label for='Level'>Level:</label><select id='Editlevel'>" + LvRows + "</select></div>" +
                "<div><label for='Level'>Suspension:</label><select id='EditSuspension'>" + SuspensionRows + "</select></div>" +
                "<div id='Editbutton'><input name='EditAcc' onclick ='memMenufun.EditMem_Click(" + col1 + ")' type='Button' value='確認編輯' />" +
                "<input name='EditCancel' id = 'EditCancel' type = 'Button'  onclick = 'EditCancel_Click()' value = '取消編輯' /></div > "
            $('#Editform').append(EditData);
            $("#EditBox").show();
        }
        //取消
        $(document.body).on('click', '#EditCancel', function () {
            if ($("#EditBox").css("display") !== "none") {
                $("#EditBox").hide();
                $("#Editform > div,#Editform >h5").remove();
            }
        });
    });


    //前往新增會員等級
    $("#AddLevelBtn").click(function () {
        location.href = "/Member/AddMemberLevel"
    });
    //前往新增會員狀態
    $("#AddSuspensionBtn").click(function () {
        location.href = "/Member/AddSuspension"
    });
    //前往購物金調整
    $("#ShopGoldBtn").click(function () {
        location.href = "/Member/AddShopGold"
    });

    //回Index
    $("#GoIndex").click(function () {
        location.href = "/index"
    });
})

var memMenufun = {
    //確認編輯帳號
    EditMem_Click: function (Id) {
        $.ajax({
            url: "/api/Member/EditMember?id=" + Id,
            type: "put",
            contentType: "application/json",
            dataType: "text",
            data: JSON.stringify({
                "Level": parseInt($("#Editlevel").val()),
                "Suspension": parseInt($("#EditSuspension").val())
            }),
            success: function (result) {
                var JsonResult = JSON.parse(result);//JSON字串轉物件

                switch (JsonResult[0].st) {
                    case 0: {
                        alert('更新成功');
                        location.reload();
                        break;
                    };
                    case 100: {
                        alert('無此會員');
                        break;
                    };
                    case 101: {
                        alert('無此等級');
                        break;
                    };
                    case 102: {
                        alert('無此狀態');
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
                }
            },
            error: function (error) {
                alert(error);
            }
        })
    },
    //組html標籤
    DrawMemList: function (memArray) {
        var htmlText = '';

        for (var i = 0; i < memArray.length; i++) {
            var memEdit = memArray[i].Id == 0 ? '' : "<input type='button' class='EditAccBtn'  name='EditAccBtn'  value='編輯帳號'/ >";
            var memDel = memArray[i].Id == 0 ? '' : "<input type='button' class='DeleteBtn'  name='DeleteBtn' value='刪除'/ >";

            htmlText += "<tr>" +
                "<td id='RegdNo'>" + memArray[i].Id + "</td>" +
                "<td id='RegdNo'>" + memArray[i].MemAcc + "</td>" +
                "<td id='Name'>" + memArray[i].Name + "</td>" +
                "<td id='phone'>" + memArray[i].Phone + "</td>" +
                "<td id='mail'>" + memArray[i].Mail + "</td>" +
                "<td id='address'>" + memArray[i].Address + "</td>" +
                "<td id='shopGold'>" + memArray[i].ShopGold + "</td>" +
                "<td id='level'>" + memArray[i].LevelName + "</td>" +
                "<td id='suspension'>" + memArray[i].SuspensionName + "</td>" +
                "<td id='createDate'>" + memArray[i].CreateDate + "</td>" +
                "<td align='center'>" + memEdit + "</td>" +
                "<td align='center'>" + memDel + "</td>" +
                "</tr>";
        }

        htmlText += "</tr>";

        $("#TableBody").html(htmlText);
    },
    //搜尋
    memListSerch: function () {
        //extend  深複製暫存檔來操作;
        var tempTable = $.extend(true, [], memJson);

        var serchvalue = $("#Search").val();

        var StrClassArr = ["Id", "MemAcc", "Name", "Phone", "Mail", "Address", "LevelName", "Suspension", "CreateDate"]
        var IntClassArr = ["ShopGold"]

        if (serchvalue === "") {

            //組HTML,覆蓋
            memMenufun.DrawMemList(memJson);

        } else {

            //字串搜尋
            var searchStr = function (searchClass) {
                tempTable = tempTable.filter((item) => {//filter搜尋json
                    if (item[searchClass].toString().indexOf(serchvalue) >= 0) {//indexOf -> 有找到所鍵入文字則回傳 >=0
                        return item //大於等於0則 return item
                    }
                })
            }

            //數字搜尋
            var searchInt = function (searchClass) {
                tempTable = tempTable.filter((item) => {
                    if (item[searchClass] == serchvalue) {
                        return item
                    }
                })
            }


            //字串搜尋
            if (StrClassArr.indexOf($("#SearchClass").val()) >= 0) {
                searchStr($("#SearchClass").val());
            }
            //數字搜尋
            else if (IntClassArr.indexOf($("#SearchClass").val()) >= 0) {
                searchInt($("#SearchClass").val());
            }

            //組HTML,覆蓋
            memMenufun.DrawMemList(tempTable);

        }
    }


}