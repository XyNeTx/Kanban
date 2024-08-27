$(document).ready(async function () {

   
    await GetSupplier();
    await GetKanban();

    await GetStore();
    await GetPartNo();

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
                $("#selectSupplier").empty();
                success.data.forEach(function (item) {
                    $("#selectSupplier").append(`<option value="${item.f_Supplier_Code}">${item.f_Supplier_Code}</option>`);
                });
            }
        },
        function (error) {
            console.error(error);
            xSwal.error("Error", "Can't Get Supplier Code");
        }
    );
}
function GetKanban() {
    var obj = {
        supplier: $("#selectSupplier").val(),
        store: $("#selectStore").val(),
        storeTo: $("#selectStoreTo").val(),
        partNo: $("#selectPartNo").val(),
        partNoTo: $("#selectPartNoTo").val(),
    }

    _xLib.AJAX_Get("/api/KBNMS008/GetKanban", obj,
        function (success) {
            if (success.status == "200") {
                //console.log(success.data);
                $("#selectKanban").empty();
                $("#selectKanbanTo").empty();
                success.data.forEach(function (item) {
                    $("#selectKanban").append(`<option value="${item}">${item}</option>`);
                    $("#selectKanbanTo").append(`<option value="${item}">${item}</option>`);
                });
                $(".SelectPicker").selectpicker("refresh");
            }
        },
        function (error) {
            console.error(error);
            xSwal.error("Error", "Can't Get Kanban");
        }
    );
}

function GetStore() {
    var obj = {
        supplier: $("#selectSupplier").val(),
        kanban: $("#selectKanban").val(),
        kanbanTo: $("#selectKanbanTo").val(),
        partNo: $("#selectPartNo").val(),
        partNoTo: $("#selectPartNoTo").val(),
    }

    _xLib.AJAX_Get("/api/KBNMS008/GetStore", obj,
        function (success) {
            if (success.status == "200") {
                $("#selectStore").empty();
                $("#selectStoreTo").empty();
                success.data.forEach(function (item) {
                    $("#selectStore").append(`<option value="${item}">${item}</option>`);
                    $("#selectStoreTo").append(`<option value="${item}">${item}</option>`);
                });
                $(".SelectPicker").selectpicker("refresh");
            }
        },
        function (error) {
            console.error(error);
            xSwal.error("Error", "Can't Get Store");
        }
    );
}

function GetPartNo() {
    var obj = {
        supplier: $("#selectSupplier").val(),
        kanban: $("#selectKanban").val(),
        kanbanTo: $("#selectKanbanTo").val(),
        store: $("#selectStore").val(),
        storeTo: $("#selectStoreTo").val(),
    }

    _xLib.AJAX_Get("/api/KBNMS008/GetPartNo", obj,
        function (success) {
            if (success.status == "200") {
                $("#selectPart").empty();
                $("#selectPartTo").empty();
                success.data.forEach(function (item) {
                    $("#selectPart").append(`<option value="${item.f_Part_No}">${item.f_Part_No}</option>`);
                    $("#selectPartTo").append(`<option value="${item.f_Part_No}">${item.f_Part_No}</option>`);
                });
                $(".SelectPicker").selectpicker("refresh");
            }
        },
        function (error) {
            console.error(error);
            xSwal.error("Error", "Can't Get Part No");
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

    GetKanban();
    GetStore();
    GetPartNo();
});

$("#selectKanban , #selectKanbanTo").change(async function () {
    if ($(this).val() == "") {
        return;
    }

    if (!($("#selectStore").val()) && !($("#selectStoreTo").val() == "")) {
        GetStore();
    }
    if ( !($("#selectPartNo").val()) && !($("#selectPartNoTo").val()) ){
        GetPartNo();
    }

});

$("#selectStore , #selectStoreTo").change(async function () {
    console.log($(this).val());
    if ($(this).val() == "") {
        return;
    }

    if (!($("#selectKanban").val()) && !($("#selectKanbanTo").val()) ) {
        GetKanban();
    }
    if (!($("#selectPartNo").val()) && !($("#selectPartNoTo").val()) ) {
        GetPartNo();
    }
});

$("#selectPartNo , #selectPartNo").change(async function () {
    if ($(this).val() == "") {
        return;
    }
    if (!($("#selectKanban").val()) && !($("#selectKanbanTo").val())) {
        GetKanban();
    }
    if (!($("#selectStore").val()) && !($("#selectStoreTo").val())) {
        GetStore();
    }
});