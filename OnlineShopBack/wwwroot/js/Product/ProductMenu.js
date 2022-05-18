$(document).ready(function () {
    //取得商品列表
    $.ajax({
        type: "GET",
        url: "/api/Product/GetCategory",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            CategoryJson = data;
            for (var i in data) {
                var categoryName = "";

                categoryName = AddCategoryFun.TransCategoryNum(data[i].f_categoryNum);

                var rows = rows + "<tr>" +
                    "<td name='" + data[i].f_categoryNum + "'>" + categoryName + "</td>" +
                    "<td name='SubCategoryNum'>" + data[i].f_subCategoryNum + "</td>" +
                    "<td name='SubCategoryName'>" + data[i].f_subCategoryName + "</td>" +
                    "<td name='" + data[i].f_categoryNum + "'>" + categoryName + "</td>" +
                    "<td name='" + data[i].f_categoryNum + "'>" + categoryName + "</td>" +
                    "<td name='" + data[i].f_categoryNum + "'>" + categoryName + "</td>" +
                    "<td align='center'> <input type='button' class='EditBtn'  name='EditBtn'  value='編輯類型名稱'/ ></td>" +
                    "<td align='center'> <input type='button' class='DeleteBtn'  name='DeleteBtn' value='刪除'/ ></td>" +
                    "</tr>";
            }
            $('#TableBody').append(rows);
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
})