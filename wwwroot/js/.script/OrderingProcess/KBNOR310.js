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
        _xLib.AJAX_Get("/api/KBNOR310/Onload", "", function (success) {
            //xSwal.xSuccess(success);
        }, function (error) {
            //xSwal.xError(error);
        });
        xSplash.hide();
        xAjax.onClick('#btnExit', function () {
            xAjax.redirect('KBNOR300');
        });
    });
});
$("#btnInterface").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        let isConfirm = yield xSwal.confirm("Are you Sure to Interface CKD In-House Data from Import Order");
        if (isConfirm) {
            _xLib.AJAX_Post("/api/KBNOR310/Interface", "", function (success) {
                xSwal.xSuccess(success);
            }, function (error) {
                xSwal.xError(error);
            });
        }
    });
});
