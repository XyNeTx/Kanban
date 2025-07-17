$(document).ready(async function () {

    await _xDataTable.InitialDataTable("#tableMain",
        {
            columns: [
                {
                    title: "No", data: "", render: function (data, type, row, meta) {
                        console.log("data:", data);
                        console.log("type:", type);
                        console.log("row:", row);
                        console.log("meta:", meta);
                        return meta.row + 1;
                    }
                },
                { title: "Supplier", data: "F_Supplier_Code" },
                { title: "Cycle Time", data: "F_Delivery_Cycle" },
                { title: "Delivery Date", data: "F_Delivery_Date" },
                { title: "Delivery Trip", data: "F_Delivery_Trip" },
                { title: "PDS No. HMMT", data: "F_OrderNo" },
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

$("#btnGenerate").click(async function () {
    await Generate();
});

var _dt = "";

async function Generate() {
    let percent = 0;

    // ตั้งค่าจุดเริ่มต้น
    $("#prgProcess").css("width", `0%`);
    $("#prgProcess_label_").text(`Process : 0.00 %`);

    let loading = setInterval(function () {
        percent = Math.min(percent + 10, 95); // วิ่งจนถึง 95% ระหว่างรอ API
        $("#prgProcess").css("width", `${percent}%`);
        $("#prgProcess").attr("aria-valuenow", percent);
        $("#prgProcess_label_").text(`Process : ${percent.toFixed(2)} %`);
    }, 100);

    _xLib.AJAX_Post("/api/KBNOR330/Generate", "",
        async function (success) {
            success = _xLib.JSONparseMixData(success);

            clearInterval(loading);
            $("#prgProcess").css("width", `100%`);
            $("#prgProcess").attr("aria-valuenow", 100);
            $("#prgProcess_label_").text(`Process : 100.00 %`);

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
                let jsonData = _xLib.JSONparseMixData(success);
                console.log(jsonData)
                _xDataTable.ClearAndAddDataDT("#tableModal", jsonData.data);
            }
            else {
                let jsonData = _xLib.JSONparseMixData(success);
                console.log(jsonData)
                _xDataTable.ClearAndAddDataDT("#tableMain", jsonData.data);
                await xSwal.xSuccessAwait(success);
            }
        },
        async function (error) {
            clearInterval(loading);
            $("#prgProcess_label_").text(`Process : Error`);
            await xSwal.xErrorAwait(error);
        }
    )
}

