let _cookieLoginDate = _xLib.GetCookie("loginDate");

$(document).ready(async function () {

   
    await GetSupplier();
    await $("#selectSupplier").val("");
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
        scrollX: true,
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
        maxDate: function () {
            return $("#selectDeliveryTo").val();
        }

    });

    $("#selectDeliveryTo").datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
        todayHighlight: true,
        minDate: function () {
            return $("#selectDelivery").val();
        }
    });

    $("#selectDate").datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
        todayHighlight: true,
        autoclose: true,
        minDate: function() {
            return $("#selectDelivery").val();
        },
        maxDate: function () {
            return $("#selectDeliveryTo").val();
        }
    });

    $("#selectDelivery").val(moment(_cookieLoginDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));
    $("#selectDeliveryTo").val(moment(_cookieLoginDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));

    $("#selectDelivery").parent().prepend(`<label class="input-group-text" for="selectDelivery">Delivery Date :</label>`);
    $("#selectDeliveryTo").parent().prepend(`<label class="input-group-text" for="selectDeliveryTo">TO : </label>`);
    $("#selectDate").parent().prepend(`<label class="input-group-text" for="selectDate">Date ===> </label>`);

    $("#selectDate").parent().find("button").prop("disabled", true);

    await xSplash.hide();
})

async function GetSupplier() {
    await _xLib.AJAX_Get("/api/KBNMS008/GetSupplier", "",
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
async function GetKanban() {
    var obj = {
        supplier: $("#selectSupplier").val(),
        store: $("#selectStore").val(),
        storeTo: $("#selectStoreTo").val(),
        partNo: $("#selectPart").val(),
        partNoTo: $("#selectPartTo").val(),
    }

    await _xLib.AJAX_Get("/api/KBNMS008/GetKanban", obj,
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

async function GetStore() {
    var obj = {
        supplier: $("#selectSupplier").val(),
        kanban: $("#selectKanban").val(),
        kanbanTo: $("#selectKanbanTo").val(),
        partNo: $("#selectPart").val(),
        partNoTo: $("#selectPartTo").val(),
    }

    await _xLib.AJAX_Get("/api/KBNMS008/GetStore", obj,
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

async function GetPartNo() {
    var obj = {
        supplier: $("#selectSupplier").val(),
        kanban: $("#selectKanban").val(),
        kanbanTo: $("#selectKanbanTo").val(),
        store: $("#selectStore").val(),
        storeTo: $("#selectStoreTo").val(),
    }

    await _xLib.AJAX_Get("/api/KBNMS008/GetPartNo", obj,
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
        async function (success) {
            if (success.status == "200") {
                console.log(success.data);
                $("#readSupplier").val(obj.supplier);
                $("#readSupplierName").val(success.data.f_Supplier_Name);
                $("#readSafetyStock").val(success.data.f_Safety_Stk);
                $("#readCycle").val(success.cycle.slice(0, 2) + "-" + success.cycle.slice(2, 4) + "-" + success.cycle.slice(4, 6));
                await GetTrip(success.cycle.slice(2, 4));
            }
        },
        function (error) {
            console.error(error);
            xSwal.error("Error", "Can't Get Supplier Detail");
        }
    );

    await $("#selectKanban").val("");
    await $("#selectKanbanTo").val("");
    await $("#selectStore").val("");
    await $("#selectStoreTo").val("");
    await $("#selectPart").val("");
    await $("#selectPartTo").val("");

    await GetKanban();
    await GetStore();
    await GetPartNo();
});

function GetTrip(cycle) {
    let columnTrip = [];

    for (let i = 1; i <= parseInt(cycle); i++) {
        columnTrip.push({ title: "Trip " + i, data: "Trip" + i });
    }

    $("#tableMain").DataTable().clear().destroy();
    $("#tableMain").empty();
    $("#tableMain").DataTable({
        "processing": false,
        "serverSide": false,
        width: '100%',
        paging: false,
        scrollCollapse: true,
        scrollX: true,
        scrollY: '350px',
        "columns": [
            { title: "Part No", data: "PartNo" },
            { title: "Kanban No", data: "KanbanNo" },
            { title: "Last Order Diff(Pcs)", data: "LastOrderDiff" },
            { title: "Forecast(Pcs)", data: "Forecast" },
            ...columnTrip,
            { title: "Qty/Pack", data: "QtyPack" },
            { title: "Total(KB)", data: "TotalKB" },
            { title: "Total(Pcs)", data: "TotalPcs" },
            { title: "Order Diff(Pcs)", data: "OrderDiff" },
        ],
        order: [[1, "asc"]]

    });

    $("#tableMain").DataTable().clear().draw();
    

}

$("#selectKanban , #selectKanbanTo").change(async function () {
    if ($(this).val() == "") {
        return;
    }

    if (!$("#selectKanban").val()) {
        $("#selectKanban").val($("#selectKanbanTo").val());
    }
    else if (!$("#selectKanbanTo").val()) {
        $("#selectKanbanTo").val($("#selectKanban").val());
    }

    //if (!($("#selectStore").val()) && !($("#selectStoreTo").val() == "")) {
    //    GetStore();
    //}
    //if ( !($("#selectPart").val()) && !($("#selectPartTo").val()) ){
    //    GetPartNo();
    //}
    await GetStore();
    await GetPartNo();
});

$("#selectStore , #selectStoreTo").change(async function () {
    console.log($(this).val());
    if ($(this).val() == "") {
        return;
    }

    if (!$("#selectStore").val()) {
        $("#selectStore").val($("#selectStoreTo").val());
    }
    else if (!$("#selectStoreTo").val()) {
        $("#selectStoreTo").val($("#selectStore").val());
    }

    if (!($("#selectKanban").val()) && !($("#selectKanbanTo").val())) {
        await GetKanban();
    }
    //if (!($("#selectPart").val()) && !($("#selectPartTo").val()) ) {
    //    GetPartNo();
    //}
    await GetPartNo();
});

$("#selectPart , #selectPartTo").change(async function () {
    if ($(this).val() == "") {
        return;
    }

    if (!$("#selectPart").val()) {
        $("#selectPart").val($("#selectPartTo").val());
    }
    if (!$("#selectPartTo").val()) {
        $("#selectPartTo").val($("#selectPart").val());
    }

    if ($("#selectPartTo").val() == "") {
        $("#selectPartTo").val($("#selectPart").val());
    }

    $("#selectPart").selectpicker("refresh");
    $("#selectPartTo").selectpicker("refresh");

    if (!($("#selectKanban").val()) && !($("#selectKanbanTo").val())) {
        await GetKanban();
    }
    if (!($("#selectStore").val()) && !($("#selectStoreTo").val())) {
        await GetStore();
    }
});

//$("#selectDelivery").change(function () {
//    if ($("#selectDeliveryTo").val() != "") {
//        $("#selectDate").parent().find("button").prop("disabled", false);
//    }
//});
//$("#selectDeliveryTo").change(function () {
//    if ($("#selectDelivery").val() != "") {
//        $("#selectDate").parent().find("button").prop("disabled", false);
//    }
//});

$("#btnSearch").click(async function () {
    var obj = {
        supplier: $("#selectSupplier").val(),
        kanban: $("#selectKanban").val(),
        kanbanTo: $("#selectKanbanTo").val(),
        store: $("#selectStore").val(),
        storeTo: $("#selectStoreTo").val(),
        partNo: $("#selectPart").val(),
        partNoTo: $("#selectPartTo").val(),
    }

    if (obj.kanban > obj.kanbanTo) {
        xSwal.error("Error", "Kanban From > Kanban To");
        return;
    }
    if (obj.store > obj.storeTo) {
        xSwal.error("Error", "Store From > Store To");
        return;
    }
    if (obj.partNo > obj.partNoTo) {
        xSwal.error("Error", "Part No From > Part No To");
        return;
    }


    await _xLib.AJAX_Get("/api/KBNMS008/Search", obj,
        function (success) {
            if (success.status == "200") {
                if ($("#selectDelivery").val() != "" && $("#selectDeliveryTo").val() != "") {
                    $("#selectDate").parent().find("button").prop("disabled", false);
                }
            }
        },
        function (error) {
            console.error(error);
            xSwal.error("Error", "Can't Get Data");
        }
    );
});