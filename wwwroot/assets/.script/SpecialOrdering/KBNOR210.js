$(document).ready(async function () {

    await Initial();

    let loginDate = _xLib.GetCookie("loginDate");
    let plant = _xLib.GetCookie("plantCode");
    //console.log(loginDate);
    //console.log(plant);


    $("#readProcessDate").val(moment(loginDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));
    $("#readProcessShift").val(loginDate.slice(10, 11) == "D" ? "1:Day Shift" : "2:Night Shift");
    $("#readPlant").val(plant);

    _xLib.AJAX_Get("/api/KBNOR210/Page_Load", "",
        function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#tableMain").DataTable().clear().rows.add(success.data).draw();
            $("table thead tr th").css("text-align", "center");
            $("table tbody tr td").css("text-align", "center");
        },
        function (error) {
            console.log(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );


    await xSplash.hide();

});

$("#btnExit").click(function () {
    if (window.location.hostname.includes("tpcap")) {
        return window.location.replace("/kanban/OrderingProcess/KBNOR200");
    }
    return window.location.replace("/OrderingProcess/KBNOR200");
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
            { title: "Customer Order No", data: "F_PDS_No" },
            { title: "Supplier", data: "F_Supplier_Code" },
            { title: "Delivery Date", data: "F_Delivery_Date" },
            { title: "Part No.", data: "F_Part_No" },
            { title: "Qty", data: "F_QTY" },
            { title: "Store Code", data: "F_Store_CD" },
            { title: "Import Type", data: "F_Type" },
            { title: "Cust.Order Type ", data: "F_Customer_OrderType" },
        ],
        order: [[1, 'asc']]
    });

    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");
}

$("#btnInterface").click(function () {

    _xLib.AJAX_Post("/api/KBNOR210/Interface", "",
        function (success) {
            console.log(success);
            xSwal.success(success.response, success.message);
            $("#tableMain").DataTable().clear().draw();

            setTimeout(() => {
                _xLib.AJAX_Get("/api/KBNOR210/Page_Load", "",
                    function (success) {
                        success = _xLib.JSONparseMixData(success);
                        console.log(success);
                        $("#tableMain").DataTable().clear().rows.add(success.data).draw();
                        $("table thead tr th").css("text-align", "center");
                        $("table tbody tr td").css("text-align", "center");
                    },
                    function (error) {
                        console.log(error);
                        xSwal.success("Success", "Data has been refreshed");
                    }
                );
            }, 100);

            clearTimeout();

        },
        function (error) {
            console.log(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );

});

