var ProductMenu = {
    OrderTable: [],
    TransportTable: "",
    TransportStatusTable: ""
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

            $('#Table').append(Result);
        },

        failure: function (data) {
            alert(data);
        },
        error: function (data) {
            alert(data);
        }
    });

    //編輯配送方式
    $("#TableBody").on('click', '.EditTransportBtn', function () {
        var currentRow = $(this).closest("tr");
        var OrderId = currentRow.find("td:eq(0)").attr('id');
        var TransportSelectTag = "";

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
        }
        //組html Tag
        if ($("#EditBox").css("display") == "none") {

            var EditData =
                '<h5>配送方式編輯</h5>' +
                '<div><label> 訂單編號:</label><label>' + Filter[0].f_orderNum + '</label></div>' +
                '<div><label> 訂購帳號:</label><label>' + Filter[0].f_acc + '</label></div>' +
                '<div><label> 配送方式:</label><select >' + TransportSelectTag + '</select ></div>' +
                "<div id='Editbutton'><input id='EditConfirm' type='Button' value='確認編輯' />" +
                "<input name='EditCancel' id = 'EditCancel' type = 'Button' value = '取消編輯' /></div > ";

            $('#Editform').html(EditData);
            $("#EditBox").show();

            //確認編輯
            $('#EditConfirm').click(function () {
                alert('配送方式確認');
            });
            //取消編輯
            $('#EditCancel').click(function () {
                if ($("#EditBox").css("display") !== "none") {
                    $("#EditBox").hide();
                }
            })
        
        }
    })

    $("#TableBody").on('click', '.EditTransportStatusBtn', function () {
        var currentRow = $(this).closest("tr");
        var OrderId = currentRow.find("td:eq(0)").attr('id');
        var TransportStatusSelectTag = "";

        //篩選所點選訂單id資料
        var Filter = OrderTable.filter(function (item) {
            if (item["f_id"] == OrderId) {
                return item
            }
        })

        //組配送狀態下拉選單
        for (var i in TransportStatusTable) {
            if (TransportStatusTable[i].f_transportStatus == Filter[0].f_transportStatus) {
                TransportStatusSelectTag += '<option selected value="' + TransportStatusTable[i].f_transportStatus + '">' + TransportStatusTable[i].f_transportStatusName + '</option >';
            } else {
                TransportStatusSelectTag += '<option value="' + TransportStatusTable[i].f_transportStatus + '">' + TransportStatusTable[i].f_transportStatusName + '</option >';
            };
        }
        //組html Tag
        if ($("#EditBox").css("display") == "none") {

            var EditData =
                '<h5>配送狀態編輯</h5>' +
                '<div><label> 訂單編號:</label><label>' + Filter[0].f_orderNum + '</label></div>' +
                '<div><label> 訂購帳號:</label><label>' + Filter[0].f_acc + '</label></div>' +
                '<div><label> 配送方式:</label><select >' + TransportStatusSelectTag + '</select ></div>' +
                "<div id='Editbutton'><input id='EditConfirm' type='Button' value='確認編輯' />" +
                "<input name='EditCancel' id = 'EditCancel' type = 'Button' value = '取消編輯' /></div > ";


            $('#Editform').html(EditData);
            $("#EditBox").show();

            //確認編輯
            $('#EditConfirm').click(function () {
                alert('配送狀態確認');
            });
            //取消編輯
            $('#EditCancel').click(function () {
                if ($("#EditBox").css("display") !== "none") {
                    $("#EditBox").hide();
                }
            })

        }
    })
});

OrderMenuFun = {

    MakeOrderMenuTag: function (MenuJson) {
        var rows = '';
        for (var i in MenuJson) {
            rows += "<tr>" +
                '<td id="' + MenuJson[i].f_id + '">' + MenuJson[i].f_orderNum + '</td>' +
                '<td name="Acc">' + MenuJson[i].f_acc + '</td>' +                
                '<td name="TransportName">' + MenuJson[i].f_transportName + '</td>' +
                '<td name="TransportStatusName">' + MenuJson[i].f_transportStatusName + '</td>' +
                '<td name="OrderDate">' + MenuJson[i].f_orderDate + '</td>' +
                "<td align='center'> <input type='button' class='EditTransportBtn'  name='EditTransportBtn01'  value='編輯配送方式'/ ></td>" +
                "<td align='center'> <input type='button' class='EditTransportStatusBtn'  name='EditTransportStatusBtn'  value='編輯配送狀態'/ ></td>" +
                "<td align='center'> <input type='button' class='DeleteBtn'  name='ReturnBtn' value='退貨'/ ></td>";
            "</tr>";
        }
        return rows
    }

};