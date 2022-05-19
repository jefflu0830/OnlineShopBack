var subCategorySelect = {
    Category10: "",
    Category20: "",
    Category30: ""
}
$(document).ready(function () {
    //取得類型列表
    $.ajax({
        type: "GET",
        url: "/api/Product/GetCategory",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            Category10 = AddProductFun.MakeSelectHtml(data.filter(function (item) {
                if (item["f_categoryNum"].indexOf(10) >= 0) {
                    return item
                }
            }));
            Category20 = AddProductFun.MakeSelectHtml(data.filter(function (item) {
                if (item["f_categoryNum"].indexOf(20) >= 0) {
                    return item
                }
            }));
            Category30 = AddProductFun.MakeSelectHtml(data.filter(function (item) {
                if (item["f_categoryNum"].indexOf(30) >= 0) {
                    return item
                }
            }));
            
            AddProductFun.SelectSubCategory();//初始值 先執行一次 
        },
        failure: function (data) {
            alert(data);
        },
        error: function (data) {
            alert(data);
        }
    });
    //點擊新增
    $("#AddButton").click(function () {
        var ErrorCode = '';   

        if ($("#Num").val() == "") {
            ErrorCode += "[商品編號]不可為空白\n"
        } else {
            if (/^[a-zA-Z0-9\u4e00-\u9fa5]*$/.test($("#Num").val()) == false) {
                errorCode += "[商品編號] 只允許輸入英文及數字。\n"
            }
            if ($("#Num").val().length > 20 || $("#Num").val().length < 3) {
                errorCode += "[商品編號] 字數介於3~20之間\n"
            }
        }
        if (/^[0-9]*$/.test($("#Price").val()) == false ||
            /^[0-9]*$/.test($("#Stock").val()) == false ) {
            CheckAccError += "[價錢][庫存] 只允許輸入數字。\n"
        }




        if (ErrorCode !== "") {
            alert(ErrorCode)
        }
        else {
            $.ajax({
                type: "post",
                url: "/api/Product/AddProduct",
                contentType: "application/json",
                dataType: "text",
                data: JSON.stringify({
                    "Num": $("#Num").val(),
                    "Category": parseInt($("#Category").val()),
                    "SubCategory": parseInt($("#SubCategory").val()),
                    "Name": $("#Name").val(),
                    "ImgPath": "TestPath/TestPath/TestPath",//$("#ImgPath").val(),
                    "Price": parseInt($("#Price").val()),
                    "Status": parseInt($("#Status").val()),
                    "Content": $("#Content").val(),
                    "Stock": parseInt($("#Stock").val())
                }),
                success: function (result) {
                    alert(result)

                    if (result == "商品新增成功") {
                        location.href = "/Product/ProductMenu"
                    } else if (result === "已從另一地點登入,轉跳至登入頁面") {
                        location.reload();
                    }
                },
                failure: function (data) {
                    alert(data);
                },
                error: function (error) {
                    alert(error);
                }
            })
        }
    });
    //上一頁
    $("#GoMenu").click(function () {
        location.href = "/Product/ProductMenu"
    });
})

var AddProductFun = {
    //組子類別select標籤
    MakeSelectHtml: function (CategoryJson) {
        var Rows = '';
        for (var i = 0; i < CategoryJson.length; i++) {
            Rows += "<option value='" + CategoryJson[i].f_subCategoryNum + "'>" + CategoryJson[i].f_subCategoryName + "</option>";
        }
        return Rows
    },
    //主類別select onchange事件
    SelectSubCategory: function () {
        //$("#SubCategory > option").remove();
        var CategoryValue = $("#Category").val();//取主類別值
       
        switch (CategoryValue) {
            case ('10'):
                $('#SubCategory').html(Category10);
                break;
            case ('20'):
                $('#SubCategory').html(Category20);
                break;
            case ('30'):
                $('#SubCategory').html(Category30);
                break;
        }


        //map深複製 第一名
        //var array = [1, 2, 3, 4, 5];
        //var array_2 = array.map(function (item) {
        //    return { a: item, b: item.toString() };
        //});
        //var tempTable = $.extend(true, [], CategoryJson);//複製到暫存 //第二名
        //var tempTable = $.assign([], CategoryJson);//assign深複製  第三名
    }
}