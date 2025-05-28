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
    $("#txtProcessDate").val(moment(_CookieProcessDate.substring(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));
    var shift = _CookieProcessDate.substring(10, 11) == "D" ? "1 - Day Shift" : "2 - Night Shift";
    $("#txtProcessShift").val(shift);
    _xLib.AJAX_Get("/api/KBNOR140/GetPDSNo", { type: "N" }, function (success) {
        success.data = JSON.parse(success.data);
        $('#itmPDS').empty();
        $('#itmPDSTo').empty();
        $("#itmPDS").selectpicker('refresh');
        $("#itmPDSTo").selectpicker('refresh');
        success.data.forEach(function (item) {
            $('#itmPDS').append(`<option value="${item.F_OrderNo}">${item.F_OrderNo}</option>`);
            $('#itmPDSTo').append(`<option value="${item.F_OrderNo}">${item.F_OrderNo}</option>`);
        });
        $("#itmPDS").selectpicker('refresh');
        $("#itmPDSTo").selectpicker('refresh');
    }, function (error) {
        xSwal.error(error.responseJSON.response, error.responseJSON.message);
    });
    xAjax.onClick('btnExit', function () {
        xAjax.redirect('KBNOR100');
    });
    xSplash.hide();
    xAjax.onClick('#btnPrintPDS', function () {
        return __awaiter(this, void 0, void 0, function* () {
            var _a, _b;
            if ($('#chkPDSNo').val() == 1 && ($('#itmPDS').val() == '' || $('#itmPDSTo').val() == ''))
                MsgBox("Please input PDS From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");
            if ($('#chkDeliveryDate').is(':checked')) {
                var dateFrom = (_a = moment($('#itmDelivery').val(), "DD/MM/YYYY").format("YYYY-MM-DD")) !== null && _a !== void 0 ? _a : '';
                var dateTo = (_b = moment($('#itmDeliveryTo').val(), "DD/MM/YYYY").format("YYYY-MM-DD")) !== null && _b !== void 0 ? _b : '';
            }
            else {
                var dateFrom = '2024-09-01';
                var dateTo = '2999-12-31';
            }
            yield xAjax.Execute({
                data: {
                    "Module": "[exec].[spKBNOR450_RPT_PDS]",
                    "@OrderType": "N",
                    "@Plant": ajexHeader.Plant,
                    "@UserCode": ajexHeader.UserCode,
                    "@itmPDSFrom": $('#itmPDS').val(),
                    "@itmPDSTo": $('#itmPDSTo').val(),
                    "@itmDelivery": dateFrom,
                    "@itmDeliveryTo": dateTo
                },
            });
            var pdsList = [];
            $("#itmPDS option").each(function () {
                pdsList.push($(this).val());
            });
            console.log(pdsList);
            yield pdsList.forEach(function (each) {
                return __awaiter(this, void 0, void 0, function* () {
                    yield xAjax.PostAsync({
                        url: 'KBNOR700/PDS_GENBARCODE',
                        data: {
                            'PDSNO': each + "D"
                        },
                    });
                });
            });
            yield xSwal.success('Success', 'Redirecting to View Report');
            console.log('spKBNOR140_RPT_PDS');
            yield window.open(`http://hmmta-tpcap/E-Report/Report.aspx?Register=PDS&PDSNoFrom=${$('#itmPDS').val()}&PDSNoTo=${$('#itmPDSTo').val()}&DateFrom=${dateFrom}&DateTo=${dateTo}`);
            if (_xLib.GetCookie('isDev') == "1") {
                yield _xLib.OpenReport("/KBNOR700PDS_X2", `pUserCode=${ajexHeader.UserCode}
                &OrderNo=${$('#itmPDS').val()}
                &OrderNoTo=${$('#itmPDSTo').val()}
                &DeliveryDate=${dateFrom}
                &DeliveryDateTo=${dateTo}
                &F_Plant=${ajexHeader.Plant}`);
            }
            else {
                yield _xLib.OpenReport("/KBNOR700PDS", `pUserCode=${ajexHeader.UserCode}
                &OrderNo=${$('#itmPDS').val()}
                &OrderNoTo=${$('#itmPDSTo').val()}
                &DeliveryDate=${dateFrom}
                &DeliveryDateTo=${dateTo}
                &F_Plant=${ajexHeader.Plant}`);
            }
        });
    });
    $("#chkDeliveryDate").change(function () {
        if ($(this).is(":checked")) {
            $("#itmDelivery").prop("disabled", false);
            $("#itmDeliveryTo").prop("disabled", false);
            $("#itmDelivery").parent().find("button").prop("disabled", false);
            $("#itmDeliveryTo").parent().find("button").prop("disabled", false);
        }
        else {
            $("#itmDelivery").prop("disabled", true);
            $("#itmDeliveryTo").prop("disabled", true);
            $("#itmDelivery").parent().find("button").prop("disabled", true);
            $("#itmDeliveryTo").parent().find("button").prop("disabled", true);
        }
    });
    xAjax.onClick('#btnPrintKanban', function () {
        return __awaiter(this, void 0, void 0, function* () {
            var _a, _b, _c, _d;
            if ($('#chkPDSNo').val() == 1 && ($('#itmPDS').val() == '' || $('#itmPDSTo').val() == ''))
                MsgBox("Please input PDS From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");
            var PDSFrom = (_a = $('#itmPDS').val()) !== null && _a !== void 0 ? _a : '';
            var PDSTo = (_b = $('#itmPDSTo').val()) !== null && _b !== void 0 ? _b : '';
            if ($("#chkDeliveryDate").is(":checked")) {
                var DeliveryFrom = (_c = moment($('#itmDelivery').val(), "DD/MM/YYYY").format("YYYY-MM-DD")) !== null && _c !== void 0 ? _c : '';
                var DeliveryTo = (_d = moment($('#itmDeliveryTo').val(), "DD/MM/YYYY").format("YYYY-MM-DD")) !== null && _d !== void 0 ? _d : '';
            }
            else {
                var DeliveryFrom = '2024-09-01';
                var DeliveryTo = '2999-12-31';
            }
            var _dt = yield xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR450_RPT_KANBAN]",
                    "@OrderType": "N",
                    "@Plant": ajexHeader.Plant,
                    "@UserCode": ajexHeader.UserCode
                },
            });
            var _pds = '', _OrderType = '', _Plant = '', _DeliveryDate = '';
            for (var i = 0; i < _dt.rows.length; i++) {
                _OrderType = Trim(_dt.rows[i].F_Order_Type);
                _OrderType = (_OrderType == 'N' ? 'NORMAL' : (_OrderType == 'S' ? 'SPECIAL' : (_OrderType == 'U' ? 'URGENT' : '')));
                _Plant = Trim(_dt.rows[i].F_Plant);
                _Plant = (_Plant == '1' ? 'SAMRONG PLANT' : (_Plant == '2' ? 'BANGPEE PLANT' : (_Plant == 'B' ? 'SAMRONG WAREHOUSE' : 'BANGPRAKONG PLANT')));
                _DeliveryDate = Trim(_dt.rows[i].F_Delivery_Date);
                _DeliveryDate = _DeliveryDate.substring(6, 8) + '/' + _DeliveryDate.substring(4, 6) + '/' + _DeliveryDate.substring(0, 4);
                _pds = _pds + '00' + Trim(_dt.rows[i].F_PartNO) + '0|';
                _pds = _pds + Trim(_dt.rows[i].F_Supplier_INT) + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Supplier_CD) + '|';
                _pds = _pds + _DeliveryDate + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Delivery_Trip) + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Delivery_Time) + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Delivery_Dock) + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Dock_Code) + '|';
                _pds = _pds + _Plant + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Kanban_No) + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Part_name) + '|';
                _pds = _pds + Trim(_dt.rows[i].F_OrderNO) + '|';
                _pds = _pds + _OrderType + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Box_Qty) + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Page) + '/' + Trim(_dt.rows[i].F_Page_Total) + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Unit_Amount) + '|';
                _pds = _pds + Trim(_dt.rows[i].F_Remark) + '|';
                _pds = (_pds != '' ? _pds + ',@@,' : _pds + '');
                //console.log(i);
            }
            yield xAjax.Post({
                url: 'KBNOR700/PDS_GENQRCODE',
                data: {
                    'PDSNO': _pds
                },
                then: function (result) {
                    if (result.status == 200) {
                        xSwal.success('Success', 'Redirecting to View Report');
                        if (_xLib.GetCookie('isDev') == "1") {
                            return _xLib.OpenReport("/KBNOR700KANBAN_X2", `pUserCode=${ajexHeader.UserCode}` +
                                `&OrderNo=${PDSFrom}&OrderNoTo=${PDSTo}&DeliveryDate=${DeliveryFrom}&DeliveryDateTo=${DeliveryTo}`);
                        }
                        else {
                            return _xLib.OpenReport("/KBNOR700KANBAN", `pUserCode=${ajexHeader.UserCode}` +
                                `&OrderNo=${PDSFrom}&OrderNoTo=${PDSTo}&DeliveryDate=${DeliveryFrom}&DeliveryDateTo=${DeliveryTo}`);
                        }
                        console.log('spKBNOR140_RPT_KANBAN');
                    }
                    return xSwal.error('Error', 'Error while generating report');
                }
            });
        });
    });
    $("#itmDelivery").prop("disabled", true);
    $("#itmDeliveryTo").prop("disabled", true);
    $("#itmDelivery").datepicker({
        format: "dd/mm/yyyy",
        autoclose: true,
        todayHighlight: true,
        showRightIcon: false,
        value: moment().format("DD/MM/YYYY")
    });
    $("#itmDeliveryTo").datepicker({
        format: "dd/mm/yyyy",
        autoclose: true,
        todayHighlight: true,
        showRightIcon: false,
        value: moment().format("DD/MM/YYYY")
    });
});
