$(document).ready(function () {
    //取得類型列表
    $.ajax({
        type: "GET",
        url: "/api/Product/GetCategory",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            CategoryJson = data;
            AddProductFun.SelectSubCategory();
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
        var ErrorCode = "";


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
                    alert(result)

                    if (result == "新增成功") {
                        location.href = "/Product/ProductMenu" 
                    } else if (result === "已從另一地點登入,轉跳至登入頁面") {
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
    $("#GoMenu").click(function () {
        location.href = "/Product/ProductMenu"
    });



})

var AddProductFun = {
    SelectSubCategory: function () {
        $("#SubCategory > option").remove();
        var CategoryValue = $("#Category").val();//取主類別值
        var tempTable = $.extend(true, [], CategoryJson);//複製到暫存

        tempTable = tempTable.filter(function (item) {//filter搜尋json
            if (item["f_categoryNum"].indexOf(CategoryValue) >= 0) {//indexOf -> 有找到所鍵入文字則回傳 >=0
                return item  //大於等於0則 return item
            }
        })
        //組子類別下拉選單
        for (var i = 0; i < tempTable.length; i++) {
            var Rows = Rows + "<option value='" + tempTable[i].f_subCategoryNum + "'>" + tempTable[i].f_subCategoryName + "</option>";
            
        }
        $('#SubCategory').append(Rows);
        //將組好的標籤append至 #level
       
    }
}