$(document).ready(function () {
    
    var result = OrderMenuFun.MakeOrderMenuTag(TransportJson);
    $('#TableBody').html(result)

    $('#AddTransport').click(function () {
        var Transport = $('#Transport').val();
        var TransportName = $('#TransportName').val();
        alert(Transport + '-----' + TransportName);
    })

});


OrderMenuFun = {
    MakeOrderMenuTag: function (MenuJson) {
        var rows = '';
        for (var i in MenuJson) {

            rows += "<tr>" +
                '<td >' + MenuJson[i].f_transport + '</td>' +
                '<td >' + MenuJson[i].f_transportName + '</td>' +
                '<td align="center"> <input type="button" class="EditTransportBtn" value="編輯名稱"/ ></td>' +
                "<td align='center'> <input type='button' class='DeleteBtn'  name='ReturnBtn' value='刪除'/ ></td>";
            "</tr>";
        }
        return rows
    }
}