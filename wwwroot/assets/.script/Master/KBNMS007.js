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
        autoclose: true,
        showRightIcon: false,
    });
    $('#readStartDate').datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
        autoclose: true,
        showRightIcon: false,
    });

    $('#readDeliveryDate').parent().prepend(`<label class= "input-group-text col-2" for="readDeliveryDate">Delivery Date</label>`);
    $('#readDeliveryDate').parent().addClass("mb-0");

    $('#readStartDate').parent().prepend(`<label class= "input-group-text col-4" for="readStartDate">Start Date</label>`);
    $('#readStartDate').parent().addClass("mb-0");

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
            xSwal.xError(error);
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
            xSwal.xError(error);
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
            xSwal.xError(error);
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
            xSwal.xError(error);
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
            xSwal.xError(error);
        }
    );

});

$("#readDeliveryDate").change(function () {
    _xLib.AJAX_Get("/api/KBNMS006/GetDeliveryTime",
        {
            F_Supplier_Code: $("#selectSupplier").val(),
            date: moment($("#readDeliveryDate").val(), "DD/MM/YYYY").format("YYYYMMDD")
        },
        function (success) {
            if (success.status == 200) {
                console.log(success.data);
                $("#readCycle").val(success.data);
            }
        },
        function (error) {
            xSwal.xError(error);
        }
    );
});

function SetInputReset() {
    //$("#radAddCycle").parent().prop("disabled", true);
    //$("#radAddTrip").parent().prop("disabled", true);
    $("#radAddCycle").prop("checked", false);
    $("#radAddTrip").prop("checked", false);
    $("#divInfoData").find("input").val("");
    $("#divInfoData").find("input").prop("readonly", true);
    $("#divStatus").find("input").val("");
    $("#divStatus").find("input").prop("readonly", true);

    $("#divAddCycle").find("input").val("");
    $("#divAddCycle").find("input").prop("readonly", true);

    $("#radAddCycle").prop("disabled", true);
    $("#radAddCycle").prop("readonly", false);
    $("#radAddTrip").prop("disabled", true);

    $("#divTrip").find("input").val("");
    $("#divTrip").find("input").prop("readonly", true);
    $('#readDeliveryDate').parent().find('button').prop('disabled', true);
}

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
            xSwal.xError(error);
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
    //$("#readAddress").prop("readonly", false);
    //$("#readDock").prop("readonly", false);

    await SetInputReset();
     
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
            xSwal.xError(error);
        }
    );

    await _xLib.AJAX_Get("/api/KBNMS007/Search", obj,
        async function (success) {
            if (success.status == 200) {
                success = _xLib.JSONparseMixData(success);
                console.log(success)
                if (!success.data) {


                    await $("#divTrip").find("input").val("");
                    await $("#divAddCycle").find("input").val("");
                    await $("#divStatus").find("input").val("");

                    $("#readStatus").val("0 : None");
                    $("#readUpdateDate").val(moment(cookieLoginDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));
                    $("#readUpdateBy").val(_xLib.GetUserName().substring(0, 10));
                    $("#readProcessDate").val(moment(cookieLoginDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));
                    $("#readProcessBy").val(_xLib.GetUserName().substring(0, 10));
                    $("#readDeliveryDate").val(moment(cookieLoginDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));


                    //$("#readStartDate").val("");
                    //$("#readShift").val("");
                    //$("#readAddQty").val("");

                    //$("#inpStartTrip").val("");
                    //$("#inpAddTrip").val("");

                    $("#divBtn").find("button").prop("disabled", true);
                    $("#btnEdit").prop("disabled", false);

                }
                else
                {
                    $("#readStatus").val(success.data.f_Status == "0" ? "0 : None" : success.data.f_Status == "1" ? "1 : Register"
                        : success.data.f_Status == "2" ? "2 : Processing" : "3 : Done"
                    );

                    $("#readUpdateDate").val(moment(success.data.f_Update_Date).format("DD/MM/YYYY"));
                    $("#readUpdateBy").val(success.data.f_Update_By);
                    $("#readProcessDate").val(moment(success.data.f_Create_Date).format("DD/MM/YYYY"));
                    $("#readProcessBy").val(success.data.f_Create_By);
                    $("#readDeliveryDate").val(moment(success.data.f_Delivery_Date, "YYYYMMDD").format("DD/MM/YYYY"));
                    $("#readStartDate").val(moment(success.data.f_Start_Date, "YYYYMMDD").format("DD/MM/YYYY"));
                    $("#readStartShift").val(success.data.f_Start_Shift);
                    $("#readAddQty").val(success.data.f_KB_Add);
                    $("#inpStartTrip").val(success.data.f_Delivery_Trip);
                    $("#inpAddTrip").val(success.data.f_KB_Add_RN);

                    for (let i = 1; i <= 30; i++) {
                        let _id = "inpTrip" + i;
                        $("#" + _id).val(success.data["f_Round" + i] == 0 ? "" : success.data["f_Round" + i]);
                    }

                    $("#divBtn").find("button").prop("disabled", true);

                    if (success.data.f_Status == "0" || success.data.f_Status == "3") {
                        $("#btnStart").prop("disabled", false);
                        $("#btnEdit").prop("disabled", false);
                    }
                    else if (success.data.f_Status == "1" || success.data.f_Status == "2") {
                        $("#btnStop").prop("disabled", false);
                    }

                }

                $("#readAddress").prop("readonly", false);
                $("#readDock").prop("readonly", false);
            }
        },
        function (error) {
            xSwal.xError(error);
        }
    );

    _xLib.AJAX_Get("/api/KBNMS006/GetDeliveryTime",
        {
            F_Supplier_Code: obj.F_Supplier_Code,
            date: moment($("#readDeliveryDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        },
        function (success) {
            if (success.status == 200) {
                console.log(success.data);
                $("#readCycle").val(success.data);
            }
        },
        function (error) {
            xSwal.xError(error);
        }
    );
});

$("#btnEdit").click(function () {
    $("#radAddCycle").prop("disabled", false);
    $("#radAddTrip").prop("disabled", false);

    $("#divBtn").find("button").prop("disabled", true);
    $("#btnCancelBottom").prop("disabled", false);
    $("#btnSave").prop("disabled", false);
    $("#readDeliveryDate").prop("readonly", false);
    //$("#readDeliveryDate").parent().find('button').prop('disabled', false);
    $("#readAddQty").prop("readonly", false);

    _xLib.AJAX_Get("/api/KBNMS006/GetDeliveryTime",
        {
            F_Supplier_Code: obj.F_Supplier_Code,
            date: moment($("#readDeliveryDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        },
        function (success) {
            if (success.status == 200) {
                console.log(success.data);
                $("#readCycle").val(success.data);
            }
        },
        function (error) {
            xSwal.xError(error);
        }
    );
});

$("#radAddCycle").change(function () {
    if($(this).is(":checked")) {
        $("#divTrip").find("input").val("");
        $("#divTrip").find("input").prop("readonly", true);
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

$("#btnSave").click(async function () {


    let obj = await GetKanbanAddObj();

    //console.log(obj);


    if (await xSwal.confirm("Confirm", "Do you sure to save data?")) {
        _xLib.AJAX_Post("/api/KBNMS007/Save", JSON.stringify(obj),
            function (success) {
                if (success.status == 200) {
                    xSwal.success(success.title, success.message);
                    $("#btnSearch").trigger("click");
                }
            },
            function (error) {
                xSwal.error("Error", error.responseJSON.message);
            }
        );
    }
});

$("#btnStart").click(async function () {

    let obj = await GetKanbanAddObj();

    if (await xSwal.confirm("Confirm", "Do you sure to start?")) {
        _xLib.AJAX_Post("/api/KBNMS007/Start", JSON.stringify(obj),
            function (success) {
                if (success.status == 200) {
                    xSwal.success(success.title, success.message);
                    $("#btnSearch").trigger("click");
                }
            },
            function (error) {
                xSwal.error("Error", error.responseJSON.message);
            }
        );
    }

});

$("#btnStop").click(async function () {

    let obj = await GetKanbanAddObj();

    if (await xSwal.confirm("Confirm", "Do you sure to stop?")) {
        _xLib.AJAX_Post("/api/KBNMS007/Stop", JSON.stringify(obj),
            function (success) {
                if (success.status == 200) {
                    xSwal.success(success.title, success.message);
                    $("#btnSearch").trigger("click");
                }
            },
            function (error) {
                xSwal.error("Error", error.responseJSON.message);
            }
        );
    }

});

$("#btnCancelBottom").click(async function () {

    await SetInputReset();
    $("#btnSearch").trigger("click");

});

function GetKanbanAddObj() {
    let obj = {
        F_Plant: _xLib.GetCookie("plantCode"),
        F_Kanban_No: $("#readKanban").val(),
        F_Supplier_Code: $("#readSupplierCode").val().split("-")[0],
        F_Supplier_Plant: $("#readSupplierCode").val().split("-")[1],
        F_Store_Code: $("#readStore").val(),
        F_Part_No: $("#readPartNo").val().split("-")[0],
        F_Ruibetsu: $("#readPartNo").val().split("-")[1],
        F_Delivery_Date: moment($("#readDeliveryDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        F_Delivery_Trip: $("#inpStartTrip").val(),
        F_Start_Date: "",
        F_Start_Shift: "",
        F_KB_Remain: $("#readAddQty").val() == "" ? 0 : $("#readAddQty").val(),
        F_KB_Add: $("#readAddQty").val() == "" ? 0 : $("#readAddQty").val(),
    }

    if ($("#radAddCycle").is(":checked")) {
        obj.F_KB_Add_RN = $("#inpAddTrip").val();
        obj.F_Delivery_Trip = $("#inpStartTrip").val();

        for (let i = 1; i <= 30; i++) {
            let _objId = "F_Round" + i;
            obj[_objId] = 0;
        }
    }
    else if ($("#radAddTrip").is(":checked")) {
        for (let i = 1; i <= parseInt($("#readCycle").val().substring(3, 5)); i++) {
            let _id = "inpTrip" + i;
            let _objId = "F_Round" + i;
            obj[_objId] = parseInt($("#" + _id).val());
        }
        //for (let i = parseInt($("#readCycle").val().substring(3, 5)) + 1; i <= 30; i++) {
        //    let _objId = "F_Round" + i;
        //    obj[_objId] = 0
        //}
        for (let i = 1; i <= 30; i++) {
            let _objId = "F_Round" + i;
            console.log(_objId);
            console.log(obj[_objId]);
            obj[_objId] = obj[_objId] == "" ? 0 : obj[_objId] == null ? 0 : isNaN(obj[_objId]) ? 0 : obj[_objId];
        }
    }

    return obj;
}
