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
            //var _dt = await xAjax.ExecuteJSON({
            //    data: {
            //        "Module": "[exec].[spKBNOR600]",
            //        "@OrderType": "U",
            //        "@Plant": ajexHeader.Plant,
            //        "@UserCode": ajexHeader.UserCode
            //    },
            //});
            //if (_dt.rows[0].KBNOR610 == 0) {
            //    $('#btnKBNOR610').attr('readonly', true);
            //    $('#btnKBNOR610').attr('class', 'btn btn-light');
            //}
            //if (_dt.rows[0].KBNOR620 == 0) {
            //    $('#btnKBNOR620').attr('readonly', true);
            //    $('#btnKBNOR620').attr('class', 'btn btn-light');
            //}
            //if (_dt.rows[0].KBNOR640 == 0) {
            //    $('#btnKBNOR640').attr('readonly', true);
            //    $('#btnKBNOR640').attr('class', 'btn btn-light');
            //}
            //if (_dt.rows[0].KBNOR650 == 0) {
            //    $('#btnKBNOR650').attr('readonly', true);
            //    $('#btnKBNOR650').attr('class', 'btn btn-light');
            //}
            //if (_dt.rows[0].KBNOR660 == 0) {
            //    $('#btnKBNOR660').attr('readonly', true);
            //    $('#btnKBNOR660').attr('class', 'btn btn-light');
            //}
            //if (_dt.rows[0].KBNOR660EX == 0) {
            //    $('#btnKBNOR660EX').attr('readonly', true);
            //    $('#btnKBNOR660EX').attr('class', 'btn btn-light');
            //}
            //if (_dt.rows[0].KBNOR670 == 0) {
            //    $('#btnKBNOR670').attr('readonly', true);
            //    $('#btnKBNOR670').attr('class', 'btn btn-light');
            //}
            xSplash.hide();
        });
    };
    initial();
    xAjax.onClick('btnKBNOR610', function () {
        //console.log('>>'+btnKBNOR610.label+'<<');
        //if (btnKBNOR610.label != 'Delete PDS Normal') {
        //    btnKBNOR610.label = 'Delete PDS Normal';
        //} else if (btnKBNOR610.label != 'ABC') {
        //    btnKBNOR610.label = 'ABC';
        //}
        xAjax.redirect('KBNOR610');
    });
    xAjax.onClick('btnKBNOR620', function () {
        xAjax.redirect('KBNOR620');
    });
    xAjax.onClick('btnKBNOR630', function () {
        xAjax.redirect('KBNOR630');
    });
    xAjax.onClick('btnKBNOR640', function () {
        xAjax.redirect('KBNOR640');
    });
});
