var ProductMenu = {
    Category10: "",
    Category20: "",
    Category30: "",
    ProductJson: "",
}
$(document).ready(function () {
    //取得類型列表
    $.ajax({
        type: "GET",
        url: "/api/Product/GetCategory",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            Category10 = ProductMenuFun.MakeSelectHtml(data.filter(function (item) {
                if (item["f_categoryNum"].indexOf(10) >= 0) {
                    return item
                }
            }));
            Category20 = ProductMenuFun.MakeSelectHtml(data.filter(function (item) {
                if (item["f_categoryNum"].indexOf(20) >= 0) {
                    return item
                }
            }));
            Category30 = ProductMenuFun.MakeSelectHtml(data.filter(function (item) {
                if (item["f_categoryNum"].indexOf(30) >= 0) {
                    return item
                }
            }));
        },
        failure: function (data) {
            alert(data);
        },
        error: function (data) {
            alert(data);
        }
    });
    //取得商品列表
    $.ajax({
        type: "GET",
        url: "/api/Product/GetProduct",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            ProductJson = data;
            for (var i in data) {

                var categoryName = ProductMenuFun.TransCategoryNum(data[i].f_category);
                var status = ProductMenuFun.TransStatus(data[i].f_status)

                var rows = rows + "<tr>" +
                    '<td id="' + data[i].f_id + '">' + data[i].f_num + '</td>' +
                    '<td name="' + data[i].f_category + '">' + categoryName + '</td>' +
                    '<td name="SubCategoryName">' + data[i].f_subCategoryName + '</td>' +
                    '<td name="Name">' + data[i].f_name + '</td>' +
                    '<td name="Price">' + data[i].f_price + '</td>' +
                    '<td name="' + data[i].f_status + '">' + status + '</td>' +
                    '<td name="Status">' + data[i].f_stock + '</td>' +
                    "<td align='center'> <input type='button' class='EditBtn'  name='EditBtn'  value='編輯商品'/ ></td>" +
                    "<td align='center'> <input type='button' class='DeleteBtn'  name='DeleteBtn' value='刪除'/ ></td>" +
                    '<td name="Status" style="display: none;">' + data[i].f_content + '</td>' +  //商品內容 隱藏
                    '<td name="Status" style="display: none;">' + data[i].f_img + '</td>' +  //商品圖片 隱藏
                    "</tr>";
            }
            $('#TableBody').append(rows);
        },

        failure: function (data) {
        },
        error: function (data) {
        }
    });


    //刪除按鈕
    $('#TableBody').on('click', '.DeleteBtn', function () {
        var currentRow = $(this).closest("tr");
        var ProductId = currentRow.find('td:eq(0)').attr('id');
        var ProductNum = currentRow.find('td:eq(0)').text();

        if (window.confirm("確定要刪除此商品嗎?")) {
            $.ajax({
                url: '/api/Product/DelProduct?ProductId=' + ProductId + '&ProductNum=' + ProductNum,
                type: 'DELETE',
                data: {},
                success: function (result) {
                    var JsonResult = JSON.parse(result)//JSON字串轉物件
                    switch (JsonResult[0].st) {
                        case 0: {
                            alert('刪除成功');
                            location.reload(); //新增成功才更新頁面
                            break;
                        }
                        case 100:
                            alert('無此商品');
                            break;
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
    //點擊編輯商品按鈕
    $("#TableBody").on('click', '.EditBtn', function () {
        var currentRow = $(this).closest("tr");
        var ProductId = currentRow.find("td:eq(0)").attr('id');
        var ProductNum = currentRow.find("td:eq(0)").text();
        var ProductCategoryNum = currentRow.find("td:eq(1)").attr('name');
        var ProductName = currentRow.find("td:eq(3)").text();
        var ProductPrice = currentRow.find("td:eq(4)").text();
        var ProductStatus = currentRow.find("td:eq(5)").attr('name');
        var ProductStock = currentRow.find("td:eq(6)").text();
        var ProductContent = currentRow.find("td:eq(9)").text();
        var ProductImg = currentRow.find("td:eq(10)").text();
        //開放狀態
        switch (ProductStatus) {
            case '0':
                ProductStatus = '<option selected value = "0" selected>開放 </option ><option value="100">不開放</option>';
                break;
            case '100':
                ProductStatus = '<option value = "0">開放 </option ><option selected value="100" >不開放</option>'
                break;

        }
        //主類別&子類別
        switch (ProductCategoryNum) {
            case '10': {
                ProductCategoryNum = '<option selected value = "10"> 3C</option ><option value="20">電腦周邊</option><option value="30">軟體</option>';
                ProductSubCategory = Category10
                break;
            }
            case '20': {
                ProductCategoryNum = '<option value = "10"> 3C</option ><option selected value="20">電腦周邊</option><option value="30">軟體</option>';
                ProductSubCategory = Category20
                break;
            }
            case '30': {
                ProductCategoryNum = '<option value = "10"> 3C</option ><option value="20">電腦周邊</option><option selected value="30">軟體</option>';
                ProductSubCategory = Category30
                break;
            }
        }


        if ($("#EditBox").css("display") == "none") {

            var EditData =
                "<h5>商品編輯</h5>" +
                //商品圖片
                '<div><label> 圖片:</label> ' +
                '<input type="text" id="ProductImg" name="ProductImg" maxlength="100" value="' + ProductImg + '"/></div>' +
                //商品代號
                '<div><label> 商品代號:</label> <label id="ProductNum">' + ProductNum + '</label></div>' +
                //主類別
                '<div><label for="EditCategroy">主類別:</label>' +
                '<select id="ProductCategory" onchange="ProductMenuFun.SelectSubCategory()">' + ProductCategoryNum + '</select ></div>' +
                //子類別
                '<div><label for="EditSubCategroy">子類別:</label>' +
                '<select id="SubCategory">' + ProductSubCategory + '</select >' +
                //名稱
                '<div><label for="ProductName">商品名稱:</label>' +
                '<input type="text" id="ProductName" name="ProductName" maxlength="20" value="' + ProductName + '"/></div>' +
                //開放狀態
                '<div><label for="ProductStatus">開放狀態:</label>' +
                '<select id="ProductStatus">' + ProductStatus + '</select ></div > ' +
                //價格
                '<div><label for="ProductPrice">價格:</label>' +
                '<input type="text" id="ProductPrice" name="ProductPrice" maxlength="9" value="' + ProductPrice + '" oninput="value=value.replace(/[^\d]/g,"")" /></div>' +
                //庫存量
                '<div><label for="ProductStock">庫存量:</label>' +
                '<input type="text" id="ProductStock" name="ProductStock" maxlength="4" value="' + ProductStock + '" /></div>' +
                //內容
                '<div><label for="Productcontent" style="display: block;">細項說明:</label>' +
                '<textarea id="ProductContent" name="ProductContent" rows="5" cols="90" maxlength="500">' + ProductContent + '</textarea></div>' +
                "<div id='Editbutton'><input name='EditCategroyName' onclick ='ProductMenuFun.EditAcc_Click()' type='Button' value='確認編輯' />" +
                "<input name='EditCancel' id = 'EditCancel' type = 'Button'  onclick = 'ProductMenuFun.EditCancel_Click()' value = '取消編輯' /></div > ";
            $('#Editform').html(EditData);
            $("#EditBox").show();
        }
    });

    //前往新增商品
    $("#AddProduct").click(function () {
        location.href = "/Product/AddProduct"
    });
    //前往新增類別
    $("#AddCategory").click(function () {
        location.href = "/Product/AddCategory"
    });
    //前往首頁
    $("#GoIndex").click(function () {
        location.href = "/index"
    });
})

ProductMenuFun = {
    //確認編輯
    EditAcc_Click: function () {
        var errorCode = '';

        //errorCode若不為空,不進行修改
        if (errorCode !== "") {
            alert(errorCode);
        }
        else {
            $.ajax({
                url: '/api/Product/UpdateProduct',
                type: 'put',
                contentType: 'application/json',
                dataType: 'text',
                data: JSON.stringify({
                    "Num": $("#ProductNum").html(),
                    "Category": parseInt($("#ProductCategory").val()),
                    "SubCategory": parseInt($("#SubCategory").val()),
                    "Name": $("#ProductName").val(),
                    "ImgPath": $("#ProductImg").val(),
                    "Price": parseInt($("#ProductPrice").val()),
                    "Status": parseInt($("#ProductStatus").val()),
                    "Content": $("#ProductContent").val(),
                    "Stock": parseInt($("#ProductStock").val())
                }),
                success: function (result) {

                    var JsonResult = JSON.parse(result)//JSON字串轉物件
                    switch (JsonResult[0].st) {
                        case 0: {
                            alert('更新成功');
                            location.reload(); //新增成功才更新頁面
                            break;
                        }
                        case 100:
                            alert('尚未建立此類別');
                            break;
                    }

                    if (result == "更新成功") {
                        location.reload(); //新增成功才更新頁面
                    }
                },
                error: function (error) {
                    alert(error);
                }
            })
        }
    },
    //取消編輯
    EditCancel_Click: function () {
        if ($("#EditBox").css("display") !== "none") {
            $("#EditBox").hide();
        }
    },
    //主類別名稱轉換
    TransCategoryNum: function (categoryNum) {
        switch (categoryNum) {
            case "10": {
                return "3C";
            }
            case "20": {
                return "電腦周邊";
            }
            case "30":
                return "軟體";
        };

    },
    //開放狀態名稱轉換
    TransStatus: function (statusCode) {
        switch (statusCode) {
            case '0': {
                return '開放';
            }
            case '100': {
                return "不開放";
            }
        };
    }, MakeSelectHtml: function (CategoryJson) {
        var Rows = '';
        for (var i = 0; i < CategoryJson.length; i++) {
            Rows += "<option value='" + CategoryJson[i].f_subCategoryNum + "'>" + CategoryJson[i].f_subCategoryName + "</option>";
        }
        return Rows
    },
    //主類別select onchange事件
    SelectSubCategory: function () {
        var CategoryValue = $("#ProductCategory").val();//取主類別值

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
    }
}