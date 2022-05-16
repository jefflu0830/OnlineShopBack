$(document).ready(function (data) {

    $("#GoAccMenu").click(function () {
        location.href = "/Account/Accountmenu"
    });

    $.validator.addMethod("stringCheck", function (value, element) {
        return this.optional(element) || /^[a-zA-Z0-9]+$/.test(value);
    }, "只能包含英文、數字等字元");

    $.validator.addMethod("MinMaxLenth", function (value, minValue, MaxValue) {
        if (value < minValue || value > MaxValue) {
            return false
        }       
    }, "只能包含英文、數字等字元");

    $('#form').validate({
        /* 常用檢測屬性
       required:必填
       noSpace:空白
       minlength:最小長度
       maxlength:最大長度
       email:信箱格式
       number:數字格式
       url:網址格式https://www.minwt.com
       */
        rules: {
            Account: {
                required: true,
                stringCheck: true,
            },
            PassWord: {
                required: true,
                stringCheck: true,
            }
        },
        messages: {
            Account: {
                required: '必填',
            },
            PassWord: {
                required: '必填'
            }
        }
    });
});

$.validator.setDefaults({
    /*submitHandler成功提交表單 做什麼事*/
    submitHandler: function (form) {
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
                alert(result)
                if (result == "帳號新增成功") {
                    location.href = "/Account/AccountMenu"
                }
                else {
                    alert(result)
                }  
            },
            error: function (error) {
                alert(error);
            }
        });
    }
});