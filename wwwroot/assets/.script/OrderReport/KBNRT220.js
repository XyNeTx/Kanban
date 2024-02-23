$(document).ready(function () {

    function hideBlank() {
        $('#SupFromBlank').hide();
        $('#SupToBlank').hide();
    }
    function initial() {
        return xAjax.Post({
            url: 'KBNRT160/Initial',
            then: function (result) {
               $.each(result.data, function (i, v) {
                   $("#F_SupFrom").append($("<option>", { value: v.Sup_CD, text: v.Sup_CD }, "</option>"));
                   $("#F_SupTo").append($("<option>", { value: v.Sup_CD, text: v.Sup_CD }, "</option>"));
               });
                hideBlank();
                xSplash.hide();
            },
            error: function (result) {
                console.error("Initial Error");
            },
        });
    }
    function deletedOldTemp() {
        return xAjax.Post({
            url: 'KBNRT220/DeleteTemp',
            success: function () {
                console.log("Success");
            },
            error: function () {
                console.error("DeleteTemp Error");
                xSplash.hide();
            },
        });
    }

    async function onPageload() {
        try {
            const deleteOldTemp = await deletedOldTemp();
            const intial = await initial();
        }
        catch (error) {
            console.error(error);
        }

    }

    onPageload();

    xAjax.onChange("#F_SupFrom, #F_SupTo", function () {
        if ($("#F_SupTo").val() === null || $("#F_SupTo").val() === "") {
            $("#F_SupTo").val($("#F_SupFrom").val()).change();
        }
    });

    xAjax.onClick("#ReportBtn", function () {
        var supFrom = $("#F_SupFrom").val();
        var supTo = $("#F_SupTo").val();
        var dateFrom = $("#F_DateFrom").val();
        var dateTo = $("#F_DateTo").val();
        var dateFromPrced = dateFrom.split("-")[0] + dateFrom.split("-")[1] + dateFrom.split("-")[2];
        var dateToPrced = dateTo.split("-")[0] + dateTo.split("-")[1] + dateTo.split("-")[2];
        if (supFrom === "" || supFrom == undefined || supTo === "" || supTo == undefined) {
            return xSwal.error("Supplier Code is empty", "Please Select Supplier Code");
        }
        if (supFrom > supTo) {
            return xSwal.error("Invalid Input Supplier Code", "Please Select Supplier Code From less than Supplier Code To");
        }
        if (dateFrom > dateTo) {
            return xSwal.error("Invalid Input Date", "Please Select Date From less than Date To");
        }
        xAjax.Post({
            url: "KBNRT220/ReportClick",
            data: {
                supFrom: supFrom,
                supTo: supTo,
                dateFrom: dateFromPrced,
                dateTo: dateToPrced,
            },
            then: function (result) {
                console.log(result);
                if (result.status === "200") {
                    var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
                    var reportUrl = "http://hmmt-app03/Reportserver/report/KB3/";
                    window.location.href = reportUrl + filename + '?HostName=' + result.data2 + '&UserName=' + result.data + '&DeliDate=' + result.data3;
                }
                else {
                    return xSwal.Error(result.title, result.message);
                }
            },
            error: function (result) {
                return console.error(result);
            }
        });
    });
});