$(document).ready(async function () {

    await Initial();

    let loginDate = _xLib.GetCookie("loginDate");
    let plant = _xLib.GetCookie("plantCode");
    //console.log(loginDate);
    //console.log(plant);


    $("#readProcessDate").val(moment(loginDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));
    $("#readProcessShift").val(loginDate.slice(10, 11) == "D" ? "1:Day Shift" : "2:Night Shift");
    $("#readPlant").val(plant);

    await LoadDataChangeDelivery();



    xSplash.hide();

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
            { title: "Prod YM", data: "Supplier" },
            { title: "Customer OrderNo.", data: "Customer OrderNo." },
            { title: "Delivery Date", data: "Delivery Date" },
        ],
        order: [[1, 'asc']]
    });

    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");
}

async function LoadDataChangeDelivery() {
    let obj = {
        PDSNo : $("#inpCustomerOrderNo").val(),
        SuppCd : $("#inpSupplier").val(),
        PartNo : $("#inpPartNo").val(),
        chkDeli: $("#chkDate").is(":checked"),
        DeliFrom: moment($("#inpDateFrom").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        DeliTo : moment($("#inpDateTo").val(), "DD/MM/YYYY").format("YYYYMMDD"),
    }

    _xLib.AJAX_Get("/api/KBNOR210_1/LoadDataChangeDelivery", obj,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            //console.log(success);

            if (!$("#inpCustomerOrderNo").val())
            {
                $("#inpCustomerOrderNo").empty();
                $("#inpCustomerOrderNo").append("<option value='' hidden></option>");
                success.data.customer.forEach(function (item) {
                    $("#inpCustomerOrderNo").append(`<option value="${item.trim()}">${item.trim()}</option>`);
                });
            }

            if (!$("#inpSupplier").val()) {
                $("#inpSupplier").empty();
                $("#inpSupplier").append("<option value='' hidden></option>");
                success.data.supplier.forEach(function (item) {
                    $("#inpSupplier").append(`<option value="${item.trim()}">${item.trim()}</option>`);
                });
            }

            if (!$("#inpPartNo").val())
            {
                $("#inpPartNo").empty();
                $("#inpPartNo").append("<option value='' hidden></option>");
                success.data.partNo.forEach(function (item) {
                    $("#inpPartNo").append(`<option value="${item.trim()}">${item.trim()}</option>`);
                });
            }
        },
        function (error) {
            console.error(error);
        }
    );

}
$("#inpCustomerOrderNo").change(async function () {
    $("#inpSupplier").val("");
    $("#inpPartNo").val("");
    await LoadDataChangeDelivery();
});
$("#inpSupplier").change(async function () {
    $("#inpPartNo").val("");
    await GetSupplierName();
    await LoadDataChangeDelivery();
});
$("#inpPartNo").change(async function () {
    await GetPartName();
    await LoadDataChangeDelivery();
});

async function GetSupplierName() {
    _xLib.AJAX_Get("/api/KBNOR210_1/GetSupplierName", { SuppCd: $("#inpSupplier").val() },
        function (success) {
            $("#readSupplier").val(success.data);
        },
        function (error) {
            console.error(error);
        }
    );
}

async function GetPartName() {
    _xLib.AJAX_Get("/api/KBNOR210_1/GetPartName", { PartNo: $("#inpPartNo").val() },
        function (success) {
            $("#readPartNo").val(success.data);
        },
        function (error) {
            console.error(error);
        }
    );
}

$("#chkDate").change(async function () {
    if ($(this).prop("checked")) {
        $("#inpDateFrom").prop("disabled", false);
        $("#inpDateTo").prop("disabled", false);
    }
    else {
        $("#inpDateFrom").prop("disabled", true);
        $("#inpDateTo").prop("disabled", true);
    }
});