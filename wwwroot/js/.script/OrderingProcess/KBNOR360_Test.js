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
let P = "TEST";
let Q = "TEST2";
let R = "TEST3";
function testFunction() {
    let result = P + " " + Q + " " + R;
    //console.log(result);
    return new Promise((resolve) => {
        resolve(result);
    });
}
window.addEventListener("load", () => __awaiter(void 0, void 0, void 0, function* () {
    //console.log("Test script loaded");
    const output = yield testFunction();
    console.log("Function returned:", output);
    //await List_Data();
    xSplash.hide();
}));
//async function List_Data_KBNOR360(): Promise<void> {
//    await _xLib.AJAX_Get("/api/KBNOR360/List_Data", null,
//        function (success: any) {
//            success = _xLib.JSONparseMixData(success);
//            _xDataTable.ClearAndAddDataDT("#tblMaster", success.data);
//        },
//        function (error: any) {
//            console.error(error);
//            //xSwal.xError(error)
//        }
//    );
//}
