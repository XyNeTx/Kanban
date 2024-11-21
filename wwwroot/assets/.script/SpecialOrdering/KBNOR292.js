$(document).ready(async () => {
    await GetSupplierSurvey();

    xSplash.hide();
});
$("#mthSurveyIssued").change(async () => {
    await GetSupplierSurvey();
});

$("#selSuppCD").change(async () => {
    await GetSupplierSurvey();
});
GetSupplierSurvey = async () => {
    let getQuery = {
        IssueDate: $("#mthSurveyIssued").val().replace(/-/g, ""),
        SupplierCD: $("#selSuppCD").val()
    }
    _xLib.AJAX_Get("/api/KBNOR292/GetSupplierSurvey", getQuery,
        async (result) => {
            result = _xLib.JSONparseMixData(result);
            if (getQuery.SupplierCD) {
                $("#spanSuppName").html(result.data[0].F_Supplier_INT + ":" + result.data[0].F_Supplier_Name);
            }
            else {
                $("#selSuppCD").addListSelectPicker(result.data, "F_Supplier_CD");
            }
        }
    );
}

$("#btnReport").click(() => {
    let obj = {
        Plant: _xLib.GetCookie("plantCode"),
        UserName: _xLib.GetUserName(),
        F_Issued_YM: $("#mthSurveyIssued").val().replace(/-/g, ""),
        F_Supplier_CD: $("#selSuppCD").val()
    }

    for (let key in obj) {
        if (obj[key] === "") {
            return xSwal.error("Error!!", 'Please select all fields.');
        }
    };

    _xLib.OpenReportObj("/KBNOR292", obj);
});