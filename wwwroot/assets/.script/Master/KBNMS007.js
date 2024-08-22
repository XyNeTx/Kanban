var cookieLoginDate = _xLib.GetCookie("loginDate");
$(document).ready(async function () {

    //await SetReadOnly();
    await GetSupplier();
    await GetKanban();
    await GetStore();
    await GetPartNo();

    await $(".SelectPicker").selectpicker();
    $("#divBtn").find("button").prop("disabled", true);

    $('#readDeliveryDate').datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
    });

    $('#readDeliveryDate').parent().prepend(`<label class="input-group-text col-2" for="readDeliveryDate">Delivery Date</label>`);
    $('#readDeliveryDate').parent().find('button').prop('disabled', true);

    xSplash.hide();
});

async function GetSupplier() {

    await _xLib.AJAX_Get("/api/KBNMS006/GetSupplier", '',
        function (success) {
            if (success.status == 200) {
                $("#selectSupplier").empty();
                $("#selectSupplier").append('<option value=""hidden></option>');
                success.data.forEach(function (item) {
                    $("#selectSupplier").append('<option value="' + item.f_Supplier_Code + '">' + item.f_Supplier_Code + '</option>');
                });
            }
        },
        function (error) {
            xSwal.error("Error", "Supplier Not Found");
        }
    );


}

async function GetPartNo() {
    //console.log($("#selectPartNo").val());
    //console.log($("#selectStore").val());

    if ($("#selectPartNo").val()) {
        return;
    }

    var F_Store_Code = { F_Store_Code: $("#selectStore").val() == null ? "" : $("#selectStore").val() }

    await _xLib.AJAX_Get("/api/KBNMS006/GetPartNo", F_Store_Code,
        function (success) {
            if (success.status == 200) {
                $("#selectPartNo").empty();
                $("#selectPartNo").append('<option value=""hidden></option>');
                success.data.forEach(function (item) {
                    $("#selectPartNo").append('<option value="' + item.f_Part_No + '">' + item.f_Part_No + '</option>');
                });
            }
        },
        function (error) {
            xSwal.error("Error", "Part No Not Found");
        }
    );
}

async function GetKanban(F_Supplier_Code) {

    await _xLib.AJAX_Get("/api/KBNMS006/GetKanban", F_Supplier_Code,
        function (success) {
            if (success.status == 200) {
                $("#selectKanban").empty();
                $("#selectKanban").append('<option value=""hidden></option>');
                success.data.forEach(function (item) {
                    $("#selectKanban").append('<option value="' + item + '">' + item + '</option>');
                });
            }
        },
        function (error) {
            xSwal.error("Error", "Kanban No Not Found");
        }
    );

}

async function GetStore() {
    //console.log($("#selectPartNo").val());
    //console.log($("#selectStore").val());

    if ($("#selectStore").val()) {
        return;
    }

    var F_Part_No = { F_Part_No: $("#selectPartNo").val() == null ? "" : $("#selectPartNo").val() }

    await _xLib.AJAX_Get("/api/KBNMS006/GetStore", F_Part_No,
        function (success) {
            if (success.status == 200) {
                $("#selectStore").empty();
                $("#selectStore").append('<option value=""hidden></option>');
                success.data.forEach(function (item) {
                    $("#selectStore").append('<option value="' + item + '">' + item + '</option>');
                });
            }
        },
        function (error) {
            xSwal.error("Error", "Store Code Not Found");
        }
    );
}

$("#selectSupplier").change(async function () {
    var F_Supplier_Code = { F_Supplier_Code: $("#selectSupplier").val() == null ? "" : $("#selectSupplier").val() }
    var F_Store_Code = { F_Store_Code: $("#selectStore").val() == null ? "" : $("#selectStore").val() }

    await GetKanban(F_Supplier_Code);
    await $("#selectKanban").selectpicker("refresh");

    //get supplier detail
    var obj = {
        F_Supplier_Code: $("#selectSupplier").val(),
        F_Store_Code: $("#selectStore").val() == null ? "" : $("#selectStore").val()
    }
    await _xLib.AJAX_Get("/api/KBNMS006/GetSupplierDetail", obj,
        function (success) {
            if (success.status == 200) {
                var data = success.data;
                $("#readSupplierCode").val($("#selectSupplier").val());
                $("#readSupplierName").val(data.f_Supplier_Name);
                $("#readCycle").val(data.f_Cycle);
            }
        },
        function (error) {
            xSwal.error("Error", "Supplier Detail Not Found");
        }
    );

});


$("#selectPartNo").change(async function () {
    await GetStore();
    await $("#selectStore").selectpicker("refresh");
});


$("#selectStore").change(async function () {
    await GetPartNo();
    await $("#selectPartNo").selectpicker("refresh");
});

$("#selectKanban").change(async function () {
    var obj = {
        F_Kanban_No: $("#selectKanban").val(),
        F_Supplier_Code: $("#selectSupplier").val(),
        F_Store_Code: $("#selectStore").val(),
        F_Part_No: $("#selectPartNo").val()
    }

    _xLib.AJAX_Get("/api/KBNMS006/GetKanbanDetail", obj,
        function (success) {
            if (success.status == 200) {
                $("#readKanban").val($("#selectKanban").val());
                $("#readBoxQty").val(success.data[0].f_qty_box);
            }
        },
        function (error) {
            xSwal.error("Error", "Kanban Detail Not Found");
        }
    );
});

$("#btnSearch").click(async function () {
    var obj = {
        F_Kanban_No: $("#selectKanban").val(),
        F_Supplier_Code: $("#selectSupplier").val(),
        F_Store_Code: $("#selectStore").val(),
        F_Part_No: $("#selectPartNo").val()
    }

    //console.log($("#boxDeliDate").val())
    $("#readAddress").prop("readonly", false);
    $("#readDock").prop("readonly", false);

    await _xLib.AJAX_Get("/api/KBNMS006/Search", obj,
        function (success) {
            if (success.status == 200) {
                success = _xLib.JSONparseMixData(success);
                console.log(success)
                $("#readKanban").val(success.data.allData[0].F_Kanban);
                $("#readBoxQty").val(success.data.allData[0].F_qty_box);
                $("#readSupplierCode").val(success.data.allData[0].F_supplier_cd);
                $("#readSupplierName").val(success.data.allData[0].F_name);
                $("#readCycle").val(success.data.allData[0].F_cycle);
                $("#readStore").val(success.data.allData[0].F_Store_cd);
                $("#readPartNo").val(success.data.allData[0].F_Part_no);
                $("#readPartName").val(success.data.allData[0].F_Part_nm);
                $("#readAddress").val(success.data.master.f_Address);
                $("#readDock").val(success.data.master.f_Supply_Code);
            }
        },
        function (error) {
            xSwal.error("Error", "Data Not Found");
        }
    );

    await _xLib.AJAX_Get("/api/KBNMS007/Search", obj,
        function (success) {
            if (success.status == 200) {
                success = _xLib.JSONparseMixData(success);
                console.log(success)
                if (!success.data) {
                    $("#divBtn").find("button").prop("disabled", true);
                    $("#btnEdit").prop("disabled", false);

                    $("#readStatus").val("0 : None");
                    $("#readUpdateDate").val(moment(cookieLoginDate.slice(0, 10),"YYYY-MM-DD").format("DD/MM/YYYY"));
                    $("#readUpdateBy").val(_xLib.GetUserName().substring(0, 10));
                    $("#readProcessDate").val(moment(cookieLoginDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));
                    $("#readProcessBy").val(_xLib.GetUserName().substring(0, 10));
                    $("#readDeliveryDate").val(moment(cookieLoginDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));
                    
                    $("#divTrip").find("input").val("");
                    $("#divAddCycle").find("input").val("");


                }
            }
        },
        function (error) {
            xSwal.error("Error", "Data Not Found");
        }
    );
});

$("#btnEdit").click(function () {
    $("#radAddCycle").prop("disabled", false);
    $("#radAddTrip").prop("disabled", false);

    $("#divBtn").find("button").prop("disabled", true);
    $("#btnCancelBottom").prop("disabled", false);
    $("#btnSave").prop("disabled", false);
    $("#readDeliveryDate").parent().find('button').prop('disabled', false);
    $("#readAddQty").prop("readonly", false);
});

$("#radAddCycle").change(function () {
    if($(this).is(":checked")) {
        $("#divTrip").find("input").prop("readonly", true);
        $("#divTrip").find("input").val("");
        $("#radAddTrip").prop("readonly", false);

        $("#divAddCycle").find("input").prop("readonly", false);
    }
});

$("#radAddTrip").change(function () {
    if ($(this).is(":checked")) {
        $("#divAddCycle").find("input").val("");
        $("#divAddCycle").find("input").prop("readonly", true);
        $("#radAddCycle").prop("readonly", false);

        //$("#divTrip").find("input").prop("readonly", false);
        let cycleB = parseInt($("#readCycle").val().substring(3, 5));

        for (let i = 1; i <= cycleB; i++) {
            let _id = "inpTrip" + i;
            $("#" + _id).prop("readonly", false);
        }
    }
});

$("#btnCancel").click(function () {
    window.location.reload();
});

$("#btnExit").click(function () {
    window.location.href = "/Home/Index";
});

$("#btnSaveKB").click(async function () {

    var isConfirm = await xSwal.confirm("Confirm", "Do you sure to save Kanban Master data?");

    if (!isConfirm) {
        return;
    }

    var TB_MS_Kanban = {
        F_Kanban_No: $("#readKanban").val(),
        F_Supplier_Code: $("#readSupplierCode").val().split("-")[0],
        F_Supplier_Plant: $("#readSupplierCode").val().split("-")[1],
        F_Store_Code: $("#readStore").val(),
        F_Part_No: $("#readPartNo").val().split("-")[0],
        F_Ruibetsu: $("#readPartNo").val().split("-")[1],
        F_Part_Name: $("#readPartName").val(),
        F_Box_Qty: $("#readBoxQty").val(),
        F_Supplier_Name: $("#readSupplierName").val(),
        F_Cycle: $("#readCycle").val().replaceAll("-", ""),
        F_Address: $("#readAddress").val(),
        F_Supply_Code: $("#readDock").val(),
        F_Plant: _xLib.GetCookie("plantCode"),
    }

    //console.log(TB_MS_Kanban)

    _xLib.AJAX_Post("/api/KBNMS006/SaveKanban", JSON.stringify(TB_MS_Kanban),
        function (success) {
            if (success.status == 200) {
                xSwal.success(success.title, success.message);
            }
        },
        function (error) {
            xSwal.error("Error", error.responseJSON.message);
        }
    );

});
