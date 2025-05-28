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
    initial = function () {
        return __awaiter(this, void 0, void 0, function* () {
            xSplash.show();
            yield _xLib.AJAX_Get('/xapi/GetProcessDate', { dateShift: _xLib.GetCookie("loginDate").replaceAll("-", "") }, function (data) {
                console.log(data[0].Column1);
                var _Date = data[0].Column1.slice(0, 4) + '-' + data[0].Column1.slice(4, 6) + '-' + data[0].Column1.slice(6, 8) + data[0].Column1.slice(8, 9);
                document.cookie = `processDate=${_Date}`;
            });
            //console.log('xXx');
            xSplash.hide();
        });
    };
    initial();
});
