$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/api/Member",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            //$("#DIV").html('');
            //var DIV = '';
            for (var i in data) {
                var rows = rows + "<tr>" +
                    "<td id='RegdNo'>" + data[i].f_id + "</td>" +
                    "<td id='RegdNo'>" + data[i].f_acc + "</td>" +
                    "<td id='Name'>" + data[i].f_name + "</td>" +
                    "<td id='phone'>" + data[i].f_phone + "</td>" +
                    "<td id='Address'>" + data[i].f_mail + "</td>" +
                    "<td id='Address'>" + data[i].f_address + "</td>" +
                    "<td id='Address'>" + data[i].f_shopGold + "</td>" +
                    "<td id='Address'>" + data[i].f_level + "</td>" +
                    "<td id='Address'>" + data[i].f_suspension + "</td>" +
                    "<td id='PhoneNo'>" + data[i].f_createDate + "</td>" +
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