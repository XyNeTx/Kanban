var cookieLoginDate = _xLib.GetCookie("loginDate");
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

    console.log($("#boxDeliDate").val())

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

                console.log(success.data.stop)

                if (success.data.stop == null) {
                    $("#divKBStop").find("input").val("");
                    $("#divKStop").find("button").prop("disabled", true);

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

                if (success.data.cut == null) {
                    $("#divKBCut").find("input").val("");
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


            }
        },
        function (error) {
            xSwal.error("Error", error.responseJSON.message);
        }
    );
});


$("#boxBtnEdit").click(function () {

    $("#boxTrip").prop("readonly", false);
    $("#boxQty").prop("readonly", false);
    $("#boxDeliDate").parent().find("button").prop("disabled", false);

    $("#boxBtnCanc").prop("disabled", false);
    $("#boxBtnSave").prop("disabled", false);
});

$("#cutBtnEdit").click(function () {

    $("#cutTrip").prop("readonly", false);
    $("#cutKB").prop("readonly", false);
    $("#cutKBCycle").prop("readonly", false);
    $("#cutDeliDate").parent().find("button").prop("disabled", false);


    $("#cutBtnCanc").prop("disabled", false);
    $("#cutBtnSave").prop("disabled", false);
});
$("#stopBtnEdit").click(function () {

    $("#stopTrip").prop("readonly", false);
    $("#stopDeliDate").parent().find("button").prop("disabled", false);


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
