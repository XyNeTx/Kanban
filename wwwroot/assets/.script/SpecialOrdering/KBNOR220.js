$(document).ready(async function () {

    let option = {
        columns: [
            {
                title: "", render: function (data, type, row, meta) {
                    return "<input type='checkbox' class='chkSelect' value='" + row.F_PDS_No + "'>";
                }
            },
            { title: "Customer Order No.", data: "F_PDS_No" },
            { title: "Store Code", data: "F_Store_CD" },
            { title: "Use for Section", data: "F_Dept_Use" },
            { title: "Debit Code", data: "F_Acc_Dr" },
            { title: "Credit Code", data: "F_Acc_Cr" },
            { title: "Work Code", data: "F_Work_Code" },
            { title: "Remark1", data: "F_Remark" },
            { title: "Remark2", data: "F_Remark2" },
            { title: "Remark3", data: "F_Remark3" },
            { title: "Color of Tag", data: "F_Remark_KB" },
            { title: "Customer Order Type", data: "F_CustomerOrder_Type" },
            { title: "Order Type", data: "F_CusOrderType_CD" },
        ],
        order: [[1, "asc"]],
        scrollCollapse: false,
    }

    _xDataTable.InitialDataTable("tableMain", option);

    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");

    await LoadListView();
    await LoadColorofTag();
    await InitialScreenCmb();

    xSplash.hide();

});

async function LoadListView() {

    await _xLib.AJAX_Get("/api/KBNOR220/LoadListView", null,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            _xDataTable.ClearAndAddDataDT("tableMain", success.data);
        },
        function (error) {
            console.log(error);
        }
    );
}

function LoadColorofTag() {

    _xLib.AJAX_Get("/api/KBNOR220/LoadColorofTag", null,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#selColorofTag").empty();
            $("#selColorofTag").append("<option value='' hidden></option>");
            success.data.forEach(function (item) {
                $("#selColorofTag").append("<option value='" + item.F_Color_Tag + "'>" + item.F_Color_Tag + "</option>");
            });
        },
        function (error) {
            console.log(error);
        }
    );
}

function InitialScreenCmb() {

    _xLib.AJAX_Get("/api/KBNOR220/InitialScreenCmb", null,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#selUseForSection").empty();
            $("#selUseForSection").append("<option value='' hidden></option>");
            success.data.deptMS.forEach(function (item) {
                $("#selUseForSection").append("<option value='" + item.F_Dept_Cd + "'>" + item.F_Dept_Cd + "</option>");
            });
            $("#selDebitCode").empty();
            $("#selDebitCode").append("<option value='' hidden></option>");
            success.data.accMS.forEach(function (item) {
                $("#selDebitCode").append("<option value='" + item.F_Acc_CD + "'>" + item.F_Acc_CD + "</option>");
            });
        },
        function (error) {
            console.log(error);
        }
    );
}

$(document).on("click", "#tableMain tbody tr", function () {
    var closestRow = $(this).closest("tr");

    closestRow.addClass("selected").siblings().removeClass("selected");

    var data = _xDataTable.GetDataDT("tableMain", closestRow);

    $(document).find("div[class='card-body'").find("[disabled]").each(function () {
        if ($(this).attr("id") == "inpCreditCode") {
            return;
        }
        //console.log(this);
        $(this).prop("disabled", false);
    });
    $(document).find("div[class='card-body'").find("[readonly]").each(function () {
        if ($(this).attr("id") == "inpCreditCode") {
            return;
        }
        //console.log(this);
        $(this).prop("readonly", false);
    });

    $(document).find("div[class='card-body'").find("select").each(function () {
        //console.log(this);
        $(this).selectpicker("refresh");
    });

    $("#inpCustomerOrder").val(data.F_PDS_No);
    $("#selColorofTag").selectpicker("val", data.F_Remark_KB);
    $("#selUseForSection").selectpicker("val", data.F_Dept_Use);
    $("#selDebitCode").selectpicker("val", data.F_Acc_Dr);
    //$("#inpCreditCode").val(data.F_Acc_Cr);
    $("#inpWorkCode").val(data.F_Work_Code);
    $("#inpRemark").val(data.F_Remark);
    $("#inpRemark2").val(data.F_Remark2);
    $("#inpRemark3").val(data.F_Remark3);
    $("input[type='radio'][name='radCustomerType'][value='" + data.F_CustomerOrder_Type + "']").prop("checked", true);
    $("#selOrderType").selectpicker("val", data.F_CusOrderType_CD);
    console.log(data);
});

$("#btnCheck").click(function () {
    $("#tableMain tbody tr").find("input[type='checkbox']").each(function () {
        $(this).prop("checked", true);
    });
});
$("#btnUncheck").click(function () {
    $("#tableMain tbody tr").find("input[type='checkbox']").each(function () {
        $(this).prop("checked", false);
    });
});

$("#inpWorkCode").on("input", function () {
    //console.log($(this).val());
    if ($(this).val().length === 4) {
        $(this).val($(this).val().slice(0, 4) + "-");
    }
    if ($(this).val().length === 7) {
        $(this).val($(this).val().slice(0, 7) + "-");
    }
    if ($(this).val().length > 11) {
        $(this).val($(this).val().slice(0, 11));
    }

});

$("#btnSave").click(function () {
    //return console.log($("input[type='radio'][name='radCustomerType']:checked").val());

    let selectedObj = $("#tableMain").DataTable().row(".selected").data();

    let data = {
        F_PDS_No: $("#inpCustomerOrder").val(),
        F_Remark_KB: $("#selColorofTag").val(),
        F_Dept_Use: $("#selUseForSection").val(),
        F_Acc_Dr: $("#selDebitCode").val(),
        F_Acc_Cr: $("#inpCreditCode").val(),
        F_Work_Code: $("#inpWorkCode").val(),
        F_Remark: $("#inpRemark").val(),
        F_Remark2: $("#inpRemark2").val(),
        F_Remark3: $("#inpRemark3").val(),
        F_CustomerOrder_Type: $("input[type='radio'][name='radCustomerType']:checked").val(),
        F_CusOrderType_CD: $("#selOrderType").val(),
        F_Store_CD: selectedObj.F_Store_CD,
        F_Issued_Date: selectedObj.F_Issued_Date,
    }

    console.log(data);

    _xLib.AJAX_Post("/api/KBNOR220/Save", data,
        async function (success) {
            await xSwal.success(success.response, success.message);

            $(document).find("div[class='card-body'").find("input[type='text']").each(function () {
                if ($(this).attr("id") == "inpCreditCode") {
                    return;
                }
                $(this).val("");
                $(this).prop("readonly", true);
            });
            $(document).find("div[class='card-body'").find("input[type='radio']").each(function () {
                $(this).prop("checked", false);
                $(this).prop("disabled", true);
            });
            $(document).find("div[class='card-body'").find("select").each(function () {
                $(this).val("");
                $(this).prop("disabled", true);
                $(this).selectpicker("refresh");
            });

            $("#inpCreditCode").val("2133102");
            LoadListView();
        },
        function (error) {
            console.log(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );

});

$("#btnGen").click(async function () {

    let data = [];

    $("#tableMain tbody tr").find("input[type='checkbox']:checked").each(function () {
        let row = $(this).closest("tr");
        let obj = _xDataTable.GetDataDT("tableMain", row);
        data.push(obj);
    });

    console.log(data);

    if (data.length === 0) {
        xSwal.error("Error", "Please select at least 1 row to generate");
        return;
    }

    await _xLib.AJAX_Post("/api/KBNOR220/Generate", data,
        async function (success) {
            await xSwal.success(success.response, success.message);
            $("#tableMain input[type='checkbox']:checked").each(function () {
                $("#tableMain").DataTable().row($(this).closest("tr")).remove().draw(false);
            });
            await LoadListView();
        },
        function (error) {
            console.log(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        }
    );

});