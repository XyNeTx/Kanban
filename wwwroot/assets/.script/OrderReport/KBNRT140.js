$(document).ready(function () {
    xAjax.Post({
        url: 'KBNRT140/Initial',
        then: function (result) {
            // console.log(result);
            $.each(result.data, function (i, v) {
                $("#F_MonthFrom").append($("<option>", { value: v.Month_Year, text: v.Month_Year }, "</option>"));
                $("#F_MonthTo").append($("<option>", { value: v.Month_Year, text: v.Month_Year }, "</option>"));
            });
            $.each(result.data2, function (i, v) {
                $("#F_StoreFrom").append($("<option>", { value: v, text: v }, "</option>"));
                $("#F_StoreTo").append($("<option>", { value: v, text: v }, "</option>"));
            });

            $('#monthFromBlank').hide();
            $('#monthToBlank').hide();
            $('#StoreFromBlank').hide();
            $('#StoreToBlank').hide();
            xSplash.hide();
        },
        error: function (result) {
            console.error("Initial Error from Order Report");
            xSplash.hide();
        },
    });
    xAjax.onChange("#F_MonthFrom , #F_MonthTo", function () {
        var monthFromVal = $("#F_MonthFrom").val();
        var monthToVal = $("#F_MonthTo").val();
        if (monthToVal == "" || monthToVal == null) {
            $("#F_MonthTo").val(monthFromVal);
            monthToVal = $("#F_MonthTo").val();
        }
        var monthFrom = monthFromVal.split('/')[1] + monthFromVal.split('/')[0];
        var monthTo = monthToVal.split('/')[1] + monthToVal.split('/')[0];
        xAjax.Post({
            url: 'KBNRT140/OnMonthChange',
            data: {
                monthFrom: monthFrom,
                monthTo: monthTo
            },
            then: function (result) {
                console.log(result);
                $("#F_StoreFrom").empty();
                $("#F_StoreTo").empty();
                $.each(result.data, function (i, v) {
                    $("#F_StoreFrom").append($("<option>", { value: v, text: v }, "</option>"));
                    $("#F_StoreTo").append($("<option>", { value: v, text: v }, "</option>"));
                });
            },
            error: function (result) {
                return console.error(result);
            }
        });
    });
});