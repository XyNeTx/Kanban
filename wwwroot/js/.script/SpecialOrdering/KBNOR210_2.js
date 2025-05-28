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
        console.log(loginDate);
        console.log(plant);
        $("#readProcessDate").val(moment(loginDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));
        $("#readProcessShift").val(loginDate.slice(10, 11) == "D" ? "1:Day Shift" : "2:Night Shift");
        $("#readPlant").val(plant);
        $("#chkDeliveryDate").prop("checked", true);
        yield GetCustomerPO();
        yield xSplash.hide();
    });
});
$("#btnExit").click(function () {
    if (window.location.hostname.includes("tpcap")) {
        return window.location.replace("/kanban/OrderingProcess/KBNOR200");
    }
    return window.location.replace("/OrderingProcess/KBNOR200");
});
$("#btnCheckAll").click(function () {
    $("#tableMain tbody tr").each(function () {
        $(this).find("input[type='checkbox']").prop("checked", true);
    });
});
$("#btnUncheckAll").click(function () {
    $("#tableMain tbody tr").each(function () {
        $(this).find("input[type='checkbox']").prop("checked", false);
    });
});
function Initial() {
    return __awaiter(this, void 0, void 0, function* () {
        yield $("#tableMain").DataTable({
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
                {
                    title: "Flag", data: "Flag", render: function () {
                        return `<input type="checkbox" class="chkboxTable" id="chkFlag" name="chkFlag">`;
                    }
                },
                { title: "Prod YM", data: "f_ProdYM" },
                { title: "Customer OrderNo.", data: "f_PDS_No" },
                { title: "Delivery Date", data: "f_Delivery_Date" },
                { title: "Cust.OrderType", data: "f_CusOrderType_CD" },
            ],
            order: [[1, 'asc']]
        });
        $("table thead tr th").css("text-align", "center");
        $("table tbody tr td").css("text-align", "center");
    });
}
$("#chkDeliveryDate").on("change", function () {
    if ($(this).is(":checked")) {
        $("#inpDeliveryDate").prop("disabled", false);
    }
    else {
        $("#inpDeliveryDate").prop("disabled", true);
    }
});
$("#chkCustomerOrderNo").on("change", function () {
    if ($(this).is(":checked")) {
        $("#inpCustomerOrderNo").prop("disabled", false);
    }
    else {
        $("#inpCustomerOrderNo").prop("disabled", true);
    }
});
function GetCustomerPO() {
    return __awaiter(this, void 0, void 0, function* () {
        yield xSplash.show();
        let obj = {};
        if ($("#chkDeliveryDate").is(":checked")) {
            obj.DeliDT = $("#inpDeliveryDate").val().replaceAll("-", "");
        }
        if ($("#chkCustomerOrderNo").is(":checked")) {
            obj.OrderNo = $("#inpCustomerOrderNo").val();
        }
        _xLib.AJAX_Get("/api/KBNOR210_2/GetCustomerPO", obj, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                console.log(success);
                success.data = _xLib.TrimArrayJSON(success.data);
                $("#tableMain").DataTable().clear().rows.add(success.data).columns.adjust().draw();
                $("table thead tr th").css("text-align", "center");
                $("table tbody tr td").css("text-align", "center");
                $("#btnCheckAll").prop("disabled", false);
                $("#btnUncheckAll").prop("disabled", false);
                $("#btnMerge").prop("disabled", false);
                yield xSplash.hide();
            });
        }, function (error) {
            console.log(error);
            $("#btnCheckAll").prop("disabled", true);
            $("#btnUncheckAll").prop("disabled", true);
            $("#btnMerge").prop("disabled", true);
        });
    });
}
;
$("#inpDeliveryDate , #inpCustomerOrderNo , #chkDeliveryDate , #chkCustomerOrderNo").change(function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield GetCustomerPO();
    });
});
$("#btnMerge").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield xSplash.show();
        let listObj = [];
        $("#tableMain input[type='checkbox']:checked").each(function () {
            let obj = $("#tableMain").DataTable().row($(this).closest("tr")).data();
            obj.F_PDS_No_New = $("#inpNewCustomerOrderNo").val();
            listObj.push(obj);
        });
        console.log(listObj);
        if ($("#inpNewCustomerOrderNo").val().includes(",")) {
            return xSwal.error("Cannot Use ',' in Customer Order No");
        }
        if (listObj.length == 0) {
            yield xSplash.hide();
            xSwal.error("Please select one customer orderno for merge data!");
            return;
        }
        if (listObj.length > 22) {
            // 22 is max row for merge
            yield xSplash.hide();
            xSwal.error("Please select Customer OrderNo not over than 22 OrderNo.");
            return;
        }
        let Flag = false;
        let OldOrderType = "";
        listObj.forEach(function (item) {
            if (OldOrderType == "") {
                OldOrderType = item.f_CusOrderType_CD;
            }
            else {
                if (OldOrderType != item.f_CusOrderType_CD) {
                    Flag = true;
                }
            }
        });
        if (Flag) {
            yield xSplash.hide();
            xSwal.error("Please select as same as type of Customer Order ");
            return;
        }
        _xLib.AJAX_Post("/api/KBNOR210_2/Merge", JSON.stringify(listObj), function (success) {
            console.log(success);
            if (success.status) {
                xSwal.success("Merge Success!");
                $("#inpNewCustomerOrderNo").val("");
                if ($("#chkCustomerOrderNo").is(":checked")) {
                    $("#chkCustomerOrderNo").prop("checked", false);
                    $("#inpCustomerOrderNo").prop("disabled", true);
                    $("#inpCustomerOrderNo").val("");
                }
                $("#tableMain input[type='checkbox']:checked").each(function () {
                    $("#tableMain").DataTable().row($(this).closest("tr")).remove().draw(false);
                });
                GetCustomerPO();
            }
            else {
                xSwal.error("Merge Fail!");
            }
        }, function (error) {
            xSwal.xError(error);
        });
        yield xSplash.hide();
    });
});
