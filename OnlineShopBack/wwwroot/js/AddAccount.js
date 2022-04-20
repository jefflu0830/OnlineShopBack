$(document).ready(function (data) {
    $("#post").click(function () {
        $.ajax({
            url: "/api/Account",
            type: "post",
            contentType: "application/json",
            dataType: "text",
            data: JSON.stringify({
                "account": $("#Account").val(),
                "Pwd": $("#PassWord").val(),
                "level": parseInt($("#level").val() //parseInt轉型成int
                )
            }),
            success: function (result) {
                alert(result)
            },
            error: function (error) {
                alert(error);
            }
        });
    });
});