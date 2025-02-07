let chkDateFrom = "";
let chkDateTo = "";
$(document).ready(function () {

    _xDataTable.InitialDataTable("#tableMain",
        {
            "processing": false,
            "serverSide": false,
            width: '100%',
            paging: false,
            sorting: false,
            searching: false,
            scrollX: false,
            scrollY: "200px",
            scrollCollapse: false,
            "columns": [
                {
                    title: "Flag", render(data, type, row) {
                        return `<input type="checkbox" class="chkbox" id="chkbox" name="chkbox">`;
                    }
                },
                {
                    title: "Order No.", data: "f_PDS_No"
                },
                {
                    title: "PDS Issued Date", data: "f_PDS_Issued_Date"
                },
                {
                    title: "Delivery Date", data: "f_Delivery_Date"
                },
            ],
            select: false,
            order: [[0, "asc"]]
        });

    $("table tbody tr td").addClass("text-center");
    $("table thead tr th").addClass("text-center");

    $(".datepicker").each(function () {
        $(this).initDatepicker();
    });


    GetOrderNo();

    xSplash.hide();
    chkDateFrom = $("#inpDeliveryDateFrom").val();
    chkDateTo = $("#inpDeliveryDateTo").val();

});

$("#btnSel").click(function () {
    $(".chkbox").prop("checked", true);
});
$("#btnDesel").click(function () {
    $(".chkbox").prop("checked", false);
});

$("#chkSearch").change(function () {
    GetOrderNo();
    $("#inpDeliveryDateFrom").prop("readonly", !$("#chkSearch").prop("checked"));
    $("#inpDeliveryDateTo").prop("readonly", !$("#chkSearch").prop("checked"));
});

$("#inpDeliveryDateFrom").change(function () {
    if (chkDateFrom !== $(this).val()) {
        chkDateFrom = $(this).val();
        GetOrderNo();
    }
});
$("#inpDeliveryDateTo").change(function () {
    if (chkDateTo !== $(this).val()) {
        chkDateTo = $(this).val();
        GetOrderNo();
    }
});

$("#btnSearch").click(function () {
    Search();
});

$("#btnUpd").click(function () {
    UpdateCycle();
});

$("#btnConf").click(function () {
    Confirm();
});

function GetOrderNo() {

    let obj = {
        isChkDate: $("#chkSearch").prop("checked"),
        DateFrom: $("#inpDeliveryDateFrom").val(),
        DateTo: $("#inpDeliveryDateTo").val(),
    }

    _xLib.AJAX_Get("/api/KBNIM001C/GetOrderNo", obj,
        function (success) {
            console.log(success);
            $("#inpOrderNo").addListSelectPicker(success.data, "f_PDS_No");
        },
        function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        });
}

function Search() {
    let obj = {
        OrderNo: $("#inpOrderNo").val(),
        DateFrom: $("#inpDeliveryDateFrom").val(),
        DateTo: $("#inpDeliveryDateTo").val(),
        isChkDate : $("#chkSearch").prop("checked")
    }


    _xLib.AJAX_Get("/api/KBNIM001C/Search", obj,
        function (success) {
            console.log(success);
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        },
        function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        });

}

async function UpdateCycle() {
    var isConfirm = await xSwal.confirm("Are you sure to update the cycle?");

    if (isConfirm) {

        _xLib.AJAX_Post("/api/KBNIM001C/UpdateCycle", null,
            function (success) {
                console.log(success);
                xSwal.success(success.response, success.message);
                Search();
            },
            function (error) {
                xSwal.error(error.responseJSON.response, error.responseJSON.message);
            }
        );

    }

}

async function Confirm() {
    var isConfirm = await xSwal.confirm("Are you sure to Confirm Order Service?");

    var listData = _xDataTable.GetSelectedDataDT("#tableMain");

    if (isConfirm) {
        _xLib.AJAX_Post("/api/KBNIM001C/Confirm", listData,
            function (success) {
                console.log(success);
                xSwal.success(success.response, success.message);
                Search();
            },
            function (error) {
                xSwal.error(error.responseJSON.response, error.responseJSON.message);
            }
        );
    }

 }