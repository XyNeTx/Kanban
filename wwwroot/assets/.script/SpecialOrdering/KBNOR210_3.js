$(document).ready(async function () {

    await Initial();

    let loginDate = _xLib.GetCookie("loginDate");
    let plant = _xLib.GetCookie("plantCode");
    //console.log(loginDate);
    //console.log(plant);


    $("#readProcessDate").val(moment(loginDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));
    $("#readProcessShift").val(loginDate.slice(10, 11) == "D" ? "1:Day Shift" : "2:Night Shift");
    $("#readPlant").val(plant);

    await LoadOrderNo();

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
            { title: "Prod YM", data: "f_ProdYM" },
            { title: "Customer OrderNo.", data: "f_PDS_No" },
            { title: "Delivery Date", data: "f_Delivery_Date" },
        ],
        order: [[1, 'asc']]
    });

    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");
}

async function LoadOrderNo() {
    await _xLib.AJAX_Get("/api/KBNOR210_3/LoadOrderNo", "",
        function (success) {
            console.log(success);
            //success.data = _xLib.TrimArrayJSON(success.data);
            $("#inpNewCustomerOrderNo").empty();
            $("#inpNewCustomerOrderNo").append("<option value='' hidden></option>");
            success.data.forEach(function (item) {
                //console.log(item);
                $("#inpNewCustomerOrderNo").append(`<option value="${item.trim()}">${item.trim()}</option>`);
            });
            $("#inpNewCustomerOrderNo").selectpicker("refresh");
        },
        function (error) {
            console.error(error);
            xSwal.error(error.responseJSON.response,error.responseJSON.message);
        }
    );
};

async function LoadCustomerPO() {

    await _xLib.AJAX_Get("/api/KBNOR210_3/LoadCustomerPO", { NewCusPO: $("#inpNewCustomerOrderNo").val()},
        function (success) {
            console.log(success);
            success.data = _xLib.TrimArrayJSON(success.data);
            $("#tableMain").DataTable().clear().rows.add(success.data).draw();
            $("#tableMain tbody tr td").css("text-align", "center");
            $("#tableMain thead tr th").css("text-align", "center");
            $("#btnUnmerge").prop("disabled", false);
        },
        function (error) {
            console.error(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );

};

$("#inpNewCustomerOrderNo").change(async function () {
    await LoadCustomerPO();
});

$("#btnUnmerge").click(async function () {

    $("#btnUnmerge").prop("disabled", true);
    await xSplash.show();

    let listObj = $("#tableMain").DataTable().rows().data().toArray();
    for (let i = 0; i < listObj.length; i++) {
        listObj[i].f_PDS_No_New = $("#inpNewCustomerOrderNo").val();
    }

    if (listObj.length == 0) {
        xSwal.error("Error", "No data to unmerge.");
        return;
    }

    _xLib.AJAX_Post("/api/KBNOR210_3/Unmerge", JSON.stringify(listObj),
        async function (success) {
            await xSplash.hide();
            console.log(success);
            xSwal.success("Success", success.message);
            $("#inpNewCustomerOrderNo").val("");
            $("#tableMain").DataTable().clear().draw();
        },
        async function (error) {
            await xSplash.hide();
            console.error(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );
});