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
var _CookieProcessDate = _xLib.GetCookie("processDate");
var _CookieLoginDate = _xLib.GetCookie("loginDate");
$(document).ready(function () {
    $("#txtProcessDate").val(_CookieProcessDate.substring(0, 4) + "-" + _CookieProcessDate.substring(5, 7) + "-" + _CookieProcessDate.substring(8, 10));
    var shift = _CookieProcessDate.substring(10, 11) == "D" ? "1 - Day Shift" : "2 - Night Shift";
    $("#txtProcessShift").val(shift);
    initial = function () {
        return __awaiter(this, void 0, void 0, function* () {
            yield _xLib.AJAX_Get('/xapi/GetCheckNormalProcess', "", function (success) {
                for (let i = 1; i <= success.data.f_Value2; i++) {
                    let _btnName = `btnKBNOR1${i}0`;
                    $(`#${_btnName}`).removeClass('btn-success').css('background-color', '#faa2c1');
                    $(`#${_btnName}`).css("color", "white");
                }
            }, function (error) {
                console.log(error);
                xSwal.error("Error", error.responseJSON.message);
            });
            yield _xLib.AJAX_Get('/api/KBNOR100/Onload', { dateShift: _xLib.GetCookie("loginDate").replaceAll("-", "") }, function (data) {
                console.log(data);
            }, function (error) {
                if (error.responseJSON.cmd) {
                    error.responseJSON.cmd.forEach(function (item) {
                        let _btnName = `btn${item}`;
                        $(`#${_btnName}`).prop('disabled', true);
                    });
                }
                xSwal.error("Error", error.responseJSON.message);
            });
            xSplash.hide();
        });
    };
    initial();
    xAjax.onClick('btnKBNOR110', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR120', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR121', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR122', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR123', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR130', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR140', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR150', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR150EX', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
    xAjax.onClick('btnKBNOR160', function (e) {
        let _redirect = e.target.id.replace('btn', '');
        _xLib.SetProcessCookie(_redirect);
        xAjax.redirect(_redirect);
    });
});
