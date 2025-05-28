"use strict";
$(document).ready(function () {
    _initial = function () {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            headers: ajexHeader,
            url: (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + _PAGE_ + '/initial',
            success: function (result) {
                if (result.response == "OK") {
                    xDropDownList.bind('#frmCondition #F_Plant', result.data.TB_MS_Factory, 'F_Plant', 'F_Plant_Name');
                    xSplash.hide();
                }
                else {
                    xSwal.error('Error', result.message);
                    xSplash.hide();
                }
            },
            error: function (result) {
                console.error(_Controller + '.Initial: ' + result.responseText);
                xSplash.hide();
            }
        });
    };
    _initial();
    _setDayInMonth = function () {
        let _sDate = xDate.Now('yyyy-MM-01');
        if ($('#F_Plant_Date').val() != '')
            _sDate = $('#F_Plant_Date').val() + '-01';
        let _date = new Date(_sDate);
        //console.log(_date);
        let _sd = new Date(_date.setMonth(_date.getMonth()));
        //console.log(_sd);
        let _e = new Date(_date.setMonth(_date.getMonth() + 1));
        //console.log(_e);
        let _ed = new Date(_e.getFullYear(), _e.getMonth(), 0);
        //console.log(_ed);
        for (var i = 0; i <= 41; i++) {
            if (i >= _sd.getDay() && i < (_sd.getDay() + _ed.getDate())) {
                let _n = ((_sd.getDay() - i) * -1) + 1;
                $('#' + i + 'D').text(_n);
                $('#' + i + 'N').text(_n);
                $('#' + i + 'D').attr('class', 'Calendar_Delivery');
                $('#' + i + 'N').attr('class', 'Calendar_Delivery');
            }
            else {
                $('#' + i + 'D').text('');
                $('#' + i + 'N').text('');
                $('#' + i + 'D').removeAttr('class');
                $('#' + i + 'N').removeAttr('class');
            }
        }
    };
    _setDayInMonth();
    xAjax.onChange('#F_Plant_Date', function () {
        _setDayInMonth();
    });
    xAjax.onClick('#tblCalendar td', function (e) {
        if ($(e.target).attr('class') == 'Calendar_Delivery') {
            $('#' + e.target.id).attr('class', 'Calendar_NonDelivery');
        }
        else if ($(e.target).attr('class') == 'Calendar_NonDelivery') {
            $('#' + e.target.id).attr('class', 'Calendar_Delivery');
        }
    });
});
