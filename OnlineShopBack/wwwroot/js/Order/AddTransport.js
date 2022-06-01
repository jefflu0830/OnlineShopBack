$(document).ready(function () {

    //組畫面
    var result = OrderMenuFun.MakeOrderMenuTag(TransportJson);
    $('#TableBody').html(result)

    //新增配送方式
    $('#AddTransport').click(function () {

        var ErrorCode = "";
        //檢測
        if ($("#Transport").val() === "" || $("#TransportName").val() === "") {
            ErrorCode += "[名稱] 或 [代號] 不可空白\n"
        } else {
            if (parseInt($("#Transport").val()) < 0 || parseInt($("#Transport").val()) > 255) {
                ErrorCode += "[編號]請輸入0～255之間。\n"
            }
            if (/^[0-9]*$/.test($("#Transport").val()) == false) {
                ErrorCode += "[編號] 只允許輸入數字。\n"
            }
            if (/^[a-zA-Z0-9\u4e00-\u9fa5]+$/.test($("#TransportName").val()) == false) {
                ErrorCode += "[名稱] 不允許中英數以外字符。\n"
            }
            if ($("#TransportName").val().length > 20) {
                ErrorCode += "[名稱]請小於20個字\n"
            }
        }

        if (ErrorCode !== "") {
            alert(ErrorCode)
        }
        else {
            $.ajax({
                type: "post",
                url: "/api/Order/AddTransport",
                contentType: "application/json",
                dataType: "text",
                data: JSON.stringify({
                    "Transport": parseInt($('#Transport').val()),
                    "TransportName": $('#TransportName').val()
                }),
                success: function (result) {
                    var JsonResult = JSON.parse(result)//JSON字串轉物件

                    switch (JsonResult[0].st) {
                        case 0: {
                            alert('新增成功');
                            location.reload();
                            break;
                        };
                        case 1: {
                            alert('資料庫新增失敗');
                            break;
                        };
                        case 100: {
                            alert('有相同配送代號');
                            location.reload();
                            break;
                        }
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
    })

    //編輯名稱
    $('.EditTransportBtn').click(function () {
        var currentRow = $(this).closest("tr");
        var TransportNum = currentRow.find("td:eq(0)").text();
        var TransportName = currentRow.find("td:eq(1)").text();

        //組html Tag
        if ($("#EditBox").css("display") == "none") {

            var EditData =
                '<h5>配送方式編輯</h5>' +
                '<div><label> 配送方式編號:</label><label>' + TransportNum + '</label></div>' +
                '<div><label> 配送方式名稱:</label><input type="text" id="EditTransportName" name="TransportName" maxlength="20" value="' + TransportName + '" /></div>' +
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
                //檢測
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
                        url: "/api/Order/UpdateTransport?TransportNum=" + TransportNum + "&TransportName=" + $('#EditTransportName').val(),
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
                                    location.reload(); //新增成功才更新頁面
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
    })

    //刪除按鈕
    $('.DeleteBtn').click(function () {
        var currentRow = $(this).closest("tr");
        var TransportNum = currentRow.find('td:eq(0)').text();


        if (window.confirm("確定要刪除此配送方式嗎?")) {
            $.ajax({
                url: '/api/Order/DelTransport?TransportNum=' + TransportNum,
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
                        case 100: {
                            alert('無此配送方式');
                            break;
                        }
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

    //前往新增配送狀態
    $("#AddTransportStatus").click(function () {
        location.href = "/Order/AddTransportStatus"
    });

    //上一頁
    $("#NextPage").click(function () {
        location.href = "/Order/OrderMenu"
    });

});


OrderMenuFun = {
    MakeOrderMenuTag: function (MenuJson) {
        var rows = '';
        for (var i in MenuJson) {

            rows += "<tr>" +
                '<td >' + MenuJson[i].f_transport + '</td>' +
                '<td id="' + MenuJson[i].f_transport+'" >' + MenuJson[i].f_transportName + '</td>' +
                '<td align="center"> <input type="button" class="EditTransportBtn" value="編輯名稱"/ ></td>' +
                "<td align='center'> <input type='button' class='DeleteBtn'  name='ReturnBtn' value='刪除'/ ></td>";
            "</tr>";
        }
        return rows
    }
}