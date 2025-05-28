"use strict";
let _cmd = "";
$(document).ready(function () {
    _initial = function () {
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            headers: ajexHeader,
            url: (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/api/' + _PAGE_ + '/initial',
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
    $("#F_YM").prop('disabled', true);
    $("#F_Store_cd").prop('disabled', true);
    _initial();
    _setDayInMonth();
    $("#tableCalendar tbody td").click(function () {
        if ($(this).text() == '')
            return;
        if (_cmd == "New" || _cmd == "Update") {
            var _inputID = $(this).find('input[type="hidden"]').attr('id');
            if ($(this).attr('class') == 'bg-success text-center fw-bolder') {
                $(this).attr('class', 'bg-danger text-center fw-bolder');
                $(`#${_inputID}`).val(0);
            }
            else {
                $(this).attr('class', 'bg-success text-center fw-bolder');
                $(`#${_inputID}`).val(1);
            }
        }
    });
});
$("#F_YM , #F_Store_cd").change(function () {
    if (_cmd == "New") {
        _setDayInMonth();
    }
    else { //(_cmd = "Inquiry") {
        if ($(this).attr("id") == "F_YM") {
            _xLib.AJAX_Get('/api/KBNMS010/Get_StoreCode', { F_YM: $("#F_YM").val().replaceAll("-", ""), IsInquiry: true }, function (success) {
                _xLib.TrimArrayJSON(success.data);
                $('#F_Store_cd').empty();
                $("#F_Store_cd").append('<option value="" hidden></option>');
                success.data.forEach(function (item) {
                    console.log(item);
                    $('#F_Store_cd').append('<option value="' + item.f_Store_cd + '">' + item.f_Store_cd + '</option>');
                });
            }, function (error) {
                xSwal.error('Error', error.responseJSON.message);
            });
        }
        if ($("#F_Store_cd").val() && $("#F_YM").val()) {
            _xLib.AJAX_Get("/api/KBNMS010/GetCalendarData", { F_YM: $("#F_YM").val().replaceAll("-", ""), F_Store_cd: $("#F_Store_cd").val() }, function (success) {
                _xLib.TrimJSON(success.data);
                console.log(success.data);
                _setDayInMonth(success.data);
            }, function (error) {
                xSwal.error('Error', error.responseJSON.message);
            });
        }
    }
});
_setDayInMonth = function (data) {
    //console.log(data);
    let _sDate = xDate.Now('yyyy-MM-01');
    if ($('#F_YM').val() != '')
        _sDate = $('#F_YM').val() + '-01';
    let _date = new Date(_sDate);
    //console.log(_date);
    let _sd = new Date(_date.setMonth(_date.getMonth()));
    //console.log(_sd);
    let _e = new Date(_date.setMonth(_date.getMonth() + 1));
    //console.log(_e);
    let _ed = new Date(_e.getFullYear(), _e.getMonth(), 0);
    //console.log(_ed);
    let j = 1;
    for (var i = 0; i <= 41; i++) {
        if (i >= _sd.getDay() && i < (_sd.getDay() + _ed.getDate())) {
            let _n = ((_sd.getDay() - i) * -1) + 1;
            if (!data && j <= 31) { // original from P'Tor
                $('#' + i + 'D').text(_n).append("<input type='hidden' name='F_workCd_D" + j + "' id='F_workCd_D" + j + "' value='" + 1 + "' />");
                $('#' + i + 'N').text(_n).append("<input type='hidden' name='F_workCd_N" + j + "' id='F_workCd_N" + j + "' value='" + 1 + "' />");
                $('#' + i + 'D').attr('class', 'bg-success text-center fw-bolder');
                $('#' + i + 'N').attr('class', 'bg-success text-center fw-bolder');
                j++;
            }
            else { // Inquiry Data in DB
                if (j > 31)
                    break;
                let _dVal = data[`f_workCd_D${j}`];
                let _nVal = data[`f_workCd_N${j}`];
                //console.log(`f_workCd_D${j} : `, _dVal);
                //console.log(`f_workCd_N${j} : `, _nVal);
                if (_dVal == 1) {
                    $('#' + i + 'D').text(_n).append("<input type='hidden' name='F_workCd_D" + j + "' id='F_workCd_D" + j + "' value='" + 1 + "' />");
                    $('#' + i + 'D').attr('class', 'bg-success text-center fw-bolder');
                }
                else {
                    $('#' + i + 'D').text(_n).append("<input type='hidden' name='F_workCd_D" + j + "' id='F_workCd_D" + j + "' value='" + 0 + "' />");
                    $('#' + i + 'D').attr('class', 'bg-danger text-center fw-bolder');
                }
                if (_nVal == 1) {
                    $('#' + i + 'N').text(_n).append("<input type='hidden' name='F_workCd_N" + j + "' id='F_workCd_N" + j + "' value='" + 1 + "' />");
                    $('#' + i + 'N').attr('class', 'bg-success text-center fw-bolder');
                }
                else {
                    $('#' + i + 'N').text(_n).append("<input type='hidden' name='F_workCd_N" + j + "' id='F_workCd_N" + j + "' value='" + 0 + "' />");
                    $('#' + i + 'N').attr('class', 'bg-danger text-center fw-bolder');
                }
                j++;
            }
        }
        else {
            $('#' + i + 'D').text('');
            $('#' + i + 'N').text('');
            $('#' + i + 'D').removeAttr('class');
            $('#' + i + 'N').removeAttr('class');
        }
    }
};
$("#buttonNew").click(function () {
    _cmd = "New";
    _setDayInMonth();
    _xLib.AJAX_Get('/api/KBNMS010/Get_StoreCode', '', function (success) {
        _xLib.TrimArrayJSON(success.data);
        $('#F_Store_cd').empty();
        $("#F_Store_cd").append('<option value="" hidden></option>');
        success.data.forEach(function (item) {
            console.log(item);
            $('#F_Store_cd').append('<option value="' + item.f_Store_cd + '">' + item.f_Store_cd + '</option>');
        });
    }, function (error) {
        xSwal.error('Error', error.responseJSON.message);
    });
    $("#F_YM").prop('disabled', false);
    $("#F_Store_cd").prop('disabled', false);
});
$("#buttonInq").click(function () {
    $("#F_YM").prop('disabled', false);
    $("#F_Store_cd").prop('disabled', false);
    $("#buttonUpd").prop('disabled', false);
    $("#buttonDel").prop('disabled', false);
    _cmd = "Inquiry";
    _xLib.AJAX_Get('/api/KBNMS010/Get_StoreCode', { F_YM: $("#F_YM").val().replaceAll("-", ""), IsInquiry: true }, function (success) {
        _xLib.TrimArrayJSON(success.data);
        $('#F_Store_cd').empty();
        $("#F_Store_cd").append('<option value="" hidden></option>');
        success.data.forEach(function (item) {
            console.log(item);
            $('#F_Store_cd').append('<option value="' + item.f_Store_cd + '">' + item.f_Store_cd + '</option>');
        });
    }, function (error) {
        xSwal.error('Error', error.responseJSON.message);
    });
});
$("#buttonUpd").click(function () {
    _cmd = "Update";
    if ($("#F_YM").val() == '' || $("#F_Store_cd").val() == '')
        return xSwal.error('Error', 'Please select Monthly and Store Code');
    //$("#F_YM").prop('disabled', true);
    //$("#F_Store_cd").prop('disabled', true);
});
$("#buttonDel").click(function () {
    _cmd = "Delete";
    if ($("#F_YM").val() == '' || $("#F_Store_cd").val() == '')
        return xSwal.error('Error', 'Please select Monthly and Store Code');
    //$("#F_YM").prop('disabled', true);
    //$("#F_Store_cd").prop('disabled', true);
});
function getFormData($form) {
    var unindexed_array = $form.serializeArray();
    var indexed_array = {};
    $.map(unindexed_array, function (n, i) {
        indexed_array[n['name']] = n['value'];
    });
    return indexed_array;
}
$("#btnCopy").click(() => {
    let F_YM = $("#F_YM").val().replaceAll("-", "");
    _xLib.AJAX_Post('/api/KBNMS010/Copy?F_YM=' + F_YM, "", function (result) {
        if (result.status == 200) {
            $("#frmCondition").trigger('reset');
            $("#F_YM").prop('disabled', true);
            $("#F_Store_cd").prop('disabled', true);
            $("#F_YM").val(F_YM.substring(0, 4) + "-" + F_YM.substring(4, 6));
            _setDayInMonth();
            return xSwal.success('Success', result.message);
        }
        else {
            return xSwal.error('Error', result.message);
        }
    }, function (result) {
        return xSwal.error('Error', result.responseJSON.message);
    });
});
$("#buttonSave").click(function () {
    var _data = getFormData($("#frmCondition"));
    _data.F_YM = $("#F_YM").val().replaceAll("-", "");
    _data.F_Store_cd = $("#F_Store_cd").val();
    for (let i = 29; i <= 31; i++) {
        if (_data.hasOwnProperty(`F_workCd_D${i}`) == false)
            _data[`F_workCd_D${i}`] = "0";
        if (_data.hasOwnProperty(`F_workCd_N${i}`) == false)
            _data[`F_workCd_N${i}`] = "0";
    }
    if (_cmd == "New") {
        //var _data = getFormData($("#frmCondition"));
        //_data.F_YM = _data.F_YM.replaceAll("-", "");
        //console.log(_data);
        _xLib.AJAX_Post('/api/KBNMS010/Save', JSON.stringify(_data), function (result) {
            if (result.status == 200) {
                $("#frmCondition").trigger('reset');
                _setDayInMonth();
                return xSwal.success('Success', result.message);
            }
            else {
                return xSwal.error('Error', result.message);
            }
        }, function (result) {
            return xSwal.error('Error', result.responseJSON.message);
            console.error(result.responseJSON.message);
        });
    }
    else if (_cmd == "Update") {
        xSwal.question("Are you sure?", "Do you want to update this data?", function () {
            //var _data = getFormData($("#frmCondition"));
            //_data.F_YM = $("#F_YM").val().replaceAll("-", "");
            //_data.F_Store_cd = $("#F_Store_cd").val();
            //console.log(_data);
            _xLib.AJAX_Post('/api/KBNMS010/Update', JSON.stringify(_data), function (result) {
                if (result.status == 200) {
                    $("#frmCondition").trigger('reset');
                    _setDayInMonth();
                    return xSwal.success('Success', result.message);
                }
                else {
                    return xSwal.error('Error', result.message);
                }
            }, function (result) {
                return xSwal.error('Error', result.responseJSON.message);
                console.error(result.responseJSON.message);
            });
        });
    }
    else if (_cmd == "Delete") {
        xSwal.question("Are you sure?", "Do you want to delete this data?", function () {
            //var _data = getFormData($("#frmCondition"));
            //_data.F_YM = $("#F_YM").val().replaceAll("-", "");
            //_data.F_Store_cd = $("#F_Store_cd").val();
            //console.log(_data);
            _xLib.AJAX_Post('/api/KBNMS010/Delete', JSON.stringify(_data), function (result) {
                if (result.status == 200) {
                    $("#frmCondition").trigger('reset');
                    _setDayInMonth();
                    return xSwal.success('Success', result.message);
                }
                else {
                    return xSwal.error('Error', result.message);
                }
            }, function (result) {
                return xSwal.error('Error', result.responseJSON.message);
                console.error(result.responseJSON.message);
            });
        });
    }
});
