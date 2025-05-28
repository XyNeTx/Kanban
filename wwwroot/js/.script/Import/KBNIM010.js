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
        yield $("#tableImport").DataTable({
            "processing": true,
            "serverSide": false,
            width: '50%',
            paging: false,
            sorting: false,
            searching: false,
            autoWidth: false,
            scrollX: false,
            scrollY: "300px", // "300px"
            scrollCollapse: true,
            ordering: false,
            columns: [
                {
                    data: "No", title: "No", render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                { title: "Type_Import", data: "F_TYPE", },
                { title: "Records", data: "F_REcords", },
            ],
        });
        yield $("#tableConfrim").DataTable({
            "processing": true,
            "serverSide": false,
            width: '50%',
            paging: false,
            sorting: false,
            searching: false,
            autoWidth: false,
            scrollX: false,
            scrollY: "300px", // "300px"
            scrollCollapse: true,
            ordering: false,
            columns: [
                {
                    data: "No", title: "No", width: "10%", render: function (data, type, row, meta) {
                        return meta.row + meta.settings._iDisplayStart + 1;
                    }
                },
                { title: "Type_Import", data: "F_TYPE", width: "20%" },
                { title: "Records", data: "F_REcords", width: "10%" },
            ],
        });
        let date = moment(_xLib.GetCookie("loginDate").substring(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY");
        let shift = _xLib.GetCookie("loginDate").substring(10, 11);
        yield _xLib.AJAX_Get("/api/KBNIM010/Onload", { Date: date, Shift: shift }, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                if (success.status == "200") {
                    yield ListData(date, shift);
                }
            });
        }, function (error) {
            xSwal.error(error.responseJSON.title, error.responseJSON.message);
            console.log(error);
        });
    });
});
function ListData(date, shift) {
    return __awaiter(this, void 0, void 0, function* () {
        yield _xLib.AJAX_Get("/api/KBNIM010/ListData", { Date: date, Shift: shift }, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                if (success.status == "200") {
                    data = JSON.parse(success.data[0]);
                    data2 = JSON.parse(success.data[1]);
                    //console.log(data);
                    //console.log(data2);
                    $("#tableImport").DataTable().rows.add(data).draw();
                    $("#tableConfrim").DataTable().rows.add(data2).draw();
                    $("#tableImport").DataTable().columns.adjust().draw();
                    $("#tableConfrim").DataTable().columns.adjust().draw();
                    yield $("table thead tr th").css("text-align", "center");
                    yield $("table tbody tr td").css("text-align", "center");
                    yield xSplash.hide();
                }
            });
        }, function (error) {
            return __awaiter(this, void 0, void 0, function* () {
                yield xSplash.hide();
                xSwal.error(error.responseJSON.title, error.responseJSON.error);
                console.log(error);
            });
        });
    });
}
$("#btnConfirm").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        let date = moment(_xLib.GetCookie("loginDate").substring(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY");
        let shift = _xLib.GetCookie("loginDate").substring(10, 11);
        let isConfirm = yield xSwal.confirm("Are you sure?", "Do you want Confirm All Import Data?");
        if (isConfirm) {
            _xLib.AJAX_Post("/api/KBNIM010/Confirm", (JSON.stringify({ date: date, shift: shift })), function (success) {
                if (success.status == "200") {
                    xSwal.success("Success", success.message);
                }
            }, function (error) {
                xSwal.error(error.responseJSON.title, error.responseJSON.message);
                console.log(error);
            });
        }
    });
});
