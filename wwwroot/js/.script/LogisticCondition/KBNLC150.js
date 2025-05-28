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
let Files = [];
$(document).ready(function () {
    let YM = moment().format('YYYYMM');
    _xLib.AJAX_Get("/api/KBNLC150/ShowRevision", { YM: YM }, function (success) {
        if (success.status == 200) {
            console.log(success.data);
            $('#F_Revision').empty();
            $('#F_Revision').append('<option value="" hidden></option>');
            success.data = JSON.parse(success.data);
            console.log(success.data);
            $('#F_Revision').append('<option value="' + success.data + '">' + success.data + '</option>');
        }
    }, function (error) {
        xSwal.error(error.responseJSON.response, error.responseJSON.message);
    }).then(function () {
        xSplash.hide();
    });
});
$("#F_Production_Month").change(function () {
    let YM = $("#F_Production_Month").val().replace(/-/g, '');
    _xLib.AJAX_Get("/api/KBNLC150/ShowRevision", { YM: YM }, function (success) {
        if (success.status == 200) {
            console.log(success.data);
            $('#F_Revision').empty();
            $('#F_Revision').append('<option value="" hidden></option>');
            success.data = JSON.parse(success.data);
            console.log(success.data);
            $('#F_Revision').append('<option value="' + success.data + '">' + success.data + '</option>');
        }
    }, function (error) {
        xSwal.error(error.responseJSON.response, error.responseJSON.message);
    });
});
$("#inpFile").change(function () {
    //console.log(this);
    Files = this.files[0];
    //console.log(Files);
});
$("#btnImport").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        if ($("#F_Revision").val() == "") {
            return xSwal.error("Import Error", "Please select Revision");
        }
        let YM = $("#F_Production_Month").val().replace(/-/g, '');
        _xLib.AJAX_Get("/api/KBNLC150/ShowRevision", { YM: YM }, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                if (success.status == 200) {
                    const file = Files;
                    //console.log(file);
                    if (!file)
                        return xSwal.error("Import File Error", "No file selected");
                    //console.log('File being processed:', file);
                    const arrayBuffer = yield file.arrayBuffer();
                    const read = yield XLSX.read(arrayBuffer);
                    let newRead = read;
                    //const oldData = XLSX.utils.sheet_to_json(read.Sheets[read.SheetNames[0]]);
                    //console.log('oldData:', oldData);
                    for (var key in newRead.Sheets[newRead.SheetNames[0]]) {
                        newRead.Sheets[newRead.SheetNames[0]][key].v = newRead.Sheets[newRead.SheetNames[0]][key].w;
                    }
                    let data = XLSX.utils.sheet_to_json(newRead.Sheets[newRead.SheetNames[0]]);
                    for (let each in data) {
                        data[each].F_Rev = $('#F_Revision').val();
                        data[each].F_Plant = _xLib.GetCookie('plantCode');
                        data[each].F_YM = $('#F_Production_Month').val().replace(/-/g, '');
                    }
                    const getProcessBar = setInterval(() => {
                        _xLib.AJAX_Get("/api/KBNLC150/GetProcessBar", null, function (success) {
                            if (success.status == 200) {
                                $('#pgsStatus').css('width', success.data + '%');
                                $('#pgsStatus').attr('aria-valuenow', success.data);
                                $('#pgsStatus').text(success.data + '%');
                            }
                        }, function (error) {
                            xSwal.error(error.responseJSON.response, error.responseJSON.message);
                        });
                    }, 1000);
                    _xLib.AJAX_Post("/api/KBNLC150/Import", JSON.stringify(data), function (success) {
                        if (success.status == 200) {
                            $("#pgsStatus").css('width', '100%');
                            $("#pgsStatus").attr('aria-valuenow', '100');
                            $("#pgsStatus").text('100%');
                            setTimeout(() => {
                                xSwal.success("Import Success", success.message);
                            }, 1000);
                            clearInterval(getProcessBar);
                        }
                    }, function (error) {
                        xSwal.error(error.responseJSON.response, error.responseJSON.message);
                        var obj = {
                            UserID: ajexHeader.UserCode,
                            Type: "IDT"
                        };
                        _xLib.OpenReportObj("/KBNIMERR", obj);
                    });
                }
            });
        }, function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
            var obj = {
                UserID: ajexHeader.UserCode,
                Type: "IDT"
            };
            _xLib.OpenReportObj("/KBNIMERR", obj);
        });
    });
});
