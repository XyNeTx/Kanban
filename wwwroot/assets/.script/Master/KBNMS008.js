let _cookieLoginDate = _xLib.GetCookie("loginDate");
let _workingTrip = 0;
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
        sorting: false,
        searching: false,
        scrollX: true,
        "columns": [
            { title: "Part No", data: "f_Part_No", },
            { title: "Kanban No", data: "f_Kanban_No", },
            { title: "Last Order Diff(Pcs)", data: "f_Last_Order", },
            { title: "Forecast(Pcs)", data: "f_FC_Max", },
            { title: "Trip 1", data: "f_Trip1" },
            { title: "Trip 2", data: "f_Trip2" },
            { title: "Qty/Pack", data: "f_Qty", },
            { title: "Total(KB)", data: "f_Total_KB", },
            { title: "Total(Pcs)", data: "f_Total_Pcs", },
            { title: "Order Diff(Pcs)", data: "f_Order_Diff", },
        ],
        order: [[1, "asc"]]

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
    });

    $("#selectDate").datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
        todayHighlight: true,
        autoclose: true,
        minDate: function () {
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
});


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
                //console.log(success.data);
                $("#readSupplier").val(obj.supplier);
                $("#readSupplierName").val(success.data.f_Supplier_Name);
                $("#readSafetyStock").val(success.data.f_Safety_Stk);
                $("#readCycle").val(success.cycle.slice(0, 2) + "-" + success.cycle.slice(2, 4) + "-" + success.cycle.slice(4, 6));
                await GetTrip();
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

function GetTrip() {
    let cycle = $("#readCycle").val().substring(3,5);
    let columnTrip = [];

    for (let i = 1; i <= parseInt(cycle); i++) {
        columnTrip.push(
            {
                title: "Trip " + i, data: "f_Trip" + i, width: '10%', id : "f_Trip" + i
            }
        );
    }

    $("#tableMain").DataTable().clear().destroy();
    $("#tableMain").empty();
    $("#tableMain").DataTable({
        "processing": false,
        "serverSide": false,
        width: '100%',
        paging: false,
        scrollX: true,
        sorting: false,
        orderable: false,
        searching: false,
        "columns": [
            {
                title: "Part No", data: function (data) {
                    //console.log(data);
                    return `<td>${data.f_Part_No}-${data.f_Ruibetsu}</td>`;
                }
                /*title : "Part No", data : "f_Part_No"*/
            },
            { title: "Kanban No", data: "f_Kanban_No" },
            { title: "Last Order Diff(Pcs)", data: "f_Last_Order" },
            { title: "Forecast(Pcs)", data: "f_FC_Max" },
            ...columnTrip,
            { title: "Qty/Pack", data: "f_Qty" },
            { title: "Total(KB)", data: "f_Total_KB" },
            { title: "Total(Pcs)", data: "f_Total_Pcs" },
            { title: "Order Diff(Pcs)", data: "f_Order_Diff" },
        ],

    });

    $("#tableMain").DataTable().columns.adjust().draw();

    

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

$("#selectDelivery").change(function () {

    let thisValue = moment($(this).val(), "DD/MM/YYYY").format("YYYYMMDD");
    let thisValueTo = moment($("#selectDeliveryTo").val(), "DD/MM/YYYY").format("YYYYMMDD");

    if(thisValue > thisValueTo) {
        $("#selectDeliveryTo").val($(this).val());
    }

});
$("#selectDeliveryTo").change(function () {
    let thisValue = moment($(this).val(), "DD/MM/YYYY").format("YYYYMMDD");
    let thisValueFrom = moment($("#selectDelivery").val(), "DD/MM/YYYY").format("YYYYMMDD");

    if(thisValue < thisValueFrom) {
        $("#selectDelivery").val($(this).val());
    }
});

$("#selectDate").change(function () {
    if ($("#selectDate").val() == "") {
        return;
    }

    let cycle = $("#readCycle").val().split("-")[2];

    let Obj = {
        F_Plant: "",
        F_Supplier_Code: $("#selectSupplier").val().split("-")[0],
        F_Supplier_Plant: $("#selectSupplier").val().split("-")[1],
        F_Delivery_Date: moment($("#selectDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        F_Store_Code: "",
        F_Kanban_No: "",
        F_Part_No: "",
        F_Ruibetsu: "",
        F_Cycle: $("#readCycle").val().replaceAll("-", ""),
    }

    for (let i = 1; i <= 32; i++) {
        if(i <= cycle){
            Obj["F_Trip" + i] = 0;
        }
        else{
            Obj["F_Trip" + i] = null;
        }
    }

    _xLib.AJAX_Post("/api/KBNMS008/SelectDateChange", JSON.stringify(Obj),
        async function (success) {
            if (success.status == "200") {
                success = _xLib.JSONparseMixData(success);
                //console.log(success.data);
                //console.log(success.data[0].F_Work);
                if (success.data.periodDay != 0) {
                    _workingTrip = success.data.periodDay;
                }
                else if (success.data.periodNight != 0) {
                    _workingTrip = success.data.periodNight;
                }
                await ShowData();
            }
        },
        function (error) {
            console.error(error);
            xSwal.error("Error", "Can't Get Data");
        }
    );

});

function ShowData() {
    var obj = {
        supplier: $("#selectSupplier").val(),
        kanban: $("#selectKanban").val(),
        kanbanTo: $("#selectKanbanTo").val(),
        store: $("#selectStore").val(),
        storeTo: $("#selectStoreTo").val(),
        partNo: $("#selectPart").val(),
        partNoTo: $("#selectPartTo").val(),
        selDate : moment($("#selectDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
    }

    if (obj.selDate == "") {
        xSwal.error("Error", "Please Select Date");
        return;
    }

    _xLib.AJAX_Get("/api/KBNMS008/Show_Data", obj,
    function (success) {
            if (success.status == "200") {
                //console.log(success.data);
                $("#tableMain").DataTable().clear();
                $("#tableMain").DataTable().columns.adjust().draw();

                success.data.forEach(function (data) {
                    let sum = 0;
                    for (let obj in data) {
                        if (obj.includes("f_Trip")) {
                            sum += parseInt(data[obj]);
                        }
                    }
                    data["f_Total_KB"] = sum;
                    data["f_Total_Pcs"] = sum * data.f_Qty;

                    console.log(data);
                });
                $("#tableMain").DataTable().rows.add(success.data).draw();

            }
        },
        function (error) {
            console.error(error);
            xSwal.error("Error", "Can't Get Data");
        }
    );
}

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
                    $("#selectDate").val($("#selectDelivery").val());
                }
                //console.log(success.data);
            }
        },
        function (error) {
            console.error(error);
            xSwal.error("Error", "Can't Get Data");
        }
    );
});

$(document).on("click", "#tableMain tbody tr td", function (e) {
    //console.log(e);
    if (e.target.tagName == "INPUT") { return; }

    var index = $(this).index();
    var header = $("#tableMain thead tr th").eq(index).text();

    if (header.includes("Trip")) {
        let text = $(this).text();
        //console.log(_workingTrip);
        if (_workingTrip == 0 || header.split(" ")[1] <= _workingTrip) {
            $(this).empty();
            $(this).append(`<input type="number" min="0" class="inputTrip form-control" value="${text}" />`);
            $(this).find("input").focus();
        }
    }
});

$(document).on("keydown blur", "#tableMain .inputTrip", function (e) {
    var key = e.keyCode || e.which;
    var trip = $(this).val();
    console.log(key);

    if (key === 13 || key === 0) {
        if (trip < 0) {
            $(this).parent().empty();
            $("#tableMain").DataTable().draw();
            return xSwal.error("Error", "Trip Must Be More Than 0");
            
        }
        var cell = $(this).parent();
        $(this).parent().empty();
        var header = $("#tableMain thead tr th").eq(cell.index()).text();
        var data = $("#tableMain").DataTable().row(cell.closest("tr")).data();
        header = header.split(" ")[1];
        data["f_Trip" + header] = trip;
        let sum = 0;
        for (let obj in data) {
            if (obj.includes("f_Trip")) {
                sum += parseInt(data[obj]);
            }
        }
        data["f_Total_KB"] = sum;
        data["f_Total_Pcs"] = sum * data.f_Qty;

        console.log(data);
        $("#tableMain").DataTable().row(cell.closest("tr")).data(data).draw();
    }

});

$("#btnConfirm").click(function () {

    let data = $("#tableMain").DataTable().data().toArray();
    data.forEach(function (item) {
        item.F_Cycle = $("#readCycle").val().replaceAll("-", "");
    });

    _xLib.AJAX_Post("/api/KBNMS008/UpToList", JSON.stringify(data),
        function (success) {
            if (success.status == "200") {
                $("#tableMain").DataTable().clear().draw();
                $("#selectKanban").selectpicker("val", "");
                $("#selectKanbanTo").selectpicker("val", "");
                $("#selectStore").selectpicker("val", "");
                $("#selectStoreTo").selectpicker("val", "");
                $("#selectPart").selectpicker("val", "");
                $("#selectPartTo").selectpicker("val", "");

                $("#selectDelivery").val(moment(_cookieLoginDate, "YYYYMMDD").format("DD/MM/YYYY"));
                $("#selectDeliveryTo").val(moment(_cookieLoginDate, "YYYYMMDD").format("DD/MM/YYYY"));
                $("#selectDate").val("");

                xSwal.success("Success", "Confirm Data Complete");
            }
        },
        function (error) {
            console.error(error);
            xSwal.error("Error", error.responseJSON.message);
        }
    );

});

let _file = null;

$("#inpFile").change(function () {
    _file = this.files[0];
});

$("#btnImport").click(async function () {
    try {

        $("#btnImport").prop("disabled", true);

        if (!_file) return xSwal.error("Import File Error", "No file selected");
        //console.log('File being processed:', file);

        const arrayBuffer = await _file.arrayBuffer();
        const read = await XLSX.read(arrayBuffer);

        const _json = XLSX.utils.sheet_to_json(read.Sheets[read.SheetNames[0]]);

        _json.forEach(function (row) {
            for (var key in row) {
                if (key.includes("T")) {
                    row["f_Trip" + key.slice(1)] = row[key];
                    delete row[key];
                }
            }

            row["f_Supplier_Code"] = row["Supplier_Code"].split("-")[0];
            row["f_Supplier_Plant"] = row["Supplier_Code"].split("-")[1];
            row["f_Part_No"] = row["Part_No"].toString().slice(0, 10);
            row["f_Ruibetsu"] = row["Part_No"].toString().slice(10, 12);
            row["f_Kanban_No"] = row["Kanban_No"];
            row["f_Delivery_Date"] = row["Delivery_Date"].toString();
            row["f_Plant"] = _xLib.GetCookie("plantCode");
            row["f_Store_Code"] = "00";
            row["f_Last_Order"] = null;
            row["f_FC_Max"] = null;
            row["f_Qty"] = null;
            row["f_Total_KB"] = null;
            row["f_Total_Pcs"] = null;
            row["f_Order_Diff"] = null;

            delete row["Supplier_Code"];
            delete row["Part_No"];
            delete row["Kanban_No"];
            delete row["Delivery_Date"];
        });

        $("#selectSupplier").selectpicker("val", _json[0].f_Supplier_Code + "-" + _json[0].f_Supplier_Plant);

        var obj = {
            supplier: $("#selectSupplier").val()
        }

        await _xLib.AJAX_Get("/api/KBNMS008/GetSupplierDetail", obj,
            async function (success) {
                if (success.status == "200") {
                    //console.log(success.data);
                    $("#readSupplier").val(obj.supplier);
                    $("#readSupplierName").val(success.data.f_Supplier_Name);
                    $("#readSafetyStock").val(success.data.f_Safety_Stk);
                    $("#readCycle").val(success.cycle.slice(0, 2) + "-" + success.cycle.slice(2, 4) + "-" + success.cycle.slice(4, 6));
                    await GetTrip();
                    //$("#tableMain").DataTable().clear().rows.add(_json).draw();
                }
            },
            function (error) {
                console.error(error);
                xSwal.error("Error", "Can't Get Supplier Detail");
            }
        );

        await _xLib.AJAX_Post("/api/KBNMS008/GetImportList", JSON.stringify(_json),
            async function (success) {
                if (success.status == "200") {

                    for (let i = 0; i < _json.length; i++) {
                        //console.log(_json[i]);
                        _json[i]["f_Cycle"] = $("#readCycle").val().replaceAll("-", "");
                        await _xLib.AJAX_Post("/api/KBNMS008/SelectDateChange?isImport=true", JSON.stringify(_json[i]),
                            async function (success) {
                                if (success.status == "200") {
                                    success = _xLib.JSONparseMixData(success);
                                    //console.log(success.data);
                                    //console.log(success.data[0].F_Work);
                                    if (success.data.periodDay != 0 || success.data.periodDay != "") {
                                        _workingTrip = success.data.periodDay;
                                    }
                                    else if (success.data.periodNight != 0 || success.data.periodNight != "") {
                                        _workingTrip = success.data.periodNight;
                                    }
                                    //console.log(_json[0].f_Store_Code);
                                    //console.log(_json[_json.length - 1].f_Store_Code);
                                    //await $("#selectStore").selectpicker("val", _json[0].f_Store_Code);
                                    //await $("#selectStoreTo").selectpicker("val", _json[_json.length - 1].f_Store_Code);
                                }
                            },
                            function (error) {
                                console.error(error);
                                xSwal.error("Error", "Can't Get Data");
                            }
                        );
                    }

                    await _xLib.AJAX_Post("/api/KBNMS008/UploadImportToKanbanPlan", "",
                        function (success) {
                            if (success.status == "200") {
                                xSwal.success("Success", "Import Data Complete");
                                $("#btnImport").prop("disabled", false);
                            }
                        },
                        function (error) {
                            console.error(error);
                            xSwal.error("Error", "Can't Import Data");
                        }
                    );
                    //await ShowData();
                }
            },
            function (error) {
                console.error(error);
                xSwal.error("Error", "Can't Get Data");
            }
        );
    }
    catch (error) {
        console.error(error);
        xSwal.error("Error", "Can't Import Data");
    }
    finally {
        () => {
            $("#btnImport").prop("disabled", false);
        };
    }

});