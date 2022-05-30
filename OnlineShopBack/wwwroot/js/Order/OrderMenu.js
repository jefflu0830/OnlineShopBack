var ProductMenu = {
    OrderTable: [],
    TransportTable: [],
    TransportStatusTable: []
}

$(document).ready(function () {

    //取得會員列表
    $.ajax({
        type: "GET",
        url: "/api/Order/GetProduct",
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
                        '<div><label> 配送狀態:</label><select id="EditTransportStatus" >' + TransportStatusSelectTag + '</select ></div>' +
                        "<div id='Editbutton'><input id='EditConfirm' type='Button' value='確認編輯' />" +
                        "<input name='EditCancel' id = 'EditCancel' type = 'Button' value = '取消編輯' /></div > ";

                    $('#Editform').html(EditData);
                    $("#EditBox").show();

                    $('#EditTransport').change(function () {
                        var SelectTag = OrderMenuFun.MakeTransportStatusSelect($('#EditTransport').val() , '');
                        $('#EditTransportStatus').html(SelectTag);
                    });

                    //確認編輯
                    $('#EditConfirm').click(function () {
                        alert('配送方式確認');
                    });
                    //取消編輯
                    $('#EditCancel').click(function () {
                        if ($("#EditBox").css("display") !== "none") {
                            $("#EditBox").hide();
                        };
                    });

                };
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

            rows += "<tr>" +
                '<td id="' + MenuJson[i].f_id + '">' + MenuJson[i].f_orderNum + '</td>' +
                '<td name="Acc">' + MenuJson[i].f_acc + '</td>' +
                '<td name="TransportName">' + TempTransport[0].f_transportName + '</td>' +
                '<td name="TransportStatusName">' + TempStatus[0].f_transportStatusName + '</td>' +
                '<td name="OrderDate">' + MenuJson[i].f_orderDate + '</td>' +
                '<td align="center"> <input type="button" class="EditTransportBtn" value="編輯配送"/ ></td>' +
                "<td align='center'> <input type='button' class='DeleteBtn'  name='ReturnBtn' value='退貨'/ ></td>";
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
        return TransportStatusTag;

    }
};