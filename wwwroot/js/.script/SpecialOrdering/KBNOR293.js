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
        $("#tableMain").DataTable({
            width: '100%',
            paging: false,
            scrollCollapse: true,
            "processing": false,
            "serverSide": false,
            scrollX: false,
            scrollY: '400px',
            searching: false,
            info: false,
            ordering: false,
            columns: [
                { title: "COLOR", data: "COLOR", width: "50%" },
                { title: "TypePart", data: "TypePart", width: "50%" },
            ],
            order: [[1, 'asc']],
        });
        yield LoadColorTag();
        xSplash.hide();
    });
});
function LoadColorTag() {
    // Load Color Tag
    let obj = {
        COLOR: "...",
        TypePart: "",
    };
    _xLib.AJAX_Get("/api/KBNOR293/LoadColorTag", "", function (success) {
        success = _xLib.JSONparseMixData(success);
        if (success.data.length > 0) {
            var table = $('#tableMain').DataTable();
            table.clear();
            table.rows.add(success.data);
            table.row.add(obj);
            table.draw();
        }
        else {
            // If there is no data, add a new row
            table.row.add(obj);
        }
        $("table thead tr th").addClass("text-center");
        $("table tbody tr td").addClass("text-center");
    }, function (error) {
        console.log(error);
    });
}
let _oriValue = "";
$(document).on('dblclick', '#tableMain tbody tr td', function () {
    if ($(".clsEdit").length > 0)
        return;
    _oriValue = $(this).text();
    let row = $(this).closest('tr');
    row.find('td').each(function () {
        let value = $(this).text();
        $(this).empty();
        $(this).append('<input type="text" class="clsEdit form-control" id="txtEdit" value="' + value + '" />');
    });
    $('#txtEdit').focus();
});
$(document).on('focusin', '#tableMain tbody tr td input[type="text"]', function () {
    _oriValue = $(this).val();
    console.log(_oriValue);
});
$(document).on('focusout  keypress', '#tableMain tbody tr td', function (e) {
    if ((e.type == 'keypress' && e.which == 13) || e.type == 'focusout') {
        let _newValue = $(this).find('input').val();
        if (_newValue == _oriValue || _newValue == "") {
            $(this).empty();
            $(this).text(_oriValue);
            return;
        }
        ;
        $(this).empty();
        $("#tableMain").DataTable().cell(this).data(_newValue).draw();
        let row = $(this).closest('tr');
        let rowIndex = $("#tableMain").DataTable().row(row).index();
        //console.log(rowIndex);
        let maxIndex = $("#tableMain").DataTable().rows().count() - 1;
        //console.log(maxIndex);
        if (rowIndex == maxIndex) {
            let obj = {
                COLOR: "...",
                TypePart: "",
            };
            $("#tableMain").DataTable().row.add(obj).draw();
            $("table tbody tr td").addClass("text-center");
        }
    }
    else {
        return;
    }
});
$("#btnConfirm").click(function () {
    SaveColorTag();
});
function SaveColorTag() {
    let listObj = $('#tableMain').DataTable().rows().data().toArray();
    listObj = listObj.filter(x => x.COLOR.trim() != "..." && x.TypePart.trim() != "");
    _xLib.AJAX_Post("/api/KBNOR293/Confirm", listObj, function (success) {
        xSwal.success(success.response, success.message);
        return LoadColorTag();
    }, (error) => __awaiter(this, void 0, void 0, function* () {
        xSwal.xError(error);
        console.log(error);
    }));
}
