$(document).ready(function (data) {
    $("#post").click(function () {
        $.ajax({
            url: "/api/Login",
            type: "post",
            contentType: "application/json",
            dataType: "text",
            data: JSON.stringify({
                "account": $("#Account").val(),
                "Pwd": $("#PassWord").val()
            }),
            success: function (result) {
                alert(result);
            },
            error: function (error) {
                alert(error);
            }
        });
    });
});