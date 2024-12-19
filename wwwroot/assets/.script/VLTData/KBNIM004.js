$(document).ready(function () {
    setTimeout(() => xSplash.hide(),1000);
});
var files = [];
$("#inputFile").on("change", function (e) {
    files = e.target.files;
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

            var maxRow = read.Sheets.Sheet1["!ref"].split(":")[1].replace(/[A-z]/g, "");

            let newRead = read;
            //console.log(maxRow);
            newRead.Sheets.Sheet1["A1"].v = "F_Production_Plan";
            newRead.Sheets.Sheet1.B1 = {
                v: "F_Line_Code",
            }
            newRead.Sheets.Sheet1.C1 = {
                v: "F_ID_No",
            }
            newRead.Sheets.Sheet1.D1 = {
                v: "F_Jig_In_Seq",
            }
            newRead.Sheets.Sheet1.E1 = {
                v: "F_Frame_Code",
            }
            newRead.Sheets.Sheet1.F1 = {
                v: "F_EDP_Type",
            }
            newRead.Sheets.Sheet1.G1 = {
                v: "F_Vehicle_Model",
            }
            newRead.Sheets.Sheet1.H1 = {
                v: "F_Frame_Type",
            }
            newRead.Sheets.Sheet1.I1 = {
                v: "F_Frame_VIN",
            }
            newRead.Sheets.Sheet1.J1 = {
                v: "F_Frame_ChK_VIN",
            }
            newRead.Sheets.Sheet1.K1 = {
                v: "F_Frame_Dummy",
            }
            newRead.Sheets.Sheet1.L1 = {
                v: "F_Frame_Plant",
            }
            newRead.Sheets.Sheet1.M1 = {
                v: "F_Frame_Serial",
            }
            newRead.Sheets.Sheet1.N1 = {
                v: "F_Stamp_VIN",
            }
            newRead.Sheets.Sheet1.O1 = {
                v: "F_Side_Panel",
            }
            newRead.Sheets.Sheet1.P1 = {
                v: "F_Tail_Gate",
            }
            newRead.Sheets.Sheet1.Q1 = {
                v: "F_RR_Axle",
            }
            newRead.Sheets.Sheet1.R1 = {
                v: "F_VHD_Order_No",
            }

            let excelColumns = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R"];

            for (let j = 0; j < excelColumns.length; j++) {
                newRead.Sheets.Sheet1[excelColumns[j] + "1"].t = "s";
                newRead.Sheets.Sheet1[excelColumns[j] + "1"].w = newRead.Sheets.Sheet1[excelColumns[j] + "1"].v;

            }

            for (var key in newRead.Sheets.Sheet1) {
                if (key.includes("D") || key.includes("H") || key.includes("A") || key.includes("K")) {
                    newRead.Sheets.Sheet1[key].t = "s";
                    newRead.Sheets.Sheet1[key].v = newRead.Sheets.Sheet1[key].w;
                }
            }
            //console.log(newRead);
            const data = XLSX.utils.sheet_to_json(newRead.Sheets[newRead.SheetNames[0]]);
            //console.log(data);

            if (data.length != (maxRow - 1)) {
                return xSwal.error("Error", "Please check the rows of data in the excel file.");
            }

            $("#divProgressBar").css("visibility", "visible");
            let count = 1;
            setInterval(() => {
                let step = ((100) / (parseInt(maxRow) - 1));
                $("#widthProgressBar").css("width", ((step * count) + 10) + "%");
                //console.log(step * count + "%");
                count++;
            }, 55);

            _xLib.AJAX_Post("/api/KBNIM004/ImportData", JSON.stringify(data),
                async function (success) {
                    if (success.status === "200") {
                        await $("#widthProgressBar").css("width", "100%");
                        setTimeout(() => {
                            $("#divProgressBar").css("visibility", "hidden");
                            return xSwal.success("Success", success.message);
                        }, 1000);
                    }
                },
                async function (error) {
                    if (error.responseJSON.message.includes("Have Some Error")) {
                        xSwal.error("Error !!", error.responseJSON.message);
                        return _xLib.OpenReport("/KBNIMERR", `UserID=${error.responseJSON.userid}&Type=${error.responseJSON.type}`);
                    }
                    return xSwal.error("Error", error.responseJSON.message);
                }
            );
        }

        catch (error) {
            console.error(error);
        }
    }
});