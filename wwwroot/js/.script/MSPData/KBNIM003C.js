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
        const KBNIM003C = new ActionTemplate({
            Controller: _PAGE_,
            Form: 'frmCondition',
            PostData: [
                { name: 'F_Plant', value: _PLANT_ }
            ]
        });
        KBNIM003C.prepare(function () {
            var tblPDS = xDataTable.Initial({
                name: 'tblMaster',
                checking: 0,
                //dom: '<"clear">',
                columnTitle: {
                    "EN": ['Order No.', 'Order Issued Date', 'Delivery Date'],
                    "TH": ['Order No.', 'Order Issued Date', 'Delivery Date'],
                    "JP": ['Order No.', 'Order Issued Date', 'Delivery Date'],
                },
                column: [
                    { "data": "F_PDS_No" },
                    { "data": "F_PDS_ISSUED_DATE" },
                    { "data": "F_Delivery_Date" }
                ],
                addnew: false,
                rowclick: (row) => {
                }
            });
        });
        yield Search();
        xSplash.hide();
    });
});
function Search() {
    return __awaiter(this, void 0, void 0, function* () {
        yield _xLib.AJAX_Get('/api/KBNIM003C/Search', '', function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                if (success.status === "200") {
                    var _arrJson = JSON.parse(success.data);
                    _xLib.TrimArrayJSON(_arrJson);
                    console.log(_arrJson);
                    yield $("#tblMaster").DataTable().clear().rows.add(_arrJson).draw();
                    yield $("#tblMaster tr td").find('input[type="checkbox"]').prop("checked", true);
                    xSplash.hide();
                }
            });
        }, function (errror) {
            return __awaiter(this, void 0, void 0, function* () {
                yield xSplash.hide();
                xSwal.error("Error !!", errror.responseJSON.message);
            });
        });
    });
}
$("#buttonConfirm").click(function () {
    var _arrData = $("#tblMaster").DataTable().rows().data().toArray();
    xSplash.show();
    _xLib.AJAX_Post('/api/KBNIM003C/Confirm', JSON.stringify(_arrData), function (success) {
        return __awaiter(this, void 0, void 0, function* () {
            if (success.status === "200") {
                $("#tblMaster").DataTable().clear().draw();
                xSwal.success("Success !!", success.message);
                //await Search();
            }
        });
    }, function (error) {
        return __awaiter(this, void 0, void 0, function* () {
            xSwal.error("Error !!", error.responseJSON.message);
        });
    });
    xSplash.hide();
});
