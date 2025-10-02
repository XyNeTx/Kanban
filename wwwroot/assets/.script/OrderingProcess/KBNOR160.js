$(document).ready(async function () {

    $("#tblMaster").DataTable({
        columns: [
            { title: "No.", data: "", render: function (data, type, row, meta) { return meta.row + 1; }, orderable: false },
            { title: "Supplier", data: "F_Supplier_Code" },
            { title: "Store Code", data: "F_Store_Code" },
            { title: "Part No", data: "F_Part_No" },
            { title: "Kanban", data: "F_Kanban_No" },
            { title: "TMT Forecast", data: "F_TMT_FO" },
            { title: "HMMT Prod.Forecast", data: "F_HMMT_Prod" },
            { title: "HMMT Order Plan", data: "F_HMMT_Order" },
            { title: "Production Volume", data: "F_Cycle_Order" },
            { title: "MRP (Actual Production)", data: "F_MRP" },
        ],
        "paging": false,
        "info": false,
        width: '100%',
        scrollCollapse: true,
        scrollY: "350px",
        scrollX: true,
        order: [ 1, 'asc' ],
    });

    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");

    xAjax.onClick('btnExit', function () {
        xAjax.redirect('KBNOR100');
    });

    xSplash.hide();

    await _xLib.AJAX_Get('/api/KBNOR160/List_Data', { conditionDate: ($("#txtDate").val()).replaceAll("-",""), MRPRadio: $("input[type='radio']:checked").val() },
        function (success) {
            if (success.status === "200") {
                success.data = JSON.parse(success.data);
                success.data = _xLib.TrimArrayJSON(success.data);
                console.log(success.data);
                $("#tblMaster").DataTable().rows.add(success.data).clear().draw();
                $("table thead tr th").css("text-align", "center");
                $("table tbody tr td").css("text-align", "center");
            }
        },
        function (error) {
            xSplash.hide();
            xSwal.error("Error", error.responseJSON.message);
        }
    );

})

$("#txtDate , input[name='rdoOrderTypeName']").on('change', async function () {
    await _xLib.AJAX_Get('/api/KBNOR160/List_Data', { conditionDate: ($("#txtDate").val()).replaceAll("-", ""), MRPRadio: $("input[type='radio']:checked").val() },
        function (success) {
            if (success.status === "200") {
                success.data = JSON.parse(success.data);
                success.data = _xLib.TrimArrayJSON(success.data);
                console.log(success.data);
                $("#tblMaster").DataTable().clear().rows.add(success.data).draw();
                $("table thead tr th").css("text-align", "center");
                $("table tbody tr td").css("text-align", "center");

            }
        },
        function (error) {
            $("#tblMaster").DataTable().clear().draw();
            xSwal.error("Error", error.responseJSON.message);
        }
    );
});

$("#inputFile").change(async function (e) {
    const file = e.target.files[0];
    if (!file) return xSwal.error("Import File Error", "No file selected");

    const arrayBuffer = await file.arrayBuffer();
    const read = await XLSX.read(arrayBuffer);

    let newRead = read;
    for (var key in newRead.Sheets[newRead.SheetNames[0]]) {
        newRead.Sheets[newRead.SheetNames[0]][key].v = newRead.Sheets[newRead.SheetNames[0]][key].w;
    }

    const data = XLSX.utils.sheet_to_json(newRead.Sheets[newRead.SheetNames[0]]);

    _xLib.AJAX_Post('/api/KBNOR160/ImportData', JSON.stringify(data),
        function (success) {
            if (success.status == "200") {
                if (success.message.includes("Error") || success.message.includes("error")) {
                    return _xLib.OpenReport("/KBNIMERR", `UserID=${ajexHeader.UserCode}&Type=KBNOR160`);
                }
                xSwal.success("Success", success.message);
            }
        },
        function (error) {
            if (error.responseJSON.error.includes("same key value")) {
                return xSwal.error("Error", "Duplicate data found in the file.");
            }
            xSwal.error("Error", error.responseJSON.message);
        }
    );
    console.log("Data: ", data);
});