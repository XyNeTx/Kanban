$(document).ready(async function () {

    $("#tblCOMPLETE").DataTable({
        columns: [
            {
                title: "No.", data: "", searchable: false, sortable: false, render: function (data, type, row, meta) {
                    return meta.row + 1;
                }
            },
            { title: "List Name of File", data: "F_File_Name" },
        ],
        "scrollX": true,
        "scrollY": "300px",
        "scrollCollapse": true,
        "paging": false,
        "ordering": false,
        "info": false,
        "searching": false,
    });

    $("#tblERROR").DataTable({
        columns: [
            {
                title: "No.", data: "", searchable: false, sortable: false, render: function (data, type, row, meta) {
                    return meta.row + 1;
                }
            },
            { title: "List Name of File", data: "F_File_Name" },
        ],
        "scrollX": true,
        "scrollY": "300px",
        "scrollCollapse": true,
        "paging": false,
        "ordering": false,
        "info": false,
        "searching": false,
    });

    xSplash.hide();
});

let files = [];
$("#inputFile").on("change", function (e) {
    files = e.target.files;
    $("#tblCOMPLETE").DataTable().clear().draw();
    $("#tblERROR").DataTable().clear().draw();
});

$("#btnImport").on("click", async function () {
    if (files.length == 0) {
        xSwal.error("Error", "Please select file to import.");
        return;
    }
    //for (let i = 0; i < files.length; i++) {
    console.log(files);
    for (const file of files) {
        await processFile(file);
        await AfterImported();
    }
});

async function processFile(file) {
    try {
        //console.log(file);
        const arrayBuffer = await file.arrayBuffer();
        const read = await XLSX.read(arrayBuffer);

        let newRead = read;
        for (var key in newRead.Sheets[newRead.SheetNames[0]]) {
            newRead.Sheets[newRead.SheetNames[0]][key].v =
                newRead.Sheets[newRead.SheetNames[0]][key].w;
        }

        const data = XLSX.utils.sheet_to_json(newRead.Sheets[read.SheetNames[0]]);

        var filterData = data.filter(f => !Object.values(f).includes("<EOF>"));
        if (filterData.some(f => Object.keys(f).includes("PO_Item_No."))) {
            filterData = filterData.map(m => {
                m["PO_Item_No."] = m["PO_Item_No."].toString();
                m["PO_Date"] = m["PO_Date"].toString();
                m["Order_type"] = m["Order_type"].toString();
                m["Depot_Code_:"] = m["Depot_Code_:"].toString();
                return m;
            });
        }

        //console.log(filterData);

        await _xLib.AJAX_Post("/api/KBNIM001/ImportData", JSON.stringify(filterData),
            async function (success) {
                await $("#tblCOMPLETE").DataTable().row.add({ F_File_Name: file.name }).draw();
            },
            async function (error) {
                await $("#tblERROR").DataTable().row.add({ F_File_Name: file.name }).draw();
            }
        );

    } catch (_error) {
        xSwal.error("Import File Error", _error);
        console.log("Error: ", _error);
    }
}


async function AfterImported() {
    var AdvanceDate = $("#AdvanceDate").val();
    await _xLib.AJAX_Get("/api/KBNIM001/AfterImported", { advDate: AdvanceDate },
        async function (success) {
            return await xSwal.success("Success", success.message);
        },
        async function (error) {
            xSwal.xError(error);
            if (error.responseJSON.message.includes("Report")) {
                var obj = {
                    UserID: ajexHeader.UserCode,
                    Type: "KBNIM001"
                };

                return _xLib.OpenReportObj("/KBNIMERR", obj);

            }
        }
    );
}