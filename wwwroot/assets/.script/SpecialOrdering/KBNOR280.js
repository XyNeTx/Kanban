$(document).ready(async () => {
    await _xDataTable.InitialDataTable("#tableMain",
        {
            columns: [
                {
                    title: "Flag", render: function (data, type, row, meta) {
                        return "<input type='checkbox' class='chkFlag' checked/>";
                    }
                },
                { title: "PDS No", data: "F_OrderNo" },
                { title: "Supplier Code", data: "F_Supp_CD" },
                { title: "Cycle Time", data: "F_Delivery_Cycle" },
                { title: "Delivery Date", data: "F_Delivery_Date" },
                { title: "Delivery Trip", data: "F_Delivery_Trip" },
            ],
            order: [[1, "asc"]],
            scrollCollapse: false,
        }
    );

    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");

    await GetPDSData();

    xSplash.hide();
});

$("#btnChkAll").click(() => {
    $(".chkFlag").prop("checked", true);
});
$("#btnUnChkAll").click(() => {
    $(".chkFlag").prop("checked", false);
});

$("#mthDeliYM").change(async () => {
    await GetPDSData();
});

$("#btnRegister").click(async () => {
    await Register();
    await GetPDSData();
});

GetPDSData = async () => {
    let GetQuery = {
        DeliYM : $("#mthDeliYM").val().replace("-", ""),
    };

    _xLib.AJAX_Get("/api/KBNOR280/GetPDSData", GetQuery,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            await _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        }
    );
}

Register = async () => {
    xSplash.show();
    let PostData = [];

    $("#tableMain tbody tr").find(".chkFlag:checked").each((i, e) => {
        //console.log($(this));
        //console.log($(e).closest("tr"));
        //var row = $(this).closest("tr");
        let data = $("#tableMain").DataTable().row($(e).closest("tr")).data();
        //console.log(data);
        PostData.push(data);
    });

    //return console.log(PostData);

    _xLib.AJAX_Post("/api/KBNOR280/Register", PostData,
        async (success) => {
            await xSplash.hide();
            await xSwal.success(success.response, success.message);
        },
        async (error) => {
            console.error(error);
        }
    );


};