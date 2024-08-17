$(document).ready(async function () {

    await SetReadOnly();
    await GetSupplier();
    await GetKanban();
    await GetStore();
    await GetPartNo();

    await $(".SelectPicker").selectpicker();
    xSplash.hide();
})
function SetReadOnly() {
    $("#divKBStop").find("input").attr("readonly", true);
    $("#divKBCut").find("input").attr("readonly", true);
    $("#divBoxChg").find("input").attr("readonly", true);
    $("#divReady").find("input").attr("readonly", true);
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
            }
        },
        function (error) {
            xSwal.error("Error", error.responseJSON.message);
        }
    );
});

