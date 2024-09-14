$(document).ready(async function () {

    await Initial();

    let loginDate = _xLib.GetCookie("loginDate");
    let plant = _xLib.GetCookie("plantCode");
    console.log(loginDate);
    console.log(plant);


    $("#readProcessDate").val(moment(loginDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));
    $("#readProcessShift").val(loginDate.slice(10, 11) == "D" ? "1:Day Shift" : "2:Night Shift");
    $("#readPlant").val(plant);

    await xSplash.hide();

});

$("#btnExit").click(function () {
    window.location.replace("/OrderingProcess/KBNOR200");
});

async function Initial() {
    await $("#tableMain").DataTable({
        width: '100%',
        paging: false,
        scrollCollapse: true,
        "processing": false,
        "serverSide": false,
        scrollX: false,
        scrollY: '400px',
        searching: false,
        info: false,
        ordering: false,
        columns: [
            {
                title: "Flag", data: "Flag", render: function ()
                {
                    return `<input type="checkbox" id="chkFlag" name="chkFlag" class="form-check-input">`
                }
            },
            { title: "Prod YM", data: "Supplier" },
            { title: "Customer OrderNo.", data: "Customer OrderNo." },
            { title: "Delivery Date", data: "Delivery Date" },
            { title: "Cust.OrderType", data: "Cust.OrderType" },
        ],
        order: [[1, 'asc']]
    });

    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");
}

$("#chkDeliveryDate").on("change", function () {
    if ($(this).is(":checked")) {
        $("#inpDeliveryDate").prop("disabled", false);
    } else {
        $("#inpDeliveryDate").prop("disabled", true);
    }
});

$("#chkCustomerOrderNo").on("change", function () {
    if ($(this).is(":checked")) {
        $("#inpCustomerOrderNo").prop("disabled", false);
    } else {
        $("#inpCustomerOrderNo").prop("disabled", true);
    }
});