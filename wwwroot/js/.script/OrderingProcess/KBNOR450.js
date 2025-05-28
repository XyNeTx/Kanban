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
        const xKBNOR450 = new MasterTemplate({
            Controller: _PAGE_,
            Table: 'tblMaster',
            ColumnTitle: {
                "EN": ['Plant', 'Parent Part', 'Ruibetsu', 'Effective Date', 'End Date'],
                "TH": ['Plant', 'Parent Part', 'Ruibetsu', 'Effective Date', 'End Date'],
                "JP": ['Plant', 'Parent Part', 'Ruibetsu', 'Effective Date', 'End Date'],
            },
            ColumnValue: [
                { "data": "F_Plant" },
                { "data": "F_Parent_Part" },
                { "data": "F_Part_Name" },
                { "data": "F_Effect_Date" },
                { "data": "F_End_Date" }
            ],
            Modal: 'modalMaster',
            Form: 'frmMaster',
            PostData: [
                { name: 'F_Plant', value: _PLANT_ }
            ],
        });
        xKBNOR450.prepare();
        xKBNOR450.initial(function (result) {
            $('#itmPDSFrom').empty();
            $('#itmPDSTo').empty();
            $("#itmPDSFrom").selectpicker('refresh');
            $("#itmPDSTo").selectpicker('refresh');
            result.data.PDSNo.forEach(function (item) {
                $('#itmPDSFrom').append(`<option value="${item.F_OrderNo}">${item.F_OrderNo}</option>`);
                $('#itmPDSTo').append(`<option value="${item.F_OrderNo}">${item.F_OrderNo}</option>`);
            });
            $("#itmPDSFrom").selectpicker('refresh');
            $("#itmPDSTo").selectpicker('refresh');
            xAjax.onClick('#chkPDSNo', function () {
                if ($('#chkPDSNo').val() == 0)
                    $('#fldPDSNo').prop('disabled', 'disabled');
                if ($('#chkPDSNo').val() == 1)
                    $('#fldPDSNo').prop('disabled', false);
            });
            xAjax.onClick('#chkSupplierCode', function () {
                if ($('#chkSupplierCode').val() == 0)
                    $('#fldSupplierCode').prop('disabled', 'disabled');
                if ($('#chkSupplierCode').val() == 1)
                    $('#fldSupplierCode').prop('disabled', false);
            });
            xAjax.onClick('#chkDeliveryDate', function () {
                if ($('#chkDeliveryDate').val() == 0)
                    $('#fldDeliveryDate').prop('disabled', 'disabled');
                if ($('#chkDeliveryDate').val() == 1)
                    $('#fldDeliveryDate').prop('disabled', false);
            });
            xSplash.hide();
        });
        xAjax.onClick('#btnPrintPDS', function () {
            return __awaiter(this, void 0, void 0, function* () {
                var _a, _b;
                if ($('#chkPDSNo').val() == 1 && ($('#itmPDSFrom').val() == '' || $('#itmPDSTo').val() == ''))
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
                        "@OrderType": "U",
                        "@Plant": ajexHeader.Plant,
                        "@UserCode": ajexHeader.UserCode,
                        "@itmPDSFrom": $('#itmPDSFrom').val(),
                        "@itmPDSTo": $('#itmPDSTo').val(),
                        "@itmDeliveryFrom": dateFrom,
                        "@itmDeliveryTo": dateTo
                    },
                });
                var pdsList = [];
                $("#itmPDSFrom option").each(function () {
                    pdsList.push($(this).val());
                });
                console.log(pdsList);
                yield pdsList.forEach(function (each) {
                    return __awaiter(this, void 0, void 0, function* () {
                        yield xAjax.Post({
                            url: 'KBNOR700/PDS_GENBARCODE',
                            data: {
                                'PDSNO': each + "D"
                            },
                        });
                    });
                });
                xSwal.success('Success', 'Redirecting to View Report');
                console.log('spKBNOR450_RPT_PDS');
                window.open(`http://hmmta-tpcap/E-Report/Report.aspx?Register=PDS&PDSNoFrom=${$('#itmPDSFrom').val()}&PDSNoTo=${$('#itmPDSTo').val()}&DateFrom=${dateFrom}&DateTo=${dateTo}`);
                if (_xLib.GetCookie('isDev') == "1") {
                    yield _xLib.OpenReport("/KBNOR700PDS_X2", `pUserCode=${ajexHeader.UserCode}
                &OrderNo=${$('#itmPDSFrom').val()}
                &OrderNoTo=${$('#itmPDSTo').val()}
                &DeliveryDate=${dateFrom}
                &DeliveryDateTo=${dateTo}
                &F_Plant=${ajexHeader.Plant}`);
                }
                else {
                    yield _xLib.OpenReport("/KBNOR700PDS", `pUserCode=${ajexHeader.UserCode}
                &OrderNo=${$('#itmPDSFrom').val()}
                &OrderNoTo=${$('#itmPDSTo').val()}
                &DeliveryDate=${dateFrom}
                &DeliveryDateTo=${dateTo}
                &F_Plant=${ajexHeader.Plant}`);
                }
            });
        });
        xAjax.onClick('#btnPrintKanban', function () {
            return __awaiter(this, void 0, void 0, function* () {
                var _a, _b, _c, _d;
                if ($('#chkPDSNo').val() == 1 && ($('#itmPDSFrom').val() == '' || $('#itmPDSTo').val() == ''))
                    MsgBox("Please input PDS From, To before print PDS...", MsgBoxStyle.Exclamation, "Exclamation");
                var PDSFrom = (_a = $('#itmPDSFrom').val()) !== null && _a !== void 0 ? _a : '';
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
                        "@OrderType": "U",
                        "@Plant": ajexHeader.Plant,
                        "@UserCode": ajexHeader.UserCode,
                        "@itmPDSFrom": PDSFrom,
                        "@itmPDSTo": PDSTo,
                        "@itmDeliveryFrom": DeliveryFrom,
                        "@itmDeliveryTo": DeliveryTo
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
                            console.log('spKBNOR450_RPT_KANBAN');
                        }
                        return xSwal.error('Error', 'Error while generating report');
                    }
                });
            });
        });
        xAjax.onClick('btnExit', function () {
            xAjax.redirect('KBNOR400');
        });
    });
});
