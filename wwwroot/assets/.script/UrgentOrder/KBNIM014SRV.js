let _files = [];

$(document).ready(function () {
    xSplash.hide();
});

$("#inputFile").change(function (e) {
    _files = e.target.files;
    console.log('Files:', _files);
});

async function UploadFile(_files) {
    try {
        const file = _files[0];
        //console.log('File being processed:', file);

        const arrayBuffer = await file.arrayBuffer();
        const read = await XLSX.read(arrayBuffer, { type: "binary" });
        const data = XLSX.utils.sheet_to_json(read.Sheets[read.SheetNames[0]]);

        var filterData = data.filter(f => !Object.values(f).includes("<EOF>"));
        if (filterData.some(f => Object.keys("PO_Item_No"))) {
            filterData = filterData.map(m => {
                m["PO_Item_No."] = m["PO_Item_No."].toString();
                m["PO_Date"] = m["PO_Date"].toString();
                m["Order_type"] = m["Order_type"].toString();
                return m;
            });
            //filterData["PO_Item_No"] = filterData["PO_Item_No"].toString();
        }
        return filterData;
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
        if (data.includes("Error")) return;

        $.ajax({
            type: "POST",
            url: "/api/KBNIM014SRV/InsertDataFromImport",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                console.log("Success: ", response);
                if (response.status === "200") {
                    return xSwal.success(response.title, response.message);
                }
                else {
                    return xSwal.error(response.title, response.message); 
                }
            },
            error: function (xhr, status, response) {
                console.error("Error: ", xhr.responseJSON);
                return xSwal.error(xhr.responseJSON.title, xhr.responseJSON.message); 
            }
        });
    } catch (error) {
        console.error("UploadFile Error: ", error);
    }
});

