$(document).ready(function (data) {
    jQuery.validator.addMethod("stringCheck", function (value, element) {
        return this.optional(element) || /^[a-zA-Z0-9]+$/.test(value);
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
                stringCheck: true
            },
            PassWord: {
                required: true
            }
        },
        messages: {
            Account: {
                required: '必填'
            },
            PassWord: {
                required: '必填'
            }
        }
    });
});

$.validator.setDefaults({
    submitHandler: function (form) {
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
                if (result == "loginOK") {
                    location.href = "/index"
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
