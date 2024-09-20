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
        if (monthToVal === "" || monthToVal === "") {
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
                $("#F_StoreFrom").append($("<option id='StoreFromBlank'></option>"));
                $("#F_StoreTo").append($("<option id='StoreToBlank'></option>"));
                $.each(result.data, function (i, v) {
                    $("#F_StoreFrom").append($("<option>", { value: v, text: v }, "</option>"));
                    $("#F_StoreTo").append($("<option>", { value: v, text: v }, "</option>"));
                });
                $('#StoreFromBlank').hide();
                $('#StoreToBlank').hide();
            },
            error: function (result) {
                return console.error(result);
            }
        });
    });
    xAjax.onChange("#F_StoreFrom", function () {
        var storeFrom = $("#F_StoreFrom").val();
        var storeTo = $("#F_StoreTo").val();
        if (storeTo === "" || storeTo === "") {
            $("#F_StoreTo").val(storeFrom);
            storeTo = $("#F_StoreTo").val();
        }
    });
    xAjax.onClick("#ReportBtn", function () {
        var storeFrom = $("#F_StoreFrom").val();
        if (storeFrom === "" || storeFrom === "")
        {
            return xSwal.error("Store From is empty", "Please select Store From then try again");
        }
        var storeTo = $("#F_StoreTo").val();
        if (storeTo === "" || storeTo === "") {
            return xSwal.error("Store To is empty", "Please select Store To then try again");
        }
        var monthFromVal = $("#F_MonthFrom").val();
        if (monthFromVal === "" || monthFromVal === "") {
            return xSwal.error("Month From is empty", "Please select Month From then try again");
        }
        var monthToVal = $("#F_MonthTo").val();
        if (monthToVal === "" || monthToVal === "") {
            return xSwal.error("Month To is empty", "Please select Month To then try again");
        }
        var monthFrom = monthFromVal.split('/')[1] + monthFromVal.split('/')[0];
        var monthTo = monthToVal.split('/')[1] + monthToVal.split('/')[0];

        var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);

        var userName = $("#profile-avatar").prop("title");

        var reportUrl = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx?/KB3/";
        window.location.href = reportUrl + filename + '&StoreFrom=' + storeFrom + '&StoreTo=' + storeTo +
        '&MonthFrom=' + monthFrom + '&MonthTo=' + monthTo + '&UserName=' + userName;
    });
});