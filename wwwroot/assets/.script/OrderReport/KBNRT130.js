$(document).ready(function () {
    xAjax.Post({
        url: 'KBNRT130/Initial',
        then: function (result) {
            // console.log(result);
            $.each(result.data, function (i, v) {
                $("#F_MonthFrom").append($("<option>", { value: v.Month_Year, text: v.Month_Year }, "</option>"));
                $("#F_MonthTo").append($("<option>", { value: v.Month_Year, text: v.Month_Year }, "</option>"));
            });
            $.each(result.data2, function (i, v) {
                $("#F_SupFrom").append($("<option>", { value: v, text: v }, "</option>"));
                $("#F_SupTo").append($("<option>", { value: v, text: v }, "</option>"));
            });

            $('#monthFromBlank').hide();
            $('#monthToBlank').hide();
            $('#supFromBlank').hide();
            $('#supToBlank').hide();
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
            url: 'KBNRT130/OnMonthChange',
            data: {
                monthFrom: monthFrom,
                monthTo: monthTo
            },
            then: function (result) {
                console.log(result);
                $("#F_SupFrom").empty();
                $("#F_SupTo").empty();
                $("#F_SupFrom").append($("<option id='supFromBlank'></option>"));
                $("#F_SupTo").append($("<option id='supToBlank'></option>"));
                $.each(result.data, function (i, v) {
                    $("#F_SupFrom").append($("<option>", { value: v, text: v }, "</option>"));
                    $("#F_SupTo").append($("<option>", { value: v, text: v }, "</option>"));
                });

                $('#supFromBlank').hide();
                $('#supToBlank').hide();
            },
            error: function (result) {
                console.error(result);
            },
        });
    });

    xAjax.onClick("#ReportBtn", function () {
        var monthFromVal = $("#F_MonthFrom").val();
        var monthToVal = $("#F_MonthTo").val();

        if (monthFromVal == "" || monthFromVal == null || monthToVal == "" || monthToVal == null) {
            return xSwal.error("Data Error", "Please Select Month From And Month To");
        }

        var monthFrom = monthFromVal.split('/')[1] + monthFromVal.split('/')[0];
        var monthTo = monthToVal.split('/')[1] + monthToVal.split('/')[0];

        if (monthFrom > monthTo) {
            return xSwal.error("Data Error", "Please Select 'Month From' Less than 'Month To");
        }

        var supFrom = $("#F_SupFrom").val();
        var supTo = $("#F_SupTo").val();

        if (supFrom == "" || supFrom == null || supTo == "" || supTo == null) {
            return xSwal.error("Data Error", "Please Select Supplier Code From And Supplier Code To");
        }

        if (supFrom > supTo) {
            return xSwal.error("Data Error", "Please Select 'Supplier Code From' Less than 'Supplier Code To");
        }

        var userName = $("#profile-avatar").prop("title");

        var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);

        var reportUrl = "http://hmmt-app03/Reportserver/report/KB3/";

        window.location.href = reportUrl + filename + '?MonthFrom=' + monthFrom + '&MonthTo=' + monthTo +
            '&SupFrom=' + supFrom + '&SupTo=' + supTo + '&UserName=' + userName;
    });
});