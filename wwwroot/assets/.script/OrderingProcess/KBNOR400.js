$(document).ready(function () {

    initial = async function () {

        //var _dt = await xAjax.ExecuteJSON({
        //    data: {
        //        "Module": "[exec].[spKBNOR400]",
        //        "@OrderType": "U",
        //        "@Plant": ajexHeader.Plant,
        //        "@UserCode": ajexHeader.UserCode
        //    },
        //});
        //if (_dt.rows[0].KBNOR410 == 0) {
        //    $('#btnKBNOR410').attr('readonly', true);
        //    $('#btnKBNOR410').attr('class', 'btn btn-light');
        //}
        //if (_dt.rows[0].KBNOR420 == 0) {
        //    $('#btnKBNOR420').attr('readonly', true);
        //    $('#btnKBNOR420').attr('class', 'btn btn-light');
        //}
        //if (_dt.rows[0].KBNOR440 == 0) {
        //    $('#btnKBNOR440').attr('readonly', true);
        //    $('#btnKBNOR440').attr('class', 'btn btn-light');
        //}
        //if (_dt.rows[0].KBNOR450 == 0) {
        //    $('#btnKBNOR450').attr('readonly', true);
        //    $('#btnKBNOR450').attr('class', 'btn btn-light');
        //}
        //if (_dt.rows[0].KBNOR460 == 0) {
        //    $('#btnKBNOR460').attr('readonly', true);
        //    $('#btnKBNOR460').attr('class', 'btn btn-light');
        //}
        //if (_dt.rows[0].KBNOR460EX == 0) {
        //    $('#btnKBNOR460EX').attr('readonly', true);
        //    $('#btnKBNOR460EX').attr('class', 'btn btn-light');
        //}
        //if (_dt.rows[0].KBNOR470 == 0) {
        //    $('#btnKBNOR470').attr('readonly', true);
        //    $('#btnKBNOR470').attr('class', 'btn btn-light');
        //}



        xSplash.hide();
    }
    initial();


    xAjax.onClick('btnKBNOR410', function () {
        xAjax.redirect('KBNOR410');
    });


    xAjax.onClick('btnKBNOR420', function () {
        xAjax.redirect('KBNOR420');
    });


    xAjax.onClick('btnKBNOR440', function () {
        xAjax.redirect('KBNOR440');
    });


    xAjax.onClick('btnKBNOR450', function () {
        xAjax.redirect('KBNOR450');
    });


    xAjax.onClick('btnKBNOR460', function () {
        xAjax.redirect('KBNOR460');
    });


    xAjax.onClick('btnKBNOR460EX', function () {
        xAjax.redirect('KBNOR460EX');
    });


    xAjax.onClick('btnKBNOR470', function () {
        xAjax.redirect('KBNOR470');
    });




});