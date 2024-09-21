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
            },

            {
                title: "Qty", render: function (data, type, row) {
                    if (row.F_Delivery_DT_2.includes("/")) {
                        return row.F_Qty_2;
                    }
                    else {
                        return "";
                    }
                },
            },

            { title: "Delivery Date", data: "F_Delivery_DT_2" },

            {
                title: " ", render: function (data, type, row) {
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
            },

            {
                title: "Qty", render: function (data, type, row) {
                    if (row.F_Delivery_DT_3.includes("/")) {
                        return row.F_Qty_3;
                    }
                    else {
                        return "";
                    }
                },
            },

            { title: "Delivery Date", data: "F_Delivery_DT_3" },

            {
                title: " ", render: function (data, type, row) {
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
            },

            {
                title: "Qty", render: function (data, type, row) {
                    if (row.F_Delivery_DT_4.includes("/")) {
                        return row.F_Qty_4;
                    }
                    else {
                        return "";
                    }
                },
            },

            { title: "Delivery Date", data: "F_Delivery_DT_4" },

            {
                title: " ", render: function (data, type, row) {
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
            },

            {
                title: "Qty", render: function (data, type, row) {
                    if (row.F_Delivery_DT_5.includes("/")) {
                        return row.F_Qty_5;
                    }
                    else {
                        return "";
                    }
                },
            },

            { title: "Delivery Date", data: "F_Delivery_DT_5" },

            {
                title: " ", render: function (data, type, row) {
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
            },

            {
                title: "Qty", render: function (data, type, row) {
                    if (row.F_Delivery_DT_6.includes("/")) {
                        return row.F_Qty_6;
                    }
                    else {
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
            }

            if (!$("#inpSupplier").val()) {
                $("#inpSupplier").empty();
                $("#inpSupplier").append("<option value='' hidden></option>");
                success.data.supplier.forEach(function (item) {
                    $("#inpSupplier").append(`<option value="${item.trim()}">${item.trim()}</option>`);
                });
            }

            if (!$("#inpPartNo").val())
            {
                $("#inpPartNo").empty();
                $("#inpPartNo").append("<option value='' hidden></option>");
                success.data.partNo.forEach(function (item) {
                    $("#inpPartNo").append(`<option value="${item.trim()}">${item.trim()}</option>`);
                });
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

//$(document).on("click", "input[type='checkbox']", function () {

//    if ($(this).prop("checked")) {
//        var checkBoxIndex = $(this).closest("td").index();

//        var obj = $("#tableMain").DataTable().row($(this).closest("tr")).data();
//        var objIndex = $("#tableMain").DataTable().row($(this).closest("tr")).index();

//        var oldPds = $("#tableMain").DataTable().row(objIndex - 1).data();

//        console.log(checkBoxIndex);
//        console.log(obj);
//        console.log(objIndex);
//        console.log(oldPds);
//    }

//});

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