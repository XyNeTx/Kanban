$(document).ready(async function () {

    await GetSupplier();

    await $(".SelectPicker").selectpicker();

    $("#tableMain").DataTable({
        "processing": true,
        "serverSide": false,
        width: '100%',
        paging: false,
        scrollCollapse: true,
        scrollX: false,
        scrollY: '350px',
        "columns": [
            { title: "Part No", data: "PartNo" },
            { title: "Kanban No", data: "KanbanNo" },
            { title: "Last Order Diff(Pcs)", data: "LastOrderDiff" },
            { title: "Forecast(Pcs)", data: "Forecast" },
            { title: "Trip 1" , data: "Trip1" },
            { title: "Trip 2" , data: "Trip2" },
            { title: "Qty/Pack" , data: "QtyPack" },
            { title: "Total(KB)" , data: "TotalKB" },
            { title: "Total(Pcs)" , data: "TotalPcs" },
            { title: "Order Diff(Pcs)" , data: "OrderDiff" },
        ],
        order : [[1,"asc"]]

    });

    $("#selectDelivery").datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
        todayHighlight: true,
        autoclose: true,

    });

    $("#selectDeliveryTo").datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
        todayHighlight: true,
        autoclose: true,

    });

    $("#selectDate").datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
        todayHighlight: true,
        autoclose: true,

    });

    $("#selectDelivery").parent().prepend(`<label class="input-group-text" for="selectDelivery">Delivery Date :</label>`);
    $("#selectDeliveryTo").parent().prepend(`<label class="input-group-text" for="selectDeliveryTo">TO : </label>`);
    $("#selectDate").parent().prepend(`<label class="input-group-text" for="selectDate">Date ===> </label>`);

    xSplash.hide();
})

function GetSupplier() {
    _xLib.AJAX_Get("/api/KBNMS008/GetSupplier", "",
        function (success) {
            if (success.status == "200") {
                $("selectSupplier").empty();
                success.data.forEach(function (item) {
                    $("#selectSupplier").append(`<option value="${item.f_Supplier_Code}">${item.f_Supplier_Code}</option>`);
                });

                $(".SelectPicker").selectpicker("refresh");
            }
        },
        function (error) {
            console.error(error);
            xSwal.error("Error", "Can't Get Supplier Code");
        }
    );
}

$("#selectSupplier").change(async function () {
    if ($(this).val() == "") {
        return;
    }

    var obj = {
        supplier : $(this).val()
    }

    _xLib.AJAX_Get("/api/KBNMS008/GetSupplierDetail", obj,
        function (success) {
            if (success.status == "200") {
                console.log(success.data);
            }
        },
        function (error) {
            console.error(error);
            xSwal.error("Error", "Can't Get Supplier Detail");
        }
    );
});

