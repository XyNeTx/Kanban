$(document).ready(async function () {
    await _xDataTable.InitialDataTable("#tblMaster",
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
                { title: "PDS No.", data: "F_OrderNo" },
                { title: "Supplier", data: "F_Supplier_Code" },
                { title: "Delivery Date", data: "F_Delivery_Date" },
                { title: "Delivery Trip", data: "F_Delivery_Trip" },
                { title: "Register", data: "Flg_Register" },
                { title: "Picking", data: "Flg_Picking" },
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
            console.log(success)
            _xDataTable.ClearAndAddDataDT("#tblMaster", success.data);
        },
        function (error) {
            console.error(error);
            //xSwal.xError(error)
        }
    );
}



async function Register() {
    var dataList = _xDataTable.GetAllDataDT("#tblMaster");
    if (dataList.length == 0) {
        xSwal.error("Please select data to register.");
        return;
    }

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

    //return console.log(dataList);
    _xLib.AJAX_Post("/api/KBNOR360/Register", dataList,
        function (success) {

            clearInterval(loading);
            $("#prgProcess").css("width", `100%`);
            $("#prgProcess").attr("aria-valuenow", 100);
            $("#prgProcess_label_").text(`Process : 100.00 %`);

            xSwal.success("Registration successful.");
            _xLib.ClearData("#tblMaster");
            ListData();
        },
        function (error) {

            clearInterval(loading);
            $("#prgProcess_label_").text(`Process : Error`);

            xSwal.xError(error);
        }
    );
}



async function GeneratePicking_Click() {

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

    _xLib.AJAX_Post("/api/KBNOR360/GeneratePicking_Click", null,
        function (success) {

            clearInterval(loading);
            $("#prgProcess").css("width", `100%`);
            $("#prgProcess").attr("aria-valuenow", 100);
            $("#prgProcess_label_").text(`Process : 100.00 %`);


            xSwal.success("Picking generation successful.");
            ListData();
            let obj = {
                Plant: _xLib.GetCookie("plantCode"),
                UserName: _xLib.GetUserName(),
                PI_Date_RemainShelf: success.data[1],
                PI_Time_RemainShelf: success.data[2],
            }
            if (obj.PI_Date_RemainShelf == "" && obj.PI_Time_RemainShelf == "") {
                return
            }
            else {
                _xLib.OpenReportObj("/KBNOR360", obj);
            }
        },
        function (error) {

            clearInterval(loading);
            $("#prgProcess_label_").text(`Process : Error`);

            xSwal.xError(error);
        }
    );



}
