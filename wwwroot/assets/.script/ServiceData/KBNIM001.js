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
    for (let i = 0; i < files.length; i++) {
        const file = files[i];
        try {

            const arrayBuffer = await file.arrayBuffer();
            const read = await XLSX.read(arrayBuffer);
            const data = XLSX.utils.sheet_to_json(read.Sheets[read.SheetNames[0]]);

            var filterData = data.filter(f => !Object.values(f).includes("<EOF>"));
            if (filterData.some(f => Object.keys("PO_Item_No"))) {
                filterData = filterData.map(m => {
                    m["PO_Item_No."] = m["PO_Item_No."].toString();
                    m["PO_Date"] = m["PO_Date"].toString();
                    m["Order_type"] = m["Order_type"].toString();
                    m['Depot_Code_:'] = m["Depot_Code_:"].toString();
                    return m;
                });
            }

            console.log(filterData);

            _xLib.AJAX_Post("/api/KBNIM001C/ImportData", JSON.stringify(filterData),
                function (success) {
                    console.log(success);
                    return $("#tblCOMPLETE").DataTable().row.add({ F_File_Name: file.name }).draw();
                },
                function (error) {
                    console.log(error);
                    return $("#tblERROR").DataTable().row.add({ F_File_Name: file.name }).draw();
                }
            );
        }
        catch (_error) {
            return xSwal.error("Import File Error", _error);
        }
    }
});