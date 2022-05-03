$(document).ready(function () {
    $("#logOut").click(function () {
        $.ajax({
            url: "api/Login/Logout",
            type: "Delete",
            contentType: "application/json",
            dataType: "text",
            data: JSON.stringify({
                "account": $("#Account").val(),
                "Pwd": $("#PassWord").val()
            }),
            success: function (result) {
                location.href = "/Index"
            },
            error: function (error) {
            }
        })
    });
});