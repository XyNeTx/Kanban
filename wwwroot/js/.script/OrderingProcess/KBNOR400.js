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
            //        "Module": "[exec].[spKBNOR400]",
            //        "@OrderType": "U",
            //        "@Plant": ajexHeader.Plant,
            //        "@UserCode": ajexHeader.UserCode
            //    },
            //});
            //if (_dt.rows[0].KBNOR410 == 0) {
            //    $('#btnKBNOR410').attr('readonly', true);
            //    $('#btnKBNOR410').attr('class', 'btn btn-light');
            //}
            //if (_dt.rows[0].KBNOR420 == 0) {
            //    $('#btnKBNOR420').attr('readonly', true);
            //    $('#btnKBNOR420').attr('class', 'btn btn-light');
            //}
            //if (_dt.rows[0].KBNOR440 == 0) {
            //    $('#btnKBNOR440').attr('readonly', true);
            //    $('#btnKBNOR440').attr('class', 'btn btn-light');
            //}
            //if (_dt.rows[0].KBNOR450 == 0) {
            //    $('#btnKBNOR450').attr('readonly', true);
            //    $('#btnKBNOR450').attr('class', 'btn btn-light');
            //}
            //if (_dt.rows[0].KBNOR460 == 0) {
            //    $('#btnKBNOR460').attr('readonly', true);
            //    $('#btnKBNOR460').attr('class', 'btn btn-light');
            //}
            //if (_dt.rows[0].KBNOR460EX == 0) {
            //    $('#btnKBNOR460EX').attr('readonly', true);
            //    $('#btnKBNOR460EX').attr('class', 'btn btn-light');
            //}
            //if (_dt.rows[0].KBNOR470 == 0) {
            //    $('#btnKBNOR470').attr('readonly', true);
            //    $('#btnKBNOR470').attr('class', 'btn btn-light');
            //}
            let _processCk = _xLib.GetProcessCookie();
            console.log(_processCk);
            if (_processCk != null) {
                _processCk.forEach(function (item) {
                    let _btnName = `btn${item}`;
                    $(`#${_btnName}`).removeClass('btn-success').css('background-color', '#faa2c1');
                    $(`#${_btnName}`).css("color", "white");
                });
            }
            xSplash.hide();
        });
    };
    initial();
    xAjax.onClick('btnKBNOR410', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR420', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR440', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR450', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR460', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR460EX', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR470', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
});
