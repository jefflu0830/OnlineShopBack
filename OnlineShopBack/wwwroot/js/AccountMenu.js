$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/api/account/GetAccount",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            //$("#DIV").html('');
            //var DIV = '';
            for (var i in data) {
                var rows = rows + "<tr>" +
                    "<td id='ID'>" + data[i].f_id + "</td>" +
                    "<td id='RegdNo'>" + data[i].f_acc + "</td>" +
                    "<td id='Name'>" + data[i].f_accPosition + "</td>" +
                    "<td id='phone'>" + data[i].f_createDate + "</td>" +
                    "<td class='DetailBtn' id='" + data[i].f_id + "'><a href=''>編輯</a></td>" +
                    "</tr>";
            }
            $('#Table').append(rows);
        },

        failure: function (data) {
        },
        error: function (data) {
        }

    });
})