$(document).ready(function (data) {
    $("#CheckAccBtn").click(function () {
        var CheckAccError = "";

        if ($("#EditBox").css("display") !== "none") {
            $("#EditBox").hide();
            $("#AdjustAmount").val("")
        }

        if ($("#CheckAcc").val().length < 3) {
            CheckAccError += "[確認帳號] 請大於3個字。\n"
        }
        if (/^[a-zA-Z0-9]*$/.test($("#CheckAcc").val()) == false) {
            CheckAccError += "[確認帳號] 只允許輸入英文及數字。\n"
        }
        if (CheckAccError !== "") {

            alert(CheckAccError);

        }
        else {


            //取得帳號列表
            $.ajax({
                type: "GET",
                url: "/api/Member/GetMemberByAcc?acc=" + $("#CheckAcc").val(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {

                    $("#CfmAcc").html(data[0].f_acc)
                    $("#CfmName").html(data[0].f_name)
                    $("#CfmEmail").html(data[0].f_mail)
                    $("#NowAmount").html(data[0].f_shopGold)
                    $("#EditBox").show();

                },

                failure: function (data) {
                },
                error: function (data) {   
                    $("#CfmAcc").html("")
                    $("#CfmName").html("")
                    $("#CfmEmail").html("")
                    $("#NowAmount").html("")
                    $("#AdjustAmount").val("")
                    alert("查無此會員,請重新輸入")
                }
            });
        }
    });

    $("#AdjustAmountBtn").click(function () {
        if (window.confirm("要調整的金額為 " + $("#AdjustAmount").val() + ",確定要調整嗎?")) {
            $.ajax({
                url: "/api/member/PutShopGold",
                type: "put",
                contentType: "application/json",
                dataType: "text",
                data: JSON.stringify({
                    "MemAcc": $("#CfmAcc").html(),
                    "NowAmount": parseInt($("#NowAmount").html()),
                    "AdjustAmount": parseInt($("#AdjustAmount").val())
                }),
                success: function (result) {
                    alert(result)

                    if (result == "更新成功") {
                        location.reload(); //新增成功才更新頁面
                    } else if (result === "已從另一地點登入,轉跳至登入頁面") {
                        location.reload();
                    }
                },
                error: function (error) {
                    alert(error);
                }
            })
        }
    })
});