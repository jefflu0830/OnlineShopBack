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
                url: "/api/member/EditShopGold",
                type: "put",
                contentType: "application/json",
                dataType: "text",
                data: JSON.stringify({
                    "MemAcc": $("#CfmAcc").html(),
                    "NowAmount": parseInt($("#NowAmount").html()),
                    "AdjustAmount": parseInt($("#AdjustAmount").val())
                }),
                success: function (result) {
                    var JsonResult = JSON.parse(result);//JSON字串轉物件

                    switch (JsonResult[0].st) {
                        case 0: {
                            alert('增加成功');
                            location.reload();
                            break;
                        };
                        case 100: {
                            alert('無此會員');
                            break;
                        };
                        case 101: {
                            alert('原始購物金與帳號不相符');
                            break;
                        };
                        case 102: {
                            alert('調整後購物金不得小於0 or 大於20000');
                            break;
                        };
                        case 200: {
                            alert('後端驗證失敗,請查詢LOG');
                            break;
                        };
                        case 201: {
                            alert('例外錯誤,請查詢LOG');
                            location.reload();
                            break;
                        }
                        default: {
                            alert(result);
                        }
                    };

                    if (result === "已從另一地點登入,轉跳至登入頁面") {
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