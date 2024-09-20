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

    xAjax.onClick("#ReportBtn", function () {
        var monthFromVal = $("#F_ProdFrom").val();
        var monthToVal = $("#F_ProdTo").val();

        if (monthFromVal === "" || monthFromVal === null || monthToVal === "" || monthToVal === null) {
            return xSwal.error("Data Error", "Please Select Month From And Month To");
        }

        var monthFrom = monthFromVal.split('/')[1] + monthFromVal.split('/')[0];
        var monthTo = monthToVal.split('/')[1] + monthToVal.split('/')[0];

        if (monthFrom > monthTo) {
            return xSwal.error("Data Error", "Please Select 'Month From' Less than 'Month To");
        }

        var supFrom = $("#F_SupFrom").val();
        var supTo = $("#F_SupTo").val();

        if (supFrom === "" || supFrom === null || supTo === "" || supTo === null) {
            return xSwal.error("Data Error", "Please Select Supplier Code From And Supplier Code To");
        }

        if (supFrom > supTo) {
            return xSwal.error("Data Error", "Please Select 'Supplier Code From' Less than 'Supplier Code To");
        }

        var userName = $("#profile-avatar").prop("title");

        xAjax.Post({
            url: 'KBNRT110/OnReportClick',
            data: {
                monthFrom: monthFrom,
                monthTo: monthTo,
                supFrom: supFrom,
                supTo: supTo
            },
            then: function (result) {
                console.log(result);
                var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
                var reportUrl = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx?/KB3/";
                
                window.location.href = reportUrl + filename + '&MonthFrom=' + monthFrom + '&MonthTo=' + monthTo +
                    '&SupFrom=' + supFrom + '&SupTo=' + supTo + '&UserName=' + userName;
            },
            error: function (result) {
                console.error(result);
            },
        });
    });
});