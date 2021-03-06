AddAccountLevelMune = {
    AccountLevel: {}
}

$(document).ready(function (data) {
    $("#GoAccMenu").click(function () {
        location.href = "/Account/Accountmenu"
    });
    $("#GoIndex").click(function () {
        location.href = "/index"
    });

    //取得權限資料
    $.ajax({
        type: "GET",
        url: "/api/account/GetAccLvList",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            for (var i in data) {
                if (data[i].accLevel == 0) {//最高權限無法更改&刪除
                    var rows = rows + "<tr>" +
                        "<td name='faccLevel'>" + data[i].accLevel + "</td>" +
                        "<td name='faccPosition'>" + data[i].accPosition + "</td>" +
                        "<td align='center'></td>" +
                        "<td align='center'></td>" +
                        "</tr>";
                } else {
                    var rows = rows + "<tr>" +
                        "<td name='faccLevel'>" + data[i].accLevel + "</td>" +
                        "<td name='faccPosition'>" + data[i].accPosition + "</td>" +
                        "<td align='center'> <input type='button'   class='EditBtn'  onclick = 'AddAccountLevel.Edit_Click(" + data[i].accLevel + ")' value='編輯'/ ></td>" +
                        "<td align='center'> <input type='button'   class='DeleteBtn'  onclick = 'AddAccountLevel.Del_Click(" + data[i].accLevel + ")' value='刪除'/ ></td>" +
                        "</tr>";
                }
            }
            $('#Table').append(rows);
        },

        failure: function (data) {
        },
        error: function (data) {
        }
    });


    //自訂檢測
    $.validator.addMethod("stringCheck", function (value, element) {
        return this.optional(element) || /^[a-zA-Z0-9\u4e00-\u9fa5]+$/.test(value);
    }, "只能包含中文,英文、數字等字元");

    $.validator.addMethod("JustIntCheck", function (value, element) {
        return this.optional(element) || /^[0-9]+$/.test(value);
    }, "只能為數字 ");

    $("#AccLevel").blur(function () {
        $("#AccLevel").val(parseInt($("#AccLevel").val()))
    })

    //validate 設定
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
        var canUseProduct = 0
        var canUseOrder = 0
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
        //canProduct  checkbox
        if ($("#canUseProduct").prop("checked")) {
            canUseProduct = 1
        }
        else {
            canUseProduct = 0
        }
        //canUseOrder  checkbox
        if ($("#canUseOrder").prop("checked")) {
            canUseOrder = 1
        }
        else {
            canUseOrder = 0
        }


        $.ajax({
            url: "/api/Account/AddAccLv",
            type: "post",
            contentType: "application/json",
            dataType: "text",
            data: JSON.stringify({
                "accLevel": parseInt($("#AccLevel").val()),
                "accPosition": $("#AccPosission").val(),
                "canUseAccount": canUseAccount,
                "canUseMember": canUseMember,
                "canUseProduct": canUseProduct,
                "canUseOrder": canUseOrder
            }),
            success: function (result) {
                var JsonResult = JSON.parse(result)//JSON字串轉物件

                switch (JsonResult[0].st) {
                    case 0: {
                        alert('新增成功');
                        location.reload(); //新增成功才更新頁面
                        break;
                    };
                    case 100: {
                        alert('權限重複');
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
        })
    }
});


var AddAccountLevel = {
    //刪除
    Del_Click: function (Id) {
        if (window.confirm("確定要刪除此權限嗎?")) {
            $.ajax({
                url: "/api/Account/DelAccLv?id=" + Id,
                type: "DELETE",
                data: {},
                success: function (result) {
                    var JsonResult = JSON.parse(result)//JSON字串轉物件

                    switch (JsonResult[0].st) {
                        case 0: {
                            alert('刪除成功');
                            location.reload(); //新增成功才更新頁面
                            break;
                        };
                        case 100: {
                            alert('此權限有人正在使用');
                            break;
                        };
                        case 101: {
                            alert('此權限尚未建立');
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
            })
        }
    },
    //點擊編輯按鈕
    Edit_Click: function (Id) {
        if ($("#EditBox").css("display") == "none") {
            $.ajax({
                type: "GET",
                url: "/api/account/GetAccLVById?id=" + Id,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    var canUseAccountChk = ""
                    var canUseMember = ""
                    var canUseProduct = ""
                    if (data[0].canUseAccount === 1) {
                        canUseAccountChk = "checked"
                    }
                    if (data[0].canUseMember === 1) {
                        canUseMember = "checked"
                    }
                    if (data[0].canUseProduct === 1) {
                        canUseProduct = "checked"
                    }
                    if (data[0].canUseOrder === 1) {
                        canUseOrder = "checked"
                    }
                    var EditData = "<div><label> 帳號:" + data[0].accLevel + "</label></div>" +
                        //f_accPosition
                        "<div><label for='EditAccPosission'>AccPosission:</label>" +
                        "<input type='text' id='EditAccPosission' name='AccPosission' maxlength='10' value='" + data[0].accPosition + "'/></div>" +
                        //canUseAccount
                        "<div><input type='checkbox' id='EditCanUseAccount'" + canUseAccountChk + " />" +
                        " <label for='EditCanUseAccount'>是否有後台帳號管理權限</label></div >" +
                        //canUseMember
                        "<div><input type='checkbox' id='EditCanUseMember'" + canUseMember + " />" +
                        "<label for='EditCanUseMember'>是否有會員管理權限</label></div >" +
                        //canUseProduct
                        "<div><input type='checkbox' id='EditCanUseProduct'" + canUseProduct + " />" +
                        "<label for='EditCanUseProduct'>是否有商品管理權限</label></div >" +
                        //canUseOrder
                        "<div><input type='checkbox' id='EditCanUseOrder'" + canUseOrder + " />" +
                        "<label for='EditCanUseOrder'>是否有訂單管理權限</label></div >" +
                        //Edit Button
                        "<div><input id='" + data[0].accLevel + "' onclick ='AddAccountLevel.EditOK_Click(this.id)' type='Button' value='Edit' />" +
                        "<input id='EditCancel' type='Button' value='Cancel' onclick='AddAccountLevel.EditCancel_Click()' /></div> "

                    $('#Editform').append(EditData);
                },
                failure: function (data) {
                },
                error: function (data) {
                }
            });
            $("#EditBox").show();
        }
    },
    //取消編輯
    EditCancel_Click: function () {
        if ($("#EditBox").css("display") !== "none") {
            $("#EditBox").hide();
            $("#Editform > div").remove();
        }
    },
    //確認編輯
    EditOK_Click: function (Id) {
        var EditcanUseAccount = 0
        var EditcanUseMember = 0
        var EditCanUseProduct = 0
        var EditCanUseOrder = 0
        //canUseMember  checkbox
        if ($("#EditCanUseAccount").prop("checked")) {
            EditcanUseAccount = 1
        }
        else {
            EditcanUseAccount = 0
        }
        //canUseMember  checkbox
        if ($("#EditCanUseMember").prop("checked")) {
            EditcanUseMember = 1
        } else {
            EditcanUseMember = 0
        }
        //canUseProduct  checkbox
        if ($("#EditCanUseProduct").prop("checked")) {
            EditCanUseProduct = 1
        } else {
            EditCanUseProduct = 0
        }
        //canUseOrder  checkbox
        if ($("#EditCanUseOrder").prop("checked")) {
            EditCanUseOrder = 1
        }
        else {
            EditCanUseOrder = 0
        }

        $.ajax({
            url: "/api/account/EditAccLv?id=" + Id,
            type: "put",
            contentType: "application/json",
            dataType: "text",
            data: JSON.stringify({
                "accPosition": $("#EditAccPosission").val(),
                "CanUseAccount": EditcanUseAccount,
                "CanUseMember": EditcanUseMember,
                "CanUseProduct": EditCanUseProduct,
                "canUseOrder": EditCanUseOrder
            }),
            success: function (result) {
                var JsonResult = JSON.parse(result)//JSON字串轉物件

                switch (JsonResult[0].st) {
                    case 0: {
                        alert('權限更新成功');
                        location.reload(); //新增成功才更新頁面
                        break;
                    };
                    case 100: {
                        alert('預設帳號不可更改');
                        break;
                    };
                    case 101: {
                        alert('尚未建立權限');
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
        })
    }
}






