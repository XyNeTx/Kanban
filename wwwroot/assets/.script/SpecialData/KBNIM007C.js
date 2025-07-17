$(document).ready(async function () {

    await _xDataTable.InitialDataTable("#tableMain",
        {
            columns: [
                {
                    title: "Flag", render(data, type, row) {
                        return `<input type="checkbox" class="chkbox" checked />`;
                    }
                },

                { title: "Order No.", data: "F_PDS_No" },
                { title: "Order Issued Date", data: "F_PDS_ISSUED_DATE" },
                { title: "Delivery Date", data: "F_Delivery_Date" },
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

//$("#selOrder").change(function () {
//    Search();
//});

//$("#selCreate").change(function () {
//    Search();
//});

$("#btnUpdCycle").click(function () {
    Update_Cycle();
});

$("#btnConfirm").click(async function () {
    await Confirm();
    $("#tableMain").DataTable().clear().draw();

    //setTimeout(() => {
    //    Search();
    //}, 100);

    //clearTimeout();
});

$("#btnSelectAll").click(function () {
    $(".chkbox").prop("checked", true);
});

$("#btnDeselectAll").click(function () {
    $(".chkbox").prop("checked", false);
});

$("#btnSearch").click(function () {
    Search();
});

async function GetPDS()
{
    let obj = ObjGet();

    _xLib.AJAX_Get("/api/KBNIM007C/GetPDS", obj,
        async function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#selOrder").addListSelectPicker(success.data, "F_PDS_No");
        },
    );
}

async function GetUser()
{
    let obj = ObjGet();

    _xLib.AJAX_Get("/api/KBNIM007C/GetUser", obj,
        async function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#selCreate").addListSelectPicker(success.data, "F_Update_By");
        },
    );
}

async function Search()
{
    let data = await ObjGet();

    _xLib.AJAX_Get("/api/KBNIM007C/GetListData", data,
        async function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        },
    );
}

async function Update_Cycle()
{
    _xLib.AJAX_Post("/api/KBNIM007C/Update_Cycle", null,
        async function (success) {
            xSwal.success("Success", "Update Cycle Success");
        },
    );
}

async function Confirm()
{
    var listObj = await _xDataTable.GetSelectedDataDT("#tableMain");

    if (listObj.length === 0) {
        xSwal.error("Error", "Please select at least one item.");
        return;
    }

    try {
        await _xLib.AJAX_Post("/api/KBNIM007C/Confirm", listObj,
            async function (success) {
                xSwal.success("Success", "Confirm Success");
            },
            async function (error) {
                console.log(error);
                xSwal.xError(error)
            }
        );
    }
    catch (ex) {
        console.log(ex, "Error");
        return xSwal.error("Cant Confirm Order");
    }

}

async function ObjGet() {
    let obj = {
        PDSNo: $("#selOrder").val(),
        User: $("#selCreate").val().split(":")[0],
        DeliDateChk: $("#chkboxDeliDate").is(":checked"),
    }

    if(obj.DeliDateChk)
    {
        obj.DeliDateFrom = $("#txtDateFrom").val();
        obj.DeliDateTo = $("#txtDateTo").val();
    }

    return obj;

}