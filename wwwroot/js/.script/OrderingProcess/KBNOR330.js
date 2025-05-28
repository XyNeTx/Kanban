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
        yield _xDataTable.InitialDataTable("#tableMain", {
            columns: [
                { title: "Customer PO", data: "F_PDS_No" },
                { title: "Part No", data: "F_Part_No" },
                { title: "Supplier", data: "F_Supplier_CD" },
                { title: "Short Name", data: "F_Short_name" },
                { title: "Store Code", data: "F_Store_CD" },
                { title: "Kanban No.", data: "F_Kanban_No" },
                { title: "Delivery Date", data: "F_Delivery_Date" },
                { title: "Delivery Trip", data: "F_Round" },
                { title: "Qty", data: "F_Qty" },
                { title: "Qty KB", data: "F_QTY_KB" },
                { title: "Import Type", data: "F_OrderType" },
            ],
            order: [[1, "asc"]],
            scrollCollapse: true,
        });
        xSplash.hide();
        xAjax.onClick('btnExit', function () {
            xAjax.redirect('KBNOR300');
        });
        //$("#btnReport").click(function () {
        //    $("#exampleModal").modal('show');
        //})
        //$("#btnGenerate").click(function () {
        //})
        //$("#exampleModal").on('show.bs.modal', async function () {
        //    //console.log($("#exampleModal .modal-body .dataTables_scrollHead").length);
        //    if ($("#exampleModal .modal-body .dataTables_scrollHead").length == 0) {
        //        await _xDataTable.InitialDataTable("#tableModal",
        //            {
        //                columns: [
        //                    { title: "Supplier Code", data: "F_Supplier_Code" },
        //                    { title: "Part No", data: "F_Part_No" },
        //                    { title: "Store Code", data: "F_Store_Code" },
        //                    { title: "Kanban No", data: "F_Kanban_No" },
        //                    { title: "Delivery Date", data: "F_Delivery_Date" },
        //                    { title: "Delivery Shift", data: "F_Delivery_Shift" },
        //                    { title: "Delivery Round", data: "F_Delivery_Round" },
        //                    { title: "KB Qty (PCS)", data: "F_Qty" },
        //                    { title: "CKD Remain Qty (PCS)", data: "CKD_Remain_Qty" }
        //                ],
        //                order: [[1, "asc"]],
        //                scrollCollapse: true,
        //            }
        //        );
        //    }
        //});
    });
});
var _dt = "";
function Generate() {
    return __awaiter(this, void 0, void 0, function* () {
        _xLib.AJAX_Post("/api/KBNOR330/Generate", "", function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                success = _xLib.JSONparseMixData(success);
                if (success.message.includes("ไม่สามารถ Generate PDS สำหรับ CKD Order ได้")) {
                    $("#exampleModal").modal('show');
                    if ($("#exampleModal .modal-body .dataTables_scrollHead").length == 0) {
                        yield _xDataTable.InitialDataTable("#tableModal", {
                            columns: [
                                { title: "Supplier Code", data: "F_Supplier_Code" },
                                { title: "Part No", data: "F_Part_No" },
                                { title: "Store Code", data: "F_Store_Code" },
                                { title: "Kanban No", data: "F_Kanban_No" },
                                { title: "Delivery Date", data: "F_Delivery_Date" },
                                { title: "Delivery Shift", data: "F_Delivery_Shift" },
                                { title: "Delivery Round", data: "F_Delivery_Round" },
                                { title: "KB Qty (PCS)", data: "F_Qty" },
                                { title: "CKD Remain Qty (PCS)", data: "CKD_Remain_Qty" }
                            ],
                            order: [[1, "asc"]],
                            scrollCollapse: true,
                        });
                    }
                    _xDataTable.ClearAndAddDataDT("#tableModal", success.data);
                }
                else {
                    yield xSwal.xSuccessAwait(success);
                }
            });
        }, function (error) {
            return __awaiter(this, void 0, void 0, function* () {
                yield xSwal.xErrorAwait(error);
            });
        });
    });
}
