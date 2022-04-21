$(document).ready(function (data) {
    $("#post").click(function () {
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
                    required: true
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
            },
            /*submitHandler成功提交表單 做什麼事*/
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
                        alert(result);
                    },
                    error: function (error) {
                        alert(error);
                    }
                });
            }
        });
    });
});
