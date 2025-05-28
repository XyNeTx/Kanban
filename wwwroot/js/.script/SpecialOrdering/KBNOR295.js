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
                { title: "Username", data: "F_User_ID", width: "15%" },
                { title: "Name", data: "F_Name", width: "15%" },
                { title: "Surname", data: "F_Surname", width: "15%" },
                { title: "F_Email", data: "F_Email", width: "10%" },
                { title: "Path File", data: "F_Path_File", width: "35%" },
                {
                    title: "", render: function (data, type, row) {
                        return '<label for="' + row.F_User_ID + '" class="btn btn-primary m-0">Browse...</label> <input type="file" class="form-control" accept = ".jpg, .jpeg, .png" id="' + row.F_User_ID + '" name="file" style="display:none">';
                    }
                }
            ],
            order: [[1, 'asc']],
        });
        yield LoadContactList();
        xSplash.hide();
    });
});
function LoadContactList() {
    // Load Color Tag
    let obj = {
        F_User_ID: "...",
        F_Name: "",
        F_Surname: "",
        F_Email: "",
        F_Path_File: ""
    };
    _xLib.AJAX_Get("/api/KBNOR295/LoadContactList", "", function (success) {
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
    //if ($(".clsEdit").length > 0) return;
    _oriValue = $(this).text();
    let row = $(this).closest('tr');
    row.find('td').each(function () {
        let value = $(this).text();
        if ($(this).index() >= 4)
            return;
        $(this).empty();
        $(this).append('<input type="text" class="clsEdit form-control" id="txtEdit" value="' + value + '" />');
    });
    $('#txtEdit').focus();
});
$(document).on('focusin', '#tableMain tbody tr td input[type="text"]', function () {
    _oriValue = $(this).val();
    //console.log(_oriValue);
});
$(document).on('keydown', '#tableMain tbody tr td', function (e) {
    return __awaiter(this, void 0, void 0, function* () {
        if (e.type == 'keydown' && (e.which == 13 || e.which == 9)) {
            let _newValue = $(this).find('input').val();
            let index = yield $(this).index();
            if (index === 0) {
                var result = yield _xLib.AJAX_Get("/api/KBNOR295/ChkAuthenApproval", { F_User_ID: _newValue }, function (success) {
                    return success;
                }, function (error) {
                    xSwal.error(error.responseJSON.response, error.responseJSON.message);
                    return false;
                });
                if (result.data.result.toLowerCase() !== "can access") {
                    yield $(this).remove('input');
                    yield $(this).text("...");
                    yield $("#tableMain tbody tr td").find('input[type="text"]').each(function () {
                        return __awaiter(this, void 0, void 0, function* () {
                            yield $(this).trigger('focusin');
                            yield $(this).trigger('focusout');
                        });
                    });
                    yield xSwal.error("Error", "Can not insert this user because not authorize for approval.");
                }
            }
            if (_newValue == _oriValue || _newValue == "") {
                $(this).remove('input');
                $(this).text(_oriValue);
                return;
            }
            ;
            $(this).remove('input');
            $("#tableMain").DataTable().cell($(this)).data(_newValue).draw();
            let row = $(this).closest('tr');
            let rowIndex = $("#tableMain").DataTable().row(row).index();
            let maxIndex = $("#tableMain").DataTable().rows().count() - 1;
            if (rowIndex == maxIndex) {
                let obj = {
                    F_User_ID: "...",
                    F_Name: "",
                    F_Surname: "",
                    F_Email: "",
                    F_Path_File: ""
                };
                $("#tableMain").DataTable().row.add(obj).draw();
                $("table tbody tr td").addClass("text-center");
            }
        }
        else {
            return;
        }
    });
});
$(document).on('change', 'input[type="file"]', function () {
    //console.log("Change File");
    let file = $(this)[0].files[0];
    let _data = new FormData();
    var blob = new Blob([file], { type: "image/jpeg" });
    _data.append("file", blob, file.name);
    let row = $(this).closest('tr');
    _xLib.AJAX_PostFile("/api/KBNOR295/UploadIMG", _data, function (success) {
        //console.log(success);
        let rowData = $("#tableMain").DataTable().row(row).data();
        //console.log(rowData);
        rowData.F_Path_File = success.data;
        $("#tableMain").DataTable().row(row).data(rowData).draw();
    }, function (error) {
        console.log(error);
        xSwal.xError(error);
    });
});
$("#btnConfirm").click(function () {
    Confirm();
});
function Confirm() {
    try {
        let listObj = $('#tableMain').DataTable().rows().data().toArray();
        //console.log(listObj);
        listObj = listObj.filter(x => x.F_User_ID.trim() != "..."
            && x.F_Name.trim() != "" && x.F_Surname.trim() != ""
            && x.F_Email.trim() != "" && x.F_Path_File.trim() != "");
        //console.log(listObj);
        listObj = listObj.filter(function (el) {
            if (!el.F_Email.includes("@")) {
                throw "Email is invalid";
            }
            return el;
        });
        _xLib.AJAX_Post("/api/KBNOR295/Confirm", listObj, function (success) {
            xSwal.success(success.response, success.message);
            return LoadContactList();
        }, function (error) {
            console.log(error);
            xSwal.xError(error);
        });
    }
    catch (ex) {
        xSwal.error("Error", ex);
    }
}
