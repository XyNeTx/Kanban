$(document).ready(function () {

    initial = async function () {

        //var _dt = await xAjax.ExecuteJSON({
        //    data: {
        //        "Module": "[exec].[spKBNOR600]",
        //        "@OrderType": "U",
        //        "@Plant": ajexHeader.Plant,
        //        "@UserCode": ajexHeader.UserCode
        //    },
        //});
        //if (_dt.rows[0].KBNOR610 == 0) {
        //    $('#btnKBNOR610').attr('readonly', true);
        //    $('#btnKBNOR610').attr('class', 'btn btn-light');
        //}
        //if (_dt.rows[0].KBNOR620 == 0) {
        //    $('#btnKBNOR620').attr('readonly', true);
        //    $('#btnKBNOR620').attr('class', 'btn btn-light');
        //}
        //if (_dt.rows[0].KBNOR640 == 0) {
        //    $('#btnKBNOR640').attr('readonly', true);
        //    $('#btnKBNOR640').attr('class', 'btn btn-light');
        //}
        //if (_dt.rows[0].KBNOR650 == 0) {
        //    $('#btnKBNOR650').attr('readonly', true);
        //    $('#btnKBNOR650').attr('class', 'btn btn-light');
        //}
        //if (_dt.rows[0].KBNOR660 == 0) {
        //    $('#btnKBNOR660').attr('readonly', true);
        //    $('#btnKBNOR660').attr('class', 'btn btn-light');
        //}
        //if (_dt.rows[0].KBNOR660EX == 0) {
        //    $('#btnKBNOR660EX').attr('readonly', true);
        //    $('#btnKBNOR660EX').attr('class', 'btn btn-light');
        //}
        //if (_dt.rows[0].KBNOR670 == 0) {
        //    $('#btnKBNOR670').attr('readonly', true);
        //    $('#btnKBNOR670').attr('class', 'btn btn-light');
        //}



        xSplash.hide();
    }
    initial();


    xAjax.onClick('btnKBNOR610', function () {
        //console.log('>>'+btnKBNOR610.label+'<<');
        //if (btnKBNOR610.label != 'Delete PDS Normal') {
        //    btnKBNOR610.label = 'Delete PDS Normal';
        //} else if (btnKBNOR610.label != 'ABC') {
        //    btnKBNOR610.label = 'ABC';
        //}
        
        xAjax.redirect('KBNOR610');
    });


    xAjax.onClick('btnKBNOR620', function () {
        xAjax.redirect('KBNOR620');
    });


    xAjax.onClick('btnKBNOR630', function () {
        xAjax.redirect('KBNOR630');
    });


    xAjax.onClick('btnKBNOR640', function () {
        xAjax.redirect('KBNOR640');
    });


});