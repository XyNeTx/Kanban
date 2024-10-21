$(document).ready(function () {

    $("#tableMain").DataTable({
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
        columns: [
            {
                title: "Flag", data: null, width: "10%", render: function (data, type, row) {
                    return "<input type='checkbox' id='chkFlag' name='chkFlag' value='" + data.F_Supplier_Code + "' />"
                }
            },
            { title: "Supplier", data: "F_Supplier_Code", width: "15%" },
            { title: "Short Name", data: "F_Short_Name", width: "15%" },
            { title: "Attention", data: "F_Attention", width: "20%" },
            { title: "Telephone", data: "F_Telephone", width: "20%" },
            { title: "Fax", data: "F_Fax", width: "20%" },
        ],
        order: [[1, 'asc']],
    });

    $("table thead tr th").addClass("text-center");
    $("table tbody tr td").addClass("text-center");

    $("#btnSave").prop("disabled", true);
    $("#btnReport").prop("disabled", true);

    SupplierDropDown(false);

    xSplash.hide();

});

$("#btnExt").click(function () {
    window.location.replace("/OrderingProcess/KBNOR200");
});

function SupplierDropDown(isNew)
{
    let obj = {
        isNew: isNew
    }

    _xLib.AJAX_Get("/api/KBNMS004/SupplierDropDown", obj,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#inpSupplier").empty();
            $("#inpSupplier").append("<option value='' hidden>-- Select Supplier --</option>");
            $.each(success.data, function (index, value) {
                $("#inpSupplier").append("<option value='" + value.F_Supplier_Code + "'>" + value.F_Supplier_Code + "</option>");
            });
            $("#inpSupplier").selectpicker("refresh");

        },
        function (error) {
            console.log(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );

}

$("#inpSupplier").change(function SupplierChanged () {
    let obj = {
        Supplier: $("#inpSupplier").val()
    }

    _xLib.AJAX_Get("/api/KBNMS004/SupplierChanged", obj,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#inpSupplierName").val(success.data[0].F_Short_Name);
            $("#inpAttention").val(success.data[0].F_Attention);
            $("#inpTelephone").val(success.data[0].F_Telephone);
            $("#inpFax").val(success.data[0].F_Fax);
        },
        function (error) {
            console.log(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );

    let activedBtn = $("#divButton").find("button:not([disabled])").attr("id");
    console.log(activedBtn)
    if (activedBtn == "btnNew") return;

    _xLib.AJAX_Get("/api/KBNMS004/ListData", obj,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#tableMain").DataTable().clear().rows.add(success.data).draw();
            $("table tbody tr td").addClass("text-center");
            $("table thead tr th").addClass("text-center");
            $("#inpSupplier").prop("disabled", false);
            $("#inpSupplier").selectpicker("refresh");

        },
        function (error) {
            console.log(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );

    //$('#btnInq').trigger('click');
});

$("#btnSel").click(function () {
    $(document).find("input[name='chkFlag']").each(function () {
        $(this).prop("checked", true);
    });
});

$("#btnDesel").click(function () {
    $(document).find("input[name='chkFlag']").each(function () {
        $(this).prop("checked", false);
    });
});

$("#btnInq").click(function List_Data() {

    $('#btnNew').prop('disabled', true);
    $('#btnUpd').prop('disabled', true);
    $('#btnDel').prop('disabled', true);

    let obj = {
        Supplier: $("#inpSupplier").val(),
    }

    _xLib.AJAX_Get("/api/KBNMS004/ListData", obj,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#tableMain").DataTable().clear().rows.add(success.data).draw();
            $("table tbody tr td").addClass("text-center");
            $("table thead tr th").addClass("text-center");
            $("#inpSupplier").prop("disabled", false);
            $("#inpSupplier").selectpicker("refresh");

        },
        function (error) {
            console.log(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );

    $("#btnSave").prop("disabled", true);
    $("#btnReport").prop("disabled", false);

});

$("#btnUpd").click(function () {

    $("#btnInq").prop("disabled", true);
    $("#btnNew").prop("disabled", true);
    $("#btnDel").prop("disabled", true);

    $("#inpSupplier").prop("disabled", false);
    $("#inpSupplierName").prop("readonly", false);
    $("#inpAttention").prop("readonly", false);
    $("#inpTelephone").prop("readonly", false);
    $("#inpFax").prop("readonly", false);

    $("#inpSupplier").selectpicker("refresh");

    $("#btnSave").prop("disabled", false);
    $("#btnReport").prop("disabled", true);
});

$("#btnDel").click(function () {

    $("#btnInq").prop("disabled", true);
    $("#btnNew").prop("disabled", true);
    $("#btnUpd").prop("disabled", true);

    $("#inpSupplier").prop("disabled", false);
    $("#inpSupplierName").prop("readonly", true);
    $("#inpAttention").prop("readonly", true);
    $("#inpTelephone").prop("readonly", true);
    $("#inpFax").prop("readonly", true);

    $("#inpSupplier").selectpicker("refresh");

    $("#btnSave").prop("disabled", false);
    $("#btnReport").prop("disabled", true);
});


$("#btnCan").click(function () {

    $("#btnInq").prop("disabled", false);
    $("#btnNew").prop("disabled", false);
    $("#btnUpd").prop("disabled", false);
    $("#btnDel").prop("disabled", false);

    $("#inpSupplier").prop("disabled", true);
    $("#inpSupplierName").prop("readonly", true);
    $("#inpAttention").prop("readonly", true);
    $("#inpTelephone").prop("readonly", true);
    $("#inpFax").prop("readonly", true);

    $("#tableMain").DataTable().clear().draw();

    $("#frmCondition").trigger("reset");

    $("#btnSave").prop("disabled", true);
    $("#btnReport").prop("disabled", true);

    $("#inpSupplier").selectpicker("refresh");
    //$("#inpSupplier").parent().find("div[class='filter-option-inner-inner']").empty().append("-- Select Supplier --");
});

$("#btnNew").click(function () {
    $("#btnInq").prop("disabled", true);
    $("#btnUpd").prop("disabled", true);
    $("#btnDel").prop("disabled", true);
    
    $("#inpSupplier").prop("disabled", false);
    $("#inpSupplierName").prop("readonly", false);
    $("#inpAttention").prop("readonly", false);
    $("#inpTelephone").prop("readonly", false);
    $("#inpFax").prop("readonly", false);
    
    $("#inpSupplier").selectpicker("refresh");

    $("#btnSave").prop("disabled", false);
    $("#btnReport").prop("disabled", true);

    SupplierDropDown(true);
});

$("#btnSave").click(async function () {
    let activedBtn = $("#divButton").find("button:not([disabled])").attr("id");

    if (activedBtn == "btnDel") {
        obj = [];
        $("#tableMain").find("input[name='chkFlag']:checked").each(function () {
            let row = $(this).closest("tr");
            let data = $("#tableMain").DataTable().row(row).data();
            data.F_Supplier_Plant = data.F_Supplier_Code.split("-")[1];
            data.F_Supplier_Code = data.F_Supplier_Code.split("-")[0];
            obj.push(data);
            console.log(data);

        });

        if (obj.length == 0) {
            xSwal.error("Error", "Please select data to delete.");
            return;
        }

        if (await xSwal.confirm("Are you sure", "This action cannot be undone.")) {
            Save("Delete", obj);
        }
    }
    else if (activedBtn == "btnNew") {
        if ($("#inpSupplier").val() == "") {
            xSwal.error("Error", "Please select supplier.");
            return;
        }

        let obj = [{
            F_Supplier_Code: $("#inpSupplier").val().split("-")[0],
            F_Supplier_Plant: $("#inpSupplier").val().split("-")[1],
            F_Short_Name: $("#inpSupplierName").val(),
            F_Attention: $("#inpAttention").val(),
            F_Telephone: $("#inpTelephone").val(),
            F_Fax: $("#inpFax").val()
        }]

        Save("New", obj);
    }
    else {
        if ($("#inpSupplier").val() == "") {
            xSwal.error("Error", "Please select supplier.");
            return;
        }

        let obj = [{
            F_Supplier_Code: $("#inpSupplier").val().split("-")[0],
            F_Supplier_Plant: $("#inpSupplier").val().split("-")[1],
            F_Short_Name: $("#inpSupplierName").val(),
            F_Attention: $("#inpAttention").val(),
            F_Telephone: $("#inpTelephone").val(),
            F_Fax: $("#inpFax").val()
        }]

        Save("Update", obj);
    }

});

function Save(Action, Obj) {

    _xLib.AJAX_Post("/api/KBNMS004/Save?Action=" + Action, Obj,
        function (success) {
            xSwal.success("Success", success.message);
            $("#btnCan").trigger("click");
        },
        function (error) {
            console.log(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );

}

$(document).on("click", "table tbody tr td", function () {
    let row = $(this).closest("tr");
    row.addClass("selected").siblings().removeClass("selected");

    let data = $("#tableMain").DataTable().row(row).data();

    $("#inpSupplier").val(data.F_Supplier_Code);
    $("#inpSupplierName").val(data.F_Short_Name);
    $("#inpAttention").val(data.F_Attention);
    $("#inpTelephone").val(data.F_Telephone);
    $("#inpFax").val(data.F_Fax);

    $("#inpSupplier").selectpicker("refresh");
});

$("#btnReport").click(function () {
    let obj = {
        UserName: _xLib.GetUserName(),
        Sup: $("#inpSupplier").val(),
        Plant: _xLib.GetCookie("plantCode"),
    }

    _xLib.OpenReportObj("/KBNMS004", obj);

});