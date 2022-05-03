$(document).ready(function () {

    $("#member").click(function () {
        location.href = "/Member/MemberMenu"
    });

    $("#Account").click(function () {
        location.href = "/Account/AccountMenu"
    });

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
