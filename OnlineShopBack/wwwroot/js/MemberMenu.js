﻿$(document).ready(function () {

    //取得會員列表
    $.ajax({
        type: "GET",
        url: "/api/member/getmember",             
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            //$("#DIV").html('');
            //var DIV = '';
            for (var i in data) {
                var rows = rows + "<tr id='istr'>" +
                    "<td id='RegdNo'>" + data[i].f_id + "</td>" +
                    "<td id='RegdNo'>" + data[i].f_acc + "</td>" +
                    "<td id='Name'>" + data[i].f_name + "</td>" +
                    "<td id='phone'>" + data[i].f_phone + "</td>" +
                    "<td id='mail'>" + data[i].f_mail + "</td>" +
                    "<td id='address'>" + data[i].f_address + "</td>" +
                    "<td id='shopGold'>" + data[i].f_shopGold + "</td>" +
                    "<td id='level'>" + data[i].f_LevelName + "</td>" +
                    "<td id='suspension'>" + data[i].f_suspensionName + "</td>" +
                    "<td id='createDate'>" + data[i].f_createDate + "</td>" +
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
                    alert(result)

                    if (result == "會員刪除成功") {
                        location.reload(); //刪除成功才更新頁面
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
        //var data = col1 + "\n" + col8 + "\n" + col9;
        //alert(data);
        
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
                "<div id='Editbutton'><input name='EditAcc' onclick ='EditMem_Click(" + col1 + ")' type='Button' value='確認編輯' />" +
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

//確認編輯帳號
function EditMem_Click(Id) {
    $.ajax({
        url: "/api/Member/PutMember?id=" + Id,
        type: "put",
        contentType: "application/json",
        dataType: "text",
        data: JSON.stringify({
            "Level": parseInt($("#Editlevel").val()),
            "Suspension": parseInt($("#EditSuspension").val())
        }),
        success: function (result) {
            alert(result)

            if (result == "更新成功") {
                location.reload(); //新增成功才更新頁面
            }
        },
        error: function (error) {
            alert(error);
        }
    })
}