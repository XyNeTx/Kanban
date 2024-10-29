$(document).ready(async function () {

    let option = {
        columns: [
            {
                title: "", render: function (data, type, row, meta)
                {
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

function LoadListView() {

    _xLib.AJAX_Get("/api/KBNOR220/LoadListView", null,
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

    $(document).find("[disabled]").each(function () {
        if ($(this).attr("id") == "inpCreditCode") {
            return;
        }
        //console.log(this);
        $(this).prop("disabled", false);
    });
    $(document).find("[readonly]").each(function () {
        if ($(this).attr("id") == "inpCreditCode") {
            return;
        }
        //console.log(this);
        $(this).prop("readonly", false);
    });

    $(document).find("select").each(function () {
        //console.log(this);
        $(this).selectpicker("refresh");
    });

    $("#inpCustomerOrder").val(data.F_PDS_No);
    $("#selColorofTag").selectpicker("val", data.F_Remark_KB);
    $("#selUseForSection").selectpicker("val", data.F_Dept_Use);
    $("#selDebitCode").selectpicker("val", data.F_Acc_Dr);
    $("#inpCreditCode").val(data.F_Acc_Cr);
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