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
        yield _xLib.AJAX_Get("/api/KBNIM0042/GetCustomerCode", '', function (success) {
            if (success.status == "200") {
                $("#F_Customer").empty();
                $("#F_Customer").append('<option value="" hidden></option>');
                success.data = JSON.parse(success.data);
                success.data = _xLib.TrimArrayJSON(success.data);
                success.data.forEach(function (item) {
                    $("#F_Customer").append('<option value="' + item.F_Customer_Cd + '">' + item.F_Customer_Cd + '</option>');
                });
            }
        });
        xSplash.hide();
    });
});
$("#F_Customer").change(function () {
    _xLib.AJAX_Get("/api/KBNIM0042/GetStartJigIn", { F_Customer_Cd: $("#F_Customer").val() }, function (success) {
        if (success.status == "200") {
            for (var each in success.data) {
                success.data[each] = JSON.parse(success.data[each]);
                success.data[each] = _xLib.TrimArrayJSON(success.data[each]);
            }
            //console.log(success.data);
            var table = $("#tableManage tbody tr td");
            var tableCKD = $("#tableCKD tbody tr td");
            //console.log(tableCKD);
            //console.log(table[0]);
            //console.log(success.data["frame"][0].F_Seq_nO);
            $(table[0]).text(success.data["frame"][0].F_Seq_nO);
            $(table[1]).text(success.data["dedion"][0].F_Seq_nO);
            $(table[2]).text(success.data["tg"][0].F_Seq_nO);
            $(table[3]).text(success.data["rr"][0].F_Seq_nO);
            $(tableCKD[0]).text(success.data["frameCKD"][0].F_Seq_nO);
            $(tableCKD[1]).text(success.data["dedionCKD"][0].F_Seq_nO);
            $(tableCKD[2]).text(success.data["tgCKD"][0].F_Seq_nO);
            $(tableCKD[3]).text(success.data["rrCKD"][0].F_Seq_nO);
        }
    });
});
$("#tableManage tbody tr td").on("change", "input", function () {
    var value = $(this).val();
    var inputIndex = $(this).closest("td").index();
    var table = $("#tableManage tbody tr td");
    var tableCKD = $("#tableCKD tbody tr td");
    var _StartJigFrame = $(table[0]).text();
    var _StartJigDedion = $(table[1]).text();
    var _StartJigTG = $(table[2]).text();
    var _StartJigRR = $(table[3]).text();
    if (!_StartJigFrame && inputIndex == 1)
        return;
    else if (!_StartJigDedion && inputIndex == 2)
        return;
    else if (!_StartJigTG && inputIndex == 3)
        return;
    else if (!_StartJigRR && inputIndex == 4)
        return;
    var F_Part_Type = $("#tableManage thead").find("th").eq(inputIndex).text();
    _xLib.AJAX_Get("/api/KBNIM0042/GetEndJigIn", { F_Customer_Cd: $("#F_Customer").val(), F_Part_Type: F_Part_Type, F_Remain_Unit: value }, function (success) {
        if (success.status == "200") {
            for (var each in success.data) {
                success.data[each] = JSON.parse(success.data[each]);
                success.data[each] = _xLib.TrimArrayJSON(success.data[each]);
            }
            var _index = inputIndex + 7;
            var _indexCKD = inputIndex + 3;
            console.log(success.data);
            $(table[_index]).text(success.data["endSeq"][0].F_Seq_nO);
            $(tableCKD[_indexCKD]).text(success.data["endSeqCKD"][0].F_Seq_nO);
        }
    });
});
$("#buttonConfirm").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        var table = $("#tableManage tbody tr td");
        var tableCKD = $("#tableCKD tbody tr td");
        let message = "";
        for (let i = 0; i < 4; i++) {
            //console.log($(table[i]).text());
            var _value = $(table[i + 4]).find("input").val();
            //console.log(_value);
            if (!_value && $(table[i]).text() != "") {
                alert("Please fill in all the fields");
                return;
            }
            var obj = {
                F_Customer_Cd: $("#F_Customer").val(),
                F_Part_Type: $("#tableManage thead").find("th").eq(i + 1).text(),
                F_Start_Jig: $(table[i]).text(),
                F_End_Jig: $(table[i + 8]).text(),
                F_Start_Jig_CKD: $(tableCKD[i]).text(),
                F_End_Jig_CKD: $(tableCKD[i + 4]).text(),
                F_Delivery_Date: $("#F_Process_Date").val().replace(/-/g, ""),
            };
            console.log(obj);
            yield _xLib.AJAX_Post("/api/KBNIM0042/Confirm", JSON.stringify(obj), function (success) {
                message = success.message;
            });
        }
        xSwal.success("Success", message);
    });
});
