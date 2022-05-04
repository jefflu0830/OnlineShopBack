$(document).ready(function () {

    $("#logOut").click(function () {
        if (window.confirm("確定登出嗎?")) {
            $.ajax({
                url: "/api/Login/Logout",
                type: "DELETE",
                data: JSON.stringify({}),
                success: function (result) {
                    location.href = "/Index"
                },
                error: function (error) {
                }
            })
        }
    });

});