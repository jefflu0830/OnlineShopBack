var ProductMenu = {
    OrderTable: [],
    TransportTable: [],
    TransportStatusTable: []
}

$(document).ready(function () {

    //取得表
    $.ajax({
        type: "GET",
        url: "/api/Order/GetOrder",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            OrderTable = $.extend(true, [], data.OrderTable);
            TransportTable = $.extend(true, [], data.TransportTable);
            TransportStatusTable = $.extend(true, [], data.TransportStatusTable);

            var Result = OrderMenuFun.MakeOrderMenuTag(OrderTable)

            $('#TableBody').html(Result);

            //編輯配送方式
            $('.EditTransportBtn').click(function () {
                var currentRow = $(this).closest("tr");
                var OrderId = currentRow.find("td:eq(0)").attr('id');
                var TransportSelectTag = "";
                var TransportStatusSelectTag = "";

                //篩選所點選訂單id資料
                var Filter = OrderTable.filter(function (item) {
                    if (item["f_id"] == OrderId) {
                        return item
                    }
                })
                //組配送方式下拉選單
                for (var i in TransportTable) {
                    if (TransportTable[i].f_transport == Filter[0].f_transport) {
                        TransportSelectTag += '<option selected value="' + TransportTable[i].f_transport + '">' + TransportTable[i].f_transportName + '</option >';
                    } else {
                        TransportSelectTag += '<option value="' + TransportTable[i].f_transport + '">' + TransportTable[i].f_transportName + '</option >';
                    };
                };

                //組配送狀態下拉選單
                TransportStatusSelectTag = OrderMenuFun.MakeTransportStatusSelect(Filter[0].f_transport, Filter[0].f_transportStatus);

                //組html Tag
                if ($("#EditBox").css("display") == "none") {

                    var EditData =
                        '<h5>配送方式編輯</h5>' +
                        '<div><label> 訂單編號:</label><label>' + Filter[0].f_orderNum + '</label></div>' +
                        '<div><label> 訂購帳號:</label><label>' + Filter[0].f_acc + '</label></div>' +
                        '<div><label> 配送方式:</label><select id="EditTransport" >' + TransportSelectTag + '</select ></div>' +
                        '<div><label> 配送狀態:</label><span style="color:red;" id="StatusSelect">' + TransportStatusSelectTag + '</span></div>' +
                        "<div id='Editbutton'><input id='EditConfirm' type='Button' value='確認編輯' />" +
                        "<input name='EditCancel' id = 'EditCancel' type = 'Button' value = '取消編輯' /></div > ";

                    $('#Editform').html(EditData);
                    $("#EditBox").show();

                    $('#EditTransport').change(function () {
                        var SelectTag = '';
                        SelectTag = OrderMenuFun.MakeTransportStatusSelect($('#EditTransport').val(), '');

                        if (SelectTag !== "") {
                            $('#StatusSelect').html(SelectTag);

                        } else {
                            SelectTag = '無設定配送狀態請新增'
                            $('#StatusSelect').html(SelectTag);
                        }
                    });

                    //確認編輯
                    $('#EditConfirm').click(function () {
                        var ErrorCode = '';

                        if ($('#EditTransportStatus').length == 0) {
                            ErrorCode = '此配送方式無設定配送狀態，請新增配送狀態';
                        }


                        if (ErrorCode != '') {
                            alert(ErrorCode);
                        } else {
                            $.ajax({
                                url: '/api/Order/UpdateOrder?OrderNum=' + Filter[0].f_orderNum + '&TransportNum=' + $('#EditTransport').val() + '&TransportStatusNum=' + $('#EditTransportStatus').val(),
                                type: "put",
                                contentType: "application/json",
                                dataType: "text",
                                data: {},
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
            });


            //退貨
            $('.ReturnBtn').click(function () {
                if (window.confirm("此訂單確定要退貨嗎?")) {
                    var currentRow = $(this).closest("tr");
                    var OrderId = currentRow.find("td:eq(0)").attr('id');
                    var OrdernNum = currentRow.find("td:eq(0)").text();

                    //篩選所點選訂單id資料
                    var Filter = OrderTable.filter(function (item) {
                        if (item["f_id"] == OrderId) {
                            return item
                        }
                    })
                    var ErrorCode = '';

                    if (Filter[0].f_orderStatus != 2) {
                        ErrorCode = '此訂單狀態不為待退貨，無法退貨';
                    }



                    if (ErrorCode != '') {
                        alert(ErrorCode);
                    } else {
                        $.ajax({
                            url: '/api/Order/OrderReturn?OrderNum=' + OrdernNum,
                            type: "put",
                            contentType: "application/json",
                            dataType: "text",
                            data: {},
                            success: function (result) {

                                var JsonResult = JSON.parse(result)//JSON字串轉物件

                                switch (JsonResult[0].st) {
                                    case 0: {
                                        alert('此訂單已完成退貨');
                                        location.reload(); //新增成功才更新頁面
                                        break;
                                    }
                                    case 100: {
                                        alert('尚未建立此訂單');
                                        break;
                                    }
                                    case 101: {
                                        alert('此訂單狀態是不是待退貨狀態');
                                        break;
                                    }
                                    case 1: {
                                        alert('更新失敗,請檢查LOG');
                                        break;
                                    }
                                    default: {
                                        alert('資料庫更新失敗');
                                    }
                                }
                            },
                            error: function (error) {
                                alert(error);
                            }
                        })
                    }
                }
            })

            //取消訂單
            $('.CancelOrder').click(function () {
                if (window.confirm("此訂單確定要取消嗎?")) {
                    var currentRow = $(this).closest("tr");
                    var OrderId = currentRow.find("td:eq(0)").attr('id');
                    var OrdernNum = currentRow.find("td:eq(0)").text();

                    //篩選所點選訂單id資料
                    var Filter = OrderTable.filter(function (item) {
                        if (item["f_id"] == OrderId) {
                            return item
                        }
                    })
                    var ErrorCode = '';

                    if (Filter[0].f_orderStatus != 0) {
                        ErrorCode = '此訂單狀態不為未取貨，無法取消';
                    }



                    if (ErrorCode != '') {
                        alert(ErrorCode);
                    } else {
                        $.ajax({
                            url: '/api/Order/OrderCancel?OrderNum=' + OrdernNum,
                            type: "put",
                            contentType: "application/json",
                            dataType: "text",
                            data: {},
                            success: function (result) {

                                var JsonResult = JSON.parse(result)//JSON字串轉物件

                                switch (JsonResult[0].st) {
                                    case 0: {
                                        alert('此訂單已取消');
                                        location.reload(); //新增成功才更新頁面
                                        break;
                                    }
                                    case 100: {
                                        alert('尚未建立此訂單');
                                        break;
                                    }
                                    case 101: {
                                        alert('此訂單狀態不是未取貨狀態');
                                        break;
                                    }
                                    case 1: {
                                        alert('更新失敗,請檢查LOG');
                                        break;
                                    }
                                    default: {
                                        alert('資料庫更新失敗');
                                    }
                                }
                            },
                            error: function (error) {
                                alert(error);
                            }
                        })
                    }
                }
            })
        },
        failure: function (data) {
            alert(data);
        },
        error: function (data) {
            alert(data);
        }
    });

    //前往新增商品
    $("#AddTransport").click(function () {
        location.href = "/Order/AddTransport"
    });
});

OrderMenuFun = {

    MakeOrderMenuTag: function (MenuJson) {
        var rows = '';
        for (var i in MenuJson) {

            var TempTransport = TransportTable.filter(function (item) {
                if (item['f_transport'].indexOf(MenuJson[i].f_transport) >= 0) {
                    return item
                }
            })
            var TempStatus = TransportStatusTable.filter(function (item) {
                if (item['f_transport'].indexOf(MenuJson[i].f_transport) >= 0 &&
                    item['f_transportStatus'].indexOf(MenuJson[i].f_transportStatus) >= 0) {
                    return item
                }
            })

            var OrderStatus = '';
            var ReturnBtn = '';
            var CancelOrderBtn = '';
            switch (MenuJson[i].f_orderStatus) {
                case '0':
                    OrderStatus = '未取貨';
                    CancelOrderBtn = '<input type="button" class="CancelOrder" name="CancelOrder" value="取消訂單" />'
                    break;
                case '1':
                    OrderStatus = '已取貨';
                    break;
                case '2':
                    OrderStatus = '待退貨';
                    ReturnBtn = '<input type="button" class="ReturnBtn" name="ReturnBtn" value="退貨" />'
                    break;
                case '3':
                    OrderStatus = '已退貨';
                    break;
                case '4':
                    OrderStatus = '訂單取消';
                    
                    break;
                default:
                    alert('訂單狀態碼參數異常');
                    break;
            }


            rows += "<tr>" +
                '<td id="' + MenuJson[i].f_id + '">' + MenuJson[i].f_orderNum + '</td>' +
                '<td name="Acc">' + MenuJson[i].f_acc + '</td>' +
                '<td name="TransportName">' + TempTransport[0].f_transportName + '</td>' +
                '<td name="TransportStatusName">' + TempStatus[0].f_transportStatusName + '</td>' +
                '<td name="OrderStatus">' + OrderStatus + '</td>' +
                '<td name="OrderDate">' + MenuJson[i].f_orderDate + '</td>' +
                '<td align="center"> <input type="button" class="EditTransportBtn" value="編輯配送"/ ></td>' +
                '<td align="center">' + ReturnBtn + ' </td>' +
                '<td align="center">' + CancelOrderBtn + ' </td>' +
                //"<td align='center'> <input type='button' class='CancelOrder'  name='CancelOrder' value='取消訂單'/ ></td>";
                "</tr>";
        }
        return rows
    },
    MakeTransportStatusSelect: function (TransportValue, TransportStatusValue) {
        var TransportStatus = TransportStatusTable.filter(function (item) {
            if (item['f_transport'].indexOf(TransportValue) >= 0) {
                return item;
            }
        })
        var TransportStatusTag = "";

        for (var i in TransportStatus) {


            if (TransportStatus[i].f_transportStatus == TransportStatusValue) {
                TransportStatusTag += '<option selected value="' + TransportStatus[i].f_transportStatus + '">' + TransportStatus[i].f_transportStatusName + '</option >';
            } else {
                TransportStatusTag += '<option value="' + TransportStatus[i].f_transportStatus + '">' + TransportStatus[i].f_transportStatusName + '</option >';
            };

        }
        var TagResult = '';
        if (TransportStatusTag !== '') {
            TagResult = '<select id="EditTransportStatus" >' + TransportStatusTag + '</select >';
        } else {
            TagResult = '';
        }


        return TagResult;

    }
};