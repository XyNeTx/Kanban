$(document).ready(async function () {
    await _xDataTable.InitialDataTable("#tblMaster",
        {
            columns: [
                {
                    title: `<input type='checkbox' class='chkBoxDT' id='chkAll'>`, data: null, orderable: false, width: "5%",
                    render: function (data, type, row) {
                        return "<input type='checkbox' class='chkBoxDT' data-id='" + row.F_PDS_No + "'>";
                    }
                },
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
    await ListData();
    xSplash.hide();
})



$("#btnGenerate").click(async function () {
    await GeneratePicking_Click();
});



$("#btnRegister").click(async function () {
    await Register();
});





async function ListData() {
    return await _xLib.AJAX_Get("/api/KBNOR360/List_Data", null,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            _xDataTable.ClearAndAddDataDT("#tblMaster", success.data);
        },
        function (error) {
            console.error(error);
            //xSwal.xError(error)
        }
    );
}



async function Register() {
    var dataList = _xDataTable.GetSelectedDataDT("#tblMaster");
    if (dataList.length == 0) {
        xSwal.error("Please select data to register.");
        return;
    }
    _xLib.AJAX_Post("/api/KBNOR360/Register", dataList,
        function (success) {
            xSwal.xSuccess("Registration successful.");
            _xLib.ClearData("#tblMaster");
            ListData();
        },
        function (error) {
            xSwal.xError(error);
        }
    );
}



async function GeneratePicking_Click() {
    _xLib.AJAX_Post("/api/KBNOR360/GeneratePicking_Click", null,
        function (success) {
            xSwal.xSuccess("Picking generation successful.");
            ListData();
            let obj = {
                Plant: _xLib.GetCookie("plantCode"),
                UserName: _xLib.GetUserName(),
                PI_Date_RemainShelf: success.data[1],
                PI_Time_RemainShelf: success.data[2],
            }
            _xLib.OpenReportObj("/KBNOR360", obj);
        },
        function (error) {
            xSwal.xError(error);
        }
    );



}
