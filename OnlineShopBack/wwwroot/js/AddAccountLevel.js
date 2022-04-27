$(document).ready(function (data) {

    $.validator.addMethod("stringCheck", function (value, element) {
        return this.optional(element) || /^[a-zA-Z0-9\u4e00-\u9fa5]+$/.test(value);
    }, "只能包含中文,英文、數字等字元");

    $.validator.addMethod("JustIntCheck", function (value, element) {
        return this.optional(element) || /^[0-9]+$/.test(value);
    }, "只能為數字 ");

    $("#AccLevel").blur(function () {
        $("#AccLevel").val(parseInt($("#AccLevel").val()))
    })

 

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
            AccLevel: {
                required: true,
                JustIntCheck: true,
                range: [0, 255]
            },
            AccPosission: {
                required: true,
                stringCheck: true,
            },

        },
        messages: {
            AccLevel: {
                required: '必填',
                range:'範圍須在0-255'
            },
            AccPosission: {
                required: '必填'
            }
        }
    });
});

$.validator.setDefaults({
    /*submitHandler成功提交表單 做什麼事*/
    submitHandler: function (form) {
        $.ajax({
            url: "/api/Account/AddAccountLevel",
            type: "post",
            contentType: "application/json",
            dataType: "text",
            data: JSON.stringify({
                "accLevel": parseInt($("#AccLevel").val()) ,
                "accPosition": $("#AccPosission").val(),
                "canUseAccount": parseInt($("#canUseAccount").val()), //parseInt轉型成int
                "canUseMember": parseInt($("#canUseAccount").val()), //parseInt轉型成int                
            }),
            success: function (result) {
                alert(result)
            },
            error: function (error) {
                alert(error);
            }
        });
    }
});