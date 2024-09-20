$(document).ready(function () {
    xAjax.Post({
        url: 'KBNRT170/Initial',
        then: function (result) {
            //console.log(result);
            $.each(result.data, function (i, v) {
                $("#F_SupFrom").append($("<option>", { value: v.F_Supplier, text: v.F_Supplier }, "</option>"));
            });
            $.each(result.data2, function (i, v) {
                $("#F_CycleFrom").append($("<option>", { value: v, text: v }, "</option>"));
                $("#F_CycleTo").append($("<option>", { value: v, text: v }, "</option>"));
            });
            $("#SupFromBlank").hide();
            $("#CycleFromBlank").hide();
            $("#CycleToBlank").hide();
            xSplash.hide();
        },
        error: function (result) {
            console.error("Initial Error from Receive Special Report");
            xSplash.hide();
        },
    });

    $("#OrderDiv").click(function () {
        var checked = $("input[name='radDate']:checked").val();
        if (checked === "Order") {
            $("#F_CycleFrom").val("");
            $("#F_CycleTo").val("");
            $("#F_CycleFrom").attr("disabled", true);
            $("#F_CycleTo").attr("disabled", true);
        }
    });
    $("#DeliveryDiv").click(function () {
        var checked = $("input[name='radDate']:checked").val();
        if (checked === "Delivery") {
            $("#F_CycleFrom").attr("disabled", false);
            $("#F_CycleTo").attr("disabled", false);
        }
    });

    xAjax.onChange("#F_SupFrom , #F_DateFrom, #F_DateTo", function () {
        var supFrom = $("#F_SupFrom").val();
        var typeDate = $("input[type='radio'][name='radDate']:checked").val();
        var dateFrom = $("#F_DateFrom").val().replaceAll('-', '');
        var dateTo = $("#F_DateTo").val().replaceAll('-', '');
        console.log(supFrom);
        console.log(typeDate);
        xAjax.Post({
            url: 'KBNRT170/OnChange',
            data: {
                supFrom: supFrom,
                typeDate: typeDate,
                dateFrom: dateFrom,
                dateTo: dateTo
            },
            then: function (result) {
                //console.log(result);
                $("#F_CycleFrom").empty();
                $("#F_CycleTo").empty();
                $("#F_CycleFrom").append($("<option id='CycleFromBlank'></option>"));
                $("#F_CycleTo").append($("<option id='CycleToBlank'></option>"));

                $.each(result.data, function (i, v) {
                    $("#F_CycleFrom").append($("<option>", { value: v, text: v }, "</option>"));
                    $("#F_CycleTo").append($("<option>", { value: v, text: v }, "</option>"));
                });
                $("#CycleFromBlank").hide();
                $("#CycleToBlank").hide();
            },
            error: function (result) {
                console.error(result);
            }
        });
    });
    async function ReportClick() {
        var supFrom = $("#F_SupFrom").val();
        var typeDate = $("input[type='radio'][name='radDate']:checked").val();
        var dateFrom = $("#F_DateFrom").val().replaceAll('-', '');
        var dateTo = $("#F_DateTo").val().replaceAll('-', '');
        var dateFromShow = dateFrom.substring(6, 8) + "/" + dateFrom.substring(4, 6) + "/" + dateFrom.substring(0, 4);
        var dateToShow = dateTo.substring(6, 8) + "/" + dateTo.substring(4, 6) + "/" + dateTo.substring(0, 4);
        var cycleFrom = $("#F_CycleFrom").val().trim();
        var cycleTo = $("#F_CycleTo").val().trim();
        if (supFrom == "" || supFrom == undefined) {
            return xSwal.error("Invalid Select Supplier", "Please Select Supplier then try again!");
        }
        if (typeDate === "Delivery") {
            if (cycleFrom == "" || cycleFrom == undefined || cycleTo == "" || cycleTo == undefined) {
                return xSwal.error("Invalid Select Cycle", "Please Select Cycle then try again!");
            }
        }
        if (cycleFrom > cycleTo) {
            return xSwal.error("Select System Cycle ERROR!", "Please System Cycle From less than System Cycle To");
        }
        if (dateFrom > dateTo) {
            return xSwal.error("Select Order Date ERROR!", "Please Select Order Date From less than Order Date To");
        }
        xAjax.Post({
            url: 'KBNRT170/OnClickReport',
            data: {
                supFrom: supFrom,
                typeDate: typeDate,
                dateFrom: dateFrom,
                dateTo: dateTo,
                cycleFrom: cycleFrom,
                cycleTo: cycleTo
            },
            then: function (result) {
                if (result.status === "200") {
                    var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
                    var reportUrl = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx?/KB3/";
                    window.location.href = reportUrl + filename + typeDate + '&Sup=' + supFrom + '&DateFrom=' + dateFrom +
                        '&DateTo=' + dateTo + '&CycleFrom=' + cycleFrom + '&CycleTo=' + cycleTo + '&DateFromShow=' + dateFromShow +
                        '&DateToShow=' + dateToShow;
                }
                else {
                    return xSwal.Error(result.title, result.message);
                }
            },
            error: function (result) {
                console.error("Initial Error " + result);
            },
        });
    }
    xAjax.onClick("#ReportBtn", async function () {
        await ReportClick();
    });
});