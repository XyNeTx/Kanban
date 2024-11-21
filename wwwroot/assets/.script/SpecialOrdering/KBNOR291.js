$(document).ready(async () => {
    xSplash.hide();
});

$("#btnReport").click(() => {
    let obj = {
        Plant: _xLib.GetCookie("plantCode"),
        UserName: _xLib.GetUserName(),
        F_Issued_Date: $("#mthSurveyIssued").val().replace(/-/g, ""),
    }

    for (let key in obj) {
        if (obj[key] === "") {
            return xSwal.error("Error!!", 'Please select all fields.');
        }
    };

    _xLib.OpenReportObj("/KBNOR291", obj);
});