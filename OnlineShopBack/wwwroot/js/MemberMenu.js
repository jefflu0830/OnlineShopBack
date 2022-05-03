$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/api/member/getmember",             
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
                    "<td id='mail'>" + data[i].f_mail + "</td>" +
                    "<td id='address'>" + data[i].f_address + "</td>" +
                    "<td id='shopGold'>" + data[i].f_shopGold + "</td>" +
                    "<td id='level'>" + data[i].f_level + "</td>" +
                    "<td id='suspension'>" + data[i].f_suspension + "</td>" +
                    "<td id='createDate'>" + data[i].f_createDate + "</td>" +
                    "<td align='center'> <input type='button' class='EditBtn'  name='EditBtn'   id = '" + data[i].f_id + "'  value='編輯'/ ></td>" +
                    "<td align='center'> <input type='button' class='DeleteBtn'  name='DeleteBtn' id = '" + data[i].f_id + "'  value='刪除'/ ></td>" +

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