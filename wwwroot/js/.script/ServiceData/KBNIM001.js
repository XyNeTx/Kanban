"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
$(document).ready(function () {
    return __awaiter(this, void 0, void 0, function* () {
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
});
let files = [];
$("#inputFile").on("change", function (e) {
    files = e.target.files;
    $("#tblCOMPLETE").DataTable().clear().draw();
    $("#tblERROR").DataTable().clear().draw();
});
$("#btnImport").on("click", function () {
    return __awaiter(this, void 0, void 0, function* () {
        if (files.length == 0) {
            xSwal.error("Error", "Please select file to import.");
            return;
        }
        //for (let i = 0; i < files.length; i++) {
        console.log(files);
        for (const file of files) {
            yield processFile(file);
            yield AfterImported();
        }
    });
});
function processFile(file) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            //console.log(file);
            const arrayBuffer = yield file.arrayBuffer();
            const read = yield XLSX.read(arrayBuffer);
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
            yield _xLib.AJAX_Post("/api/KBNIM001/ImportData", JSON.stringify(filterData), function (success) {
                return __awaiter(this, void 0, void 0, function* () {
                    yield $("#tblCOMPLETE").DataTable().row.add({ F_File_Name: file.name }).draw();
                });
            }, function (error) {
                return __awaiter(this, void 0, void 0, function* () {
                    yield $("#tblERROR").DataTable().row.add({ F_File_Name: file.name }).draw();
                });
            });
        }
        catch (_error) {
            xSwal.error("Import File Error", _error);
            console.log("Error: ", _error);
        }
    });
}
function AfterImported() {
    return __awaiter(this, void 0, void 0, function* () {
        var AdvanceDate = $("#AdvanceDate").val();
        yield _xLib.AJAX_Get("/api/KBNIM001/AfterImported", { advDate: AdvanceDate }, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                return yield xSwal.success("Success", success.message);
            });
        }, function (error) {
            return __awaiter(this, void 0, void 0, function* () {
                xSwal.xError(error);
                if (error.responseJSON.message.includes("Report")) {
                    var obj = {
                        UserID: ajexHeader.UserCode,
                        Type: "KBNIM001"
                    };
                    return _xLib.OpenReportObj("/KBNIMERR", obj);
                }
            });
        });
    });
}
