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
        yield _xDataTable.InitialDataTable("#tableSub", {
            columns: [
                { title: "Order No.", data: "F_PDS_No" },
                { title: "Parent Part", data: "F_Part_No" },
                { title: "Store Cd.", data: "F_Store_CD" },
                { title: "Supplier", data: "F_SUpplier" },
                { title: "Deli Date", data: "F_Delivery_Date" },
                { title: "Trip", data: "F_ROund" },
                { title: "Qty", data: "F_Qty" },
                { title: "Child Part", data: "F_Child_Part" },
                { title: "ST. Child", data: "F_Store_Child" },
                { title: "Child Name", data: "f_Child_Name" },
                { title: "Order Type", data: "F_OrderType" },
            ],
            scrollX: false,
            order: [[1, "asc"]],
            scrollCollapse: true,
            scrollY: 500,
        });
        $("#inpDate").initDatepicker();
        yield GetPO();
        xSplash.hide();
        yield ShowCalendar();
    });
});
var _command = "";
$("#btnNew").click(() => __awaiter(void 0, void 0, void 0, function* () {
    yield GetStoreCD();
    //await GetPartNo();
    $("#selCustomerOrder").parent().remove();
    $("label[for='selCustomerOrder']").parent().append("<input type='text' class='form-control col-5' id='selCustomerOrder' name='selCustomerOrder' />");
    $("#selCustomerOrder").val("");
    $("#divInput").find("input").prop("disabled", false);
    $("#divInput").find("select").prop("disabled", false);
    $(".selectpicker").selectpicker("refresh");
    $("#divBtn").find("button").prop("disabled", true);
    $("#btnNew").prop("disabled", false);
    $("#btnCan").prop("disabled", false);
    $("#btnSave").prop("disabled", false);
    _command = "NEW";
}));
$("#btnDel").click(() => __awaiter(void 0, void 0, void 0, function* () {
    $("#divInput").find("input").prop("disabled", false);
    $("#divInput").find("select").prop("disabled", false);
    $("#divInput").find("select").each(function () {
        $(this).selectpicker("refresh");
    });
    $("#divBtn").find("button").prop("disabled", true);
    $("#btnDel").prop("disabled", false);
    $("#btnCan").prop("disabled", false);
    $("#btnSave").prop("disabled", false);
    yield ShowCalendar();
    yield GetPO();
    _command = "DEL";
}));
$("#btnUpd").click(() => __awaiter(void 0, void 0, void 0, function* () {
    $("#divInput").find("input").prop("disabled", false);
    $("#divInput").find("select").prop("disabled", false);
    $("#divInput").find("select").each(function () {
        $(this).selectpicker("refresh");
    });
    $("#divBtn").find("button").prop("disabled", true);
    $("#btnUpd").prop("disabled", false);
    $("#btnCan").prop("disabled", false);
    yield ShowCalendar();
    yield GetPO();
    _command = "UPD";
}));
$("#btnInq").click(() => __awaiter(void 0, void 0, void 0, function* () {
    $("#divInput").find("input").prop("disabled", false);
    $("#divInput").find("select").prop("disabled", false);
    $("#divInput").find("select").each(function () {
        $(this).selectpicker("refresh");
    });
    $("#divBtn").find("button").prop("disabled", true);
    $("#btnInq").prop("disabled", false);
    $("#btnCan").prop("disabled", false);
    $("#btnRptPar").prop("disabled", false);
    $("#btnRpt").prop("disabled", false);
    yield ShowCalendar();
    yield GetPO();
    _command = "INQ";
}));
$("#mthDeliYM").change(() => __awaiter(void 0, void 0, void 0, function* () {
    yield ShowCalendar();
    yield GetPO();
}));
//$("#selCustomerOrder").change(async () => {
//    await GetStoreCD();
//});
$("#fileIMP").change((e) => __awaiter(void 0, void 0, void 0, function* () {
    //console.log(e);
    let inputFile = e.target.files;
    if (inputFile.length == 0)
        return;
    //console.log(inputFile);
    let file = inputFile[0];
    if (yield xSwal.confirm("Are you sure?", "This action can't be undone")) {
        yield Import(file);
    }
}));
$("#fileSCP").change((e) => __awaiter(void 0, void 0, void 0, function* () {
    let inputFile = e.target.files;
    if (inputFile.length == 0)
        return;
    let file = inputFile[0];
    let result = yield xSwal.text("Importing File", "Please input advanced date.");
    if (result.isConfirmed) {
        yield ImportSCP(file, result.value);
    }
}));
$("#fileIPMS").change((e) => __awaiter(void 0, void 0, void 0, function* () {
    let inputFile = e.target.files;
    if (inputFile.length == 0)
        return;
    let file = inputFile[0];
    let result = yield xSwal.text("Importing File", "Please input advanced date.");
    if (result.isConfirmed) {
        yield ImportIPMS(file, result.value);
    }
}));
$("#btnRptPar").click(() => __awaiter(void 0, void 0, void 0, function* () {
    let obj = {
        Plant: _xLib.GetCookie("plantCode"),
        UserName: _xLib.GetUserName(),
        F_PDS_No: $("#selCustomerOrder").val().trim(),
        F_Part_Order: $("#selPartNo").val() ? $("#selPartNo").val().split("-")[0] : "",
        F_Ruibetsu_Order: $("#selPartNo").val() ? $("#selPartNo").val().split("-")[1] : "",
        F_Store_CD: $("#selStoreCode").val(),
        F_Delivery_Date: $("#mthDeliYM").val().replaceAll("-", ""),
        Type: "Special"
    };
    _xLib.OpenReportObj("/KBNIM007_Parent", obj);
}));
$("#btnRpt").click(() => __awaiter(void 0, void 0, void 0, function* () {
    let obj = {
        Plant: _xLib.GetCookie("plantCode"),
        UserName: _xLib.GetUserName(),
        F_PDS_No: $("#selCustomerOrder").val().trim(),
        F_Part_Order: $("#selPartNo").val() ? $("#selPartNo").val().split("-")[0] : "",
        F_Ruibetsu_Order: $("#selPartNo").val() ? $("#selPartNo").val().split("-")[1] : "",
        Type: "Special",
        TypeSPC: "Special"
    };
    _xLib.OpenReportObj("/KBNIM007", obj);
}));
$(document).on("change", "#selCustomerOrder", () => __awaiter(void 0, void 0, void 0, function* () {
    yield GetStoreCD();
    yield ListDataTable();
}));
$("#selStoreCode").change(() => __awaiter(void 0, void 0, void 0, function* () {
    yield GetPartNo();
}));
$("#selPartNo").change(() => __awaiter(void 0, void 0, void 0, function* () {
    yield PartNoSelected();
    yield ListCalendar();
    yield SetCalendar();
}));
$("#btnCan").click(() => __awaiter(void 0, void 0, void 0, function* () {
    if ($("#selCustomerOrder").prop("tagName") == "INPUT") {
        $("#selCustomerOrder").remove();
        yield $("label[for='selCustomerOrder']").parent().append("<select class='selectpicker col-5 p-0' id='selCustomerOrder' name='selCustomerOrder' data-live-search='true' data-size='7' disabled></select>");
        yield $("#selCustomerOrder").append("<option value='' hidden></option>");
        yield $("#selCustomerOrder").selectpicker("destroy");
        yield $("#selCustomerOrder").selectpicker("refresh");
        yield $("#selCustomerOrder").selectpicker("render");
    }
    $("#divInput").find("select").each(function () {
        $(this).empty();
        $(this).prop("disabled", true);
        $(this).resetSelectPicker();
    });
    $("#spanPartName").text("");
    $("#divBtn").find("button").prop("disabled", false);
    $("#btnCan").prop("disabled", true);
    $("#btnSave").prop("disabled", true);
}));
$("#btnSave").click(() => __awaiter(void 0, void 0, void 0, function* () {
    let isConfirm = yield xSwal.confirm("Are you sure?", "This action can't be undone");
    if (!isConfirm)
        return;
    yield Save();
}));
ShowCalendar = () => __awaiter(void 0, void 0, void 0, function* () {
    $("#tableMain").find("tbody").remove();
    let YM = $("#mthDeliYM").val();
    let Table = $("#tableMain");
    Table.append("<tbody></tbody>");
    let day = 1;
    const startDate = new Date(YM + "-01");
    const endDate = new Date(startDate.getFullYear(), startDate.getMonth() + 1, 0);
    for (let mockDay = 1; mockDay <= 42; mockDay++) {
        //let readonly = "readonly";
        if (mockDay % 7 == 1) {
            Table.find("tbody").append("<tr>");
        }
        if (mockDay > startDate.getDay() && mockDay <= endDate.getDate() + startDate.getDay()) {
            Table.find("tbody").find("tr:last").append(`<td class='text-center'>
                            <span class='fs-6'>${day}</span></br>
                            <div class='row justify-content-center mt-1'>
                                <input type='number' class='form-control fw-bolder' min='0' max='9999' style='width: 85%;'
                                    id='QTY_${YM.replaceAll("-", "")}${day.toString().length == 1 ? "0" + day : day}'
                                    name='QTY_${YM.replaceAll("-", "")}${day.toString().length == 1 ? "0" + day : day}' />
                            </div>    
                        </td >`);
            day++;
        }
        else {
            Table.find("tbody").find("tr:last").append("<td></td>");
        }
        if (mockDay % 7 == 0) {
            Table.find("tbody").append("</tr>");
        }
    }
    $("#tableMain").find("tbody tr:last").find("td").each(function () {
        if ($(this).text() == "") {
            $(this).remove();
        }
    });
});
GetPO = () => __awaiter(void 0, void 0, void 0, function* () {
    let obj = yield getQueryObj();
    _xLib.AJAX_Get("/api/KBNIM007/GetPO", obj, (success) => __awaiter(void 0, void 0, void 0, function* () {
        //console.log(success);
        $("#selCustomerOrder").empty();
        $("#selCustomerOrder").append("<option value='' hidden></option>");
        success.data.forEach((item) => {
            $("#selCustomerOrder").append(`<option value='${item}'>${item}</option>`);
        });
        $("#selCustomerOrder").selectpicker("refresh");
    }));
});
GetStoreCD = () => __awaiter(void 0, void 0, void 0, function* () {
    let obj = yield getQueryObj();
    _xLib.AJAX_Get("/api/KBNIM007/GetStoreCD", obj, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        //console.log(success);
        $("#selStoreCode").addListSelectPicker(success.data, "F_Store_CD");
    }));
});
ListCalendar = () => __awaiter(void 0, void 0, void 0, function* () {
    let obj = yield getQueryObj();
    _xLib.AJAX_Get("/api/KBNIM007/ListCalendar", obj, (success) => __awaiter(void 0, void 0, void 0, function* () {
        //success = _xLib.JSONparseMixData(success);
        //console.log(success);
        $("#tableMain").find("input").val("");
        $(success.data).each(function () {
            let date = moment(this.f_Delivery_Date, "DD/MM/YYYY").format("YYYYMMDD");
            $(`#QTY_${date}`).val(this.f_Qty);
        });
    }));
});
SetCalendar = () => __awaiter(void 0, void 0, void 0, function* () {
    let obj = yield getQueryObj();
    _xLib.AJAX_Get("/api/KBNIM007/SetCalendar", obj, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        //console.log(success);
        $("#tableMain").find("input").prop("readonly", false);
        $("#tableMain").find("input").removeClass("bg-warning");
        $("#tableMain").find("input").removeClass("bg-danger");
        Object.keys(success.data[0]).forEach((key) => {
            //console.log(key);
            if (key.includes("F_workCd_D")) {
                let date = key.split("F_workCd_D")[1];
                //console.log(date);
                if (date.length == 1)
                    date = "0" + date;
                if (success.data[0][key] == "1") {
                    //console.log(obj.F_Date);
                    if (success.data[0].F_Date <= obj.YM + date) {
                        $(`#QTY_${obj.YM}${date}`).prop("readonly", false);
                    }
                    else {
                        $(`#QTY_${obj.YM}${date}`).prop("readonly", true);
                        $(`#QTY_${obj.YM}${date}`).addClass("bg-warning");
                    }
                }
                else {
                    $(`#QTY_${obj.YM}${date}`).prop("readonly", true);
                    $(`#QTY_${obj.YM}${date}`).addClass("bg-danger");
                }
            }
        });
    }));
});
GetPartNo = () => __awaiter(void 0, void 0, void 0, function* () {
    let obj = yield getQueryObj();
    _xLib.AJAX_Get("/api/KBNIM007/GetPartNo", obj, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        //console.log(success);
        $("#selPartNo").addListSelectPicker(success.data, "F_Part_No");
    }));
});
PartNoSelected = () => __awaiter(void 0, void 0, void 0, function* () {
    let obj = yield getQueryObj();
    $("#spanPartName").text("");
    _xLib.AJAX_Get("/api/KBNIM007/PartNoSelected", obj, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        //console.log(success);
        if (success.data.length > 0) {
            $("#spanPartName").text(success.data[0].F_Part_NM);
        }
    }));
});
getQueryObj = () => __awaiter(void 0, void 0, void 0, function* () {
    let obj = {
        YM: $("#mthDeliYM").val().replaceAll("-", ""),
        PO: $("#selCustomerOrder").val(),
        StoreCD: $("#selStoreCode").val(),
        PartNo: $("#selPartNo").val(),
        isNew: !($("#btnNew").prop("disabled"))
    };
    return obj;
});
getSaveObj = () => __awaiter(void 0, void 0, void 0, function* () {
    let list = [];
    $("#tableMain").find("input").each(function () {
        let obj = {
            PDS: $("#selCustomerOrder").val().trim(),
            IssuedDate: moment($("#inpDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
            PartNo: $("#selPartNo").val(),
            StoreCD: $("#selStoreCode").val(),
            DeliveryDate: $("#mthDeliYM").val().replaceAll("-", ""),
            Qty: 0,
            Trip: 1,
        };
        if ($(this).val() != "") {
            obj.Qty = $(this).val();
            let date = $(this).attr("id").split("_")[1];
            obj.DeliveryDate = date;
            list.push(obj);
        }
        //for Delete Command and Update Command
        if (_command == "DEL") {
            list.push(obj);
        }
    });
    return list;
});
Save = () => __awaiter(void 0, void 0, void 0, function* () {
    let listObj = yield getSaveObj();
    _xLib.AJAX_Post("/api/KBNIM007/Save?action=" + _command, listObj, (success) => __awaiter(void 0, void 0, void 0, function* () {
        //console.log(success);
        xSwal.success(success.response, success.message);
        $("#selCustomerOrder").trigger("change");
        $("#selPartNo").trigger("change");
    }));
});
Import = (inputFile) => __awaiter(void 0, void 0, void 0, function* () {
    let obj = new FormData();
    let file = new Blob([inputFile], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
    obj.append("file", file, inputFile.name);
    _xLib.AJAX_PostFile("/api/KBNIM007/Import", obj, (success) => __awaiter(void 0, void 0, void 0, function* () {
        //console.log(success);
        xSwal.success(success.response, success.message);
    }));
});
ImportIPMS = (inputFile, advanceDate) => __awaiter(void 0, void 0, void 0, function* () {
    let formData = new FormData();
    //console.log(inputFile);
    let file = new Blob([inputFile], { type: "text/csv" });
    formData.append("file", file, inputFile.name);
    _xLib.AJAX_PostFile("/api/KBNIM007/ImportIPMS?BackDate=" + advanceDate, formData, (success) => __awaiter(void 0, void 0, void 0, function* () {
        //console.log(success);
        xSwal.success(success.response, success.message);
    }));
});
ImportSCP = (inputFile, advanceDate) => __awaiter(void 0, void 0, void 0, function* () {
    let formData = new FormData();
    console.log(inputFile);
    let file = new Blob([inputFile], { type: "text/txt" });
    formData.append("file", file, inputFile.name);
    _xLib.AJAX_PostFile("/api/KBNIM007/ImportSCP?BackDate=" + advanceDate, formData, (success) => __awaiter(void 0, void 0, void 0, function* () {
        //console.log(success);
        xSwal.success(success.response, success.message);
    }));
});
ListDataTable = () => __awaiter(void 0, void 0, void 0, function* () {
    let obj = yield getQueryObj();
    _xLib.AJAX_Get("/api/KBNIM007/ListDataTable", obj, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        //console.log(success);
        _xDataTable.ClearAndAddDataDT("#tableSub", success.data);
        //$("#tableSub").DataTable().clear().draw();
        //$("#tableSub").DataTable().rows.add(success.data).draw();
    }));
});
