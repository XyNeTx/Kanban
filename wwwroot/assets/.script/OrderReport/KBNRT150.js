$(document).ready(function () {
    xAjax.Post({
        url: 'KBNRT150/Initial',
        then: function (result) {
            // console.log(result);
            $.each(result.data, function (i, v) {
                $("#F_ImpTypeFrom").append($("<option>", { value: v.Type, text: v.Type }, "</option>"));
                $("#F_ImpTypeTo").append($("<option>", { value: v.Type, text: v.Type }, "</option>"));
            });

            $('#ImpTypeFromBlank').hide();
            $('#ImpTypeToBlank').hide();
            xSplash.hide();
        },
        error: function (result) {
            console.error("Initial Error from Order Report");
            xSplash.hide();
        },
    });
    xAjax.onChange("#F_OrderFrom, #F_OrderTo", function () {
        var orderFrom = $("#F_OrderFrom").val().replaceAll('-', '');
        var orderTo = $("#F_OrderTo").val().replaceAll('-', '');
        //console.log(orderFrom, orderTo);
        xAjax.Post({
            url: 'KBNRT150/OnOrderChange',
            data: {
                orderFrom: orderFrom,
                orderTo: orderTo
            },
            then: function (result) {
                // console.log(result);
                $("#F_ImpTypeFrom").empty();
                $("#F_ImpTypeTo").empty();
                $("#F_ImpTypeFrom").append($("<option id='ImpTypeFromBlank'></option>"));
                $("#F_ImpTypeTo").append($("<option id='ImpTypeToBlank'></option>"));
                $.each(result.data, function (i, v) {
                    $("#F_ImpTypeFrom").append($("<option>", { value: v.Type, text: v.Type }, "</option>"));
                    $("#F_ImpTypeTo").append($("<option>", { value: v.Type, text: v.Type }, "</option>"));
                });

                $('#ImpTypeFromBlank').hide();
                $('#ImpTypeToBlank').hide();
                xSplash.hide();
            },
            error: function (result) {
                console.error("Initial Error from Order Report");
                xSplash.hide();
            },
        });
    });
});