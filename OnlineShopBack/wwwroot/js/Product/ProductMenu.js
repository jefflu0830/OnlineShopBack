var ProductMenu = {
    Category10: "",
    Category20: "",
    Category30: "",
    ProductJson: "",
    CategotyJson: "",
    MainTempTable: "",
    FilterTemp: ""
}
$(document).ready(function () {
    //取得類型列表
    $.ajax({
        type: "GET",
        url: "/api/Product/GetCategory",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            CategotyJson = data

            Category10 = ProductMenuFun.MakeSelectHtml(data.filter(function (item) {
                if (item["CategoryNum"].toString().indexOf(10) >= 0) {
                    return item
                }
            }));
            Category20 = ProductMenuFun.MakeSelectHtml(data.filter(function (item) {
                if (item["CategoryNum"].toString().indexOf(20) >= 0) {
                    return item
                }
            }));
            Category30 = ProductMenuFun.MakeSelectHtml(data.filter(function (item) {
                if (item["CategoryNum"].toString().indexOf(30) >= 0) {
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
            MainTempTable = $.extend(true, [], ProductJson);

            var rows = ProductMenuFun.MakeProductMenuTag(ProductJson);
            $('#TableBody').html(rows);

            //刪除按鈕
            $('.DeleteBtn').click(function () {
                var currentRow = $(this).closest("tr");
                var ProductId = currentRow.find('td:eq(0)').attr('id');

                //Id搜尋要刪除的商品
                var DelSearchById = function (JsonCol, SearchItem) {
                    var tempTable = ProductJson.filter((item) => {
                        if (item[JsonCol].toString().indexOf(SearchItem) >= 0) {
                            return item
                        }
                    })
                    return tempTable;
                }

                var DelProduct = DelSearchById('Id', ProductId)

                if (window.confirm("確定要刪除此商品嗎?")) {
                    $.ajax({
                        url: '/api/Product/DelProduct?ProductId=' + DelProduct[0].Id + '&ProductNum=' + DelProduct[0].Num + '&ImgName=' + DelProduct[0].ImgPath,
                        type: 'DELETE',
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
                                    alert('無此商品');
                                    break
                                };
                                case 200: {
                                    alert('後端驗證失敗,請查詢LOG');
                                    break;
                                };
                                case 201: {
                                    alert('例外錯誤,請查詢LOG');
                                    location.reload();
                                    break;
                                };
                                case 202: {
                                    alert('圖片上傳失敗,請檢查格式');
                                    location.reload();
                                    break;
                                };
                                default: {
                                    alert(result);
                                };
                            };
                            if (result === "已從另一地點登入,轉跳至登入頁面") {
                                location.reload();
                            };
                        },
                        error: function (error) {
                            alert(error);
                        }
                    })
                }
            });

            //點擊編輯商品按鈕
            $('.EditBtn').click(function () {
                var currentRow = $(this).closest("tr");
                var ProductId = currentRow.find("td:eq(0)").attr('id');

                //Id搜尋要更新的商品
                var EditSearchById = function (JsonCol, SearchItem) {
                    var tempTable = ProductJson.filter((item) => {
                        if (item[JsonCol].toString().indexOf(SearchItem) >= 0) {
                            return item
                        }
                    })
                    return tempTable;
                }

                var EditProduct = EditSearchById('Id', ProductId)

                //開放狀態
                switch (EditProduct[0].Status) {
                    case 0:
                        ProductStatus = '<option selected value = "0" selected>開放 </option ><option value="100">不開放</option>';
                        break;
                    case 100:
                        ProductStatus = '<option value = "0">開放 </option ><option selected value="100" >不開放</option>'
                        break;

                }
                //主類別&子類別
                switch (EditProduct[0].Category) {
                    case 10: {
                        ProductCategoryNum = '<option selected value = "10"> 3C</option ><option value="20">電腦周邊</option><option value="30">軟體</option>';
                        ProductSubCategory = Category10
                        break;
                    }
                    case 20: {
                        ProductCategoryNum = '<option value = "10"> 3C</option ><option selected value="20">電腦周邊</option><option value="30">軟體</option>';
                        ProductSubCategory = Category20
                        break;
                    }
                    case 30: {
                        ProductCategoryNum = '<option value = "10"> 3C</option ><option value="20">電腦周邊</option><option selected value="30">軟體</option>';
                        ProductSubCategory = Category30
                        break;
                    }
                }

                if ($("#EditBox").css("display") == "none") {

                    var EditData =
                        '<h5>商品編輯</h5>' +
                        //商品圖片
                        '<div><label> 圖片:</label> ' +
                        '<input type="file" name="ImgPath" id="ImgPath">' +
                        //商品代號
                        '<div><label> 商品代號:</label> <label id="ProductNum">' + EditProduct[0].Num + '</label></div>' +
                        //主類別
                        '<div><label for="EditProductCategory">主類別:</label>' +
                        '<select id="EditProductCategory" onchange="ProductMenuFun.SelectSubCategory()">' + ProductCategoryNum + '</select ></div>' +
                        //子類別
                        '<div><label for="EditSubCategory">子類別:</label>' +
                        '<select id="EditSubCategory">' + ProductSubCategory + '</select >' +
                        //名稱
                        '<div><label for="ProductName">商品名稱:</label>' +
                        '<input type="text" id="ProductName" name="ProductName" maxlength="20" value="' + EditProduct[0].Name + '"/></div>' +
                        //開放狀態
                        '<div><label for="ProductStatus">開放狀態:</label>' +
                        '<select id="ProductStatus">' + ProductStatus + '</select ></div > ' +
                        //價格
                        '<div><label for="ProductPrice">價格:</label>' +
                        '<input type="text" id="ProductPrice" name="ProductPrice" maxlength="9" value="' + EditProduct[0].Price + '" oninput="value=value.replace(/[^\d]/g,"")" /></div>' +
                        //庫存量
                        '<div><label for="ProductStock">庫存量:</label>' +
                        '<input type="text" id="ProductStock" name="ProductStock" maxlength="4" value="' + EditProduct[0].Stock + '" /></div>' +
                        //熱門度
                        '<div><label for="Popularity">熱門度:</label>' +
                        '<input type="text" id="Popularity" name="Popularity" maxlength="3" value="' + EditProduct[0].Popularity + '" /></div>' +
                        //內容
                        '<div><label for="Productcontent" style="display: block;">細項說明:</label>' +
                        '<textarea id="ProductContent" name="ProductContent" rows="5" cols="90" maxlength="500">' + EditProduct[0].Content + '</textarea></div>' +
                        "<div id='Editbutton'><input id='EditConfirm' type='Button' value='確認編輯' />" +
                        "<input name='EditCancel' id = 'EditCancel' type = 'Button' value = '取消編輯' /></div > ";
                    $('#Editform').html(EditData);
                    $("#EditBox").show();

                    //確認編輯
                    $('#EditConfirm').click(function () {
                        var errorCode = '';

                        //errorCode若不為空,不進行修改
                        if (errorCode !== "") {
                            alert(errorCode);
                        }
                        else {
                            var data = new FormData(document.getElementById("Editform"));

                            var EditProduct = JSON.stringify({
                                Num: $("#ProductNum").html(),
                                Category: parseInt($("#EditProductCategory").val()),
                                SubCategory: parseInt($("#EditSubCategory").val()),
                                Name: $("#ProductName").val(),
                                ImgPath: $("#ProductImg").val(),
                                Price: parseInt($("#ProductPrice").val()),
                                Status: parseInt($("#ProductStatus").val()),
                                Content: $("#ProductContent").val(),
                                Stock: parseInt($("#ProductStock").val()),
                                Popularity: parseInt($("#Popularity").val())
                            })


                            data.append("EditProductFrom", EditProduct);

                            $.ajax({
                                url: '/api/Product/UpdateProduct',
                                type: 'put',
                                contentType: 'application/json',
                                dataType: 'text',
                                data: data,
                                contentType: false,
                                processData: false,
                                success: function (result) {

                                    var JsonResult = JSON.parse(result);//JSON字串轉物件

                                    switch (JsonResult[0].st) {
                                        case 0: {
                                            alert('編輯成功');
                                            location.reload();
                                            break;
                                        };
                                        case 100: {
                                            alert('商品不存在');
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
                                        case 202: {
                                            alert('圖片上傳失敗,請檢查格式');
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
                        }
                    })
                    //取消編輯
                    $('#EditCancel').click(function () {
                        if ($("#EditBox").css("display") !== "none") {
                            $("#EditBox").hide();
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

    //主類別下拉欄位chnage事件
    $('#SerchCategory').change(function () {
        //初始化
        MainTempTable = $.extend(true, [], ProductJson);

        if ($('#SerchCategory').val() === '0') {

            $("#SearchSubCategoryBox").hide();

            ProductMenuFun.SearchInput()
        } else {
            var AllStrTag = '<option value="All">全部</option>';
            $("#SearchSubCategoryBox").show();

            switch ($('#SerchCategory').val()) {
                case '10':
                    $('#SubCategory').html(AllStrTag + Category10);
                    break;
                case '20':
                    $('#SubCategory').html(AllStrTag + Category20);

                    break;
                case '30':
                    $('#SubCategory').html(AllStrTag + Category30);
                    break;
            };

            //篩選 主類別
            MainTempTable = ProductMenuFun.JsonFilter(MainTempTable, "Category", $('#SerchCategory').val())
            FilterTemp = $.extend(true, [], MainTempTable);
            ProductMenuFun.SearchInput()
        }
    });
    //子類別下拉欄位chnage事件
    $('#SubCategory').change(function () {
        MainTempTable = $.extend(true, [], FilterTemp);

        if ($('#SubCategory').val() !== 'All') {
            MainTempTable = ProductMenuFun.JsonFilter(MainTempTable, "SubCategory", $('#SubCategory').val())
        }

        ProductMenuFun.SearchInput()
    });
});

ProductMenuFun = {
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
    //開放狀態名稱轉換
    TransStatus: function (statusCode) {
        switch (statusCode) {
            case 0: {
                return '開放';
            }
            case 100: {
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
        var CategoryValue = $("#EditProductCategory").val();//取主類別值

        switch (CategoryValue) {
            case ('10'):
                $('#EditSubCategory').html(Category10);
                break;
            case ('20'):
                $('#EditSubCategory').html(Category20);
                break;
            case ('30'):
                $('#EditSubCategory').html(Category30);
                break;
        }
    },
    //組table HTML TAG
    MakeProductMenuTag: function (ProudctJson) {

        var rows = '';
        for (var i in ProudctJson) {
            var categoryName = ProductMenuFun.TransCategoryNum(ProudctJson[i].Category);
            var status = ProductMenuFun.TransStatus(ProudctJson[i].Status)
            rows += "<tr>" +
                '<td id="' + ProudctJson[i].Id + '">' + ProudctJson[i].Num + '</td>' +
                '<td name="' + ProudctJson[i].Category + '">' + categoryName + '</td>' +
                '<td name="SubCategoryName">' + ProudctJson[i].SubCategoryName + '</td>' +
                '<td name="Name">' + ProudctJson[i].Name + '</td>' +
                '<td name="Price">' + ProudctJson[i].Price + '</td>' +
                '<td name="' + ProudctJson[i].Status + '">' + status + '</td>' +
                '<td name="Status">' + ProudctJson[i].Stock + '</td>' +
                '<td name="Status">' + ProudctJson[i].Popularity + '</td>' +
                "<td align='center'> <input type='button' class='EditBtn'  name='EditBtn'  value='編輯商品'/ ></td>" +
                "<td align='center'> <input type='button' class='DeleteBtn'  name='DeleteBtn' value='刪除'/ ></td>";
            "</tr>";
        }
        return rows
    },
    //Json查詢  //JsonTable=>要Filter的JSON, ItemName=>查詢的欄位, Searchvalue=>要查詢的值
    JsonFilter: function (JsonTable, ItemName, Searchvalue) {
        var Filter = JsonTable.filter(function (item) {
            if (item[ItemName].toString().indexOf(Searchvalue) >= 0) {
                return item
            }
        })
        return Filter;
    },
    //搜尋INPUT
    SearchInput: function () {
        if ($('#Search').val() == '') {
            var rows = ProductMenuFun.MakeProductMenuTag(MainTempTable);

        } else {
            var SearchResult = ProductMenuFun.JsonFilter(MainTempTable, $('#SearchClass').val(), $('#Search').val())
            var rows = ProductMenuFun.MakeProductMenuTag(SearchResult);
        }

        $('#TableBody').html(rows);

    }

}