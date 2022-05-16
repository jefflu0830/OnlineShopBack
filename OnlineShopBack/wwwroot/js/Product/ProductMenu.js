$(document).ready(function () {
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