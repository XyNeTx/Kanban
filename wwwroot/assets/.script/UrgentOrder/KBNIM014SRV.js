let _files = [];

$(document).ready(function () {
    xSplash.hide();
});

$("input[name='radioType']").change(function (e) {
    let _type = $("input[name='radioType']:checked").val();
    $("#inputFile").val("");
    $("#inputFile").prop("accept", _type);
});

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

        let newRead = read;

        for (var key in newRead.Sheets[newRead.SheetNames[0]]) {
            newRead.Sheets[newRead.SheetNames[0]][key].v = newRead.Sheets[newRead.SheetNames[0]][key].w;
        }

        const data = XLSX.utils.sheet_to_json(newRead.Sheets[newRead.SheetNames[0]]);

        var filterData = data.filter(f => !Object.values(f).includes("<EOF>"));
        if (filterData.some(f => Object.keys("PO_Item_No"))) {
            filterData = filterData.map(m => {
                m["PO_Item_No."] = m["PO_Item_No."].toString();
                m["PO_Date"] = m["PO_Date"].toString();
                m["Order_type"] = m["Order_type"].toString();
                m['Depot_Code_:'] = m["Depot_Code_:"].toString();
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
    $("#btnImport").prop("disabled", true);

    if ($("input[name='radioType']:checked").length === 0) return xSwal.error("Import File Error", "Please select file type");

    if ($("input[name='radioType']:checked").val() === ".txt") {
        try {
            const data = await UploadFile(_files);
            //console.log('Data: ', data);
            if (data.includes("Error")) return;

            let _url = "/api/KBNIM014SRV/InsertDataFromImport"
            if (window.location.hostname.includes("tpcap")) {
                _url = "/kanban" + _url;
            }

            $.ajax({
                type: "POST",
                url: _url,
                data: JSON.stringify(data),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                headers: ajexHeader,
                success: function (response) {
                    console.log("Success: ", response);
                    if (response.status === "200") {
                        $("#btnImport").prop("disabled", false);
                        return xSwal.success(response.title, response.message);
                    }
                    else {
                        return xSwal.error(response.title, response.message);
                    }
                },
                error: function (xhr, status, response) {
                    console.error("Error: ", xhr.responseJSON);
                    if (xhr.responseJSON.message.includes("Have Some Error")) {
                        xSwal.error("Error !!", xhr.responseJSON.message);
                        return _xLib.OpenReport("/KBNIMERR", `UserID=${xhr.responseJSON.userid}&Type=${xhr.responseJSON.type}`);
                    }
                    return xSwal.error(xhr.responseJSON.title, xhr.responseJSON.message);
                }
            });
        } catch (error) {
            console.error("UploadFile Error: ", error);
        }
        finally {
            $("#btnImport").prop("disabled", false);
        }
    }

    else if ($("input[name='radioType']:checked").val() === ".xlsx,.xls")
    {
        try {
            const file = _files[0];
            if (!file) return xSwal.error("Import File Error", "No file selected");
            //console.log('File being processed:', file);

            const arrayBuffer = await file.arrayBuffer();
            const read = await XLSX.read(arrayBuffer);

            let newRead = read;

            //const oldData = XLSX.utils.sheet_to_json(read.Sheets[read.SheetNames[0]]);
            //console.log('oldData:', oldData);

            for (var key in newRead.Sheets[newRead.SheetNames[0]]) {
                newRead.Sheets[newRead.SheetNames[0]][key].v = newRead.Sheets[newRead.SheetNames[0]][key].w;
            }

            const data = XLSX.utils.sheet_to_json(newRead.Sheets[newRead.SheetNames[0]]);

            let convertedData = data;

            //console.log('Parsed data:', convertedData);

            for (var key in convertedData) {

                //convertedData[key]["Customer Order Number"] = convertedData[key]["Customer\n Order Number"];
                //delete convertedData[key]["Customer\n Order Number"];

                console.log('Converted Data:', convertedData[key]["Amount / Item"]);
                if (convertedData[key]["Amount / Item"] === undefined) {
                    return xSwal.error("Import File Error", "Please check the column name 'Amount / Item'");
                }
                convertedData[key]["Amount_/_Item"] = parseFloat(convertedData[key]["Amount / Item"].replace(/,/g, ""));
                delete convertedData[key]["Amount / Item"];

                convertedData[key]["F_Delivery_Qty"] = convertedData[key]["Delivery\n Qty"];
                delete convertedData[key]["Delivery\n Qty"];

                //convertedData[key]["Customer Order Number"] = convertedData[key]["Destination\n Code"];
                //delete convertedData[key]["Destination\n Code"];

                //convertedData[key]["Customer Order Number"] = convertedData[key]["Destination\n Name"];
                //delete convertedData[key]["Destination\n Name"];

                convertedData[key]["Delivery_Date"] = convertedData[key]["Delivery Date"];
                delete convertedData[key]["Delivery Date"];

                convertedData[key]["Order_Type"] = convertedData[key]["Order\n Type"];
                delete convertedData[key]["Order\n Type"];

                convertedData[key]["PO_Date"] = convertedData[key]["PO Date"];
                delete convertedData[key]["PO Date"];


                convertedData[key]["PO_Item_No."] = convertedData[key]["PO Item\n No."];
                delete convertedData[key]["PO Item\n No."];

                convertedData[key]["PO_NO._/_Shift_No."] = convertedData[key]["PO No. /\n Shift No."];
                delete convertedData[key]["PO No. /\n Shift No."];

                convertedData[key]["PO_P/No."] = convertedData[key]["PO P/No."];
                delete convertedData[key]["PO P/No."];

                convertedData[key]["PO_Qty"] = convertedData[key]["PO Qty"];
                delete convertedData[key]["PO Qty"];

                convertedData[key]["Part_Name"] = convertedData[key]["Part Name"];
                delete convertedData[key]["Part Name"];

                if (convertedData[key]["Price / Unit"] === undefined) {
                    return xSwal.error("Import File Error", "Please check the column name 'Price / Unit'");
                }
                convertedData[key]["Price_/_Unit"] = parseFloat(convertedData[key]["Price / Unit"].replace(/,/g, ""));
                delete convertedData[key]["Price / Unit"];

                //convertedData[key]["Customer Order Number"] = convertedData[key]["Receiving Case\n No."];
                //delete convertedData[key]["Receiving Case\n No."];

                //convertedData[key]["Customer Order Number"] = convertedData[key]["Supplier\n P/No."];
                //delete convertedData[key]["Supplier\n P/No."];

                //convertedData[key]["Customer Order Number"] = convertedData[key]["Supplier\n P/No."];
                //delete convertedData[key]["Supplier\n P/No."];
            }

            console.log('Parsed data:', convertedData);

            let _url = "/api/KBNIM014SRV/InsertDataFromImportExcel"
            if (window.location.hostname.includes("tpcap")) {
                _url = "/kanban" + _url;
            }

            $.ajax({
                type: "POST",
                url: _url,
                data: JSON.stringify(convertedData),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                headers: ajexHeader,
                success: function (response) {
                    console.log("Success: ", response);
                    if (response.status === "200") {
                        $("#btnImport").prop("disabled", false);
                        return xSwal.success(response.title, response.message);
                    }
                    else {
                        return xSwal.error(response.title, response.message);
                    }
                },
                error: function (xhr, status, response) {
                    console.error("Error: ", xhr.responseJSON);
                    if (xhr.responseJSON.message.includes("Have Some Error")) {
                        xSwal.error("Error !!", xhr.responseJSON.message);
                        return _xLib.OpenReport("/KBNIMERR", `UserID=${xhr.responseJSON.userid}&Type=${xhr.responseJSON.type}`);
                    }
                    return xSwal.error(xhr.responseJSON.title, xhr.responseJSON.message);
                }
            });
        }
        catch (error) {
            xSwal.error("Import File Error", error);
            console.error("UploadFile Error: ", error);
        }
        finally {
            $("#btnImport").prop("disabled", false);
        }
    }
});

