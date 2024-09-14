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
            { title: "Customer Order No", data: "CustomerOrderNo" },
            { title: "Supplier", data: "Supplier" },
            { title: "Delivery Date", data: "DeliveryDate" },
            { title: "Part No.", data: "Part No." },
            { title: "Qty", data: "Qty" },
            { title: "Store Code", data: "Store Code" },
            { title: "Import Type", data: "Import Type" },
            { title: "Cust.Order Type ", data: "Cust.Order Type " },
        ],
        order: [[1, 'asc']]
    });

    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");
}