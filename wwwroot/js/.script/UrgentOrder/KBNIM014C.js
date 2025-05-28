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
        const KBNIM014C = new MasterTemplate({
            Controller: _PAGE_,
            Table: 'tblMaster',
            ColumnTitle: {
                "EN": ['Order No.', 'Order Issued Date', 'Delivery Date'],
                "TH": ['Order No.', 'Order Issued Date', 'Delivery Date'],
                "JP": ['Order No.', 'Order Issued Date', 'Delivery Date'],
            },
            ColumnValue: [
                { "data": "F_PDS_No" },
                { "data": "F_PDS_Issued_Date" },
                { "data": "F_Delivery_Date" }
            ],
            Modal: 'modalMaster',
            Form: 'frmMaster',
            PostData: [
                { name: 'F_Plant', value: _PLANT_ }
            ],
        });
        yield _xLib.AJAX_Get('/api/KBNIM014C/search', '', function (response) {
            return __awaiter(this, void 0, void 0, function* () {
                if (response.status === "200") {
                    var _jsondata = yield JSON.parse(response.data);
                    yield _xLib.TrimArrayJSON(_jsondata);
                    return yield _jsondata.forEach(function (item) {
                        $("#SelectPDSNo").append('<option value="' + item.F_PDS_No + '">' + item.F_PDS_No + '</option>');
                    });
                }
            });
        }, function (err) {
            return xSwal.error("Error !!", err.responseJSON.message);
        });
        yield KBNIM014C.prepare();
        yield KBNIM014C.initial();
    });
});
$("#chkDeliveryDate").change(function () {
    if (this.checked) {
        $("#F_DeliveryFrom").prop("disabled", false);
        $("#F_DeliveryTo").prop("disabled", false);
    }
    else {
        $("#F_DeliveryFrom").prop("disabled", true);
        $("#F_DeliveryTo").prop("disabled", true);
    }
});
$("#buttonSearch").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        console.log("Clicked !!! !! !!");
        var DeliveryDateFrom = null;
        var DeliveryDateTo = null;
        if ($("#chkDeliveryDate").prop("checked")) {
            DeliveryDateFrom = $("#F_DeliveryFrom").val();
            DeliveryDateTo = $("#F_DeliveryTo").val();
        }
        let _url = "/api/KBNIM014C/search";
        if (window.location.hostname.includes("tpcap")) {
            _url = "/kanban" + _url;
        }
        $.ajax({
            type: "GET",
            url: _url,
            data: {
                F_PDS_NO: $("#SelectPDSNo").val(),
                chkDeliveryDate: $("#chkDeliveryDate").prop("checked"),
                F_DeliveryFrom: DeliveryDateFrom,
                F_DeliveryTo: DeliveryDateTo
            },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            headers: ajexHeader,
            success: function (response) {
                return __awaiter(this, void 0, void 0, function* () {
                    //console.log("Success: ", response);
                    if (response.status === "200") {
                        var jsonResult = JSON.parse(response.data);
                        //console.log("jsonResult: ", jsonResult);
                        _xLib.TrimArrayJSON(jsonResult);
                        yield $("#tblMaster").DataTable().clear().rows.add(jsonResult).draw();
                        yield $("input[type='checkbox']").prop("checked", true);
                    }
                    else {
                        return xSwal.error(response.title, response.message);
                    }
                });
            },
            error: function (xhr, status, response) {
                console.error("Error: ", xhr.responseJSON);
                return xSwal.error(xhr.responseJSON.title, xhr.responseJSON.message);
            }
        });
    });
});
$("#buttonSave").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        var checkedRow = $("#tblMaster input[type='checkbox']:checked");
        if (checkedRow.length == 0)
            return xSwal.error("Error !!", "No data selected.");
        //console.log("Checked Row: ", checkedRow);
        var _arrCheckedData = $("#tblMaster").DataTable().rows(checkedRow.closest("tr")).data().toArray();
        if (_arrCheckedData.length == 0)
            return xSwal.error("Error !!", "No data selected.");
        //return console.log(_arrCheckedData);
        var title, message;
        for (const each in _arrCheckedData) {
            //console.log("Data: ", _arrCheckedData[each]);
            var _jsonData = JSON.stringify(_arrCheckedData[each]);
            //console.log("JSON Data: ", _jsonData);
            yield _xLib.AJAX_Post('/api/KBNIM014C/Save', _jsonData, function (response) {
                return __awaiter(this, void 0, void 0, function* () {
                    //console.log("Success: ", response);
                    title = response.title;
                    message = response.message;
                });
            }, function (err) {
                return __awaiter(this, void 0, void 0, function* () {
                    let confirm = yield xSwal.error("Error !!", err.responseJSON.message);
                    if (confirm.isConfirmed) {
                        return;
                    }
                });
            });
        }
        xSwal.success(title, message);
        $("#buttonSearch").click();
    });
});
