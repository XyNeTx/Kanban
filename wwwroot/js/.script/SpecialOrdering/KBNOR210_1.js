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
        yield Initial();
        let loginDate = _xLib.GetCookie("loginDate");
        let plant = _xLib.GetCookie("plantCode");
        //console.log(loginDate);
        //console.log(plant);
        $("#readProcessDate").val(moment(loginDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));
        $("#readProcessShift").val(loginDate.slice(10, 11) == "D" ? "1:Day Shift" : "2:Night Shift");
        $("#readPlant").val(plant);
        $("#inpDateFrom").datepicker({
            uiLibrary: 'materialdesign',
            format: 'dd/mm/yyyy',
            autoclose: true,
            showRightIcon: false,
        });
        $("#inpDateTo").datepicker({
            uiLibrary: 'materialdesign',
            format: 'dd/mm/yyyy',
            autoclose: true,
            showRightIcon: false,
        });
        $("#inpDel_YM").datepicker({
            format: 'mm/yyyy',
            autoclose: true,
            uiLibrary: 'materialdesign',
            value: moment().format("MM/YYYY"),
            showRightIcon: false,
            //select: function (e) {
            //    console.log(e);
            //}
        });
        $("#inpNewDeliveryDate").datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true,
            uiLibrary: 'materialdesign',
            showRightIcon: false,
        });
        $("#inpDateFrom").parent().addClass("col-5 ps-0 pe-0 me-0");
        yield LoadDataChangeDelivery();
        $(".selectpicker").selectpicker("refresh");
        xSplash.hide();
    });
});
$("#btnExit").click(function () {
    if (window.location.hostname.includes("tpcap")) {
        return window.location.replace("/kanban/OrderingProcess/KBNOR200");
    }
    return window.location.replace("/OrderingProcess/KBNOR200");
});
function Initial() {
    return __awaiter(this, void 0, void 0, function* () {
        yield $("#tableMain").DataTable({
            width: '100%',
            paging: false,
            scrollCollapse: true,
            "processing": false,
            "serverSide": false,
            scrollX: true,
            scrollY: '400px',
            searching: false,
            info: false,
            ordering: false,
            columns: [
                { title: "Part No.", data: "F_part_no" },
                { title: "Supplier Code", data: "F_Supplier_CD" },
                {
                    title: " ", render: function (data, type, row) {
                        if (row.F_Delivery_DT_1.includes("/")) {
                            return `<input type="checkbox" id="chk${row.F_Color_1}" value="${row.F_Color_1}" />`;
                        }
                        return "";
                    }
                },
                {
                    title: "Qty", render: function (data, type, row) {
                        if (row.F_Delivery_DT_1.includes("/")) {
                            return row.F_Qty_1;
                        }
                        else {
                            return "";
                        }
                    },
                },
                { title: "Delivery Date", data: "F_Delivery_DT_1" },
                {
                    title: " ", render: function (data, type, row) {
                        try {
                            if (row.F_Delivery_DT_2.includes("/")) {
                                if (row.F_Qty_2 == 0) {
                                    return `<input type="checkbox" id="chk${row.F_Color_2}" value="${row.F_Color_2}" disabled/>`;
                                }
                                else {
                                    return `<input type="checkbox" id="chk${row.F_Color_2}" value="${row.F_Color_2}" />`;
                                }
                            }
                            return "";
                        }
                        catch (error) {
                            return "";
                        }
                    }
                },
                {
                    title: "Qty", render: function (data, type, row) {
                        try {
                            if (row.F_Delivery_DT_2.includes("/")) {
                                return row.F_Qty_2;
                            }
                            else {
                                return "";
                            }
                        }
                        catch (error) {
                            return "";
                        }
                    },
                },
                { title: "Delivery Date", data: "F_Delivery_DT_2" },
                {
                    title: " ", render: function (data, type, row) {
                        try {
                            if (row.F_Delivery_DT_3.includes("/")) {
                                if (row.F_Qty_3 == 0) {
                                    return `<input type="checkbox" id="chk${row.F_Color_3}" value="${row.F_Color_3}" disabled/>`;
                                }
                                else {
                                    return `<input type="checkbox" id="chk${row.F_Color_3}" value="${row.F_Color_3}" />`;
                                }
                            }
                            return "";
                        }
                        catch (error) {
                            return "";
                        }
                    }
                },
                {
                    title: "Qty", render: function (data, type, row) {
                        try {
                            if (row.F_Delivery_DT_3.includes("/")) {
                                return row.F_Qty_3;
                            }
                            else {
                                return "";
                            }
                        }
                        catch (error) {
                            return "";
                        }
                    },
                },
                { title: "Delivery Date", data: "F_Delivery_DT_3" },
                {
                    title: " ", render: function (data, type, row) {
                        try {
                            if (row.F_Delivery_DT_4.includes("/")) {
                                if (row.F_Qty_4 == 0) {
                                    return `<input type="checkbox" id="chk${row.F_Color_4}" value="${row.F_Color_4}" disabled/>`;
                                }
                                else {
                                    return `<input type="checkbox" id="chk${row.F_Color_4}" value="${row.F_Color_4}" />`;
                                }
                            }
                            return "";
                        }
                        catch (error) {
                            return "";
                        }
                    }
                },
                {
                    title: "Qty", render: function (data, type, row) {
                        try {
                            if (row.F_Delivery_DT_4.includes("/")) {
                                return row.F_Qty_4;
                            }
                            else {
                                return "";
                            }
                        }
                        catch (error) {
                            return "";
                        }
                    },
                },
                { title: "Delivery Date", data: "F_Delivery_DT_4" },
                {
                    title: " ", render: function (data, type, row) {
                        try {
                            if (row.F_Delivery_DT_5.includes("/")) {
                                if (row.F_Qty_5 == 0) {
                                    return `<input type="checkbox" id="chk${row.F_Color_5}" value="${row.F_Color_5}" disabled/>`;
                                }
                                else {
                                    return `<input type="checkbox" id="chk${row.F_Color_5}" value="${row.F_Color_5}" />`;
                                }
                            }
                            return "";
                        }
                        catch (error) {
                            return "";
                        }
                    }
                },
                {
                    title: "Qty", render: function (data, type, row) {
                        try {
                            if (row.F_Delivery_DT_5.includes("/")) {
                                return row.F_Qty_5;
                            }
                            else {
                                return "";
                            }
                        }
                        catch (error) {
                            return "";
                        }
                    },
                },
                { title: "Delivery Date", data: "F_Delivery_DT_5" },
                {
                    title: " ", render: function (data, type, row) {
                        try {
                            if (row.F_Delivery_DT_6.includes("/")) {
                                if (row.F_Qty_6 == 0) {
                                    return `<input type="checkbox" id="chk${row.F_Color_6}" value="${row.F_Color_6}" disabled/>`;
                                }
                                else {
                                    return `<input type="checkbox" id="chk${row.F_Color_6}" value="${row.F_Color_6}" />`;
                                }
                            }
                            return "";
                        }
                        catch (error) {
                            return "";
                        }
                    }
                },
                {
                    title: "Qty", render: function (data, type, row) {
                        try {
                            if (row.F_Delivery_DT_6.includes("/")) {
                                return row.F_Qty_6;
                            }
                            else {
                                return "";
                            }
                        }
                        catch (error) {
                            return "";
                        }
                    },
                },
                { title: "Delivery Date", data: "F_Delivery_DT_6" },
            ],
            order: [[1, 'asc']]
        });
        $("table thead tr th").css("text-align", "center");
        $("table tbody tr td").css("text-align", "center");
    });
}
function LoadDataChangeDelivery() {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = {
            PDSNo: $("#inpCustomerOrderNo").val(),
            SuppCd: $("#inpSupplier").val(),
            PartNo: $("#inpPartNo").val(),
            chkDeli: $("#chkDate").is(":checked"),
            DeliFrom: moment($("#inpDateFrom").val(), "DD/MM/YYYY").format("YYYYMMDD"),
            DeliTo: moment($("#inpDateTo").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        };
        _xLib.AJAX_Get("/api/KBNOR210_1/LoadDataChangeDelivery", obj, function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            //console.log(success);
            if (!$("#inpCustomerOrderNo").val()) {
                $("#inpCustomerOrderNo").empty();
                $("#inpCustomerOrderNo").append("<option value='' hidden></option>");
                success.data.customer.forEach(function (item) {
                    $("#inpCustomerOrderNo").append(`<option value="${item.trim()}">${item.trim()}</option>`);
                });
                $("#inpCustomerOrderNo").selectpicker("refresh");
            }
            if (!$("#inpSupplier").val()) {
                $("#inpSupplier").empty();
                $("#inpSupplier").append("<option value='' hidden></option>");
                success.data.supplier.forEach(function (item) {
                    $("#inpSupplier").append(`<option value="${item.trim()}">${item.trim()}</option>`);
                });
                $("#inpSupplier").selectpicker("refresh");
            }
            if (!$("#inpPartNo").val()) {
                $("#inpPartNo").empty();
                $("#inpPartNo").append("<option value='' hidden></option>");
                success.data.partNo.forEach(function (item) {
                    $("#inpPartNo").append(`<option value="${item.trim()}">${item.trim()}</option>`);
                });
                $("#inpPartNo").selectpicker("refresh");
            }
        }, function (error) {
            console.error(error);
        });
    });
}
$("#inpCustomerOrderNo").change(function () {
    return __awaiter(this, void 0, void 0, function* () {
        $("#inpSupplier").val("");
        $("#inpPartNo").val("");
        yield LoadDataChangeDelivery();
    });
});
$("#inpSupplier").change(function () {
    return __awaiter(this, void 0, void 0, function* () {
        $("#inpPartNo").val("");
        yield GetSupplierName();
        yield LoadDataChangeDelivery();
    });
});
$("#inpPartNo").change(function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield GetPartName();
        yield LoadDataChangeDelivery();
    });
});
function GetSupplierName() {
    return __awaiter(this, void 0, void 0, function* () {
        _xLib.AJAX_Get("/api/KBNOR210_1/GetSupplierName", { SuppCd: $("#inpSupplier").val() }, function (success) {
            $("#readSupplier").val(success.data);
        }, function (error) {
            console.error(error);
        });
    });
}
function GetPartName() {
    return __awaiter(this, void 0, void 0, function* () {
        _xLib.AJAX_Get("/api/KBNOR210_1/GetPartName", { PartNo: $("#inpPartNo").val() }, function (success) {
            $("#readPartNo").val(success.data);
        }, function (error) {
            console.error(error);
        });
    });
}
$("#chkDate").change(function () {
    return __awaiter(this, void 0, void 0, function* () {
        if ($(this).prop("checked")) {
            $("#inpDateFrom").prop("disabled", false);
            $("#inpDateTo").prop("disabled", false);
        }
        else {
            $("#inpDateFrom").prop("disabled", true);
            $("#inpDateTo").prop("disabled", true);
        }
    });
});
$("#btnSearch").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = {
            PDSNo: $("#inpCustomerOrderNo").val(),
            SuppCd: $("#inpSupplier").val(),
            PartNo: $("#inpPartNo").val(),
            chkDeli: $("#chkDate").is(":checked"),
            DeliFrom: moment($("#inpDateFrom").val(), "DD/MM/YYYY").format("YYYYMMDD"),
            DeliTo: moment($("#inpDateTo").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        };
        _xLib.AJAX_Get("/api/KBNOR210_1/GetPOMergeData", obj, function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#tableMain").DataTable().clear().rows.add(success.data).draw();
            $("#tableMain tbody tr td").css("text-align", "center");
            $("#tableMain thead tr th").css("text-align", "center");
        }, function (error) {
            console.error(error);
        });
    });
});
$("#btnCheck").click(function () {
    $("#tableMain input[type='checkbox'").filter(function () {
        if ($(this).prop("disabled") == false) {
            $(this).prop("checked", true);
        }
    });
});
$("#btnUncheck").click(function () {
    $("#tableMain input[type='checkbox'").filter(function () {
        if ($(this).prop("disabled") == false) {
            $(this).prop("checked", false);
        }
    });
});
$("#btnMergeReport").click(function () {
    let obj = {
        SuppCD: $("#inpSupplier").val(),
        PartNo: $("#inpPartNo").val(),
        PDS_No: $("#inpCustomerOrderNo").val(),
        Plant: _xLib.GetCookie("plantCode"),
        UserName: _xLib.GetUserName(),
    };
    if (!$("#inpCustomerOrderNo").val()) {
        return xSwal.error("Error", "Please select Customer OrderNo.");
    }
    _xLib.OpenReportObj("/KBNOR210_1", obj);
});
$("#btnSave").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        let listObj = [];
        $("#tableMain tbody tr td").find("input[type='checkbox']:checked").each(function () {
            let dataObj = $("#tableMain").DataTable().row($(this).closest("tr")).data();
            var objIndex = $("#tableMain").DataTable().row($(this).closest("tr")).index();
            var oldPds = $("#tableMain").DataTable().row(objIndex - 1).data().F_Delivery_DT_1;
            //console.log(dataObj);
            //console.log(oldPds);
            let obj = {
                F_PDS_NO: dataObj.F_PO_Number,
                F_PDS_No_New: oldPds,
                F_Qty: dataObj.F_Qty_1,
                F_Part_No: dataObj.F_part_no,
                F_Delivery_Date: moment(dataObj.F_Delivery_DT_1, "DD/MM/YYYY").format("YYYYMMDD"),
                F_Delivery_Date_New: moment($("#inpNewDeliveryDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
                F_Supplier_CD: dataObj.F_Supplier_CD,
            };
            listObj.push(obj);
        });
        console.log(listObj);
        let objPO = {
            PDSNo: $("#inpCustomerOrderNo").val(),
            SuppCd: $("#inpSupplier").val(),
            PartNo: $("#inpPartNo").val(),
            chkDeli: $("#chkDate").is(":checked"),
            DeliFrom: moment($("#inpDateFrom").val(), "DD/MM/YYYY").format("YYYYMMDD"),
            DeliTo: moment($("#inpDateTo").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        };
        _xLib.AJAX_Post("/api/KBNOR210_1/Save", listObj, function (success) {
            //console.log(success);
            xSwal.success("Success", success.message);
            _xLib.AJAX_Get("/api/KBNOR210_1/GetPOMergeData", objPO, function (success) {
                success = _xLib.JSONparseMixData(success);
                console.log(success);
                $("#tableMain").DataTable().clear().rows.add(success.data).draw();
                $("#tableMain tbody tr td").css("text-align", "center");
                $("#tableMain thead tr th").css("text-align", "center");
            }, function (error) {
                console.error(error);
            });
        }, function (error) {
            console.error(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        });
    });
});
//Modal USE COMMON PARTS (KBNOR210_1_STC_3) : Use Stock Part
$(document).on("show.bs.modal", "#KBNOR210_1_STC_3", function (e) {
    return __awaiter(this, void 0, void 0, function* () {
        if (!$("#inpCustomerOrderNo").val()) {
            $("#KBNOR210_1_STC_3").modal("hide");
            return xSwal.error("Error", "Please select Customer Order No.");
        }
        $("#btnUseCommonParts").attr("data-bs-orderno", $("#inpCustomerOrderNo").val());
        //$("body").css("visibility", "hidden")
        xSplash.show();
        if ($("#tableKBNOR210_1_STC_3").find("thead").length != 0) {
            $("#tableKBNOR210_1_STC_3").DataTable().clear().destroy();
        }
        yield $("#tableKBNOR210_1_STC_3").DataTable({
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
            autoWidth: true, // Enable autoWidth
            columns: [
                { title: "Supplier Code", data: "F_Supplier" },
                { title: "Part No.", data: "F_Part_No" },
                { title: "Store Code", data: "F_Store_Cd" },
                { title: "Actual Stock\n(PCS.)", data: "F_Actual_Qty" },
                {
                    title: "", render: function () {
                        return `<input type="checkbox" />`;
                    }
                },
                { title: "Order No.", data: "F_OrderNo" },
                { title: "Customer \nOrder Qty", data: "F_Qty" },
                { title: "Use Stock\nQty", data: "F_Use_StockQty" },
                {
                    title: "RemainQty", render: function (x, y, data) {
                        //console.log(data);
                        if (data.F_Remain == 0) {
                            let remain = parseInt(data.F_Qty) - parseInt(data.F_Use_StockQty);
                            //console.log(remain);
                            return `<td style="color:red;">${remain}</td>`;
                        }
                        else {
                            if (data.remain == undefined)
                                return;
                            return data.F_Remain;
                        }
                    }
                },
            ],
            order: [[1, 'asc']]
        });
        $("#tableKBNOR210_1_STC_3 thead tr th").css("text-align", "center");
        // Make sure the modal is fully shown before adjusting columns
        yield $('#KBNOR210_1_STC_3').on('shown.bs.modal', function (e) {
            $("#spanCustomerOrderNo").text(OrderNo);
            $("#tableKBNOR210_1_STC_3").DataTable().columns.adjust().draw();
            xSplash.hide();
        });
        let OrderNo = $(e.relatedTarget).data("bs-orderno");
        _xLib.AJAX_Get("/api/KBNOR210_1/LoadGridData", { OrderNo: OrderNo }, function (success) {
            success.data = JSON.parse(success.data);
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#tableKBNOR210_1_STC_3").DataTable().clear().rows.add(success.data).draw();
            $("#tableKBNOR210_1_STC_3 tbody tr td").css("text-align", "center");
            $("#tableKBNOR210_1_STC_3 thead tr th").css("text-align", "center");
        }, function (error) {
            console.error(error);
        });
    });
});
let ObjActualStock = 0;
//Modal INSIDE Modal (KBNOR210_1_STC_3_1)
$(document).on("dblclick", "#tableKBNOR210_1_STC_3 tbody tr td", function () {
    return __awaiter(this, void 0, void 0, function* () {
        let index = $("#tableKBNOR210_1_STC_3").DataTable().column(this).index();
        let obj = $("#tableKBNOR210_1_STC_3").DataTable().row($(this).closest("tr")).data();
        if (index != 5) {
            //console.log("Show Modal");
            return;
        }
        $("#KBNOR210_1_STC_3_1").modal("show");
        xSplash.show();
        if ($("#tableKBNOR210_1_STC_3_1").find("thead").length != 0) {
            $("#tableKBNOR210_1_STC_3_1").DataTable().clear().destroy();
        }
        yield $("#tableKBNOR210_1_STC_3_1").DataTable({
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
            autoWidth: true, // Enable autoWidth
            columns: [
                { title: "PO Customer", data: "F_PO_Customer" },
                { title: "Delivery Date", data: "F_Delivery_Date" },
                { title: "Part No.", data: "F_Part_No" },
                { title: "Store Code", data: "F_Store_Cd" },
                { title: "Order Qty.", data: "F_Order_Qty" },
                { title: "Use Stock Qty", data: "F_Use_Qty" },
                { title: "Order Qty Remain", data: "F_Remain_Qty" },
            ],
            order: [[1, 'asc']]
        });
        $("#tableKBNOR210_1_STC_3_1 thead tr th").css("text-align", "center");
        console.log(obj);
        ObjActualStock = obj.F_Actual_Qty;
        // Make sure the modal is fully shown before adjusting columns
        yield $('#KBNOR210_1_STC_3_1').on('shown.bs.modal', function (e) {
            $("#tableKBNOR210_1_STC_3_1").DataTable().columns.adjust().draw();
        });
        _xLib.AJAX_Get("/api/KBNOR210_1/GetDataKBNOR210_1_STC_3_1", obj, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                success = yield _xLib.JSONparseMixData(success);
                console.log(success);
                $("#tableKBNOR210_1_STC_3_1").DataTable().clear().rows.add(success.data).draw();
                $("#tableKBNOR210_1_STC_3_1 tbody tr td").css("text-align", "center");
                $("#tableKBNOR210_1_STC_3_1 thead tr th").css("text-align", "center");
                xSplash.hide();
            });
        }, function (error) {
            xSplash.hide();
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        });
    });
});
let _oriValue = 0;
$(document).on("click", "#tableKBNOR210_1_STC_3_1 tbody tr td", function () {
    var index = $("#tableKBNOR210_1_STC_3_1").DataTable().column(this).index();
    if (index < 4) {
        return;
    }
    _oriValue = $(this).text();
    $(this).empty();
    $(this).append(`<input type="number" min="0" value="" />`);
    $(this).find("input").focus();
});
$(document).on("focusout keypress", "#tableKBNOR210_1_STC_3_1 tbody tr td", function (e) {
    if (e.type == "keypress" && e.which != 13) {
        return;
    }
    let index = $("#tableKBNOR210_1_STC_3_1").DataTable().column(this).index();
    if (index < 4 && index >= 6) {
        return;
    }
    //console.log(_oriValue);
    let _value = $(this).find("input").val();
    //console.log(_value);
    $(this).empty();
    if (_value == "") {
        _value = _oriValue;
    }
    if (index == 4) {
        let useStock = $(this).closest("tr").find("td:eq(5)").text();
        if (parseInt(_value) < parseInt(useStock)) {
            //$(this).closest("tr").find("td:eq(5)").css("color", "red");
            $("#tableKBNOR210_1_STC_3_1").DataTable().cell(this).data(_oriValue).draw();
            return xSwal.error("Error", "Use Stock Qty must be less than Order Qty");
        }
        else {
            let orderRemain = parseInt(_value) - parseInt(useStock);
            $("#tableKBNOR210_1_STC_3_1").DataTable().cell({
                row: $(this).closest("tr").index(),
                column: 6
            }).data(orderRemain).draw();
        }
    }
    else {
        //index == 5
        let orderQty = $(this).closest("tr").find("td:eq(4)").text();
        let useStock = $(this).closest("tr").find("td:eq(5)").text();
        if (parseInt(orderQty) < parseInt(_value)) {
            //$(this).closest("tr").find("td:eq(4)").css("color", "red");
            $("#tableKBNOR210_1_STC_3_1").DataTable().cell(this).data(_oriValue).draw();
            return xSwal.error("Error", "Use Stock Qty must be less than Order Qty");
        }
        let orderRemain = parseInt(orderQty) - parseInt(_value);
        $("#tableKBNOR210_1_STC_3_1").DataTable().cell({
            row: $(this).closest("tr").index(),
            column: 6
        }).data(orderRemain).draw();
    }
    $("#tableKBNOR210_1_STC_3_1").DataTable().cell(this).data(_value).draw();
    let sum = 0;
    $("#tableKBNOR210_1_STC_3_1").DataTable().rows().data().each(function (value, index) {
        sum += parseInt(value.F_Use_Qty);
    });
    //console.log(sum);
    //console.log(ObjActualStock);
    if (sum > ObjActualStock) {
        $("#tableKBNOR210_1_STC_3_1").DataTable().cell(this).data(_oriValue).draw();
        return xSwal.error("Error", "Total Use Stock Qty must be less than Actual Stock Qty");
    }
});
$("#btnSTC_3_1Save").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        let listObj = $("#tableKBNOR210_1_STC_3_1").DataTable().rows().data().toArray();
        let isZero = true;
        listObj.forEach(function (item) {
            if (parseInt(item.F_Use_Qty) != 0) {
                isZero = false;
            }
        });
        if (isZero) {
            return xSwal.error("Error", "Use Stock Qty must be greater than 0");
        }
        yield _xLib.AJAX_Post("/api/KBNOR210_1/SaveKBNOR210_1_STC_3_1", listObj, function (success) {
            console.log(success);
            xSwal.success(success.response, success.message);
        }, function (error) {
            console.error(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        });
        $("#KBNOR210_1_STC_3_1").modal("hide");
    });
});
$("#btnSTC_3Save").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        let listObj = [];
        if ($("#tableKBNOR210_1_STC_3 tbody tr td").find("input[type='checkbox']:checked").length == 0) {
            return xSwal.error("Error", "Please select at least 1 row");
        }
        $("#tableKBNOR210_1_STC_3 tbody tr td").find("input[type='checkbox']:checked").each(function () {
            let obj = $("#tableKBNOR210_1_STC_3").DataTable().row($(this).closest("tr")).data();
            //console.log(obj);
            listObj.push(obj);
        });
        let isZero = true;
        listObj.forEach(function (item) {
            if (parseInt(item.F_Use_Qty) != 0) {
                isZero = false;
            }
        });
        if (isZero) {
            return xSwal.error("Error", "Use Stock Qty must be greater than 0");
        }
        //return console.log(listObj);
        yield _xLib.AJAX_Post("/api/KBNOR210_1/SaveKBNOR210_1_STC_3", listObj, function (success) {
            console.log(success);
            xSwal.success(success.response, success.message);
        }, function (error) {
            console.error(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        });
        $("#KBNOR210_1_STC_3").modal("hide");
    });
});
$(document).on("show.bs.modal", "#KBNOR210_1_STC_1", function (e) {
    return __awaiter(this, void 0, void 0, function* () {
        xSplash.show();
        if ($("#tableKBNOR210_1_STC_1").find("thead").length != 0) {
            $("#tableKBNOR210_1_STC_1").DataTable().clear().destroy();
        }
        $('.datepicker ').datepicker({
            uiLibrary: 'materialdesign',
            format: 'dd/mm/yyyy',
            autoclose: true,
            showRightIcon: true,
            //iconsLibrary: 'fontawesome'
        });
        yield $("#tableKBNOR210_1_STC_1").DataTable({
            width: '100%',
            paging: false,
            scrollCollapse: true,
            "processing": false,
            "serverSide": false,
            scrollX: false,
            scrollY: '300px',
            searching: false,
            info: false,
            ordering: false,
            autoWidth: true, // Enable autoWidth
            columns: [
                { title: "Stock Date", data: "F_Stock_Date" },
                { title: "Supplier Code", data: "F_Supplier_code" },
                { title: "Part No", data: "F_Part_No" },
                { title: "Kanban No", data: "F_Kanban_No" },
                { title: "Store CD", data: "F_Store_CD" },
                { title: "Qty/Package", data: "F_Qty_Pack" },
                { title: "Actual KB", data: "F_Actual_KB" },
                { title: "Actual PCS", data: "F_Actual_PCS" },
                { title: "Check By", data: "F_Check_By" },
                { title: "Update Date", data: "F_Update_Date" },
                { title: "Update By", data: "F_Update_By" },
            ],
            order: [[1, 'asc']]
        });
        // Make sure the modal is fully shown before adjusting columns
        yield $('#KBNOR210_1_STC_1').on('shown.bs.modal', function (e) {
            $("#tableKBNOR210_1_STC_1").DataTable().columns.adjust().draw();
            $("#tableKBNOR210_1_STC_1 thead tr th").css("text-align", "center");
            $("#tableKBNOR210_1_STC_1 tbody tr td").css("text-align", "center");
            xSplash.hide();
        });
    });
});
$('#btnKBNOR210_STC_1_Inq').click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        //STC_1_GetSupplierCode();
        STC_1_DisabledControl(true);
        STC_1_DisabledToolbars();
        _xLib.AJAX_Get("/api/KBNOR210_1/STC_1_ListDatatogrid", {}, function (success) {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#tableKBNOR210_1_STC_1").DataTable().clear().rows.add(success.data).draw();
            $("#tableKBNOR210_1_STC_1 thead tr th").css("text-align", "center");
            $("#tableKBNOR210_1_STC_1 tbody tr td").css("text-align", "center");
        }, function (error) {
            console.error(error);
        });
    });
});
function STC_1_DisabledControl(status) {
    if (status == true) {
        $("#inpKBNOR210_1_STC_1_StockDate").prop('disabled', true);
        $("#inpKBNOR210_1_STC_1_Supplier").prop("disabled", true);
        $("#inpKBNOR210_1_STC_1_PartNo").prop("disabled", true);
        $("#inpKBNOR210_1_STC_1_Store").prop("disabled", true);
        $("#inpKBNOR210_1_STC_1_Kanban").prop("readonly", true);
        $("#inpKBNOR210_1_STC_1_Actual").prop("readonly", true);
        $("#inpKBNOR210_1_STC_1_CheckStk").prop("disabled", true);
        $("#inpKBNOR210_1_STC_1_Remark").prop("readonly", true);
        $('#inpKBNOR210_1_STC_1_StockDate').val(moment().format("DD/MM/YYYY"));
        $("#inpKBNOR210_1_STC_1_Supplier").selectpicker("val", "");
        $("#inpKBNOR210_1_STC_1_PartNo").selectpicker("val", "");
        $("#inpKBNOR210_1_STC_1_Store").selectpicker("val", "");
        $("#inpKBNOR210_1_STC_1_Supplier").selectpicker("refresh");
        $("#inpKBNOR210_1_STC_1_PartNo").selectpicker("refresh");
        $("#inpKBNOR210_1_STC_1_Store").selectpicker("refresh");
        $("#inpKBNOR210_1_STC_1_CheckStk").selectpicker("refresh");
        $("#inpKBNOR210_1_STC_1_StockDate").parent().find("i").prop("hidden", true);
    }
    else {
        $("#inpKBNOR210_1_STC_1_StockDate").prop("disabled", false);
        $("#inpKBNOR210_1_STC_1_Supplier").prop("disabled", false);
        $("#inpKBNOR210_1_STC_1_PartNo").prop("disabled", false);
        $("#inpKBNOR210_1_STC_1_Store").prop("disabled", false);
        $("#inpKBNOR210_1_STC_1_Kanban").prop("readonly", true);
        $("#inpKBNOR210_1_STC_1_Actual").prop("readonly", false);
        $("#inpKBNOR210_1_STC_1_CheckStk").prop("disabled", false);
        $("#inpKBNOR210_1_STC_1_Remark").prop("readonly", false);
        $('#inpKBNOR210_1_STC_1_StockDate').val(moment().format("DD/MM/YYYY"));
        $("#inpKBNOR210_1_STC_1_Supplier").selectpicker("val", "");
        $("#inpKBNOR210_1_STC_1_PartNo").selectpicker("val", "");
        $("#inpKBNOR210_1_STC_1_Store").selectpicker("val", "");
        $("#inpKBNOR210_1_STC_1_Supplier").selectpicker("refresh");
        $("#inpKBNOR210_1_STC_1_PartNo").selectpicker("refresh");
        $("#inpKBNOR210_1_STC_1_Store").selectpicker("refresh");
        $("#inpKBNOR210_1_STC_1_CheckStk").selectpicker("refresh");
        $("#inpKBNOR210_1_STC_1_StockDate").parent().find("i").prop("hidden", false);
    }
}
function STC_1_ClearControl() {
    $("#divForm").find("input").val("");
    $("#divForm").find("select").val("");
    $("#inpKBNOR210_1_STC_1_Supplier").selectpicker("refresh");
    $("#inpKBNOR210_1_STC_1_PartNo").selectpicker("refresh");
    $("#inpKBNOR210_1_STC_1_Store").selectpicker("refresh");
    $("#inpKBNOR210_1_STC_1_CheckStk").selectpicker("refresh");
    $(".selectpicker").selectpicker("refresh");
}
function STC_1_DisabledToolbars() {
    $("#btnKBNOR210_STC_1_Inq").prop("disabled", true);
    $("#btnKBNOR210_STC_1_New").prop("disabled", true);
    $("#btnKBNOR210_STC_1_Upd").prop("disabled", true);
    $("#btnKBNOR210_STC_1_Del").prop("disabled", true);
    $("#btnKBNOR210_STC_1_Imp").prop("disabled", true);
}
let isNew = false;
let STC_1_Action = "";
$("#btnKBNOR210_STC_1_New").click(function () {
    isNew = true;
    STC_1_Action = "New";
    STC_1_GetSupplierCode();
    STC_1_GetPartNo();
    STC_1_GetStore();
    STC_1_DisabledControl(false);
    STC_1_DisabledToolbars();
    $("#btnSTC_1_Save").prop("disabled", false);
    $("#btnKBNOR210_STC_1_Cancel").prop("disabled", false);
    $("#btnKBNOR210_STC_1_New").prop("disabled", false);
});
$("#btnKBNOR210_STC_1_Upd").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        isNew = true;
        yield STC_1_GetSupplierCode();
        yield STC_1_GetPartNo();
        yield STC_1_GetStore();
        STC_1_Action = "Upd";
        isNew = false;
        STC_1_DisabledControl(false);
        STC_1_DisabledToolbars();
        $("#btnSTC_1_Save").prop("disabled", false);
        $("#btnKBNOR210_STC_1_Cancel").prop("disabled", false);
        $("#btnKBNOR210_STC_1_Upd").prop("disabled", false);
        _xLib.AJAX_Get("/api/KBNOR210_1/STC_1_ListDatatogrid", {}, function (success) {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#tableKBNOR210_1_STC_1").DataTable().clear().rows.add(success.data).draw();
            $("#tableKBNOR210_1_STC_1 thead tr th").css("text-align", "center");
            $("#tableKBNOR210_1_STC_1 tbody tr td").css("text-align", "center");
        }, function (error) {
            console.error(error);
        });
    });
});
$("#btnKBNOR210_STC_1_Del").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        isNew = true;
        yield STC_1_GetSupplierCode();
        yield STC_1_GetPartNo();
        yield STC_1_GetStore();
        STC_1_Action = "Del";
        isNew = false;
        STC_1_DisabledControl(false);
        STC_1_DisabledToolbars();
        $("#btnSTC_1_Save").prop("disabled", false);
        $("#btnKBNOR210_STC_1_Can").prop("disabled", false);
        $("#btnKBNOR210_STC_1_Del").prop("disabled", false);
        _xLib.AJAX_Get("/api/KBNOR210_1/STC_1_ListDatatogrid", {}, function (success) {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#tableKBNOR210_1_STC_1").DataTable().clear().rows.add(success.data).draw();
            $("#tableKBNOR210_1_STC_1 thead tr th").css("text-align", "center");
            $("#tableKBNOR210_1_STC_1 tbody tr td").css("text-align", "center");
        }, function (error) {
            console.error(error);
        });
    });
});
$("#btnKBNOR210_STC_1_Can").click(function () {
    isNew = false;
    STC_1_Action = "";
    STC_1_DisabledControl(true);
    $("#STC_1_divBtn").find("button").prop("disabled", false);
    $("#btnSTC_1_Save").prop("disabled", true);
    $("#tableKBNOR210_1_STC_1").DataTable().clear().draw();
});
$("#inpKBNOR210_1_STC_1_Supplier").change(function () {
    STC_1_GetPartNo();
    STC_1_GetStore();
    STC_1_GetSupplierName();
});
$("#inpKBNOR210_1_STC_1_PartNo").change(function () {
    STC_1_GetStore();
    STC_1_GetPartName();
});
$("#inpKBNOR210_1_STC_1_Store").change(function () {
    STC_1_GetKB_Qty();
});
function STC_1_GetSupplierCode() {
    let obj = {
        isNew: isNew,
        StockDate: moment($("#inpKBNOR210_1_STC_1_StockDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
    };
    _xLib.AJAX_Get("/api/KBNOR210_1/STC_1_GetSupplierCode", obj, function (success) {
        success = _xLib.JSONparseMixData(success);
        console.log(success);
        $("#inpKBNOR210_1_STC_1_Supplier").empty();
        $("#inpKBNOR210_1_STC_1_Supplier").append("<option value='' hidden></option>");
        success.data.forEach(function (item) {
            $("#inpKBNOR210_1_STC_1_Supplier").append(`<option value="${item.F_Supplier}">${item.F_Supplier}</option>`);
        });
        $("#inpKBNOR210_1_STC_1_Supplier").selectpicker("refresh");
    }, function (error) {
        console.error(error);
    });
}
function STC_1_GetPartNo() {
    let obj = {
        isNew: isNew,
        StockDate: moment($("#inpKBNOR210_1_STC_1_StockDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        Supplier_Code: $("#inpKBNOR210_1_STC_1_Supplier").val(),
    };
    _xLib.AJAX_Get("/api/KBNOR210_1/STC_1_GetPartNo", obj, function (success) {
        success = _xLib.JSONparseMixData(success);
        console.log(success);
        $("#inpKBNOR210_1_STC_1_PartNo").empty();
        $("#inpKBNOR210_1_STC_1_PartNo").append("<option value='' hidden></option>");
        success.data.forEach(function (item) {
            $("#inpKBNOR210_1_STC_1_PartNo").append(`<option value="${item.F_Part_no}">${item.F_Part_no}</option>`);
        });
        $("#inpKBNOR210_1_STC_1_PartNo").selectpicker("refresh");
    }, function (error) {
        console.error(error);
    });
}
function STC_1_GetStore() {
    let obj = {
        isNew: isNew,
        StockDate: moment($("#inpKBNOR210_1_STC_1_StockDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        Supplier_Code: $("#inpKBNOR210_1_STC_1_Supplier").val(),
        Part_No: $("#inpKBNOR210_1_STC_1_PartNo").val(),
    };
    _xLib.AJAX_Get("/api/KBNOR210_1/STC_1_GetStore", obj, function (success) {
        success = _xLib.JSONparseMixData(success);
        console.log(success);
        $("#inpKBNOR210_1_STC_1_Store").empty();
        $("#inpKBNOR210_1_STC_1_Store").append("<option value='' hidden></option>");
        success.data.forEach(function (item) {
            $("#inpKBNOR210_1_STC_1_Store").append(`<option value="${item.F_Store_CD}">${item.F_Store_CD}</option>`);
        });
        $("#inpKBNOR210_1_STC_1_Store").selectpicker("refresh");
    }, function (error) {
        console.error(error);
    });
}
function STC_1_GetSupplierName() {
    let obj = {
        Supplier_Code: $("#inpKBNOR210_1_STC_1_Supplier").val(),
    };
    _xLib.AJAX_Get("/api/KBNOR210_1/STC_1_GetSupplierName", obj, function (success) {
        $("#inpKBNOR210_1_STC_1_SupName").val(success.data);
    }, function (error) {
        console.error(error);
    });
}
function STC_1_GetPartName() {
    let obj = {
        Supplier_Code: $("#inpKBNOR210_1_STC_1_Supplier").val(),
        Part_No: $("#inpKBNOR210_1_STC_1_PartNo").val(),
    };
    _xLib.AJAX_Get("/api/KBNOR210_1/STC_1_GetPartName", obj, function (success) {
        $("#inpKBNOR210_1_STC_1_PartName").val(success.data);
    }, function (error) {
        console.error(error);
    });
}
function STC_1_GetKB_Qty() {
    let obj = {
        Supplier_Code: $("#inpKBNOR210_1_STC_1_Supplier").val(),
        Part_No: $("#inpKBNOR210_1_STC_1_PartNo").val(),
        Store_Code: $("#inpKBNOR210_1_STC_1_Store").val(),
        Stock_Date: moment($("#inpKBNOR210_1_STC_1_StockDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
    };
    _xLib.AJAX_Get("/api/KBNOR210_1/STC_1_GetKB_Qty", obj, function (success) {
        success = _xLib.JSONparseMixData(success);
        //console.log(success);
        $("#inpKBNOR210_1_STC_1_Kanban").val(success.data[0].F_Kanban_No);
        $("#inpKBNOR210_1_STC_1_Qty").val(success.data[0].F_Qty_Box);
    }, function (error) {
        console.error(error);
    });
}
$(document).on("dblclick", "#tableKBNOR210_1_STC_1 tbody tr td", function () {
    return __awaiter(this, void 0, void 0, function* () {
        if (STC_1_Action != "Upd" && STC_1_Action != "Del") {
            return;
        }
        $(this).closest("tr").addClass("selected").siblings().removeClass("selected");
        let obj = $("#tableKBNOR210_1_STC_1").DataTable().row($(this).closest("tr")).data();
        console.log(obj);
        yield $("#inpKBNOR210_1_STC_1_StockDate").val(moment(obj.F_Stock_Date, "YYYYMMDD").format("DD/MM/YYYY"));
        yield $("#inpKBNOR210_1_STC_1_Supplier").selectpicker("val", obj.F_Supplier_code);
        yield $("#inpKBNOR210_1_STC_1_PartNo").selectpicker("val", obj.F_Part_No);
        yield $("#inpKBNOR210_1_STC_1_Store").selectpicker("val", obj.F_Store_CD);
        yield $("#inpKBNOR210_1_STC_1_Kanban").val(obj.F_Kanban_No);
        yield $("#inpKBNOR210_1_STC_1_Actual").val(obj.F_Actual_PCS);
        yield $("#inpKBNOR210_1_STC_1_CheckStk").selectpicker("val", obj.F_Check_By);
        yield $("#inpKBNOR210_1_STC_1_Remark").val(obj.F_Remark);
        yield $("#inpKBNOR210_1_STC_1_Qty").val(obj.F_Qty_Pack);
        $("#inpKBNOR210_1_STC_1_StockDate").prop("disabled", true);
        $("#inpKBNOR210_1_STC_1_StockDate").parent().find("i").prop("hidden", true);
        $("#inpKBNOR210_1_STC_1_Supplier").prop("disabled", true);
        $("#inpKBNOR210_1_STC_1_PartNo").prop("disabled", true);
        $("#inpKBNOR210_1_STC_1_Store").prop("disabled", true);
        $(".selectpicker").selectpicker("refresh");
        let GetSupplierName = {
            Supplier_Code: obj.F_Supplier_code,
        };
        let GetPartName = {
            Supplier_Code: obj.F_Supplier_code,
            Part_No: obj.F_Part_No,
        };
        _xLib.AJAX_Get("/api/KBNOR210_1/STC_1_GetSupplierName", GetSupplierName, function (success) {
            $("#inpKBNOR210_1_STC_1_SupName").val(success.data);
        }, function (error) {
            console.error(error);
        });
        _xLib.AJAX_Get("/api/KBNOR210_1/STC_1_GetPartName", GetPartName, function (success) {
            $("#inpKBNOR210_1_STC_1_PartName").val(success.data);
        }, function (error) {
            console.error(error);
        });
    });
});
$("#btnSTC_1_Save").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = {
            Action: STC_1_Action,
            F_Stock_Date: moment($("#inpKBNOR210_1_STC_1_StockDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
            F_Supplier_code: $("#inpKBNOR210_1_STC_1_Supplier").val(),
            F_Part_No: $("#inpKBNOR210_1_STC_1_PartNo").val(),
            F_Store_CD: $("#inpKBNOR210_1_STC_1_Store").val(),
            F_Kanban_No: $("#inpKBNOR210_1_STC_1_Kanban").val(),
            F_Actual_PCS: $("#inpKBNOR210_1_STC_1_Actual").val(),
            F_Check_By: $("#inpKBNOR210_1_STC_1_CheckStk").val(),
            F_Remark: $("#inpKBNOR210_1_STC_1_Remark").val(),
            F_Qty_Pack: $("#inpKBNOR210_1_STC_1_Qty").val(),
        };
        yield _xLib.AJAX_Post("/api/KBNOR210_1/STC_1_Save", obj, function (success) {
            xSwal.success(success.response, success.message);
        }, function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        });
        $("#btnKBNOR210_STC_1_Can").click();
        $("#STC_1_divForm").find("input").val("");
        $("#STC_1_divForm").find("select").val("");
        $(".selectpicker").selectpicker("refresh");
    });
});
let _file = "";
$("#inpSTC_1_File").change(function (e) {
    _file = e.target.files[0];
});
$("#btnSTC_1_Import").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        if (_file == "") {
            return xSwal.error("Error", "Please select file");
        }
        const arrayBuffer = yield _file.arrayBuffer();
        const read = yield XLSX.read(arrayBuffer);
        const data = XLSX.utils.sheet_to_json(read.Sheets[read.SheetNames[0]]);
        console.log(data);
    });
});
$(document).on('show.bs.modal', '#KBNOR210_1_STC_2', function (e) {
    return __awaiter(this, void 0, void 0, function* () {
        yield xSplash.show();
        $('#inpSTC_2_DeliveryDateFrom').datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true,
            todayHighlight: true,
            uiLibrary: 'materialdesign',
            value: moment().format("DD/MM/YYYY")
        });
        $('#inpSTC_2_StockDateFrom').datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true,
            todayHighlight: true,
            uiLibrary: 'materialdesign',
            value: moment().format("DD/MM/YYYY")
        });
        $('#inpSTC_2_DeliveryDateTo').datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true,
            todayHighlight: true,
            uiLibrary: 'materialdesign',
            value: moment().format("DD/MM/YYYY")
        });
        $('#inpSTC_2_StockDateTo').datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true,
            todayHighlight: true,
            uiLibrary: 'materialdesign',
            value: moment().format("DD/MM/YYYY")
        });
        $("#inpSTC_2_DeliveryDateFrom").prop('disabled', true).parent().find("i").prop("hidden", true);
        $("#inpSTC_2_StockDateFrom").prop('disabled', true).parent().find("i").prop("hidden", true);
        $("#inpSTC_2_DeliveryDateTo").prop('disabled', true).parent().find("i").prop("hidden", true);
        $("#inpSTC_2_StockDateTo").prop('disabled', true).parent().find("i").prop("hidden", true);
        xSplash.hide();
        yield STC_2_LoadSupplier();
    });
});
$(document).on('click', '#STC_2_DeliveryDateChkBox', function () {
    if ($(this).prop("checked")) {
        $("#inpSTC_2_DeliveryDateFrom").prop('disabled', false).parent().find("i").prop("hidden", false);
        $("#inpSTC_2_DeliveryDateTo").prop('disabled', false).parent().find("i").prop("hidden", false);
    }
    else {
        $("#inpSTC_2_DeliveryDateFrom").prop('disabled', true).parent().find("i").prop("hidden", true);
        $("#inpSTC_2_DeliveryDateTo").prop('disabled', true).parent().find("i").prop("hidden", true);
    }
});
$(document).on('click', '#STC_2_StockDateChkBox', function () {
    if ($(this).prop("checked")) {
        $("#inpSTC_2_StockDateFrom").prop('disabled', false).parent().find("i").prop("hidden", false);
        $("#inpSTC_2_StockDateTo").prop('disabled', false).parent().find("i").prop("hidden", false);
    }
    else {
        $("#inpSTC_2_StockDateFrom").prop('disabled', true).parent().find("i").prop("hidden", true);
        $("#inpSTC_2_StockDateTo").prop('disabled', true).parent().find("i").prop("hidden", true);
    }
});
function STC_2_LoadSupplier() {
    let obj = {
        Type: $("#STC_2_divRadio").find("input[name='STC_2_Radio']:checked").val(),
        //SuppF = $("#inpSTC_2_SupplierFrom").val(),
        //SuppT = $("#inpSTC_2_SupplierTo").val(),
        chkFlg: $("#STC_2_DeliveryDateChkBox").prop("checked"),
        chkFlgDT: $("#STC_2_StockDateChkBox").prop("checked"),
    };
    if (obj.chkFlg) {
        obj.DateFrom = moment($("#inpSTC_2_DeliveryDateFrom").val(), "DD/MM/YYYY").format("YYYYMMDD");
        obj.DateTo = moment($("#inpSTC_2_DeliveryDateTo").val(), "DD/MM/YYYY").format("YYYYMMDD");
    }
    else {
        obj.DateFrom = moment($("#inpSTC_2_StockDateFrom").val(), "DD/MM/YYYY").format("YYYYMMDD");
        obj.DateTo = moment($("#inpSTC_2_StockDateTo").val(), "DD/MM/YYYY").format("YYYYMMDD");
    }
    _xLib.AJAX_Get("/api/KBNOR210_1/STC_2_GetSupplier", obj, function (success) {
        success = _xLib.JSONparseMixData(success);
        $('#inpSTC_2_SupplierFrom').empty();
        $('#inpSTC_2_SupplierTo').empty();
        $('#inpSTC_2_SupplierFrom').append("<option value='' hidden></option>");
        $('#inpSTC_2_SupplierTo').append("<option value='' hidden></option>");
        success.data.forEach(function (item) {
            $('#inpSTC_2_SupplierFrom').append(`<option value="${item.F_Supplier}">${item.F_Supplier}</option>`);
            $('#inpSTC_2_SupplierTo').append(`<option value="${item.F_Supplier}">${item.F_Supplier}</option>`);
        });
        $('#inpSTC_2_SupplierFrom').selectpicker("refresh");
        $('#inpSTC_2_SupplierTo').selectpicker("refresh");
    }, function (error) {
        console.error(error);
    });
}
function STC_2_LoadPartNo() {
    let obj = {
        Type: $("#STC_2_divRadio").find("input[name='STC_2_Radio']:checked").val(),
        SuppF: $("#inpSTC_2_SupplierFrom").val(),
        SuppT: $("#inpSTC_2_SupplierTo").val(),
        chkFlg: $("#STC_2_DeliveryDateChkBox").prop("checked"),
        chkFlgDT: $("#STC_2_StockDateChkBox").prop("checked"),
    };
    if (obj.chkFlg) {
        obj.DateFrom = moment($("#inpSTC_2_DeliveryDateFrom").val(), "DD/MM/YYYY").format("YYYYMMDD");
        obj.DateTo = moment($("#inpSTC_2_DeliveryDateTo").val(), "DD/MM/YYYY").format("YYYYMMDD");
    }
    else {
        obj.DateFrom = moment($("#inpSTC_2_StockDateFrom").val(), "DD/MM/YYYY").format("YYYYMMDD");
        obj.DateTo = moment($("#inpSTC_2_StockDateTo").val(), "DD/MM/YYYY").format("YYYYMMDD");
    }
    _xLib.AJAX_Get("/api/KBNOR210_1/STC_2_GetPartNo", obj, function (success) {
        success = _xLib.JSONparseMixData(success);
        $('#inpSTC_2_PartFrom').empty();
        $('#inpSTC_2_PartTo').empty();
        $('#inpSTC_2_PartFrom').append("<option value='' hidden></option>");
        $('#inpSTC_2_PartTo').append("<option value='' hidden></option>");
        success.data.forEach(function (item) {
            $('#inpSTC_2_PartFrom').append(`<option value="${item.F_Part_No}">${item.F_Part_No}</option>`);
            $('#inpSTC_2_PartTo').append(`<option value="${item.F_Part_No}">${item.F_Part_No}</option>`);
        });
        $('#inpSTC_2_PartFrom').selectpicker("refresh");
        $('#inpSTC_2_PartTo').selectpicker("refresh");
    }, function (error) {
        console.error(error);
    });
}
$("#inpSTC_2_SupplierFrom").change(function () {
    $('#inpSTC_2_SupplierTo').val($(this).val());
    $('#inpSTC_2_SupplierTo').selectpicker("refresh");
    $("#inpSTC_2_SupplierTo").change();
});
$("#inpSTC_2_SupplierTo").change(function () {
    STC_2_LoadPartNo();
});
$('#btnSTC_2_Print').click(function () {
    let Type = $("#STC_2_divRadio").find("input[name='STC_2_Radio']:checked").val();
    let chkFlgDT = $("#STC_2_StockDateChkBox").prop("checked");
    let obj = {
        SuppF: $("#inpSTC_2_SupplierFrom").val(),
        SuppT: $("#inpSTC_2_SupplierTo").val(),
        chkFlg: $("#STC_2_DeliveryDateChkBox").prop("checked"),
        PartF: $("#inpSTC_2_PartFrom").val(),
        PartT: $("#inpSTC_2_PartTo").val(),
        DateFrom: "",
        DateTo: "",
        Plant: _xLib.GetCookie("plantCode"),
        UserName: _xLib.GetUserName(),
    };
    if (obj.chkFlg && Type == "History") {
        obj.DateFrom = moment($("#inpSTC_2_DeliveryDateFrom").val(), "DD/MM/YYYY").format("YYYYMMDD");
        obj.DateTo = moment($("#inpSTC_2_DeliveryDateTo").val(), "DD/MM/YYYY").format("YYYYMMDD");
    }
    else if (chkFlgDT && Type == "Remain") {
        obj.chkFlg = true;
        obj.DateFrom = moment($("#inpSTC_2_StockDateFrom").val(), "DD/MM/YYYY").format("YYYYMMDD");
        obj.DateTo = moment($("#inpSTC_2_StockDateTo").val(), "DD/MM/YYYY").format("YYYYMMDD");
    }
    if (Type == "History") {
        _xLib.OpenReportObj("/KBNOR210_STC_2_History", obj);
    }
    else if (Type == "Remain") {
        _xLib.OpenReportObj("/KBNOR210_STC_2_Remain", obj);
    }
});
function Del_LoadSupplier() {
    var obj = {
        DeliYM: moment($("#inpDel_YM").val(), "MM/YYYY").format("YYYYMM")
    };
    _xLib.AJAX_Get("/api/KBNOR210_1/Del_LoadSupplier", obj, function (success) {
        success = _xLib.JSONparseMixData(success);
        $('#inpDel_Supplier').empty();
        $('#inpDel_Supplier').append("<option value='' hidden></option>");
        success.data.forEach(function (item) {
            $('#inpDel_Supplier').append(`<option value="${item.F_Supplier}">${item.F_Supplier}</option>`);
        });
        $('#inpDel_Supplier').selectpicker("refresh");
    }, function (error) {
        console.error(error);
        //xSwal.error("Error", error.responseJSON.message);
    });
}
function Del_LoadPartNo() {
    var obj = {
        DeliYM: moment($("#inpDel_YM").val(), "MM/YYYY").format("YYYYMM"),
        Supplier: $("#inpDel_Supplier").val()
    };
    _xLib.AJAX_Get("/api/KBNOR210_1/Del_LoadPartNo", obj, function (success) {
        success = _xLib.JSONparseMixData(success);
        $('#inpDel_PartFrom').empty();
        $('#inpDel_PartFrom').append("<option value='' hidden></option>");
        $('#inpDel_PartTo').empty();
        $('#inpDel_PartTo').append("<option value='' hidden></option>");
        success.data.forEach(function (item) {
            $('#inpDel_PartFrom').append(`<option value="${item.F_Part_No}">${item.F_Part_No}</option>`);
            $('#inpDel_PartTo').append(`<option value="${item.F_Part_No}">${item.F_Part_No}</option>`);
        });
        $('#inpDel_PartFrom').selectpicker("refresh");
        $('#inpDel_PartTo').selectpicker("refresh");
    }, function (error) {
        console.error(error);
        //xSwal.error("Error", error.responseJSON.message);
    });
}
$(document).on('show.bs.modal', '#KBNOR210_1_Del', function (e) {
    return __awaiter(this, void 0, void 0, function* () {
        yield xSplash.show();
        Del_LoadSupplier();
        xSplash.hide();
    });
});
$("#inpDel_Supplier").change(function () {
    Del_LoadPartNo();
});
let inpDel_YM = "";
$(document).on('change', '#inpDel_YM', function () {
    if (inpDel_YM == $("#inpDel_YM").val()) {
        return;
    }
    Del_LoadSupplier();
});
//$("#inpDel_YM").change(function () {
//    Del_LoadSupplier();
//});
$("#btnDel_Report").click(function () {
    let shift = _xLib.GetCookie("loginDate").toString().slice(11, 1) == "D" ? "1 : Day Shift" : "2 : Night Shift";
    console.log(shift);
    let obj = {
        DeliYM: moment($("#inpDel_YM").val(), "MM/YYYY").format("YYYYMM"),
        SuppCD: $("#inpDel_Supplier").val(),
        PartFrom: $("#inpDel_PartFrom").val(),
        PartTo: $("#inpDel_PartTo").val(),
        Plant: _xLib.GetCookie("plantCode"),
        UserName: _xLib.GetUserName(),
        Shift: shift
    };
    _xLib.OpenReportObj("/KBNOR210_1_Del", obj);
});
