$(document).ready(function () {

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

    $('#btnInq').trigger('click');
});

$("#btnInq").click(function List_Data() {
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

        },
        function (error) {
            console.log(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );
});

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