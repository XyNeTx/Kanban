var cookieLoginDate = _xLib.GetCookie("loginDate");
var command = "";
$(document).ready(async function () {

    await SetReadOnly();
    await GetSupplier();
    await GetKanban();
    await GetStore();
    await GetPartNo();

    await $(".SelectPicker").selectpicker();

    await prependDateLabel();
    
    xSplash.hide();
})

function prependDateLabel() {

    $('#stopDeliDate').datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
    });
    $('#stopStartDate').datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
    });
    $('#stopUpdateDate').datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
    });
    $('#stopProcessDate ').datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
    });

    $("#stopDeliDate").parent().prepend(`<label class="input-group-text col-5" for="boxDeliDate">Delivery Date</label>`);
    $("#stopStartDate").parent().prepend(`<label class="input-group-text col-5" for="boxUpdateDate">Start Date</label>`);
    $("#stopUpdateDate").parent().prepend(`<label class="input-group-text col-5" for="boxUpdateDate">Update Date</label>`);
    $("#stopProcessDate").parent().prepend(`<label class="input-group-text col-5" for="boxProcessDate">Process Date</label>`);

    //$("#stopDeliDate").parent().find("button").prop("disabled", true);
    //$("#stopStartDate").parent().find("button").prop("disabled", true);
    //$("#stopUpdateDate").parent().find("button").prop("disabled", true);
    //$("#stopProcessDate").parent().find("button").prop("disabled", true);

    $("#divKBStop").find("button").prop("disabled", true);

    $('#cutDeliDate').datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
    });
    $('#cutStartDate').datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
    });
    $('#cutUpdateDate').datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
    });
    $('#cutProcessDate ').datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
    });

    $("#cutDeliDate").parent().prepend(`<label class="input-group-text col-5" for="boxDeliDate">Delivery Date</label>`);
    $("#cutStartDate").parent().prepend(`<label class="input-group-text col-5" for="boxUpdateDate">Start Date</label>`);
    $("#cutUpdateDate").parent().prepend(`<label class="input-group-text col-5" for="boxUpdateDate">Update Date</label>`);
    $("#cutProcessDate").parent().prepend(`<label class="input-group-text col-5" for="boxProcessDate">Process Date</label>`);

    $("#divKBCut").find("button").prop("disabled", true);

    $('#boxDeliDate').datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
    });
    $('#boxStartDate').datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
    });
    $('#boxUpdateDate').datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
    });
    $('#boxProcessDate ').datepicker({
        uiLibrary: 'bootstrap5',
        format: 'dd/mm/yyyy',
    });

    $("#boxDeliDate").parent().prepend(`<label class="input-group-text col-5" for="boxDeliDate">Delivery Date</label>`);
    $("#boxStartDate").parent().prepend(`<label class="input-group-text col-5" for="boxUpdateDate">Start Date</label>`);
    $("#boxUpdateDate").parent().prepend(`<label class="input-group-text col-5" for="boxUpdateDate">Update Date</label>`);
    $("#boxProcessDate").parent().prepend(`<label class="input-group-text col-5" for="boxProcessDate">Process Date</label>`);

    $("#divBoxChg").find("button").prop("disabled", true);
}

function SetReadOnly() {
    $("#divKBStop").find("input").prop("readonly", true);
    $("#divKBCut").find("input").prop("readonly", true);
    $("#divBoxChg").find("input").prop("readonly", true);
    $("#divReady").find("input").prop("readonly", true);
}

async function GetSupplier() {

    await _xLib.AJAX_Get("/api/KBNMS006/GetSupplier", '',
        function (success) {
            if (success.status == 200) {
                $("#selectSupplier").empty();
                $("#selectSupplier").append('<option value=""hidden></option>');
                success.data.forEach(function (item) {
                    $("#selectSupplier").append('<option value="' + item.f_Supplier_Code + '">' + item.f_Supplier_Code +'</option>');
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
        F_Part_No : $("#selectPartNo").val()
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

    _xLib.AJAX_Get("/api/KBNMS006/Search", obj,
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

                if (success.data.chgQty == null) {
                    $("#divBoxChg").find("input").val("");
                    $("#divBoxChg").find("input").prop("readonly", true);
                    $("#divBoxChg").find("button").prop("disabled", true);

                    $("#boxBtnEdit").prop("disabled", false);

                    $("#boxStatus").val("0 : None");
                    $("#boxDeliDate").val(_xLib.LoginDateDD());
                    $("#boxStartDate").val(_xLib.LoginDateDD());
                    $("#boxShift").val(cookieLoginDate.slice(-1));
                    $("#boxUpdateBy").val(_xLib.GetUserName().substring(0, 10))
                    $("#boxUpdateDate").val(_xLib.LoginDateDD());
                    $("#boxProcessBy").val(_xLib.GetUserName().substring(0, 10))
                    $("#boxProcessDate").val(_xLib.LoginDateDD());

                }
                else
                {
                    $("#divBoxChg").find("input").val("");
                    $("#divBoxChg").find("input").prop("readonly", true);
                    $("#divBoxChg").find("button").prop("disabled", true);

                    if (success.data.chgQty.f_Status == "0" || success.data.chgQty.f_Status == "3") {
                        $("#boxBtnEdit").prop("disabled", false);
                        $("#boxBtnStart").prop("disabled", false);
                    }
                    else {
                        $("#boxBtnStop").prop("disabled", false);
                    }

                    $("#boxStatus").val(
                        success.data.chgQty.f_Status == 0 ? "0 : None" :
                            success.data.chgQty.f_Status == 1 ? "1 : Register" :
                                success.data.chgQty.f_Status == 2 ? "2 : Processing" :
                                    "3 : Done"
                    );

                    $("#boxDeliDate").val(moment(success.data.chgQty.f_Delivery_Date, "YYYYMMDD").format("DD/MM/YYYY"));
                    $("#boxStartDate").val(moment(success.data.chgQty.f_Start_Date, "YYYYMMDD").format("DD/MM/YYYY"));
                    $("#boxShift").val(success.data.chgQty.f_Start_Shift);
                    $("#boxUpdateBy").val(success.data.chgQty.f_Update_By);
                    $("#boxUpdateDate").val(moment(success.data.chgQty.f_Update_Date).format("DD/MM/YYYY"));
                    $("#boxProcessBy").val(success.data.chgQty.f_Create_By);
                    $("#boxProcessDate").val(moment(success.data.chgQty.f_Create_Date).format("DD/MM/YYYY"));
                    $("#boxTrip").val(success.data.chgQty.f_Delivery_Trip);
                    $("#boxQty").val(success.data.chgQty.f_New_Qty);

                }

                //console.log(success.data.stop)

                if (success.data.stop == null) {
                    $("#divKBStop").find("input").val("");
                    $("#divKBStop").find("input").prop("readonly", true);
                    $("#divKBStop").find("button").prop("disabled", true);

                    $("#stopBtnEdit").prop("disabled", false);

                    $("#stopStatus").val("0 : None");
                    $("#stopDeliDate").val(_xLib.LoginDateDD());
                    $("#stopStartDate").val(_xLib.LoginDateDD());
                    $("#stopShift").val(cookieLoginDate.slice(-1));
                    $("#stopUpdateBy").val(_xLib.GetUserName().substring(0, 10))
                    $("#stopUpdateDate").val(_xLib.LoginDateDD());
                    $("#stopProcessBy").val(_xLib.GetUserName().substring(0, 10))
                    $("#stopProcessDate").val(_xLib.LoginDateDD());
                }
                else {
                    $("#divKBStop").find("input").val("");
                    $("#divKBStop").find("input").prop("readonly", true);
                    $("#divKBStop").find("button").prop("disabled", true);

                    if (success.data.stop.f_Status == "0" || success.data.stop.f_Status == "3")
                    {
                        $("#stopBtnEdit").prop("disabled", false);
                        $("#stopBtnStart").prop("disabled", false);
                    }
                    else {
                        $("#stopBtnStop").prop("disabled", false);
                    }

                    $("#stopStatus").val(
                        success.data.stop.f_Status == 0 ? "0 : None" :
                            success.data.stop.f_Status == 1 ? "1 : Register" :
                                success.data.stop.f_Status == 2 ? "2 : Processing" :
                                    "3 : Done"
                    );

                    $("#stopDeliDate").val(moment(success.data.stop.f_Delivery_Date, "YYYYMMDD").format("DD/MM/YYYY"));
                    $("#stopStartDate").val(moment(success.data.stop.f_Start_Date, "YYYYMMDD").format("DD/MM/YYYY"));
                    $("#stopShift").val(success.data.stop.f_Start_Shift);
                    $("#stopUpdateBy").val(success.data.stop.f_Update_By);
                    $("#stopUpdateDate").val(moment(success.data.stop.f_Update_Date).format("DD/MM/YYYY"));
                    $("#stopProcessBy").val(success.data.stop.f_Create_By);
                    $("#stopProcessDate").val(moment(success.data.stop.f_Create_Date).format("DD/MM/YYYY"));
                    $("#stopTrip").val(success.data.stop.f_Delivery_Trip);

                }

                if (success.data.cut == null) {
                    $("#divKBCut").find("input").val("");
                    $("#divKBCut").find("input").prop("readonly", true);
                    $("#divKBCut").find("button").prop("disabled", true);

                    $("#cutBtnEdit").prop("disabled", false);

                    $("#cutStatus").val("0 : None");
                    $("#cutDeliDate").val(_xLib.LoginDateDD());
                    $("#cutStartDate").val(_xLib.LoginDateDD());
                    $("#cutShift").val(cookieLoginDate.slice(-1));
                    $("#cutUpdateBy").val(_xLib.GetUserName().substring(0, 10))
                    $("#cutUpdateDate").val(_xLib.LoginDateDD());
                    $("#cutProcessBy").val(_xLib.GetUserName().substring(0, 10))
                    $("#cutProcessDate").val(_xLib.LoginDateDD());
                }

                else {
                    $("#divKBCut").find("input").val("");
                    $("#divKBCut").find("input").prop("readonly", true);
                    $("#divKBCut").find("button").prop("disabled", true);

                    if (success.data.cut.f_Status == "0" || success.data.cut.f_Status == "3") {
                        $("#cutBtnEdit").prop("disabled", false);
                        $("#cutBtnStart").prop("disabled", false);
                    }
                    else {
                        $("#cutBtnStop").prop("disabled", false);
                    }

                    $("#cutStatus").val(
                        success.data.cut.f_Status == 0 ? "0 : None" :
                            success.data.cut.f_Status == 1 ? "1 : Register" :
                                success.data.cut.f_Status == 2 ? "2 : Processing" :
                                    "3 : Done"
                    );

                    $("#cutDeliDate").val(moment(success.data.cut.f_Delivery_Date, "YYYYMMDD").format("DD/MM/YYYY"));
                    $("#cutStartDate").val(moment(success.data.cut.f_Start_Date, "YYYYMMDD").format("DD/MM/YYYY"));
                    $("#cutShift").val(success.data.cut.f_Start_Shift);
                    $("#cutUpdateBy").val(success.data.cut.f_Update_By);
                    $("#cutUpdateDate").val(moment(success.data.cut.f_Update_Date).format("DD/MM/YYYY"));
                    $("#cutProcessBy").val(success.data.cut.f_Create_By);
                    $("#cutProcessDate").val(moment(success.data.cut.f_Create_Date).format("DD/MM/YYYY"));
                    $("#cutTrip").val(success.data.cut.f_Delivery_Trip);
                    $("#cutKB").val(success.data.cut.f_KB_Cut);
                    $("#cutKBCycle").val(success.data.cut.f_KB_Cut_RN);

                }

                if (success.data.master == null) {
                    $("#readAddress").val("");
                    $("#readDock").val("");
                }
                else {
                    $("#readAddress").val(success.data.master.f_Address);
                    $("#readDock").val(success.data.master.f_Supply_Code);
                }
            }
        },
        function (error) {
            xSwal.error("Error", error.responseJSON.message);
        }
    );
});

$("#btnCancel").click(function () {
    window.location.reload();
});

$("#btnExit").click(function () {
    window.location.href = "/Home/Index";
});


$("#boxBtnEdit").click(function () {

    $("#boxTrip").prop("readonly", false);
    $("#boxQty").prop("readonly", false);
    $("#boxDeliDate").parent().find("button").prop("disabled", false);

    $("#boxBtnCanc").parent().find("button").prop("disabled", true);
    $("#boxBtnCanc").prop("disabled", false);
    $("#boxBtnSave").prop("disabled", false);
});

$("#cutBtnEdit").click(function () {

    $("#cutTrip").prop("readonly", false);
    $("#cutKB").prop("readonly", false);
    $("#cutKBCycle").prop("readonly", false);
    $("#cutDeliDate").parent().find("button").prop("disabled", false);


    $("#cutBtnCanc").parent().find("button").prop("disabled", true);
    $("#cutBtnCanc").prop("disabled", false);
    $("#cutBtnSave").prop("disabled", false);
});
$("#stopBtnEdit").click(function () {

    $("#stopTrip").prop("readonly", false);
    $("#stopDeliDate").parent().find("button").prop("disabled", false);

    $("#stopBtnCanc").parent().find("button").prop("disabled", true);
    $("#stopBtnCanc").prop("disabled", false);
    $("#stopBtnSave").prop("disabled", false);
});


$("#boxBtnCanc").click(function () {
    $("#btnSearch").trigger("click");
});
$("#cutBtnCanc").click(function () {
    $("#btnSearch").trigger("click");
});
$("#stopBtnCanc").click(function () {
    $("#btnSearch").trigger("click");
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

$("#boxBtnSave").click(async function () {
    var isConfirm = await xSwal.confirm("Confirm", "Do you sure to SAVE Kanban Box Change data?");
    if (!isConfirm) {
        return;
    }

    //console.log(TB_Kanban_Chg)

    _xLib.AJAX_Post("/api/KBNMS006/SaveBoxQtyChg", JSON.stringify(GetTB_Kanban_Chg()),
        function (success) {
            if (success.status == 200) {
                $("#btnSearch").trigger("click");
                xSwal.success(success.title, success.message);
            }
        },
        function (error) {
            xSwal.error("Error", error.responseJSON.message);
        }
    );

});

$("#boxBtnStart").click(async function () {

    var isConfirm = await xSwal.confirm("Confirm", "Do you sure to START Kanban Box Change data?");
    if( !isConfirm) {
        return;
    }

    var obj = GetTB_Kanban_Chg();

    //console.log(TB_Kanban_Chg)

    _xLib.AJAX_Post("/api/KBNMS006/StartBoxQtyChg", JSON.stringify(obj),
        function (success) {
            if (success.status == 200) {
                $("#btnSearch").trigger("click");
                xSwal.success(success.title, success.message);
            }
        },
        function (error) {
            xSwal.error("Error", error.responseJSON.message);
        }
    );

});

$("#boxBtnStop").click(async function () {

    var isConfirm = await xSwal.confirm("Confirm", "Do you sure to STOP Kanban Box Change data?");
    if( !isConfirm) {
        return;
    }

    var obj = GetTB_Kanban_Chg();

    //console.log(TB_Kanban_Chg)

    _xLib.AJAX_Post("/api/KBNMS006/StopBoxQtyChg", JSON.stringify(obj),
        function (success) {
            if (success.status == 200) {
                $("#btnSearch").trigger("click");
                xSwal.success(success.title, success.message);
            }
        },
        function (error) {
            xSwal.error("Error", error.responseJSON.message);
        }
    );

});

$("#stopBtnSave").click(async function () {
    var isConfirm = await xSwal.confirm("Confirm", "Do you sure to SAVE Kanban Stop data?");
    if (!isConfirm) {
        return;
    }

    //console.log(TB_Kanban_Chg)

    _xLib.AJAX_Post("/api/KBNMS006/SaveKBStop", JSON.stringify(Get_Kanban_Stop()),
        function (success) {
            if (success.status == 200) {
                $("#btnSearch").trigger("click");
                xSwal.success(success.title, success.message);
            }
        },
        function (error) {
            xSwal.error("Error", error.responseJSON.message);
        }
    );

});

$("#stopBtnStart").click(async function () {

    var isConfirm = await xSwal.confirm("Confirm", "Do you sure to START Kanban Stop data?");
    if (!isConfirm) {
        return;
    }

    //console.log(TB_Kanban_Chg)

    _xLib.AJAX_Post("/api/KBNMS006/StartKBStop", JSON.stringify(Get_Kanban_Stop()),
        function (success) {
            if (success.status == 200) {
                $("#btnSearch").trigger("click");
                xSwal.success(success.title, success.message);
            }
        },
        function (error) {
            xSwal.error("Error", error.responseJSON.message);
        }
    );

});

$("#stopBtnStop").click(async function () {

    var isConfirm = await xSwal.confirm("Confirm", "Do you sure to STOP Kanban Stop data?");
    if (!isConfirm) {
        return;
    }

    //console.log(TB_Kanban_Chg)

    _xLib.AJAX_Post("/api/KBNMS006/StopKBStop", JSON.stringify(Get_Kanban_Stop()),
        function (success) {
            if (success.status == 200) {
                $("#btnSearch").trigger("click");
                xSwal.success(success.title, success.message);
            }
        },
        function (error) {
            xSwal.error("Error", error.responseJSON.message);
        }
    );

});

$("#cutBtnSave").click(async function () {
    var isConfirm = await xSwal.confirm("Confirm", "Do you sure to SAVE Kanban Cut data?");
    if (!isConfirm) {
        return;
    }

    //console.log(TB_Kanban_Chg)

    _xLib.AJAX_Post("/api/KBNMS006/SaveKBCut", JSON.stringify(Get_Kanban_Cut()),
        function (success) {
            if (success.status == 200) {
                $("#btnSearch").trigger("click");
                xSwal.success(success.title, success.message);
            }
        },
        function (error) {
            xSwal.error("Error", error.responseJSON.message);
        }
    );

});

$("#cutBtnStart").click(async function () {

    var isConfirm = await xSwal.confirm("Confirm", "Do you sure to START Kanban Cut data?");
    if (!isConfirm) {
        return;
    }

    //console.log(TB_Kanban_Chg)

    _xLib.AJAX_Post("/api/KBNMS006/StartKBCut", JSON.stringify(Get_Kanban_Cut()),
        function (success) {
            if (success.status == 200) {
                $("#btnSearch").trigger("click");
                xSwal.success(success.title, success.message);
            }
        },
        function (error) {
            xSwal.error("Error", error.responseJSON.message);
        }
    );

});

$("#cutBtnStop").click(async function () {

    var isConfirm = await xSwal.confirm("Confirm", "Do you sure to STOP Kanban Cut data?");
    if (!isConfirm) {
        return;
    }

    //console.log(TB_Kanban_Chg)

    _xLib.AJAX_Post("/api/KBNMS006/StopKBCut", JSON.stringify(Get_Kanban_Cut()),
        function (success) {
            if (success.status == 200) {
                $("#btnSearch").trigger("click");
                xSwal.success(success.title, success.message);
            }
        },
        function (error) {
            xSwal.error("Error", error.responseJSON.message);
        }
    );

});

function GetTB_Kanban_Chg() {
    var TB_Kanban_Chg = {
        F_Plant: _xLib.GetCookie("plantCode"),
        F_Supplier_Code: $("#readSupplierCode").val().split("-")[0],
        F_Supplier_Plant: $("#readSupplierCode").val().split("-")[1],
        F_Store_Code: $("#readStore").val(),
        F_Kanban_No: $("#readKanban").val(),
        F_Part_No: $("#readPartNo").val().split("-")[0],
        F_Ruibetsu: $("#readPartNo").val().split("-")[1],
        F_Delivery_Date: moment($("#boxDeliDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        F_Delivery_Trip: $("#boxTrip").val(),
        F_Start_Date: moment($("#boxStartDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        F_Start_Shift: $("#boxShift").val(),
        F_New_Qty: $("#boxQty").val(),
    }

    return TB_Kanban_Chg;
}

function Get_Kanban_Stop() {
    var Kanban_Stop = {
        F_Plant: _xLib.GetCookie("plantCode"),
        F_Supplier_Code: $("#readSupplierCode").val().split("-")[0],
        F_Supplier_Plant: $("#readSupplierCode").val().split("-")[1],
        F_Store_Code: $("#readStore").val(),
        F_Kanban_No: $("#readKanban").val(),
        F_Part_No: $("#readPartNo").val().split("-")[0],
        F_Ruibetsu: $("#readPartNo").val().split("-")[1],
        F_Delivery_Date: moment($("#stopDeliDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        F_Delivery_Trip: $("#stopTrip").val(),
        F_Start_Date: moment($("#stopStartDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        F_Start_Shift: $("#stopShift").val(),
    }

    return Kanban_Stop;
}

function Get_Kanban_Cut() {
    var Kanban_Cut = {
        F_Plant: _xLib.GetCookie("plantCode"),
        F_Supplier_Code: $("#readSupplierCode").val().split("-")[0],
        F_Supplier_Plant: $("#readSupplierCode").val().split("-")[1],
        F_Store_Code: $("#readStore").val(),
        F_Kanban_No: $("#readKanban").val(),
        F_Part_No: $("#readPartNo").val().split("-")[0],
        F_Ruibetsu: $("#readPartNo").val().split("-")[1],
        F_Delivery_Date: moment($("#stopDeliDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        F_Delivery_Trip: $("#cutTrip").val(),
        F_Start_Date: moment($("#stopStartDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
        F_Start_Shift: $("#stopShift").val(),
        F_KB_Cut: $("#cutKB").val(),
        F_KB_Cut_RN: $("#cutKBCycle").val(),
    }

    return Kanban_Cut;
}