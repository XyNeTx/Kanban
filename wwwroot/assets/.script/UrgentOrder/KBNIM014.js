$(document).ready(function () {
    xSplash.hide();
});

let _files = [];

$("#inputFile").change(function (e) {
    _files = e.target.files;
    console.log('Files:', _files);
});

async function UploadFile(_files) {
    try {
        const file = _files[0];
        if (!file) return xSwal.error("Import File Error", "No file selected");
        //console.log('File being processed:', file);

        const arrayBuffer = await file.arrayBuffer();
        const read = await XLSX.read(arrayBuffer);
        const data = XLSX.utils.sheet_to_csv(read.Sheets[read.SheetNames[0]]);

        const _dataHead = "F_ITEM_NO,F_Supplier,F_Supplier_Plant,F_Supplier_Name,F_Plant,F_Plant_Name,F_Receive_Place,F_Order_Type,F_PDS_No,F_EKBPDS_No," +
            "F_Collect_Date,F_Collect_Time,F_Arrival_Date,F_Arrival_Time,F_Main_route_Grp_Code,F_Main_route_Order_Seq,F_Sub_route_Grp_Code1,F_Sub_route_Order_Seq1," +
            "F_Crs1_route,F_Crs1_dock,F_Crs1_arv_Date,F_Crs1_arv_Time,F_Crs1_dpt_Date,F_Crs1_dpt_Time,F_Crs2_route,F_Crs2_dock," +
            "F_Crs2_arv_Date,F_Crs2_arv_Time,F_Crs2_dpt_Date,F_Crs2_dpt_Time,F_Crs3_route,F_Crs3_dock,F_Crs3_arv_Date,F_Crs3_arv_Time,F_Crs3_dpt_Date,F_Crs3_dpt_Time," +
            "F_Supplier_Type,F_No,F_Part_No,F_Part_Name,F_Kanban_No,F_Line_Addr,F_Pack_Qty,F_Qty,F_Pack,F_Zero_Order,F_Sort_Lane," +
            "F_Shipping_Date,F_Shipping_Time,F_Kb_print_Date_p,F_Kb_print_Time_p,F_Kb_print_Date_i,F_Kb_print_Time_i,F_Remark," +
            "F_Order_Release_Date,F_Order_Release_Time,F_Main_route_Date,F_Bill_Out_Flag,F_Shipping_Dock,F_Pack_Code,"

        const mergeData = _dataHead + "\n" + data;
        //console.log('Data:', mergeData);

        const csvFile = new Blob([mergeData], { type: "text/csv" });

        const csvFileArrayBuffer = await csvFile.arrayBuffer();
        const csvFileRead = await XLSX.read(csvFileArrayBuffer);

        const csvSheet = csvFileRead.Sheets[csvFileRead.SheetNames[0]];

        for (const each in csvSheet) {
            csvSheet[each].v = csvSheet[each].w;
        }

        //console.log(csvSheet);

        const csvFileData = XLSX.utils.sheet_to_json(csvSheet, { raw: true });
        //console.log('CSV File Data:', csvFileData);

        return csvFileData;
        //console.log('Parsed data:', data);
    }
    catch (_error) {
        return xSwal.error("Import File Error", _error);
    }
}

$("#btnImport").click(async function () {
    try {
        const data = await UploadFile(_files);
        console.log('Data: ', data);
        if (data.includes("Error")) return console.error("Error: ", data);

        //$.ajax({
        //    type: "POST",
        //    url: "/api/KBNIM014SRV/InsertDataFromImport",
        //    data: JSON.stringify(data),
        //    contentType: "application/json; charset=utf-8",
        //    dataType: "json",
        //    success: function (response) {
        //        console.log("Success: ", response);
        //        if (response.status === "200") {
        //            return xSwal.success(response.title, response.message);
        //        }
        //        else {
        //            return xSwal.error(response.title, response.message);
        //        }
        //    },
        //    error: function (xhr, status, response) {
        //        console.error("Error: ", xhr.responseJSON);
        //        return xSwal.error(xhr.responseJSON.title, xhr.responseJSON.message);
        //    }
        //});
    } catch (error) {
        console.error("UploadFile Error: ", error);
    }
});