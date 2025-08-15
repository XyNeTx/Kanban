$(document).ready(async function () {
    $(".btn-toolbar").remove();

    await _xDataTable.InitialDataTable("#tableImpData",
        {
            columns: [
                {
                    title: "No", data: "",width: "7%", render: function (data, type, row, meta) {
                        //console.log("data:", data);
                        //console.log("type:", type);
                        //console.log("row:", row);
                        //console.log("meta:", meta);
                        return meta.row + 1;
                    }
                },
                { title: "Part No", data: "L1", width: "17%" },
                { title: "Sebango", data: "V1", width: "12%" },
                { title: "Part Name", data: "M1", width: "24%" },
                { title: "Packs", data: "N1", width: "10%" },
                {
                    title: "Delivery Qty", data: "O1", width: "10%"/*, render: function (data, type, row) {*/
                        //console.log("row[Delivery Qty]", row["Delivery Qty."]);
                        //return row["Delivery Qty."] ? row["Delivery Qty."] : row.Packs * row["Lot Size"];
                    //}
                },
                { title: "Unit Price", data: "P1", width: "10%" },
                //{ title: "Lot Size", data: "Lot Size", width: "10%" },
            ],
            order: [[0, "asc"]],
            scrollCollapse: false,
            width: "100%",
            scrollX: true,
            scrollY: '325px',
        }
    );

    await _xDataTable.InitialDataTable("#tableOrdData",
        {
            columns: [
                {
                    title: "No", data: "", render: function (data, type, row, meta) {
                        //console.log("data:", data);
                        //console.log("type:", type);
                        //console.log("row:", row);
                        //console.log("meta:", meta);
                        return meta.row + 1;
                    }
                },
                {
                    title: "PDS No", data: "f_PDS_No"
                },
                {
                    title: "Supplier Code", data: "f_Supplier_CD"
                },
                {
                    title: "Part No", data: "f_Part_No"
                },
                //{
                //    title: "Ruibetsu", data: "f_Ruibetsu"
                //},
                {
                    title: "Kanban No", data: "f_Kanban_No"
                },
                {
                    title: "Store Code", data: "f_Store_CD"
                },
                {
                    title: "Part Name", data: "f_Part_Name"
                },
                {
                    title: "Delivery Date", data: "f_Delivery_Date"
                },
                {
                    title: "Part No Order", data: "f_Part_Order"
                },
                //{
                //    title: "Ruibetsu Order", data: "f_Ruibetsu_Order"
                //},
                {
                    title: "Store Order", data: "f_Store_Order"
                },
                {
                    title: "Part Name Order", data: "f_Name_Order"
                },
                {
                    title: "Qty", data: "f_Qty."
                },
            ],
            order: [[0, "asc"]],
            scrollCollapse: false,
            width: "100%",
            scrollX: true,
            scrollY: '325px',
        }
    );

    xSplash.hide();
});

$("#btnOrd").click(async function () {
    $("#divOrdData").addClass("d-none");
    $("#divImpData").removeClass("d-none");
    $("#tableImpData").DataTable().columns.adjust().draw();
});
$("#btnImp").click(async function () {
    $("#divImpData").addClass("d-none");
    $("#divOrdData").removeClass("d-none");

    $("#tableOrdData").find("td").each((i,e) => {
        //console.log($(this));
        //console.log(e);
        $(e).addClass("p-2");
        //$(this).addClass("p-2");
    });

    $("#tableOrdData").DataTable().columns.adjust().draw();
});

$("#inpFiles").change(async function (e) {
    xSplash.show("Processing file...");
    let _arrData = [];
    let _headers = ["A1", "B1", "C1", "D1", "E1", "F1", "G1", "H1", "I1", "J1", "K1", "L1", "M1", "N1", "O1", "P1", "Q1", "R1", "S1", "T1", "U1", "V1", "W1", "X1", "Y1", "Z1"
        , "AA1", "AB1", "AC1", "AD1", "AE1", "AF1", "AG1", "AH1", "AI1", "AJ1", "AK1", "AL1", "AM1", "AN1", "AO1", "AP1", "AQ1", "AR1", "AS1", "AT1", "AU1", "AV1", "AW1", "AX1"];


    for (let i = 0; i < e.target.files.length; i++)
    {
        let file = e.target.files[i];

        var workbook = XLSX.read(await file.arrayBuffer(), { type: 'array' });
        var sheetName = workbook.SheetNames[0]; // Assuming you want the first sheet
        var worksheet = workbook.Sheets[sheetName];
        var range = XLSX.utils.decode_range(worksheet['!ref']);
        range.s.r = 0; // Start from the second row to skip headers
        range.e.r = range.e.r + 1; // Keep the end row the same
        worksheet['!ref'] = XLSX.utils.encode_range(range); // Update the range to exclude the first row
        console.log(worksheet);

        for (let i = Object.keys(worksheet).length - 1; i >= 0; i--) {
            //for (let cell in worksheet) {
            let cell = Object.keys(worksheet)[i];
            console.log(cell);
            if (cell[0] === '!') continue; // Skip metadata cells
            //console.log("cell:", cell, "value:", worksheet[cell].v);
            let match = cell.match(/^([A-Z]+)([0-9]+)$/);
            let col = "";
            let row = 0;
            if (match) {
                col = match[1]; // column part (e.g., "AA")
                row = parseInt(match[2]) + 1; // row part as number (e.g., 10)
            }
            let newCell = col + row.toString();
            if (worksheet[cell].v !== undefined) {
                console.log("cell" + cell);
                console.log("newCell" + newCell);
                if (worksheet[cell].v === undefined) worksheet[cell].v = ""; // Ensure all cells have a value
                let cellValue = worksheet[cell].w;
                console.log(worksheet[cell]);
                console.log(worksheet[newCell]);
                //if (workbook.Sheets[sheetName][newCell].v === undefined) return;
                if (worksheet[newCell] === undefined) {
                    worksheet[newCell] = {
                        w: worksheet[cell].w.replace(/[\u200B-\u200D\uFEFF']/g, ''),
                        t: worksheet[cell].t.replace(/[\u200B-\u200D\uFEFF']/g, ''),
                        v: worksheet[cell].w.replace(/[\u200B-\u200D\uFEFF']/g, '')
                    }
                }
                else {
                    worksheet[newCell].w = cellValue.replace(/[\u200B-\u200D\uFEFF']/g, '');
                    worksheet[newCell].v = cellValue.replace(/[\u200B-\u200D\uFEFF']/g, '');
                }
            }
        }
        _headers.forEach(x => {
            worksheet[x] = {
                w: x,
                v: x,
                t: 's'
            }
        });
        //return console.log("workbook.Sheets[sheetName] => ", workbook.Sheets[sheetName]);
        let _jsonData = XLSX.utils.sheet_to_json(worksheet);
        console.log("file:", file.name, "_jsonData:", _jsonData);
        _arrData.push(..._jsonData); // Spread operator to flatten the array
        //return console.log("_arrData:", _arrData);
    }
    await GetOrdData(_arrData); // Call the function to get order data for each file
    await new Promise(resolve => setTimeout(resolve, 1000)); // Wait for all files to be read
    _xDataTable.ClearAndAddDataDT("#tableImpData", _arrData);
    xSplash.hide();
});

const isNumeric = (n) => !isNaN(n);

async function GetOrdData(impData) {
    xSplash.show("Loading order data...");
    await _xLib.AJAX_Post("/api/KBNIM017R/ImportToOrder", impData,
        async (success) => {
            console.log("success:", success);
            await _xDataTable.ClearAndAddDataDT("#tableOrdData", success.data);
        },
        async (error) => {
            await xSwal.xError(error);
        }
    );
    //console.log("_dt:", _dt);
    return xSplash.hide();
}