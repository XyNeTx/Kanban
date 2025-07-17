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
$("#btnInterface").click(async function () {
    let isConfirm = await xSwal.confirm("Are you Sure to Interface CKD In-House Data from Import Order");

    if (isConfirm) {
        let percent = 0;

        // ตั้งค่าจุดเริ่มต้น
        $("#prgProcess").css("width", `0%`);
        $("#prgProcess_label_").text(`Process : 0.00 %`);

        let loading = setInterval(function () {
            percent = Math.min(percent + 10, 95); // วิ่งจนถึง 95% ระหว่างรอ API
            $("#prgProcess").css("width", `${percent}%`);
            $("#prgProcess").attr("aria-valuenow", percent);
            $("#prgProcess_label_").text(`Process : ${percent.toFixed(2)} %`);
        }, 100);

        _xLib.AJAX_Post("/api/KBNOR310/Interface", "",
            function (success) {
                clearInterval(loading);
                $("#prgProcess").css("width", `100%`);
                $("#prgProcess").attr("aria-valuenow", 100);
                $("#prgProcess_label_").text(`Process : 100.00 %`);
                xSwal.xSuccess(success);
            },
            function (error) {
                clearInterval(loading);
                $("#prgProcess_label_").text(`Process : Error`);
                xSwal.xError(error);
            }
        );
    }
});
