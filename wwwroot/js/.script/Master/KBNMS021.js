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
        _xDataTable.InitialDataTable("#tableMain", {
            columns: [
                {
                    title: "Flag", render: function (data, type, row, meta) {
                        return `<input type="checkbox" class="chkbox">`;
                    }, width: "8%"
                },
                { title: "Line", data: "f_Line", width: "12%" },
                { title: "Part Code", data: "f_Code", width: "15%" },
                { title: "Part No.", data: "f_Part_No", width: "15%" },
                { title: "Part Name", data: "f_name", width: "25%" },
                {
                    title: "Bridge", render: function (data, type, row, meta) {
                        if (row.f_Bridge.toUpperCase() == "TRUE") {
                            return `<input type="checkbox" class="chkbox" checked>`;
                        }
                        return `<input type="checkbox" class="chkbox">`;
                    }, width: "10%"
                },
                { title: "Detail", data: "f_Detail", width: "15%" },
            ],
            scrollX: true,
            scrollY: "250px",
            scrollCollapse: false,
            paging: false,
            ordering: false,
            width: "100%"
        });
        $("table thead tr th").addClass("text-center");
        $("table tbody tr td").addClass("text-center");
        $(".selectpicker").each(function () {
            $(this).prop("disabled", true);
            $(this).selectpicker("refresh");
        });
        yield xSplash.hide();
    });
});
$(document).on("click", "#tableMain tbody tr td", function () {
    if ($("#btnUpd").prop("disabled") == true) {
        return;
    }
    //console.log($(this).index());
    if ($(this).index() == 6) {
        if ($(this).find("input").length == 0) {
            let value = $(this).text();
            $(this).empty();
            $(this).append(`<input type="text" class="form-control" id="f_Detail" name="f_Detail" value="${value}">`);
            $("input[name='f_Detail']").focus();
        }
    }
});
$(document).on("focusout , keydown", "#f_Detail", function (e) {
    //console.log(e);
    if (e.type == "focusout" || e.keyCode == 13) {
        let value = $(this).val();
        //console.log(value);
        let row = $(this).closest("tr");
        let data = $("#tableMain").DataTable().row(row).data();
        data.f_Detail = value;
        $("#tableMain").DataTable().row(row).data(data).draw();
        //$(this).find("input").remove();
    }
});
$("#btnCan").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        $(".selectpicker").each(function () {
            let id = $(this).attr("id");
            //console.log(id);
            $(`#${id}`).resetSelectPicker();
            $(this).prop("disabled", true);
            $(this).selectpicker("refresh");
        });
        $(document).find("input").each(function () {
            $(this).val("");
        });
        DisableButton(false);
        $("#readDetail").prop("readonly", true);
        $("#readPartName").prop("readonly", true);
        if ($("#inpPartCode").prop("tagName") == "INPUT" || $("#inpPartNo").prop("tagName") == "INPUT") {
            $("#inpPartCode").remove();
            $("#inpPartNo").remove();
            $("label[for='inpPartCode']").parent().append(`<select class="selectpicker col-6 ps-0" id="inpPartCode" data-live-search="true" data-size="6" disabled></select>`);
            $("label[for='inpPartNo']").parent().append(`<select class="selectpicker col-6 ps-0" id="inpPartNo" data-live-search="true" data-size="6" disabled></select>`);
            $("#inpPartCode").selectpicker("refresh");
            $("#inpPartNo").selectpicker("refresh");
        }
        else {
            $("#tableMain").DataTable().clear().draw();
            return;
        }
        //await GetLine();
        //await GetPartCode();
        //await GetPartNo();
    });
});
$("#btnInq").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        $(".selectpicker").each(function () {
            $(this).prop("disabled", false);
            $(this).selectpicker("refresh");
            $("#inpLine").resetSelectPicker();
        });
        DisableButton(true);
        $("#btnInq").prop("disabled", false);
        yield GetLine();
        yield GetPartCode();
        yield GetPartNo();
    });
});
function DisableButton(bool) {
    return __awaiter(this, void 0, void 0, function* () {
        $("#btnInq").prop("disabled", bool);
        $("#btnNew").prop("disabled", bool);
        $("#btnDel").prop("disabled", bool);
        $("#btnUpd").prop("disabled", bool);
        $("#btnSave").prop("disabled", !bool);
        $("#readDetail").prop("readonly", bool);
        $("#readPartName").prop("readonly", bool);
    });
}
$("#btnNew").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        let listLine = [
            { f_Line: "De Dion" },
            { f_Line: "Frame" },
            { f_Line: "Rear Axle" },
            { f_Line: "Side Panel" },
            { f_Line: "Tail Gate" }
        ];
        DisableButton(true);
        $("#btnNew").prop("disabled", false);
        $("#inpLine").prop("disabled", false);
        $("#inpLine").addListSelectPicker(listLine, "f_Line");
        $("#inpLine").selectpicker("refresh");
        $("#inpLine").resetSelectPicker();
        if ($("#inpPartCode").prop("tagName") == "SELECT") {
            $("#inpPartNo").parent().remove();
            $("label[for='inpPartNo']").parent().append("<input type='text' class='form-control col-6' id='inpPartNo' required>");
            $("#inpPartCode").parent().remove();
            $("label[for='inpPartCode']").parent().append("<input type='text' class='form-control col-6' id='inpPartCode' required>");
        }
        $("#readDetail").val("");
        $("#readPartName").val("");
        $("#readDetail").prop("readonly", false);
        $("#readPartName").prop("readonly", false);
    });
});
$("#btnDel").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        DisableButton(true);
        $("#btnDel").prop("disabled", false);
        $(".selectpicker").each(function () {
            $(this).prop("disabled", false);
            $(this).selectpicker("refresh");
            $(this).resetSelectPicker();
        });
        yield GetLine();
        yield GetPartCode();
        yield GetPartNo();
    });
});
$("#btnUpd").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        $(".selectpicker").each(function () {
            $(this).prop("disabled", false);
            $(this).selectpicker("refresh");
            $(this).resetSelectPicker();
        });
        DisableButton(true);
        $("#readDetail").prop("readonly", false);
        yield GetLine();
        yield GetPartCode();
        yield GetPartNo();
        $("#btnUpd").prop("disabled", false);
        //$("#readDetail").prop("readonly", false);
    });
});
$("#btnSave").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield Save();
    });
});
function GetObj() {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = {
            Line: $("#inpLine").val().substring(0, 1),
            PartCode: $("#inpPartCode").val(),
            PartNo: $("#inpPartNo").val(),
        };
        return obj;
    });
}
function GetListDataTables(chgFrom) {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = yield GetObj();
        _xLib.AJAX_Get("/api/KBNMS021/GetListDataTables", obj, function (success) {
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
            if (chgFrom == "inpPartNo") {
                $("#readPartName").val(success.data[0].f_name);
                $("#readDetail").val(success.data[0].f_Detail);
            }
        });
    });
}
function GetLine() {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = yield GetObj();
        _xLib.AJAX_Get("/api/KBNMS021/GetLine", obj, function (success) {
            $("#inpLine").addListSelectPicker(success.data, "f_Line");
            $("#inpLine").resetSelectPicker();
        });
    });
}
function GetPartCode() {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = yield GetObj();
        _xLib.AJAX_Get("/api/KBNMS021/GetPartCode", obj, function (success) {
            $("#inpPartCode").addListSelectPicker(success.data, "f_Code");
            $("#inpPartCode").resetSelectPicker();
        });
    });
}
function GetPartNo() {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = yield GetObj();
        _xLib.AJAX_Get("/api/KBNMS021/GetPartNo", obj, function (success) {
            $("#inpPartNo").addListSelectPicker(success.data, "f_Part_No");
            $("#inpPartNo").resetSelectPicker();
        });
    });
}
function Save() {
    return __awaiter(this, void 0, void 0, function* () {
        let listObj = _xDataTable.GetSelectedDataDT("#tableMain");
        if ($("#btnNew").prop("disabled") == false) {
            listObj = [];
            //console.log($("#inpPartNo").val());
            //var ruibetsu = $("#inpPartNo").val().split("-")[1];
            //console.log(ruibetsu);
            let obj = {
                f_Line: $("#inpLine").val().toString().substring(0, 1),
                f_Code: $("#inpPartCode").val(),
                f_Ruibetsu: $("#inpPartNo").val().split("-")[1],
                f_Part_No: $("#inpPartNo").val().split("-")[0],
                f_Name: $("#readPartName").val(),
                f_Bridge: "N",
                f_Detail: $("#readDetail").val(),
                f_Create_By: "edit in back",
                f_Update_By: "edit in back",
                f_Create_Date: new Date().toISOString(),
                f_Update_Date: new Date().toISOString()
            };
            //console.log(obj);
            listObj.push(obj);
        }
        else {
            if (listObj == null) {
                return;
            }
            else {
                listObj.forEach(function (item) {
                    //console.log(item);
                    item.f_Bridge = $(`#tableMain tbody tr td:contains('${item.f_Part_No}')`).parent().find("td:eq(5) input").prop("checked") == true ? "Y" : "N";
                    item.f_Ruibetsu = item.f_Part_No.split("-")[1];
                    item.f_Part_No = item.f_Part_No.split("-")[0];
                    item.f_Line = item.f_Line.substring(0, 1);
                    item.f_Create_Date = new Date().toISOString();
                    item.f_Update_Date = new Date().toISOString();
                    item.f_Create_By = "edit in back";
                    item.f_Update_By = "edit in back";
                });
            }
        }
        //console.log($("#divBtn").find("button:not(:disabled)").attr("id"));
        let action = $("#divBtn").find("button:not(:disabled)").attr("id").split("btn")[1];
        _xLib.AJAX_Post("/api/KBNMS021/Save?action=" + action, listObj, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                xSwal.success(success.response, success.message);
                if (listObj.some(x => x.f_Line == "S")) {
                    if (success.data.length > 0) {
                        //await $("#btnCan").trigger("click");
                        xSwal.error("Error", "Please Input Pair Part Together");
                        yield $("#btnNew").trigger("click");
                        $("#inpLine").resetSelectPicker();
                    }
                    else {
                        $("#btnCan").trigger("click");
                    }
                }
                else {
                    $("#btnCan").trigger("click");
                }
            });
        }, function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        });
    });
}
$(document).on("change", "#inpLine", function () {
    return __awaiter(this, void 0, void 0, function* () {
        $("#readLine").val($("#inpLine").val());
        if ($("#divBtn").find("button:not(:disabled)").attr("id") == "btnNew") {
            return;
        }
        if ($("#inpPartCode").val() == "") {
            yield GetPartCode();
        }
        if ($("#inpPartNo").val() == "") {
            yield GetPartNo();
        }
        yield GetListDataTables();
    });
});
$(document).on("change", "#inpPartCode", function () {
    return __awaiter(this, void 0, void 0, function* () {
        $("#readPartCode").val($("#inpPartCode").val());
        if ($("#divBtn").find("button:not(:disabled)").attr("id") == "btnNew") {
            return;
        }
        if ($("#inpLine").val() == "") {
            yield GetLine();
        }
        if ($("#inpPartNo").val() == "") {
            yield GetPartNo();
        }
        yield GetListDataTables();
    });
});
$(document).on("change", "#inpPartNo", function () {
    return __awaiter(this, void 0, void 0, function* () {
        $("#readPartNo").val($("#inpPartNo").val());
        if ($("#divBtn").find("button:not(:disabled)").attr("id") == "btnNew") {
            return;
        }
        if ($("#inpLine").val() == "") {
            yield GetLine();
        }
        if ($("#inpPartCode").val() == "") {
            yield GetPartCode();
        }
        yield GetListDataTables("inpPartNo");
    });
});
