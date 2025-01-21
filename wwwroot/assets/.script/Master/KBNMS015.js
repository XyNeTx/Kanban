$(document).ready(function () {


    _xDataTable.InitialDataTable("#tableMain",
        {
            "processing": false,
            "serverSide": false,
            width: '100%',
            paging: false,
            sorting: false,
            searching: false,
            scrollX: false,
            scrollY: "200px",
            scrollCollapse: true,
            "columns": [
                {
                    title: "Flag", render(data, type, row) {
                        return `<input type="checkbox" class="chkbox" id="chkbox" name="chkbox" value="${row.f_Supplier_Plant}">`;
                    }
                },
                {
                    title: "Supplier Code", data: "F_Supplier_Code"
                },
                {
                    title: "Cycle", data: "F_Cycle"
                },
                {
                    title: "Kanban No", data: "F_Kanban_No"
                },
                {
                    title: "Store Code", data: "F_Store_Code"
                },
                {
                    title: "Part No.", data: "F_Part_No"
                },
                {
                    title: "Start Date", data: "F_Start_Date"
                },
                {
                    title: "End Date", data: "F_End_Date"
                },
                {
                    title: "Color", data: "F_Color"
                },
                {
                    title: "Description", data: "F_Text"
                },
            ],
            select: false,
            order: [[0, "asc"]]
        });

    $("table tbody tr td").addClass("text-center");
    $("table thead tr th").addClass("text-center");

    $(".datepicker").each(function () {
        $(this).initDatepicker();
    });

    $("#F_Start_Date").val(moment().format('DD/MM/YYYY'));
    $("#F_End_Date").val("31/12/2999");



    xSplash.hide();

})

$("#divBtn").on("click", "button", async function () {
    //console.log("click");

    $("#divBtn").find("button").prop("disabled", true);
    await $(this).prop("disabled", false);
    //console.log("click 1");

    $(document).find("select:disabled").each(function () {
        $(this).prop("disabled", false);
        $(this).selectpicker("refresh");
    });

    //console.log("click 2");

    GetDropDown();

});

$("#btnCan").click(function () {
    $("#divBtn").find("button").prop("disabled", false);
    $(document).find("select:not(:disabled)").each(function () {
        $(this).prop("disabled", true);
        $(this).resetSelectPicker();
    });
    $(document).find("input:not(:disabled)").each(function () {
        $(this).val("");
    });

    $("#tableMain").DataTable().clear().draw();
});

$("#F_Supplier_Code").change(function () {
    SupplierChanged();
    GetDropDown();
    GetListData();
});

$("#F_Kanban_No").change(function () {
    GetDropDown();
    $("#F_Kanban_No_Rd").val($("#F_Kanban_No").val());
    GetListData();
});

$("#F_Store_Code").change(function () {
    GetDropDown();
    $("#F_Store_Code_Rd").val($("#F_Store_Code").val());
    GetListData();
});

$("#F_Part_No").change(function () {
    GetDropDown();
    $("#F_Part_No_Rd").val($("#F_Part_No").val());
    PartNoSelected();
    GetListData();
});

$("#btnSave").click(function () {
    Save();
});

function GetDropDown() {
    let obj = {
        isNew: !($("#btnNew").prop("disabled")),
        Supplier: $("#F_Supplier_Code").val(),
        Kanban: $("#F_Kanban_No").val(),
        StoreCode: $("#F_Store_Code").val(),
        PartNo: $("#F_Part_No").val()
    };

    _xLib.AJAX_Get("/api/KBNMS015/GetDropDown", obj,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);

            if ($("#F_Supplier_Code").val() == "") {
                $("#F_Supplier_Code").addListSelectPicker(success.data[0], "F_supplier_cd");
            }

            if ($("#F_Kanban_No").val() == "") {
                $("#F_Kanban_No").addListSelectPicker(success.data[1], "F_Kanban_No");
            }

            if ($("#F_Part_No").val() == "") {
                $("#F_Part_No").addListSelectPicker(success.data[2], "F_Part_No");
            }

            if ($("#F_Store_Code").val() == "") {
                $("#F_Store_Code").addListSelectPicker(success.data[3], "F_Store_cd");
            }

        },
        function (error) {
        }
    );
}

function SupplierChanged() {
    let obj = {
        SupplierCode : $("#F_Supplier_Code").val(),
        StoreCode : $("#F_Store_Code").val()
    }

    _xLib.AJAX_Get("/api/KBNMS015/SupplierChanged", obj,
        function (success) {
            //success = _xLib.JSONparseMixData(success);
            success.data = _xLib.TrimJSON(success.data);
            console.log(success);
            $("#F_Supplier_Name").val(success.data.f_name);
            let cycle = success.data.f_Cycle_A.length == 1 ? "0" + success.data.f_Cycle_A : success.data.f_Cycle_A;
            cycle += success.data.f_Cycle_B.length == 1 ? "-" + "0" + success.data.f_Cycle_B : "-" +  success.data.f_Cycle_B;
            cycle += success.data.f_Cycle_C.length == 1 ? "-" + "0" + success.data.f_Cycle_C : "-" +  success.data.f_Cycle_C;
            $("#F_Cycle").val(cycle);
            
        },
        function (error) {
        }
    );
}

function PartNoSelected() {

    let obj = {
        PartNo: $("#F_Part_No").val(),
        SupplierCode: $("#F_Supplier_Code").val(),
        StoreCode: $("#F_Store_Code").val(),
        Kanban: $("#F_Kanban_No").val(),
        IsNew : !($("#btnNew").prop("disabled"))
    }

    _xLib.AJAX_Get("/api/KBNMS015/PartNoSelected", obj,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#F_Part_Name_Rd").val(success.data.F_Part_nm);
            $("#F_Color").val(success.data.F_Color);
            $("#F_Description").val(success.data.F_Text);
        },
        function (error) {
        }
    );

}

async function GetListData() {

    let obj = {
        SupplierCode: $("#F_Supplier_Code").val(),
        StoreCode: $("#F_Store_Code").val(),
        PartNo: $("#F_Part_No").val(),
        Kanban: $("#F_Kanban_No").val()
    }

    _xLib.AJAX_Get("/api/KBNMS015/GetListData", obj,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        },
        function (error) {
        }
    );
};

async function Save()
{
    let listObj = [];
    let obj = {};

    $(".card-body").find("input:not(:disabled), select:not(:disabled)").each(function () {
        if ($(this).attr("id") == undefined) return;

        obj[$(this).attr("id")] = $(this).val();

        if($(this).attr("class").includes("datepicker"))
        {
            obj[$(this).attr("id")] = moment($(this).val(),"DD/MM/YYYY").format("YYYYMMDD");
        }

    });
    obj.F_Cycle = await obj.F_Cycle.replace(/-/g, "");

    await listObj.push(obj);

    if (!$("#btnDel").prop("disabled")) {

        listObj = _xDataTable.GetSelectedDataDT("#tableMain");

        //change date format
        listObj.forEach(function (item) {
            item.F_Start_Date = moment(item.F_Start_Date, "DD/MM/YYYY").format("YYYYMMDD");
            item.F_End_Date = moment(item.F_End_Date, "DD/MM/YYYY").format("YYYYMMDD");
        });
    }

    let action = $("#divBtn").find("button:not(:disabled)").attr("id").split("btn")[1];

    _xLib.AJAX_Post("/api/KBNMS015/Save?action=" + action, listObj,
        function (success) {
            xSwal.success(success.response, success.message);
        },
        function (error) {
        }
    );
}