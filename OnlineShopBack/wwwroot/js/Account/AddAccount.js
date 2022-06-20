$(document).ready(function (data) {
    AddAccountFun.MakeSelect(accLV, accPosition)

    $('#Addbtn').click(function () {
        var ErrorCode = "";

        if ($("#Account").val() === "" || $("#PassWord").val() === "") {
            ErrorCode += "[帳號] 或 [密碼] 不可空白\n"
        } else {
            if ((/^[a-zA-Z0-9]*$/.test($("#Account").val()) == false) ||
                (/^[a-zA-Z0-9]*$/.test($("#PassWord").val()) == false)) {
                ErrorCode += "[帳號] 或 [密碼] 只允許輸入英文及數字。\n"
            }
            if (($("#Account").val().length > 20) ||
                ($("#Account").val().length < 3)) {
                ErrorCode += "[帳號]請介於3~20個字\n"
            }
            if (($("#PassWord").val().length > 16) ||
                ($("#PassWord").val().length < 8)) {
                ErrorCode += "[密碼]請介於8~16個字\n"
            }

        }

        if (ErrorCode !== "") {
            alert(ErrorCode)
        }
        else {

            $.ajax({
                url: "/api/Account/AddAcc",
                type: "post",
                contentType: "application/json",
                dataType: "text",
                data: JSON.stringify({
                    "account": $("#Account").val(),
                    "Pwd": $("#PassWord").val(),
                    "level": parseInt($("#level").val()) //parseInt轉型成int

                }),
                success: function (result) {
                    var JsonResult = JSON.parse(result)//JSON字串轉物件

                    switch (JsonResult[0].st) {
                        case 0: {
                            alert('新增成功');
                            location.href = "/Account/AccountMenu"
                            break;
                        };
                        case 100: {
                            alert('帳號重複');
                            break;
                        };
                        case 101: {
                            alert('權限不存在');
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
                    }
                },
                error: function (error) {
                    alert(error);
                }
            });
        }
    });
});

AddAccountFun = {
    MakeSelect: function (accLV, accPosition) {
        //組標籤
        for (var i = 0; i < accLV.length - 1; i++) {
            var LvRows = LvRows + "<option value='" + accLV[i] + "'>" + accPosition[i] + "</option>"
        }

        //將組好的標籤append至 #level
        $('#level').html(LvRows);
    }
}