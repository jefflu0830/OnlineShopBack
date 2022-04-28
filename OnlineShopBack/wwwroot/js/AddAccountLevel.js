function Del_Click(DelBtnId) {
    if (window.confirm("確定要刪除嗎")) {
        $.ajax({
            url: "/api/Account/DelAccountLevel/" + DelBtnId,
            type: "DELETE",
            data: {},
            success: function (result) {
                alert(result)

                if (result == "刪除成功") {
                    location.reload(); //刪除成功才更新頁面
                }
            },
            error: function (error) {
                alert(error);
            }
        })
    }
}

function Edit_Click(Editid) {
    if ($("#EditBox").css("display") == "none") {
        $.ajax({
            type: "GET",
            url: "/api/account/GetAccountLV/" + Editid,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                var canUseAccountChk = ""
                var canUseMember=""
                if (data[0].f_canUseAccount === "True") {
                    canUseAccountChk="checked"
                }
                if (data[0].f_canUseMember === "True") {
                    canUseMember = "checked"
                }

                var rows = "<div><label> AccLevel:" + data[0].f_accLevel + "</label></div>" +
                    //f_accPosition
                    "<div><label for='AccPosission'>AccPosission:</label>" +
                    "<input type='text' id='AccPosission' name='AccPosission' maxlength='10' value='" + data[0].f_accPosition + "'/></div>" +
                    //canUseAccount
                    "<div><input type='checkbox' id='canUseAccount'" + canUseAccountChk + " />" +
                    " <label for='canUseAccount'>是否有後台帳號管理權限</label></div >" +
                    //canUseMember
                    "<div><input type='checkbox' id='canUseMember'" + canUseMember + " />" +
                    "<label for='canUseMember'>是否有會員管理權限</label></div >" +
                    //Edit Button
                    "<div><input id='EditAccLV' type='submit' value='Edit' /></div>"

                $('#Editform').append(rows);
            },
            failure: function (data) {
            },
            error: function (data) {
            }
        });
        $("#EditBox").show();
    } else {
        $("#EditBox").hide();
        $("#Editform > div").remove();
    }
}

$(document).ready(function (data) {
    $.ajax({
        type: "GET",
        url: "/api/account/GetAccountLV",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            for (var i in data) {

                var rows = rows + "<tr>" +
                    "<td name='faccLevel'>" + data[i].f_accLevel + "</td>" +
                    "<td name='faccPosition'>" + data[i].f_accPosition + "</td>" +
                    "<td name='fcanUseAccount'>" + data[i].f_canUseAccount + "</td>" +
                    "<td name='fcanUseMember'>" + data[i].f_canUseMember + "</td>" +
                    "<td align='center'> <input type='button'   class='EditBtn'   id = '" + data[i].f_accLevel + "' onclick = 'Edit_Click(this.id)' value='編輯'/ ></Button></td>" +
                    "<td align='center'> <input type='button'   class='DeleteBtn' id = '" + data[i].f_accLevel + "' onclick = 'Del_Click(this.id)' value='刪除'/ ></Button></td>" +
                    "</tr>";
            }
            $('#Table').append(rows);
        },

        failure: function (data) {
        },
        error: function (data) {
        }

    });



    $.validator.addMethod("stringCheck", function (value, element) {
        return this.optional(element) || /^[a-zA-Z0-9\u4e00-\u9fa5]+$/.test(value);
    }, "只能包含中文,英文、數字等字元");

    $.validator.addMethod("JustIntCheck", function (value, element) {
        return this.optional(element) || /^[0-9]+$/.test(value);
    }, "只能為數字 ");

    $("#AccLevel").blur(function () {
        $("#AccLevel").val(parseInt($("#AccLevel").val()))
    })



    $('#Addform').validate({
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
                range: '範圍須在0-255'
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
        var canUseAccount = 0
        var canUseMember = 0
        //canUseMember  checkbox
        if ($("#canUseAccount").prop("checked")) {
            canUseAccount = 1
        }
        else {
            canUseAccount = 0
        }
        //canUseMember  checkbox
        if ($("#canUseMember").prop("checked")) {
            canUseMember = 1
        }
        else {
            canUseMember = 0
        }
        $.ajax({
            url: "/api/Account/AddAccountLevel",
            type: "post",
            contentType: "application/json",
            dataType: "text",
            data: JSON.stringify({
                "accLevel": parseInt($("#AccLevel").val()),
                "accPosition": $("#AccPosission").val(),
                "canUseAccount": canUseAccount, 
                "canUseMember": canUseMember   
                //"canUseAccount": parseInt($("#canUseAccount").val()), //parseInt轉型成int
                //"canUseMember": parseInt($("#canUseMember").val()), //parseInt轉型成int                
            }),
            success: function (result) {
                alert(result)

                if (result == "權限新增成功") {
                    location.reload(); //新增成功才更新頁面
                }
            },
            error: function (error) {
                alert(error);
            }
        })


    }
});