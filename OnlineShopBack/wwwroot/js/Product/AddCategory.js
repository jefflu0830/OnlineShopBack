$(document).ready(function () {
    //取得類型列表
    $.ajax({
        type: "GET",
        url: "/api/Product/GetCategory",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            CategoryJson = data;
            for (var i in data) {

                var categoryName = AddCategoryFun.TransCategoryNum(data[i].CategoryNum);

                var rows = rows + "<tr>" +
                    "<td name='" + data[i].CategoryNum + "'>" + categoryName + "</td>" +
                    "<td name='SubCategoryNum'>" + data[i].SubCategoryNum + "</td>" +
                    "<td name='SubCategoryName'>" + data[i].SubCategoryName + "</td>" +
                    "<td align='center'> <input type='button' class='EditBtn'  name='EditBtn'  value='編輯類型名稱'/ ></td>" +
                    "<td align='center'> <input type='button' class='DeleteBtn'  name='DeleteBtn' value='刪除'/ ></td>" +
                    "</tr>";
            }
            $('#TableBody').append(rows);

            //點擊編輯名稱按鈕
            //$("#TableBody").on('click', '.EditBtn', function () {
            $(".EditBtn").click(function () {
                var currentRow = $(this).closest("tr");

                var Num = currentRow.find("td:eq(0)").attr('name');
                var SubNum = currentRow.find("td:eq(1)").text();
                var SubName = currentRow.find("td:eq(2)").text();
                var categoryName = "";

                if ($("#EditBox").css("display") == "none") {

                    categoryName = AddCategoryFun.TransCategoryNum(Num);

                    var EditData =
                        "<h5>名稱編輯</h5>" +
                        "<div><label> 主類別:</label>    <label>" + categoryName + "</label>    </div>" +
                        "<div><label> 子類別編號:</label>    <label>" + SubNum + "</label>    </div>" +
                        //子類別名稱
                        "<div><label for='EditCategroyName'>子類別名稱:</label>" +
                        "<input type='text' id='EditCategroyName' name='EditCategroyName' maxlength='20' value='" + SubName + "'/></div>" +
                        "<div id='Editbutton'><input id='EditConfirmEdit' type='Button' value='確認編輯' />" +
                        "<input name='EditCancel' id = 'EditCancel' type = 'Button' value = '取消編輯' /></div > ";
                    $('#Editform').html(EditData);
                    $("#EditBox").show();
                };

                //確認編輯
                $('#EditConfirmEdit ').click(function () {
                    $.ajax({
                        url: "/api/Product/UpdateCategory?Num=" + Num + "&SubNum=" + SubNum,
                        type: "put",
                        contentType: "application/json",
                        dataType: "text",
                        data: JSON.stringify({
                            "SubCategoryName": $("#EditCategroyName").val()
                        }),
                        success: function (result) {

                            var JsonResult = JSON.parse(result)//JSON字串轉物件
                            switch (JsonResult[0].st) {
                                case 0: {
                                    alert('更新成功');
                                    location.reload(); //新增成功才更新頁面
                                    break;
                                }
                                case 100: {
                                    alert('尚未建立此類別');
                                    break;
                                }
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

                            if (result == "更新成功") {
                                location.reload(); //新增成功才更新頁面
                            }
                        },
                        error: function (error) {
                            alert(error);
                        }
                    })
                });

                //取消編輯
                $('#EditCancel ').click(function () {
                    if ($("#EditBox").css("display") !== "none") {
                        $("#EditBox").hide();
                    }
                });

            });
            //刪除按鈕
            $(".DeleteBtn").click(function () {
                var currentRow = $(this).closest("tr");
                var Num = currentRow.find("td:eq(0)").attr('name');
                var SubNum = currentRow.find("td:eq(1)").text();

                if (window.confirm("確定要刪除此類別嗎?")) {
                    $.ajax({
                        url: "/api/Product/DelCategory?Num=" + Num + "&SubNum=" + SubNum,
                        type: "DELETE",
                        data: {},
                        success: function (result) {
                            var JsonResult = JSON.parse(result)//JSON字串轉物件
                            switch (JsonResult[0].st) {
                                case 0: {
                                    alert('刪除成功');
                                    location.reload(); //新增成功才更新頁面
                                    break;
                                }
                                case 100: {
                                    alert('無此帳號');
                                    break;
                                }
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

                            if (result === "已從另一地點登入,轉跳至登入頁面") {
                                location.reload();
                            }
                        },
                        error: function (error) {
                            alert(error);
                        }
                    })
                }
            });
        },

        failure: function (data) {
        },
        error: function (data) {
        }
    });

    //新增按鈕
    $("#AddButton").click(function () {
        var ErrorCode = "";

        //輸入驗證
        if ($("#SubCategoryNum").val() === "" || $("#SubCategoryName").val() === "") {
            ErrorCode += "[商品子類別編號] 或 [商品子類別名稱] 不可空白\n"
        } else {
            if (parseInt($("#SubCategoryNum").val()) < 0 || parseInt($("#SubCategoryNum").val()) > 999) {
                ErrorCode += "[商品子類別編號]請輸入0～999之間。\n"
            }
            if (/^[0-9]*$/.test($("#SubCategoryNum").val()) == false) {
                ErrorCode += "[商品子類別編號] 只允許輸入數字。\n"
            }
            if (/^[a-zA-Z0-9\u4e00-\u9fa5]+$/.test($("#SubCategoryName").val()) == false) {
                ErrorCode += "[商品子類別名稱] 不允許中英數以外字符。\n"
            }
            if ($("#SubCategoryName").val().length > 20) {
                ErrorCode += "[商品子類別名稱]請小於20個字\n"
            }
        }

        if (ErrorCode !== "") {
            alert(ErrorCode)
        }
        else {
            $.ajax({
                type: "post",
                url: "/api/Product/AddCategory",
                contentType: "application/json",
                dataType: "text",
                data: JSON.stringify({
                    "CategoryNum": parseInt($("#CategoryNum").val()),
                    "SubCategoryNum": parseInt($("#SubCategoryNum").val()),
                    "SubCategoryName": $("#SubCategoryName").val(),
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
                            alert('已有相同類別');
                            break;
                        };
                        case 101: {
                            alert('未有此商品類型');
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
    });

    //上一頁
    $("#NextPage").click(function () {
        location.href = "/Product/ProductMenu"
    });

})

var AddCategoryFun = {
    //主類別名稱轉換
    TransCategoryNum: function (categoryNum) {
        switch (categoryNum) {
            case 10: {
                return "3C";
            }
            case 20: {
                return "電腦周邊";
            }
            case 30:
                return "軟體";
        };

    },
    //組html標籤
    DrawCategoryList: function (Array) {
        var htmlText = '';
        var categoryName = "";

        for (var i = 0; i < Array.length; i++) {
            var accEdit = Array[i].f_id == 0 ? '' : "<input type='button' class='EditAccBtn'  name='EditAccBtn'  value='編輯類型名稱'/ >";
            var accDel = Array[i].f_id == 0 ? '' : "<input type='button' class='DeleteBtn'  name='DeleteBtn' value='刪除'/ >";
            var categoryName = "";


            categoryName = AddCategoryFun.TransCategoryNum(Array[i].f_categoryNum);

            htmlText += "<tr>" +
                "<td name='" + Array[i].f_categoryNum + "'>" + categoryName + "</td>" +
                "<td name='SubCategoryNum'>" + Array[i].f_subCategoryNum + "</td>" +
                "<td name='SubCategoryName' >" + Array[i].f_subCategoryName + "</td>" +
                "<td align='center'> <input type='button' class='EditBtn'  name='EditBtn'  value='編輯類型名稱'/ ></td>" +
                "<td align='center'> <input type='button' class='DeleteBtn'  name='DeleteBtn' value='刪除'/ ></td>";
        }

        htmlText += "</tr>";

        $("#TableBody").html(htmlText);
    },
    //選擇主類別
    SelectCategory: function () {
        var tempTable = $.extend(true, [], CategoryJson);
        var serchvalue = $("#CategoryGroup").val();

        if ($("#CategoryGroup").val() == 0) {
            AddCategoryFun.DrawCategoryList(CategoryJson);

        } else {
            tempTable = tempTable.filter(function (item) {//filter搜尋json
                if (item["f_categoryNum"].indexOf(serchvalue) >= 0) {//indexOf -> 有找到所鍵入文字則回傳 >=0
                    return item //大於等於0則 return item
                }
            })
            AddCategoryFun.DrawCategoryList(tempTable);

        }
    }


}