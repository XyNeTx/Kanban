$(document).ready(function () {



    initial = async function () {

        //var _dt = await xAjax.ExecuteJSON({
        //    data: {
        //        "Module": "[exec].[spKBNOR100]",
        //        "@OrderType": "U",
        //        "@Plant": ajexHeader.Plant,
        //        "@UserCode": ajexHeader.UserCode
        //    },
        //});
        //if (_dt.rows[0].KBNOR110 == 0) {
        //    $('#btnKBNOR110').attr('readonly', true);
        //    $('#btnKBNOR110').attr('class', 'btn btn-light');
        //}
        //if (_dt.rows[0].KBNOR120 == 0) {
        //    $('#btnKBNOR120').attr('readonly', true);
        //    $('#btnKBNOR120').attr('class', 'btn btn-light');
        //}
        //if (_dt.rows[0].KBNOR140 == 0) {
        //    $('#btnKBNOR140').attr('readonly', true);
        //    $('#btnKBNOR140').attr('class', 'btn btn-light');
        //}
        //if (_dt.rows[0].KBNOR150 == 0) {
        //    $('#btnKBNOR150').attr('readonly', true);
        //    $('#btnKBNOR150').attr('class', 'btn btn-light');
        //}
        //if (_dt.rows[0].KBNOR160 == 0) {
        //    $('#btnKBNOR160').attr('readonly', true);
        //    $('#btnKBNOR160').attr('class', 'btn btn-light');
        //}
        //if (_dt.rows[0].KBNOR160EX == 0) {
        //    $('#btnKBNOR160EX').attr('readonly', true);
        //    $('#btnKBNOR160EX').attr('class', 'btn btn-light');
        //}
        //if (_dt.rows[0].KBNOR170 == 0) {
        //    $('#btnKBNOR170').attr('readonly', true);
        //    $('#btnKBNOR170').attr('class', 'btn btn-light');
        //}



        xSplash.hide();
    }
    initial();


    xAjax.onClick('btnKBNOR110', function () {
        xAjax.redirect('KBNOR110');
    });


    xAjax.onClick('btnKBNOR120', function () {
        xAjax.redirect('KBNOR120');
    });
    xAjax.onClick('btnKBNOR121', function () {
        xAjax.redirect('KBNOR121');
    });
    xAjax.onClick('btnKBNOR122', function () {
        xAjax.redirect('KBNOR122');
    });
    xAjax.onClick('btnKBNOR123', function () {
        xAjax.redirect('KBNOR123');
    });


    xAjax.onClick('btnKBNOR130', function () {
        xAjax.redirect('KBNOR130');
    });



    xAjax.onClick('btnKBNOR140', function () {
        xAjax.redirect('KBNOR140');
    });


    xAjax.onClick('btnKBNOR150', function () {
        xAjax.redirect('KBNOR150');
    });
    xAjax.onClick('btnKBNOR150EX', function () {
        xAjax.redirect('KBNOR150EX');
    });


    xAjax.onClick('btnKBNOR160', function () {
        xAjax.redirect('KBNOR160');
    });


});