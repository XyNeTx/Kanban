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
let _cmd = "";
$(document).ready(function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield $("#tableMaster").DataTable({
            columns: [
                { title: "Supplier Code", data: "F_Supplier_Code" },
                { title: "Part No.", data: "F_Part_No" },
                { title: "Kanban No.", data: "F_Kanban_No" },
                { title: "Store Code", data: "F_Store_Code" },
                { title: "Qty/Pack", data: "F_Qty" },
                { title: "Actual KB", data: "F_BL_Kanban" },
                { title: "Actual PCS", data: "F_BL_PCS" },
                { title: "Total Actual", data: "F_Total_Actual" },
                { title: "Total System", data: "F_Total_System" }
            ],
            order: [[0, "asc"]],
            width: '100%',
            paging: false,
            scrollX: false,
            scrollY: '350px',
            scrollCollapse: true,
            select: true,
            info: false,
            searching: false,
        });
        $("#divButton").find("button").prop("disabled", false);
        yield GetDateAndShift();
        $(".dataTable thead tr th").addClass("text-center");
        xSplash.hide();
    });
});
function GetDateAndShift() {
    return __awaiter(this, void 0, void 0, function* () {
        yield _xLib.AJAX_Get('/api/KBNMS005S/GetDateAndShift', "", function (success) {
            if (success.status == "200") {
                let data = JSON.parse(success.data);
                let date = data[0].F_Date.slice(0, 4) + "-" + data[0].F_Date.slice(4, 6) + "-" + data[0].F_Date.slice(6, 8);
                $("#SelectShift").val(data[0].F_Shift);
                $("#inputDate").val(date);
            }
        }, function (error) {
            xSwal.error("Error !!", error.responseJSON.message);
        });
    });
}
$("#buttonInq, #buttonNew, #buttonUpd, #buttonDel").click(function (e) {
    return __awaiter(this, void 0, void 0, function* () {
        $("#divButton").find("button").prop("disabled", true);
        $(this).prop("disabled", false);
        var id = $(this).attr("id");
        var IsCmdNew = false;
        //console.log(id);
        if (id == "buttonNew")
            IsCmdNew = true;
        _cmd = id.replace("button", "");
        yield _xLib.AJAX_Get('/api/KBNMS005S/GetSupplierCode', { IsCmdNew: IsCmdNew }, function (success) {
            if (success.status == "200") {
                $("#SelectSupplierCode").empty();
                $("#SelectSupplierCode").append('<option value="" hidden></option>');
                success.data.forEach(function (item) {
                    $("#SelectSupplierCode").append('<option value="' + item.f_Supplier_Code + '">' + item.f_Supplier_Code + '</option>');
                });
            }
        }, function (error) {
            xSwal.error("Error !!", error.responseJSON.message);
        });
        return GetStoreCode(IsCmdNew);
    });
});
function GetStoreCode(IsCmdNew) {
    return __awaiter(this, void 0, void 0, function* () {
        return _xLib.AJAX_Get('/api/KBNMS005S/GetStoreCode', { IsCmdNew: IsCmdNew, F_Supplier_Code: $("#SelectSupplierCode").val() }, function (success) {
            if (success.status == "200") {
                $("#SelectStoreCode").empty();
                $("#SelectStoreCode").append('<option value="" hidden></option>');
                success.data.forEach(function (item) {
                    $("#SelectStoreCode").append('<option value="' + item.f_Store_Code + '">' + item.f_Store_Code + '</option>');
                });
                $("#ReadSupplierName").val(success.supplier_name);
                $("#formSearchData").find("input, select").prop("disabled", false);
            }
        }, function (error) {
            xSwal.error("Error !!", error.responseJSON.message);
        });
    });
}
$("#SelectSupplierCode").change(function () {
    var IsCmdNew = false;
    if (_cmd == "New" || _cmd == "Upd")
        IsCmdNew = true;
    return GetStoreCode(IsCmdNew);
});
$("#buttonShow").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        let data = {
            IsCmdNew: _cmd == "New" ? true : false,
            F_Supplier_Code: $("#SelectSupplierCode").val(),
            F_Store_Code: $("#SelectStoreCode").val(),
            F_Date: $("#inputDate").val().replaceAll(/-/g, ""),
            F_Shift: $("#SelectShift").val()
        };
        yield _xLib.AJAX_Get('/api/KBNMS005S/SearchClick', data, function (success) {
            if (success.status == "200") {
                success.data = JSON.parse(success.data);
                console.log(success.data);
                const table = $("#tableMaster").DataTable();
                table.scrollY = "300px";
                table.clear().rows.add(success.data).draw();
            }
        }, function (error) {
            return xSwal.error("Error !!", error.responseJSON.message);
        });
    });
});
let _oldValue = "";
$(document).on("click", "#tableMaster tbody tr td", function () {
    if (_cmd == "New" || _cmd == "Upd") {
        var _columnIndex = $("#tableMaster").DataTable().cell(this).index().column;
        if ($(this).find("input").length > 0)
            return;
        if (_columnIndex == 5 || _columnIndex == 6) {
            let value = $("#tableMaster").DataTable().cell(this).data();
            _oldValue = value;
            //console.log("value", value);
            $(this).empty();
            $(this).append('<input type="number" class="form-control" value="">');
            $(this).find("input").focus();
        }
    }
});
$(document).on(" blur , keypress ", "#tableMaster tbody tr td", function (e) {
    return __awaiter(this, void 0, void 0, function* () {
        if (e.type == "keypress" && e.key != "Enter")
            return;
        let _columnIndex = $("#tableMaster").DataTable().cell(this).index().column;
        let _rowIndex = $("#tableMaster").DataTable().cell(this).index().row;
        if (_columnIndex == 5 || _columnIndex == 6) {
            let _newValue = $(this).find("input").val();
            if (_newValue == "")
                _newValue = _oldValue;
            yield $("#tableMaster").DataTable().cell({ row: _rowIndex, column: _columnIndex }).data(_newValue).draw();
            let QtyPack = $("#tableMaster").DataTable().cell({ row: _rowIndex, column: 4 }).data();
            let ActualKB = $("#tableMaster").DataTable().cell({ row: _rowIndex, column: 5 }).data();
            let ActualPCS = $("#tableMaster").DataTable().cell({ row: _rowIndex, column: 6 }).data();
            //console.log(QtyPack, ActualKB, ActualPCS);
            let TotalActual = (parseInt(QtyPack) * parseInt(ActualKB)) + parseInt(ActualPCS);
            //console.log(TotalActual);
            yield $("#tableMaster").DataTable().cell({ row: _rowIndex, column: 7 }).data(TotalActual).draw();
        }
    });
});
$("#buttonCcl").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        $("#divButton").find("button").prop("disabled", false);
        $("#formSearchData").find("input, select").prop("disabled", true);
        $("#formSearchData").find("input, select").val("");
        yield GetDateAndShift();
        $("#tableMaster").DataTable().clear().draw();
        _cmd = "";
    });
});
$("#buttonSave").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        $("#buttonSave").prop("disabled", true);
        $("#buttonSave").parent().append(`<div id='spinLoader' class="d-flex justify-content-center">
    <div class="spinner-border text-danger" role="status">
        <span class="visually-hidden">Loading...</span>
    </div>
    </div>`);
        if (_cmd == "New" || _cmd == "Upd") {
            var arrayObj = $("#tableMaster").DataTable().rows().data().toArray();
            for (var each of arrayObj) {
                arrayObj[arrayObj.indexOf(each)].F_Date = $("#inputDate").val().replaceAll(/-/g, "");
                arrayObj[arrayObj.indexOf(each)].F_Shift = $("#SelectShift").val();
                arrayObj[arrayObj.indexOf(each)].F_Sup_Cd = arrayObj[arrayObj.indexOf(each)].F_Supplier_Code.split("-")[0];
                arrayObj[arrayObj.indexOf(each)].F_Sup_Plant = arrayObj[arrayObj.indexOf(each)].F_Supplier_Code.split("-")[1];
                arrayObj[arrayObj.indexOf(each)].F_Ruibetsu = arrayObj[arrayObj.indexOf(each)].F_Part_No.split("-")[1];
                arrayObj[arrayObj.indexOf(each)].F_Part_No = arrayObj[arrayObj.indexOf(each)].F_Part_No.split("-")[0];
                arrayObj[arrayObj.indexOf(each)].F_BL_PCS = parseInt(arrayObj[arrayObj.indexOf(each)].F_BL_PCS);
                arrayObj[arrayObj.indexOf(each)].F_BL_Kanban = parseInt(arrayObj[arrayObj.indexOf(each)].F_BL_Kanban);
            }
            _xLib.AJAX_Post('/api/KBNMS005S/Save', JSON.stringify(arrayObj), function (success) {
                return __awaiter(this, void 0, void 0, function* () {
                    if (success.status == "200") {
                        yield xSwal.success("Success !!", success.message);
                        yield $("#tableMaster").DataTable().clear().draw();
                        yield $("#buttonSave").prop("disabled", false);
                        yield $("#spinLoader").remove();
                    }
                });
            }, function (error) {
                xSwal.error("Error !!", error.responseJSON.message);
            });
        }
        else if (_cmd == "Del") {
            var arrayObj = $("#tableMaster").DataTable().row('.selected').data();
            if (arrayObj.length == 0)
                return xSwal.error("Error !!", "Please select data to delete.");
            arrayObj.F_Date = $("#inputDate").val().replaceAll(/-/g, "");
            arrayObj.F_Shift = $("#SelectShift").val();
            arrayObj.F_Sup_Cd = arrayObj.F_Supplier_Code.split("-")[0];
            arrayObj.F_Sup_Plant = arrayObj.F_Supplier_Code.split("-")[1];
            arrayObj.F_Ruibetsu = arrayObj.F_Part_No.split("-")[1];
            arrayObj.F_Part_No = arrayObj.F_Part_No.split("-")[0];
            arrayObj.F_BL_PCS = parseInt(arrayObj.F_BL_PCS);
            arrayObj.F_BL_Kanban = parseInt(arrayObj.F_BL_Kanban);
            arrayObj.F_Total_Actual = parseInt(arrayObj.F_Total_Actual);
            _xLib.AJAX_Post('/api/KBNMS005S/Delete', JSON.stringify(arrayObj), function (success) {
                return __awaiter(this, void 0, void 0, function* () {
                    if (success.status == "200") {
                        yield $("#tableMaster").DataTable().rows('.selected').remove().draw();
                        yield xSwal.success("Success !!", success.message);
                        yield $("#buttonSave").prop("disabled", false);
                        yield $("#spinLoader").remove();
                    }
                });
            }, function (error) {
                xSwal.error("Error !!", error.responseJSON.message);
            });
        }
    });
});
