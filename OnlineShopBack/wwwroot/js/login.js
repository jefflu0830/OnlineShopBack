$(document).ready(function (data) {
    $('#LoginPost').click(function () {
        var ErrorCode = ''


        //檢測
        if ($("#Account").val() === "" || $("#PassWord").val() === "") {
            ErrorCode += "[帳號] 或 [密碼] 不可空白\n"
        } else {
            if (/^[a-zA-Z0-9]+$/.test($("#Account").val()) == false ||
                /^[a-zA-Z0-9]+$/.test($("#PassWord").val()) == false) {
                ErrorCode += "[帳號] 或 [密碼] 不允許中英數以外字符。\n"
            }
            if ($("#PassWord").val().length > 16 ||
                $("#PassWord").val().length < 8) {
                ErrorCode += "[密碼]請介於 8 - 16個數之間\n"
            }
            if ($("#Account").val().length > 20) {
                ErrorCode += "[帳號]請介於 3 - 20個數之間\n"
            }
        }

        if (ErrorCode !== '') {
            alert(ErrorCode);
        } else {
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
                    var JsonResult = JSON.parse(result);//JSON字串轉物件

                    switch (JsonResult[0].st) {
                        case 0: {
                            alert('登入成功');
                            location.href = "/index";
                            break;
                        }
                        case 100: {
                            alert('密碼錯誤登入失敗');
                            break;
                        }
                        case 101: {
                            if (window.confirm("有使用者正在連線,已清除連線,要繼續登入嗎?")) {
                                location.href = "/index";
                            } 
                            break;
                        }
                        case 201: {
                            if (window.confirm("後端驗證失敗")) {
                            }
                            break;
                        }
                        case 202: {
                            if (window.confirm("例外錯誤")) {
                            }
                            break;
                        }
                        default:
                            alert(result);
                    }
                },
                error: function (error) {
                    alert(error);
                }
            });
        }
    });

});


