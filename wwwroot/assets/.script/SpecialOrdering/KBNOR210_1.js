$(document).ready(async function () {

    await Initial();

    let loginDate = _xLib.GetCookie("loginDate");
    let plant = _xLib.GetCookie("plantCode");
    //console.log(loginDate);
    //console.log(plant);


    $("#readProcessDate").val(moment(loginDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));
    $("#readProcessShift").val(loginDate.slice(10, 11) == "D" ? "1:Day Shift" : "2:Night Shift");
    $("#readPlant").val(plant);

    await LoadDataChangeDelivery();

    $(".selectpicker").selectpicker("refresh");

    xSplash.hide();

});

$("#btnExit").click(function () {
    window.location.replace("/OrderingProcess/KBNOR200");
});

async function Initial() {
    await $("#tableMain").DataTable({
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
}

async function LoadDataChangeDelivery() {
    let obj = {
        PDSNo : $("#inpCustomerOrderNo").val(),
        SuppCd : $("#inpSupplier").val(),
        PartNo : $("#inpPartNo").val(),
        chkDeli: $("#chkDate").is(":checked"),
        DeliFrom: moment($("#inpDateFrom").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        DeliTo : moment($("#inpDateTo").val(), "DD/MM/YYYY").format("YYYYMMDD"),
    }

    _xLib.AJAX_Get("/api/KBNOR210_1/LoadDataChangeDelivery", obj,
        function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            //console.log(success);

            if (!$("#inpCustomerOrderNo").val())
            {
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

            if (!$("#inpPartNo").val())
            {
                $("#inpPartNo").empty();
                $("#inpPartNo").append("<option value='' hidden></option>");
                success.data.partNo.forEach(function (item) {
                    $("#inpPartNo").append(`<option value="${item.trim()}">${item.trim()}</option>`);
                });

                $("#inpPartNo").selectpicker("refresh");
            }
        },
        function (error) {
            console.error(error);
        }
    );

}
$("#inpCustomerOrderNo").change(async function () {
    $("#inpSupplier").val("");
    $("#inpPartNo").val("");
    await LoadDataChangeDelivery();
});
$("#inpSupplier").change(async function () {
    $("#inpPartNo").val("");
    await GetSupplierName();
    await LoadDataChangeDelivery();
});
$("#inpPartNo").change(async function () {
    await GetPartName();
    await LoadDataChangeDelivery();
});

async function GetSupplierName() {
    _xLib.AJAX_Get("/api/KBNOR210_1/GetSupplierName", { SuppCd: $("#inpSupplier").val() },
        function (success) {
            $("#readSupplier").val(success.data);
        },
        function (error) {
            console.error(error);
        }
    );
}

async function GetPartName() {
    _xLib.AJAX_Get("/api/KBNOR210_1/GetPartName", { PartNo: $("#inpPartNo").val() },
        function (success) {
            $("#readPartNo").val(success.data);
        },
        function (error) {
            console.error(error);
        }
    );
}

$("#chkDate").change(async function () {
    if ($(this).prop("checked")) {
        $("#inpDateFrom").prop("disabled", false);
        $("#inpDateTo").prop("disabled", false);
    }
    else {
        $("#inpDateFrom").prop("disabled", true);
        $("#inpDateTo").prop("disabled", true);
    }
});

$("#btnSearch").click(async function () {

    let obj = {
        PDSNo: $("#inpCustomerOrderNo").val(),
        SuppCd: $("#inpSupplier").val(),
        PartNo: $("#inpPartNo").val(),
        chkDeli: $("#chkDate").is(":checked"),
        DeliFrom: moment($("#inpDateFrom").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        DeliTo: moment($("#inpDateTo").val(), "DD/MM/YYYY").format("YYYYMMDD"),
    }

    _xLib.AJAX_Get("/api/KBNOR210_1/GetPOMergeData", obj,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#tableMain").DataTable().clear().rows.add(success.data).draw();
            $("#tableMain tbody tr td").css("text-align", "center");
            $("#tableMain thead tr th").css("text-align", "center");
        },
        function (error) {
            console.error(error);
        }
    );
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

$("#btnSave").click(async function () {
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
        }

        listObj.push(obj);
    });

    console.log(listObj);

    _xLib.AJAX_Post("/api/KBNOR210_1/Save", listObj,
        function (success) {
            console.log(success);
            xSwal.success("Success", success.message);
        },
        function (error) {
            console.error(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );

});

//Modal USE COMMON PARTS (KBNOR210_1_STC_3) : Use Stock Part
$(document).on("show.bs.modal", "#KBNOR210_1_STC_3", async function (e) {

    if (!$("#inpCustomerOrderNo").val())
    {
        $("#KBNOR210_1_STC_3").modal("hide");

        return xSwal.error("Error", "Please select Customer Order No.");
    }

    $("#btnUseCommonParts").attr("data-bs-orderno", $("#inpCustomerOrderNo").val());
    //$("body").css("visibility", "hidden")
    xSplash.show();

    if ($("#tableKBNOR210_1_STC_3").find("thead").length != 0) {
        $("#tableKBNOR210_1_STC_3").DataTable().clear().destroy();
    }

    await $("#tableKBNOR210_1_STC_3").DataTable({
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
        autoWidth: true,  // Enable autoWidth
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
                        if (data.remain == undefined) return;
                        return data.F_Remain;
                    }

                }
            },
        ],
        order: [[1, 'asc']]
    });

    $("#tableKBNOR210_1_STC_3 thead tr th").css("text-align", "center");

    // Make sure the modal is fully shown before adjusting columns
    await $('#KBNOR210_1_STC_3').on('shown.bs.modal', function (e) {

        $("#spanCustomerOrderNo").text(OrderNo);

        $("#tableKBNOR210_1_STC_3").DataTable().columns.adjust().draw();


        xSplash.hide();
    });

    let OrderNo = $(e.relatedTarget).data("bs-orderno");
    _xLib.AJAX_Get("/api/KBNOR210_1/LoadGridData", { OrderNo: OrderNo },
        function (success) {
            success.data = JSON.parse(success.data);
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#tableKBNOR210_1_STC_3").DataTable().clear().rows.add(success.data).draw();
            $("#tableKBNOR210_1_STC_3 tbody tr td").css("text-align", "center");
            $("#tableKBNOR210_1_STC_3 thead tr th").css("text-align", "center");
        },
        function (error) {
            console.error(error);
        }
    );

    
});

let ObjActualStock = 0;
//Modal INSIDE Modal (KBNOR210_1_STC_3_1)
$(document).on("dblclick", "#tableKBNOR210_1_STC_3 tbody tr td", async function () {
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

    await $("#tableKBNOR210_1_STC_3_1").DataTable({
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
        autoWidth: true,  // Enable autoWidth
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
    await $('#KBNOR210_1_STC_3_1').on('shown.bs.modal', function (e) {
        $("#tableKBNOR210_1_STC_3_1").DataTable().columns.adjust().draw();    
    });
    
    _xLib.AJAX_Get("/api/KBNOR210_1/GetDataKBNOR210_1_STC_3_1", obj,
        async function (success) {
            success = await _xLib.JSONparseMixData(success);
            console.log(success);
            $("#tableKBNOR210_1_STC_3_1").DataTable().clear().rows.add(success.data).draw();
            $("#tableKBNOR210_1_STC_3_1 tbody tr td").css("text-align", "center");
            $("#tableKBNOR210_1_STC_3_1 thead tr th").css("text-align", "center");
            xSplash.hide();
        },
        function (error) {
            xSplash.hide();
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    )

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
    if(sum > ObjActualStock)
    {
        $("#tableKBNOR210_1_STC_3_1").DataTable().cell(this).data(_oriValue).draw();
        return xSwal.error("Error", "Total Use Stock Qty must be less than Actual Stock Qty");
    }


});

$("#btnSTC_3_1Save").click(async function () {

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

    await _xLib.AJAX_Post("/api/KBNOR210_1/SaveKBNOR210_1_STC_3_1", listObj,
        function (success) {
            console.log(success);
            xSwal.success(success.response, success.message);
        },
        function (error) {
            console.error(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );

    $("#KBNOR210_1_STC_3_1").modal("hide");

});
$("#btnSTC_3Save").click(async function () {

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

    await _xLib.AJAX_Post("/api/KBNOR210_1/SaveKBNOR210_1_STC_3", listObj,
        function (success) {
            console.log(success);
            xSwal.success(success.response, success.message);
        },
        function (error) {
            console.error(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );

    $("#KBNOR210_1_STC_3").modal("hide");

});

$(document).on("show.bs.modal", "#KBNOR210_1_STC_1", async function (e) {

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

    await $("#tableKBNOR210_1_STC_1").DataTable({
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
        autoWidth: true,  // Enable autoWidth
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
            { title: "Update Date", data: "F_Update_By" },
            { title: "Update By", data: "F_Update_Date" },
        ],
        order: [[1, 'asc']]
    });


    // Make sure the modal is fully shown before adjusting columns
    await $('#KBNOR210_1_STC_1').on('shown.bs.modal', function (e) {

        $("#tableKBNOR210_1_STC_1").DataTable().columns.adjust().draw();
        $("#tableKBNOR210_1_STC_1 thead tr th").css("text-align", "center");
        $("#tableKBNOR210_1_STC_1 tbody tr td").css("text-align", "center");
        xSplash.hide();
    });



});

$('#btnKBNOR210_STC_1_Inq').click(async function () {
    //STC_1_GetSupplierCode();

    STC_1_DisabledControl(true);
    STC_1_DisabledToolbars();

    _xLib.AJAX_Get("/api/KBNOR210_1/STC_1_ListDatatogrid", {},
        function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#tableKBNOR210_1_STC_1").DataTable().clear().rows.add(success.data).draw();
        },
        function (error) {
            console.error(error);
        }
    );
});


function STC_1_DisabledControl(status)
{

    if (status == true) {

        $("#inpKBNOR210_1_STC_1_StockDate").prop('disabled', true);
        $("#inpKBNOR210_1_STC_1_Supplier").prop("disabled", true);
        $("#inpKBNOR210_1_STC_1_PartNo").prop("disabled", true);
        $("#inpKBNOR210_1_STC_1_Store").prop("disabled", true);
        $("#inpKBNOR210_1_STC_1_Kanban").prop("readonly", true);
        $("#inpKBNOR210_1_STC_1_Actual").prop("readonly", true);
        $("#inpKBNOR210_1_STC_1_CheckStk").selectpicker("disabled", true);
        $("#inpKBNOR210_1_STC_1_Remark").prop("readonly", true);

        $('#inpKBNOR210_1_STC_1_StockDate').val(moment().format("DD/MM/YYYY"));
        $("#inpKBNOR210_1_STC_1_Supplier").selectpicker("val", "");
        $("#inpKBNOR210_1_STC_1_PartNo").selectpicker("val", "");
        $("#inpKBNOR210_1_STC_1_Store").selectpicker("val", "");

        $("#inpKBNOR210_1_STC_1_Supplier").selectpicker("refresh");
        $("#inpKBNOR210_1_STC_1_PartNo").selectpicker("refresh");
        $("#inpKBNOR210_1_STC_1_Store").selectpicker("refresh");

        $("#inpKBNOR210_1_STC_1_StockDate").parent().find("i").prop("hidden", true);
    }
    else {

        $("#inpKBNOR210_1_STC_1_StockDate").prop("disabled", false);
        $("#inpKBNOR210_1_STC_1_Supplier").prop("disabled", false);
        $("#inpKBNOR210_1_STC_1_PartNo").prop("disabled", false);
        $("#inpKBNOR210_1_STC_1_Store").prop("disabled", false);
        $("#inpKBNOR210_1_STC_1_Kanban").prop("readonly", true);
        $("#inpKBNOR210_1_STC_1_Actual").prop("readonly", false);
        $("#inpKBNOR210_1_STC_1_CheckStk").selectpicker("disabled", false);
        $("#inpKBNOR210_1_STC_1_Remark").prop("readonly", false);

        $('#inpKBNOR210_1_STC_1_StockDate').val(moment().format("DD/MM/YYYY"));
        $("#inpKBNOR210_1_STC_1_Supplier").selectpicker("val", "");
        $("#inpKBNOR210_1_STC_1_PartNo").selectpicker("val", "");
        $("#inpKBNOR210_1_STC_1_Store").selectpicker("val", "");

        $("#inpKBNOR210_1_STC_1_Supplier").selectpicker("refresh");
        $("#inpKBNOR210_1_STC_1_PartNo").selectpicker("refresh");
        $("#inpKBNOR210_1_STC_1_Store").selectpicker("refresh");

        $("#inpKBNOR210_1_STC_1_StockDate").parent().find("i").prop("hidden", false);
    }

}

function STC_1_DisabledToolbars() {

    $("#btnKBNOR210_STC_1_Inq").prop("disabled", true);
    $("#btnKBNOR210_STC_1_New").prop("disabled", true);
    $("#btnKBNOR210_STC_1_Upd").prop("disabled", true);
    $("#btnKBNOR210_STC_1_Del").prop("disabled", true);
    $("#btnKBNOR210_STC_1_Imp").prop("disabled", true);



}

let isNew = false;

function STC_1_GetSupplierCode() {

    let obj = {
        isNew: isNew,
        StockDate: moment($("#inpKBNOR210_1_STC_1_StockDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
    }

    _xLib.AJAX_Get("/api/KBNOR210_1/STC_1_GetSupplierCode", obj,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#inpKBNOR210_1_STC_1_Supplier").empty();
            $("#inpKBNOR210_1_STC_1_Supplier").append("<option value='' hidden></option>");
            success.data.forEach(function (item) {
                $("#inpKBNOR210_1_STC_1_Supplier").append(`<option value="${item.trim()}">${item.trim()}</option>`);
            });

            $("#inpKBNOR210_1_STC_1_Supplier").selectpicker("refresh");
        },
        function (error) {
            console.error(error);
        }
    );

}