$(document).ready(async function () {

    _xLib.AJAX_Get("/api/KBNOR310/Onload", "",
        function (success) {
            //xSwal.xSuccess(success);
        },
        function (error) {
            //xSwal.xError(error);
        }
    );

    xSplash.hide();

    xAjax.onClick('#btnExit', function () {
        xAjax.redirect('KBNOR300');
    });


})

$("#btnInterface").click(function () {

    _xLib.AJAX_Post("/api/KBNOR310/Interface", "",
        function (success) {
            xSwal.xSuccess(success);
        },
        function (error) {
            xSwal.xError(error);
        }
    );

})