$(document).ready(function () {
    xAjax.Post({
        url: 'KBNRT110/Initial',
        then: function (result) {
            console.log(result);
            $.each(result.data, function (i, v) {
                $("#F_ProdFrom").append($("<option>", { value: v, text: v }, "</option>"));
                $("#F_ProdTo").append($("<option>", { value: v, text: v }, "</option>"));
            });
            $.each(result.data2, function (i, v) {
                $("#F_SupFrom").append($("<option>", { value: v, text: v }, "</option>"));
                $("#F_SupTo").append($("<option>", { value: v, text: v }, "</option>"));
            });

            $('#supFromBlank').hide();
            $('#supToBlank').hide();
            $('#prodFromBlank').hide();
            $('#prodToBlank').hide();
            xSplash.hide();
        },
        error: function (result) {
            console.error("Initial Error from Receive Special Report");
            xSplash.hide();
        },
    });
});