$(document).ready(function () {

    //組Transport下拉選單
    var TransportSelectTag = '';
    for (i in TransportJson) {
        TransportSelectTag += ' <option value="' + TransportJson[i].Transport + '">' + TransportJson[i].TransportName + '</option>'
    }
    $('#TransportSelect').html(TransportSelectTag)

    //列表內容初始化
    AddTransStatusFun.TransportSelectOnChange();

    //Transport 下拉選單 change
    $('#TransportSelect').change(function () {
        AddTransStatusFun.TransportSelectOnChange();
        if ($("#EditBox").css("display") !== "none") {
            $("#EditBox").hide();
        };
    });

    //點擊新增
    $('#AddTransportStatus').click(function () {
        var Transport = $('#TransportSelect').val();
        var TransportStatus = $('#TransportStatus').val();
        var TransportStatusName = $('#TransportStatusName').val();
        var ErrorCode = "";

        //檢測
        if ($("#Transport").val() === "" || $("#TransportStatus").val() === "" || $("#TransportStatusName").val() === "") {
            ErrorCode += "[名稱] 或 [代號] 不可空白\n"
        } else {
            if (parseInt($("#TransportStatus").val()) < 0 || parseInt($("#TransportStatus").val()) > 255) {
                ErrorCode += "[會員等級編號]請輸入0～255之間。\n"
            }
            if (/^[0-9]*$/.test($("#TransportStatus").val()) == false) {
                ErrorCode += "[會員等級編號] 只允許輸入數字。\n"
            }
            if (/^[a-zA-Z0-9\u4e00-\u9fa5]+$/.test($("#TransportStatusName").val()) == false) {
                ErrorCode += "[會員等級名稱] 不允許中英數以外字符。\n"
            }
            if ($("#TransportStatusName").val().length > 10) {
                ErrorCode += "[會員等級名稱]請小於10個字\n"
            }
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
                            AddTransStatusFun.ReMakeList();
                            $('#TransportStatus').val('')
                            $('#TransportStatusName').val('')
                            break;
                        }
                        case 1:
                            alert('資料庫新增失敗');
                            break;
                        default: {
                            alert('資料庫新增失敗');
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

    //刪除按鈕
    //$('.DeleteBtn').click(function () {
    $('#TableBody').on('click', '.DeleteBtn', function () {
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
                            AddTransStatusFun.ReMakeList();
                            //location.reload(); //新增成功才更新頁面
                            break;
                        }
                        case 100:
                            alert('無此配送方式');
                            break;
                        default: {
                            alert('資料庫新增失敗');
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

    //編輯名稱按鈕
    $('#TableBody').on('click', '.EditTransportBtn', function () {
        var currentRow = $(this).closest("tr");
        var TransportNum = currentRow.find("td:eq(0)").attr('id');
        var TransportName = currentRow.find("td:eq(0)").text();
        var TransportStatusNum = currentRow.find("td:eq(1)").text();
        var TransportStatusName = currentRow.find("td:eq(2)").text();
        //組html Tag
        if ($("#EditBox").css("display") == "none") {

            var EditData =
                '<h5>配送方式編輯</h5>' +
                '<div><label> 配送方式:</label><label >' + TransportName + '</label></div>' +
                '<div><label> 配送狀態代號:</label><label>' + TransportStatusNum + '</label></div>' +
                '<div><label> 配送狀態名稱:</label><input type="text" id="EditTransportName" name="TransportName" maxlength="20" value="' + TransportStatusName + '" /></div>' +
                "<div id='Editbutton'><input id='EditConfirm' type='Button' value='確認編輯' />" +
                "<input name='EditCancel' id = 'EditCancel' type = 'Button' value = '取消編輯' /></div > ";

            $('#Editform').html(EditData);
            $("#EditBox").show();

            $('#EditTransport').change(function () {
                var SelectTag = OrderMenuFun.MakeTransportStatusSelect($('#EditTransport').val(), '');
                $('#EditTransportStatus').html(SelectTag);
            });

            //確認編輯
            $('#EditConfirm').click(function () {                                                  

                var ErrorCode = "";
                ////檢測
                if ($('#EditTransportName').val() === "") {
                    ErrorCode += "[名稱] 不可空白\n"
                } else {
                    if (/^[a-zA-Z0-9\u4e00-\u9fa5]+$/.test($('#EditTransportName').val()) == false) {
                        ErrorCode += "[名稱] 不允許中英數以外字符。\n"
                    }
                    if ($('#EditTransportName').val().length > 20) {
                        ErrorCode += "[名稱]請小於20個字\n"
                    }
                }

                if (ErrorCode !== "") {
                    alert(ErrorCode)
                }
                else {
                    $.ajax({
                        url: '/api/Order/UpdateTransportStatus?TransportNum=' + TransportNum + '&TransportStatusNum=' + TransportStatusNum + '&TransportStatusName=' + $('#EditTransportName').val(),
                        type: "put",
                        contentType: "application/json",
                        dataType: "text",
                        data: JSON.stringify({
                            "SubCategoryName": $("#EditCategroyName").val()
                        }),
                        success: function (result) {

                            var JsonResult = JSON.parse(result)//JSON字串轉物件
                            switch (JsonResult[0].st) {
                                case 0: {
                                    alert('更新成功');
                                    AddTransStatusFun.ReMakeList();
                                    $("#EditBox").hide();
                                    //location.reload(); //新增成功才更新頁面
                                    break;
                                }
                                case 100: {
                                    alert('尚未建立此配送方式');
                                    break;
                                }
                                default: {
                                    alert('資料庫新增失敗');
                                }
                            }
                        },
                        error: function (error) {
                            alert(error);
                        }
                    })
                }
            });
            //取消編輯
            $('#EditCancel').click(function () {
                if ($("#EditBox").css("display") !== "none") {
                    $("#EditBox").hide();
                };
            });
        };
    });

    //上一頁
    $("#NextPage").click(function () {
        location.href = "/Order/AddTransport"
    });
});

AddTransStatusFun = {
    //組Tanle Html標籤
    MakeTransStatusTag: function (MenuJson) {
        var rows = '';
        for (var i in MenuJson) {

            rows += "<tr>" +
                '<td id="' + MenuJson[i].Transport + '" >' + $('#TransportSelect option:selected').text() + '</td>' +
                '<td >' + MenuJson[i].TransportStatus + '</td>' +
                '<td >' + MenuJson[i].TransportStatusName + '</td>' +
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
            if (item['Transport'] == parseInt(Transport)) {
                return item
            }
        });
        var rows = AddTransStatusFun.MakeTransStatusTag(temp);
        $('#TableBody').html(rows)
    },
    //重取列表資訊
    ReMakeList: function () {

        //取得表
        $.ajax({
            type: "GET",
            url: "/api/Order/GetTransportStatus",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                TransportJson = data.TransportTable;
                TransportStatusJson = data.TransportStatusTable;

                AddTransStatusFun.TransportSelectOnChange();
            },
            failure: function (data) {
                alert(data);
            },
            error: function (data) {
                alert(data);
            }
        });


    }
}