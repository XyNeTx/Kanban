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
    xAjax.onClick("#ReportBtn", function () {
        var typeFrom = $("#F_ImpTypeFrom").val();
        var typeTo = $("#F_ImpTypeTo").val();
        var orderFrom = $("#F_OrderFrom").val().replaceAll('-', '');
        var orderTo = $("#F_OrderTo").val().replaceAll('-', '');

        if (typeFrom == null || typeFrom == undefined || typeTo == null || typeTo == undefined) {
            return xSwal.error("Supplier Code is empty", "Please Select Supplier Code");
        }
        if (typeFrom > typeTo) {
            return xSwal.error("Invalid Input Supplier Code", "Please Select Supplier Code From less than Supplier Code To");
        }
        if (orderFrom > orderTo) {
            return xSwal.error("Invalid Input Order Date", "Please Select Order Date From less than Order Date To");
        }

        xAjax.Post({
            url: 'KBNRT150/OnReportBtnClick',
            data: {
                orderFrom: orderFrom,
                orderTo: orderTo,
                typeFrom: typeFrom,
                typeTo: typeTo
            },
            then: function (result) {
                console.log(result);
                var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
                var reportUrl = "http://hmmt-app03/Reportserver/report/KB3/";
                window.location.href = reportUrl + filename + '?HostName=' + result.data2 + '&UserName=' + result.data;
            },
            error: function (result) {
                console.error(result);
            }
        });
    });
});