$(document).ready(async () => {

    await _xDataTable.InitialDataTable("#tableMain",
        {
            columns: [
                {
                    title: "Flag", render: function (data, type, row, meta) {
                        return "<input type='checkbox' class='chkFlag' data-id='" + row.F_OrderNO + "' checked/>";
                    }
                },
                { title: "PDS No", data: "F_OrderNO" },
                { title: "PDS Issued Date", data: "F_PDS_Issued" },
                { title: "Supplier Code", data: "F_Supplier_CD" },
                { title: "Issued By", data: "F_Issued_By" },
                { title: "Send Approve Date", data: "F_Send_Date" },
                {
                    title: "Preview", render: function (data, type, row, meta) {
                        return "<a class='btn text-light btn-rounded btn-primary' onclick=Preview($(this))>Preview</a>";
                    }
                }
            ],
            order: [[1, "asc"]],
            scrollCollapse: false,
        }
    );

    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");

    await GetPDSWaitApprove();

    xSplash.hide();
});


$("#btnChkAll").click(async () => {
    $(".chkFlag").prop("checked", true);
});
$("#btnUnChkAll").click(async () => {
    $(".chkFlag").prop("checked", false);
});

$("#btnApprove").click(async () => {
    await Approve();
});

Approve = async () => {
    let data = _xDataTable.GetSelectedDataDT("#tableMain");
    if (data.length == 0) {
        xSwal.error("Please select data.");
        return;
    }

    _xLib.AJAX_Post(`/api/KBNOR261/Approve`, data,
        async (success) => {
            xSwal.success(success.response, success.message);
            $("#tableMain").DataTable().clear().draw();
            //GetPDSWaitApprove();
        },
        async (error) => {
            xSwal.xError(error);
        }
    );


}

Preview = async (e) => {
    console.log(e);
    let row = $(e).closest("tr");
    let data = _xDataTable.GetDataDT("#tableMain", row);
    console.log(data);

    _xLib.AJAX_Post(`/api/KBNOR261/Preview`, data,
        async (success) => {
            console.log(success);
            xSwal.success(success.response, success.message);
            let link = `http://hmmta-tpcap/E-Report/Report.aspx?Register=REC&PDSNoFrom=${data.F_OrderNO}&PDSNoTo=${data.F_OrderNO}&DateFrom=2024-07-01&DateTo=2999-12-31`;

            window.open(link, "_blank");
        },
        async (error) => {
            xSwal.xError(error);
        }
    );
}

GetPDSWaitApprove = async () => {
    _xLib.AJAX_Get(`/api/KBNOR261/GetPDSWaitApprove`, null,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        }
    );
};