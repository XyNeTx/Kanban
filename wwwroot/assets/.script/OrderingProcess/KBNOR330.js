$(document).ready(async function () {

    await _xDataTable.InitialDataTable("#tableMain",
        {
            columns: [
                { title: "Customer PO", data: "F_PDS_No" },
                { title: "Part No", data: "F_Part_No" },
                { title: "Supplier", data: "F_Supplier_CD" },
                { title: "Short Name", data: "F_Short_name" },
                { title: "Store Code", data: "F_Store_CD" },
                { title: "Kanban No.", data: "F_Kanban_No" },
                { title: "Delivery Date", data: "F_Delivery_Date" },
                { title: "Delivery Trip", data: "F_Round" },
                { title: "Qty", data: "F_Qty" },
                { title: "Qty KB", data: "F_QTY_KB" },
                { title: "Import Type", data: "F_OrderType" },
            ],
            order: [[1, "asc"]],
            scrollCollapse: true,
        }
    );
    xSplash.hide();

    xAjax.onClick('btnExit', function () {
        xAjax.redirect('KBNOR300');
    });

    //$("#btnReport").click(function () {
    //    $("#exampleModal").modal('show');
    //})

    //$("#btnGenerate").click(function () {

    //})

    //$("#exampleModal").on('show.bs.modal', async function () {
    //    //console.log($("#exampleModal .modal-body .dataTables_scrollHead").length);
    //    if ($("#exampleModal .modal-body .dataTables_scrollHead").length == 0) {
    //        await _xDataTable.InitialDataTable("#tableModal",
    //            {
    //                columns: [
    //                    { title: "Supplier Code", data: "F_Supplier_Code" },
    //                    { title: "Part No", data: "F_Part_No" },
    //                    { title: "Store Code", data: "F_Store_Code" },
    //                    { title: "Kanban No", data: "F_Kanban_No" },
    //                    { title: "Delivery Date", data: "F_Delivery_Date" },
    //                    { title: "Delivery Shift", data: "F_Delivery_Shift" },
    //                    { title: "Delivery Round", data: "F_Delivery_Round" },
    //                    { title: "KB Qty (PCS)", data: "F_Qty" },
    //                    { title: "CKD Remain Qty (PCS)", data: "CKD_Remain_Qty" }
    //                ],
    //                order: [[1, "asc"]],
    //                scrollCollapse: true,
    //            }
    //        );
    //    }
    //});

})

var _dt = "";

async function Generate() {
    _xLib.AJAX_Post("/api/KBNOR330/Generate", "",
        async function (success) {
            success = _xLib.JSONparseMixData(success);
            if (success.message.includes("ไม่สามารถ Generate PDS สำหรับ CKD Order ได้")) {
                $("#exampleModal").modal('show');
                if ($("#exampleModal .modal-body .dataTables_scrollHead").length == 0) {
                    await _xDataTable.InitialDataTable("#tableModal",
                        {
                            columns: [
                                { title: "Supplier Code", data: "F_Supplier_Code" },
                                { title: "Part No", data: "F_Part_No" },
                                { title: "Store Code", data: "F_Store_Code" },
                                { title: "Kanban No", data: "F_Kanban_No" },
                                { title: "Delivery Date", data: "F_Delivery_Date" },
                                { title: "Delivery Shift", data: "F_Delivery_Shift" },
                                { title: "Delivery Round", data: "F_Delivery_Round" },
                                { title: "KB Qty (PCS)", data: "F_Qty" },
                                { title: "CKD Remain Qty (PCS)", data: "CKD_Remain_Qty" }
                            ],
                            order: [[1, "asc"]],
                            scrollCollapse: true,
                        }
                    );
                }
                _xDataTable.ClearAndAddDataDT("#tableModal", success.data);
            }
            else {
                await xSwal.xSuccessAwait(success);
            }
        },
        async function (error) {
            await xSwal.xErrorAwait(error);
        }
    )
}

