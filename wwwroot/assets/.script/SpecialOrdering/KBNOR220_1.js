$(document).ready(async function () {

    let option = {
        columns: [
            {
                title: "Flag", render: function (data, type, row, meta) {
                    return "<input type='checkbox' class='chkSelect' value='" + row.F_PDS_No + "'>";
                }
            },
            { title: "Survey Doc.", data: "F_Survey_Doc" },
            { title: "Supplier Code", data: "F_Supplier_Code" },
            { title: "Short Name", data: "F_Short_name" },
            { title: "Customer Order NO.", data: "F_PO_Customer" },
            { title: "Issued Date", data: "F_Issued_Date" },
            { title: "Use of Section", data: "F_Dept_Code" },
            { title: "Debit Code", data: "F_Acc_Dr" },
            { title: "Credit Code", data: "F_Acc_Cr" },
            { title: "Work Code", data: "F_Wk_code" },
            { title: "Remark", data: "F_Remark" },
            { title: "Remark2", data: "F_Remark2" },
            { title: "Remark3", data: "F_Remark3" },
            { title: "Color of Tag", data: "F_Remark_KB" },
            { title: "Customer Order Tag", data: "F_CustomerOrder_Type" },
        ],
        order: [[1, "asc"]],
        scrollCollapse: true,
        selectable : true,
    }

    _xDataTable.InitialDataTable("tableMain", option);

    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");
    $("table tbody tr td").css("vertical-align", "middle");

    await LoadColorofTag();
    await InitialScreenCmb();
    await LoadSurveyDoc();

    xSplash.hide();
});

function LoadColorofTag() {

    _xLib.AJAX_Get("/api/KBNOR220/LoadColorofTag", null,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
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

function LoadSurveyDoc() {

    _xLib.AJAX_Get("/api/KBNOR220_1/LoadSurveyDoc", null,
        function (success) {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#selSurveyDoc").empty();
            $("#selSurveyDoc").append("<option value='' hidden></option>");
            success.data.forEach(function (item) {
                $("#selSurveyDoc").append("<option value='" + item.F_Survey_Doc + "'>" + item.F_Survey_Doc + "</option>");
            });
            $("#selSurveyDoc").selectpicker("refresh");
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
            //console.log(success);
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

$("#selSurveyDoc").change(SurveyDocChanged = async () => {
    let data = {
        surveyDoc: $("#selSurveyDoc").val(),
    }
    _xLib.AJAX_Get("/api/KBNOR220_1/LoadSurveyDoc", data,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            _xDataTable.ClearAndAddDataDT("tableMain", success.data);
        },
        async (error) => {
            console.log(error);
        }
    );
});

let mode;
$("#btnInq").click(InqClicked = async () => {
    mode = "inquiry";

    $("#btnUpd").prop("disabled", true);
    $("#btnDel").prop("disabled", true);

    $("#selPlant").prop("disabled", false);
    $("#selSurveyDoc").prop("disabled", false);

    $("#selSurveyDoc").val("");
    $("#selPlant").val("3");

    $("#selPlant").selectpicker("refresh");
    $("#selSurveyDoc").selectpicker("refresh");

});

$("#btnUpd").click(UpdClicked = async () => {

    mode = "update";

    $("#btnInq").prop("disabled", true);
    $("#btnDel").prop("disabled", true);
    $("#btnSave").prop("disabled", false);

    $("#selSurveyDoc").val("");
    $("#selPlant").val("3");

    $(document).find("select").prop("disabled", false);
    $(document).find("select").selectpicker("refresh");
    $(document).find("input").prop("disabled", false);

    $("#inpCustomerOrderNo").prop("disabled", true);
    $("#inpCreditCode").prop("disabled", true);

    $("#btnSave").prop("disabled", false);
});

$("#btnDel").click(DelClicked = async () => {
    mode = "delete";

    let data = {
        mode: mode
    }
    _xLib.AJAX_Get("/api/KBNOR220_1/LoadSurveyDoc", data,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#selSurveyDoc").empty();
            $("#selSurveyDoc").append("<option value='' hidden></option>");
            success.data.forEach(function (item) {
                $("#selSurveyDoc").append("<option value='" + item.F_Survey_Doc + "'>" + item.F_Survey_Doc + "</option>");
            });
            $("#selSurveyDoc").selectpicker("refresh");
        },
        async (error) => {
            console.log(error);
        }
    );

    $("#btnInq").prop("disabled", true);
    $("#btnUpd").prop("disabled", true);
    $("#btnSave").prop("disabled", false);

    $("#selSurveyDoc").val("");
    $("#selPlant").val("3");

    $("#selPlant").prop("disabled", false);
    $("#selSurveyDoc").prop("disabled", false);

    $("#selPlant").selectpicker("refresh");
    $("#selSurveyDoc").selectpicker("refresh");
});

$("#btnCan").click(CanClicked = async () => {
    $("#btnInq").prop("disabled", false);
    $("#btnUpd").prop("disabled", false);
    $("#btnDel").prop("disabled", false);
    
    $("#selPlant").prop("disabled", true);
    $("#selSurveyDoc").prop("disabled", true);
    
    $(document).find("select").prop("disabled", true);
    $(document).find("select").val("");
    $(document).find("input[type='text']").prop("disabled", true);
    $(document).find("input[type='text']").val("");
    $(document).find("input[type='radio']").prop("disabled", true);
    $(document).find("input[type='radio']:checked").prop("checked", false);
    $("#tableMain").DataTable().clear().draw();

    $(document).find("select").selectpicker("refresh");
    
    $("#btnSave").prop("disabled", true);
});

$(document).on("click", "#tableMain tbody tr", async function () {

    $(this).addClass("selected").siblings().removeClass("selected");
    let obj = _xDataTable.GetDataDT("tableMain", $(this).closest("tr"));

    $("#inpCustomerOrderNo").val(obj.F_PO_Customer);
    $("#selColorofTag").val(obj.F_Remark_KB);
    $("#selUseForSection").val(obj.F_Dept_Code);
    $("#selDebitCode").val(obj.F_Acc_Dr);
    $("#inpCreditCode").val(obj.F_Acc_Cr);
    $("#inpWorkCode").val(obj.F_Wk_code);
    $("#inpRemark").val(obj.F_Remark);
    $("#inpRemark2").val(obj.F_Remark2);
    $("#inpRemark3").val(obj.F_Remark3);
    $("input[name='radCustomerType'][value='" + obj.F_CustomerOrder_Type + "']").prop("checked", true);

    $(document).find("select").selectpicker("refresh");
});

$("#btnSave").click(SaveClicked = async () => {

    let isDel = mode == "delete" ? true : false;

    let listObj = $("#tableMain").DataTable().rows().data().toArray();

    listObj.forEach(function (item) {
        item.F_Color_Tag = $("#selColorofTag").val();
        item.F_Dept_Code = $("#selUseForSection").val();
        item.F_Acc_Dr = $("#selDebitCode").val();
        item.F_Acc_Cr = $("#inpCreditCode").val();
        item.F_Wk_code = $("#inpWorkCode").val();
        item.F_Issued_Date = moment(item.F_Issued_Date,"DD/MM/YYYY").format("YYYYMMDD");
        item.F_Remark = $("#inpRemark").val();
        item.F_Remark2 = $("#inpRemark2").val();
        item.F_Remark3 = $("#inpRemark3").val();
        item.F_CustomerOrder_Type = $("input[name='radCustomerType']:checked").val();
    });

    _xLib.AJAX_Post("/api/KBNOR220_1/Save?isDel=" + isDel, listObj,
        async (success) => {
            xSwal.success(success.response, success.message);
        },
        async (error) => {
            console.log(error);
        }
    );

});