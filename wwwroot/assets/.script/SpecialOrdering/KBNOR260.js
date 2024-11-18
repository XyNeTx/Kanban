$(document).ready(async () => {
    await _xDataTable.InitialDataTable("#tableMain",
        {
            columns: [
                {
                    title: "Flag", render: function (data, type, row, meta) {
                        return "<input type='checkbox' class='chkFlag' data-id='" + row.f_OrderNo + "' checked/>";
                    }
                },
                { title: "PDS No", data: "f_OrderNo" },
                { title: "Customer OrderNo", data: "f_PO_Customer" },
                { title: "Supplier Code", data: "f_Supp_CD" },
                { title: "Delivery Date", data: "f_Delivery_Date" },
                { title: "Status", data: "f_Status" },
                {
                    title: "Approve", data: "f_Approver", render: function () {
                        return "";
                    }
                }
            ],
            order: [[1, "asc"]],
            scrollCollapse: false,
        }
    );

    await _xDataTable.InitialDataTable("#modalTableMain",
        {
            columns: [
                {
                    title: "Flag", render: function (data, type, row, meta) {
                        return "<input type='checkbox' class='chkFlag' data-id='" + row.f_OrderNo + "' checked/>";
                    }
                },
                { title: "PDS No", data: "f_OrderNo" },
                { title: "Survey Doc.", data: "f_PO_Customer" },
                { title: "Supplier Code", data: "f_Supp_CD" },
                { title: "Delivery Date", data: "f_Delivery_Date" },
                { title: "Status", data: "f_Status" },
                { title: "Approve", data: "f_Approver" }
            ],
            order: [[1, "asc"]],
            scrollCollapse: false,
        }
    );

    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");

    await GetApproverList();
    await GetPDSDataNoApprove();

    xSplash.hide();
});

$("#btnChkAll").click(async () => {
    $(".chkFlag").prop("checked", true);
});
$("#btnUnChkAll").click(async () => {
    $(".chkFlag").prop("checked", false);
});

$("#modalBtnChkAll").click(async () => {
    $("#modalTableMain .chkFlag").prop("checked", true);
});
$("#modalBtnUnChkAll").click(async () => {
    $("#modalTableMain .chkFlag").prop("checked", false);
});

$("#btnSendApprove").click(async () => {
    await SendApprove();
});

GetPDSDataNoApprove = async () => {
    let getQuery = {
        fac: _xLib.GetCookie("plantCode")
    }
    _xLib.AJAX_Get("/api/KBNOR260/GetPDSDataNoApprove", getQuery,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            await _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        }
    );
}

GetApproverList = async () => {
    _xLib.AJAX_Get("/api/KBNOR260/GetApproverList", null,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);

            $("#selApprove").addListSelectPicker(success.data, "F_Name");
            $("#modalSelApprove").addListSelectPicker(success.data, "F_Name")
        }
    );
}

SendApprove = async () => {
    if (!$("#selApprove").val()) {
        await xSwal.error("Error !!!", "Please select approver.");
        return;
    }
    let postQuery = _xDataTable.GetSelectedDataDT("#tableMain");
    postQuery.forEach((item) => {
        item.f_Approver = $("#selApprove").val();
    });

    console.log(postQuery);

    _xLib.AJAX_Post("/api/KBNOR260/SendApprove", postQuery,
        async (success) => {
            await xSwal.success(success.response, success.message);
            await GetPDSDataNoApprove();
        }
    );
}

$(document).on('show.bs.modal', '#KBNOR260_1', async (e) => {
    xSplash.show();
    await $("#KBNOR260_1").on('shown.bs.modal', async (e) => {
        $("#modalTableMain").DataTable().columns.adjust().clear().draw();
        $("table thead tr th").css("text-align", "center");
        $("table tbody tr td").css("text-align", "center");
        await GetPDSWaitApprove();
        xSplash.hide();
    });
});

GetPDSWaitApprove = async () => {
    let getQuery = {
        fac: _xLib.GetCookie("plantCode")
    }
    _xLib.AJAX_Get("/api/KBNOR260/GetPDSWaitApprove", getQuery,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            await _xDataTable.ClearAndAddDataDT("#modalTableMain", success.data);
        }
    );

}

$("#modalBtnResend").click(async () => {
    if (!$("#modalSelApprove").val()) {
        await xSwal.error("Error !!!","Please select approver.");
        return;
    }
    let postQuery = _xDataTable.GetSelectedDataDT("#modalTableMain");
    postQuery.forEach((item) => {
        item.f_Approver = $("#modalSelApprove").val();
    });

    console.log(postQuery);

    _xLib.AJAX_Post("/api/KBNOR260/SendApprove", postQuery,
        async (success) => {
            await xSwal.success(success.response, success.message);
            await GetPDSDataNoApprove();
        }
    );
});