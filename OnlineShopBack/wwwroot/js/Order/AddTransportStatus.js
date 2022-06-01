﻿$(document).ready(function () {
    //組Transport下拉選單
    var TransportSelectTag = '';
    for (i in TransportJson) {
        TransportSelectTag += ' <option value="' + TransportJson[i].f_transport + '">' + TransportJson[i].f_transportName + '</option>'
    }
    $('#TransportSelect').html(TransportSelectTag)
    //Table 內容初始化
    AddTransStatusFun.TransportSelectOnChange();

    //Transport 下拉選單 change
    $('#TransportSelect').change(function () {
        AddTransStatusFun.TransportSelectOnChange();
    });

    //點擊新增
    $('#AddTransportStatus').click(function () {
        var Transport = $('#TransportSelect').val();
        var TransportStatus = $('#TransportStatus').val();
        var TransportStatusName = $('#TransportStatusName').val();
        var ErrorCode = "";

        //檢測
        if ($("#LevelName").val() === "" || $("#memberLevel").val() === "") {
            ErrorCode += "[名稱] 或 [代號] 不可空白\n"
        } else {
            //    if (parseInt($("#memberLevel").val()) < 0 || parseInt($("#memberLevel").val()) > 255) {
            //        ErrorCode += "[會員等級編號]請輸入0～255之間。\n"
            //    }
            //    if (/^[0-9]*$/.test($("#memberLevel").val()) == false) {
            //        ErrorCode += "[會員等級編號] 只允許輸入數字。\n"
            //    }
            //    if (/^[a-zA-Z0-9\u4e00-\u9fa5]+$/.test($("#LevelName").val()) == false) {
            //        ErrorCode += "[會員等級名稱] 不允許中英數以外字符。\n"
            //    }
            //    if ($("#LevelName").val().length > 10) {
            //        ErrorCode += "[會員等級名稱]請小於10個字\n"
            //    }
        }

        if (ErrorCode !== "") {
            alert(ErrorCode)
        }
        else {
            $.ajax({
                type: "post",
                url: "/api/Order/AddTransportStatus",
                contentType: "application/json",
                dataType: "text",
                data: JSON.stringify({
                    "Transport": parseInt(Transport),
                    "TransportStatus": parseInt(TransportStatus),
                    "TransportStatusName": TransportStatusName
                }),
                success: function (result) {
                    var JsonResult = JSON.parse(result)//JSON字串轉物件

                    switch (JsonResult[0].st) {
                        case 0: {
                            alert('新增成功');
                            location.reload();
                            break;
                        }
                        case 1:
                            alert('資料庫新增失敗');
                            break;
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

    //刪除按鈕
    //
    //$('.DeleteBtn').click(function () {
    $('#TableBody').on('click','.DeleteBtn',function () {
        var currentRow = $(this).closest("tr");
        var TransportNum = $('#TransportSelect').val();
        var TransportStatusNum = currentRow.find('td:eq(1)').text();
              

        if (window.confirm("確定要刪除此配送方式嗎?")) {
            $.ajax({
                url: '/api/Order/DelTransportStatus?TransportNum=' + parseInt(TransportNum) + '&TransportStatusNum=' + parseInt(TransportStatusNum),
                type: 'DELETE',
                data: {},
                success: function (result) {
                    var JsonResult = JSON.parse(result)//JSON字串轉物件
                    switch (JsonResult[0].st) {
                        case 0: {
                            alert('刪除成功');
                            location.reload(); //新增成功才更新頁面
                            break;
                        }
                        case 100:
                            alert('無此配送方式');
                            break;
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
});

AddTransStatusFun = {
    //組Tanle Html標籤
    MakeTransStatusTag: function (MenuJson) {
        var rows = '';
        for (var i in MenuJson) {

            rows += "<tr>" +
                '<td id="' + MenuJson[i].f_transport + '" >' + $('#TransportSelect option:selected').text() + '</td>' +
                '<td >' + MenuJson[i].f_transportStatus + '</td>' +
                '<td >' + MenuJson[i].f_transportStatusName + '</td>' +
                '<td align="center"> <input type="button" class="EditTransportBtn" value="編輯名稱"/ ></td>' +
                "<td align='center'> <input type='button' class='DeleteBtn'  name='ReturnBtn' value='刪除'/ ></td>";
            "</tr>";
        }
        return rows
    },
    //下拉選單chnage 改變 Tanle內容
    TransportSelectOnChange: function () {
        var Transport = $('#TransportSelect').val();
        var temp = TransportStatusJson.filter(function (item) {
            if (item['f_transport'] == parseInt(Transport) ) {
                return item
            }
        });
        var rows = AddTransStatusFun.MakeTransStatusTag(temp);
        $('#TableBody').html(rows)
    }
}