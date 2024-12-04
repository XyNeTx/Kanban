$(document).ready(async function () {

    await _xDataTable.InitialDataTable("#tableMain",
        {
            columns: [
                {
                    title: "Flag", render(data, type, row) {
                        return `<input type="checkbox" class="form-check-input" class="chkbox" checked />`;
                    }
                },

                { title: "Order No.", data: "F_Part_No" },
                { title: "Order Issued Date", data: "F_Store_CD" },
                { title: "Delivery Date", data: "F_Supplier" },
            ],
            scrollX: false,
            order: [[1, "asc"]],
            scrollCollapse: false,
            scrollY: 200,
        }
    );

    $("table thead th").addClass("text-center");
    $("table tbody td").addClass("text-center");

    $("#txtDateFrom").initDatepicker();
    $("#txtDateTo").initDatepicker();

    await GetPDS();
    await GetUser();

    xSplash.hide();
});

$("#chkboxDeliDate").change(function () {
    if (this.checked) {
        $("#txtDateFrom").prop("disabled", false);
        $("#txtDateTo").prop("disabled", false);
    } else {
        $("#txtDateFrom").prop("disabled", true);
        $("#txtDateTo").prop("disabled", true);
    }
});

async function GetPDS()
{
    _xLib.AJAX_Get("/api/KBNIM007C/GetPDS", null,
        async function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#selOrder").addListSelectPicker(success.data, "F_PDS_No");
        },
    );
}

async function GetUser()
{
    _xLib.AJAX_Get("/api/KBNIM007C/GetUser", null,
        async function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#selCreate").addListSelectPicker(success.data, "F_Update_By");
        },
    );
}