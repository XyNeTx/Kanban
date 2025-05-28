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
                    title: "Flag", render: function (data, type, row) {
                        return '<input type="checkbox" class="chkboxTable" id="chkFlag" name="chkFlag">';
                    }
                },
                { title: "Supplier Code", data: "f_Supplier_Code" },
                { title: "StoreCD", data: "f_Store_cd" },
                { title: "Delivery Date", data: "f_Delivery_Date" },
                { title: "Trip", data: "f_Delivery_Trip" },
                { title: "Keep Order", data: "f_Keep_Order" },
                { title: "SlideDateTo", data: "f_Slide_Date" },
                { title: "TripNext", data: "f_Slide_Trip" },
            ],
            scrollY: "250px",
            scrollCollapse: false,
            ordering: false,
            order: [[1, "asc"]],
        });
        $(document).find("input[class='datepicker form-control']").each(function () {
            $(this).initDatepicker();
        });
        yield GetSupplier();
        yield GetStoreCD();
        xSplash.hide();
    });
});
function GetSupplier() {
    return __awaiter(this, void 0, void 0, function* () {
        var getQuery = yield GetQuery();
        _xLib.AJAX_Get("/api/KBNOC120/GetSupplier", getQuery, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                yield $("#inpSupplier").addListSelectPicker(success.data, "f_Supplier_Code");
                $("#inpSupplier").resetSelectPicker();
            });
        }, function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        });
    });
}
function GetStoreCD() {
    return __awaiter(this, void 0, void 0, function* () {
        var getQuery = yield GetQuery();
        _xLib.AJAX_Get("/api/KBNOC120/GetStore", getQuery, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                yield $("#inpStoreCd").addListSelectPicker(success.data, "f_Store_cd");
                $("#inpStoreCd").resetSelectPicker();
            });
        }, function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        });
    });
}
function GetListData() {
    return __awaiter(this, void 0, void 0, function* () {
        var getQuery = yield GetQuery();
        _xLib.AJAX_Get("/api/KBNOC120/GetListData", getQuery, function (success) {
            return _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        }, function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        });
    });
}
$("#inpSupplier").on("change", function () {
    return __awaiter(this, void 0, void 0, function* () {
        if ($("#inpStoreCd").val() == "") {
            yield GetStoreCD();
        }
        yield GetListData();
    });
});
$("#inpStoreCd").on("change", function () {
    return __awaiter(this, void 0, void 0, function* () {
        if ($("#inpSupplier").val() == "") {
            yield GetSupplier();
        }
        //await GetSupplier();
        yield GetListData();
    });
});
$("#btnSelectAll").click(function () {
    $(".chkboxTable").prop("checked", true);
});
$('#btnDeselectAll').click(function () {
    $(".chkboxTable").prop("checked", false);
});
$("#divBtn").on("click", "button[type='button']", function () {
    return __awaiter(this, void 0, void 0, function* () {
        $("#divBtn").find("button").prop("disabled", true);
        $(this).prop("disabled", false);
        $("#inpSupplier").prop("disabled", false);
        $("#inpStoreCd").prop("disabled", false);
        $("#inpSupplier").resetSelectPicker();
        $("#inpStoreCd").resetSelectPicker();
        if ($(this).attr("id") != "btnNew") {
            yield GetListData();
        }
        if ($(this).attr("id") == "btnInq" || $(this).attr("id") == "btnDel") {
            return;
        }
        $("#inpDeliveryDate").prop("disabled", false);
        $("#inpTrip").prop("disabled", false);
        $("#chkSlideOrder").prop("disabled", false);
        $("#inpSlideDate").prop("disabled", true);
        $("#inpTripNext").prop("disabled", true);
    });
});
$(document).on("dblclick", "#tableMain tbody tr", function () {
    $(this).closest("tr").find("input[type='checkbox']").prop("checked", true);
    var data = _xDataTable.GetDataDT("#tableMain", $(this).closest("tr"));
    //console.log(data);
    $("#inpSupplier").val(data.f_Supplier_Code);
    $("#inpStoreCd").val(data.f_Store_cd);
    $("#inpDeliveryDate").val(data.f_Delivery_Date);
    $("#inpTrip").val(data.f_Delivery_Trip);
    $("#chkSlideOrder").prop("checked", data.f_Keep_Order == "Cancel" ? false : true);
    $("#inpSlideDate").val(data.f_Slide_Date);
    $("#inpTripNext").val(data.f_Slide_Trip);
    $("#inpSupplier").selectpicker("refresh");
    $("#inpStoreCd").selectpicker("refresh");
    if ($("#divBtn #btnUpd").prop("disabled") === false) {
        $("#inpSupplier").prop("disabled", true);
        $("#inpStoreCd").prop("disabled", true);
        $("#inpDeliveryDate").prop("disabled", true);
        $("#inpTrip").prop("disabled", true);
        $("#inpSupplier").selectpicker("refresh");
        $("#inpStoreCd").selectpicker("refresh");
    }
});
$("#btnCan").on("click", function () {
    return __awaiter(this, void 0, void 0, function* () {
        $("#divBtn").find("button").prop("disabled", false);
        $("#tableMain").DataTable().clear().draw();
        //await $("#inpSupplier").resetSelectPicker();
        //await $("#inpStoreCd").resetSelectPicker();
        $("#inpDeliveryDate").val(moment().format("DD/MM/YYYY"));
        $("#inpTrip").val("1");
        $("#chkSlideOrder").prop("checked", false);
        $("#inpSlideDate").val(moment().format("DD/MM/YYYY"));
        $("#inpTripNext").val("1");
        $("#inpSupplier").prop("disabled", true);
        $("#inpStoreCd").prop("disabled", true);
        $("#inpDeliveryDate").prop("disabled", true);
        $("#inpTrip").prop("disabled", true);
        $("#chkSlideOrder").prop("disabled", true);
        $("#inpSlideDate").prop("disabled", true);
        $("#inpTripNext").prop("disabled", true);
        yield $("#inpSupplier").resetSelectPicker();
        yield $("#inpStoreCd").resetSelectPicker();
    });
});
$("#chkSlideOrder").change(function () {
    if ($(this).prop("checked")) {
        $("#inpSlideDate").prop("disabled", false);
        $("#inpTripNext").prop("disabled", false);
    }
    else {
        $("#inpSlideDate").prop("disabled", true);
        $("#inpTripNext").prop("disabled", true);
    }
});
$("#btnSave").on("click", function () {
    return __awaiter(this, void 0, void 0, function* () {
        var postObj = yield PostObj();
        let action = $("#divBtn").find("button[type='button']:not([disabled])").attr("id").split("btn")[1];
        _xLib.AJAX_Post("/api/KBNOC120/Save?action=" + action, postObj, function (success) {
            xSwal.success("Success", "Data has been saved.");
            GetListData();
        }, function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        });
    });
});
function GetQuery() {
    return __awaiter(this, void 0, void 0, function* () {
        var getQuery = {
            SupplierCD: $("#inpSupplier").val(),
            StoreCD: $("#inpStoreCd").val(),
        };
        return getQuery;
    });
}
function PostObj() {
    return __awaiter(this, void 0, void 0, function* () {
        var listObj = [];
        let action = $("#divBtn").find("button[type='button']:not([disabled])").attr("id").split("btn")[1];
        //console.log($("#chkSlideOrder").prop("checked"));
        var postObj = {
            Supplier: $("#inpSupplier").val(),
            StoreCD: $("#inpStoreCd").val(),
            DeliveryDate: moment($("#inpDeliveryDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
            Trip: $("#inpTrip").val(),
            IsSlideOrder: $("#chkSlideOrder").prop("checked"),
            SlideDateTo: moment($("#inpSlideDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
            TripNext: $("#inpTripNext").val()
        };
        if (action == "Del") {
            listObj = _xDataTable.GetSelectedDataDT("#tableMain");
            if (listObj.length == 0) {
                xSwal.error("Error", "Please select data.");
                return false;
            }
            listObj = listObj.map(function (data) {
                console.log(data.f_Keep_Order);
                return {
                    Supplier: data.f_Supplier_Code,
                    StoreCD: data.f_Store_cd,
                    DeliveryDate: moment(data.f_Delivery_Date, "DD/MM/YYYY").format("YYYYMMDD"),
                    Trip: data.f_Delivery_Trip.toString(),
                    IsSlideOrder: data.f_Keep_Order == "Cancel" ? false : true,
                    SlideDateTo: moment(data.f_Slide_Date, "DD/MM/YYYY").format("YYYYMMDD"),
                    TripNext: data.f_Slide_Trip.toString()
                };
            });
            return listObj;
        }
        listObj.push(postObj);
        return listObj;
    });
}
let inpFile = "";
$("#inpFile").change(function () {
    inpFile = this.files[0];
});
$("#btnRpt").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        var obj = {
            UserName: _xLib.GetUserName(),
        };
        return _xLib.OpenReportObj("/KBNOC120", obj);
    });
});
$("#btnImport").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        //console.log('Files:', inpFile);
        const arrayBuffer = yield inpFile.arrayBuffer();
        const read = yield XLSX.read(arrayBuffer);
        let newRead = read;
        for (var key in newRead.Sheets[newRead.SheetNames[0]]) {
            newRead.Sheets[newRead.SheetNames[0]][key].v = newRead.Sheets[newRead.SheetNames[0]][key].w;
        }
        const data = XLSX.utils.sheet_to_json(newRead.Sheets[newRead.SheetNames[0]]);
        for (let i = 0; i < data.length; i++) {
            try {
                let mockObj = {
                    Supplier: data[i].SupplierCode,
                    StoreCD: data[i]["Store Code"],
                    DeliveryDate: moment(data[i].DeliveryDate, "DD/MM/YYYY").format("YYYYMMDD"),
                    Trip: data[i].DeliveryTrip,
                    IsSlideOrder: data[i].KeepOrder == "Y" ? true : false,
                    SlideDateTo: moment(data[i]["Slide to Date"], "DD/MM/YYYY").format("YYYYMMDD"),
                    TripNext: data[i]["Slide to Trip"]
                };
                let postObj = [];
                postObj.push(mockObj);
                //console.log(postObj);
                //continue;
                _xLib.AJAX_Post("/api/KBNOC120/Save?action=New", postObj, function (success) {
                    console.log(postObj[i]);
                    console.log("Success");
                }, function (error) {
                    console.log("Error");
                });
            }
            catch (e) {
                xSwal.error("Error", `Row ${i + 1} : ${e.message}`);
            }
        }
        yield xSwal.success("Success", "Data has been imported.");
        GetListData();
    });
});
