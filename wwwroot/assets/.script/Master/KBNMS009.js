$(document).ready(async function () {

    _xDataTable.InitialDataTable("tableSup",
    {
        "processing": false,
        "serverSide": false,
        width: '100%',
        paging: false,
        sorting: false,
        searching: false,
        scrollX: false,
        scrollY: "250px",
        scrollCollapse: true,
        "columns": [
        {
                title: "Supplier Code", data: "f_supplier_cd"
        },
        ],
        select: true,
        order: [[0, "asc"]]
    });

    _xDataTable.InitialDataTable("tableMain",
    {
        "processing": false,
        "serverSide": false,
        width: '100%',
        paging: false,
        sorting: false,
        searching: false,
        scrollX: false,
        scrollY: true,
        scrollCollapse: true,
        "columns": [
            {
                title: "Supplier Code", data: "f_Supplier_Code"
            },
            {
                title: "Supplier Plant", data: "f_Supplier_Plant"
            },
            {
                title: "Kanban No", data: "f_Kanban_No"
            },
            {
                title: "Dock", data: "f_Supply_Code"
            },
            {
                title: "Number", data: "f_Number"
            },
        ],
        select: false,
        order: [[0, "asc"]]
    });

    $("table tbody tr td , table thead tr th").addClass("text-center");

    await GetSupplier();

    xSplash.hide();
});


async function GetSupplier() {
    xSplash.show();
    await _xLib.AJAX_Get("/api/KBNMS009/GetSupplier", null,
        async function (success) {
            await _xDataTable.ClearAndAddDataDT("tableSup", success);
        }
    );
    xSplash.hide();
}

$(document).on("click", "#tableSup tbody tr", async function () {
    xSplash.show();
    var data = _xDataTable.GetDataDT("tableSup", this);
    await _xLib.AJAX_Get("/api/KBNMS009/SupplierClicked", { Supplier: data.f_supplier_cd },
        async function (success) {
            await _xDataTable.ClearAndAddDataDT("tableMain", success);
            if (success.length == 0) {
                xSwal.error("Data Not Found");
            }
        },
        async function (error) {
            xSwal.xError(error);
        }
    );
    xSplash.hide();
});

$(document).on("click", "#tableMain tbody tr td", async function () {
    console.log($(this).index());
    if ($(this).index() == 4) {
        var value = $(this).text();
        console.log(value);
        $(this).empty();
        $(this).append("<input type='text' class='form-control' value='" + value + "' />");
        $(this).find("input").focus();
    }
});

$(document).on("focusout", "#tableMain tbody tr td", async function () {
    var value = $(this).find("input").val();
    console.log(value);
    $("#tableMain").DataTable().cell(this).data(value).draw();

    var data = _xDataTable.GetDataDT("tableMain", this);
    console.log(data);
});


$("#btnSave").click(async function () {
    await Save();
});

$("#btnCancel").click(async function () {
    await GetSupplier();
    $("#tableMain").DataTable().clear().draw();
});

$("#btnPrint").click(async function () {
    let obj = $("#tableMain").DataTable().rows().data().toArray()[0];

    let reportObj = {
        F_Update_By: obj.f_Update_By,
    }

    if (obj == null) {
        xSwal.error("Warning", "No data to print");
        return;
    }

    _xLib.OpenReportObj("/KBNMS009", reportObj);

});

async function Save() {
    xSplash.show();
    var data = $("#tableMain").DataTable().rows().data().toArray();
    await _xLib.AJAX_Post("/api/KBNMS009/Save", data,
        async function (success) {
            xSwal.success("Success", "Data has been saved successfully");
            xSplash.hide();
        }
    );
}
