class xLib {

    TrimArrayJSON(jsonResult) {
        for (let each in jsonResult) {
            for (let eachObj in jsonResult[each]) {
                if (jsonResult[each][eachObj] == null) {
                    jsonResult[each][eachObj] = '';
                }
                else {
                    if(typeof jsonResult[each][eachObj] == 'string') jsonResult[each][eachObj] = jsonResult[each][eachObj].trim();
                }
            }
        }
        return jsonResult;
    }

    TrimJSON(jsonResult) {
        for (let each in jsonResult.data) {
            if (jsonResult.data[each] == null) {
                jsonResult.data[each] = '';
            }
            else {
                if (typeof jsonResult[each][eachObj] == 'string') jsonResult[each][eachObj] = jsonResult[each][eachObj].trim();
            }
        }
        return jsonResult;
    }

    AJAX_Get(url, data, successFn, errorFn) {
        if (window.location.hostname.includes("tpcap")) {
            url = "/kanban" + url;
        }
        return $.ajax({
            type: "GET",
            url: url,
            data: data,
            headers: ajexHeader,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: successFn,
            error: async function (xhr, status, error) {
                if (xhr.status == 401) {
                    await xSwal.error("Error", "Session expired. Please login again.", function () {
                        if (window.location.hostname.includes("tpcap")) {
                            return window.open("/kanban/Login", "_self");
                        }
                        return window.open("/Login", "_self");
                    });
                }
                else {
                    await console.error(xhr);
                    errorFn(xhr, status, error)
                }
            }
        });
    }

    AJAX_Post(url, data, successFn, errorFn) {
        if (window.location.hostname.includes("tpcap")) {
            url = "/kanban" + url;
        }
        return $.ajax({
            type: "POST",
            url: url,
            data: data,
            headers: ajexHeader,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: successFn,
            error: async function (xhr, status, error) {
                if (xhr.status == 401) {
                    await xSwal.error("Error", "Session expired. Please login again.", function () {
                        if (window.location.hostname.includes("tpcap")) {
                            return window.open("/kanban/Login", "_self");
                        }
                        return window.open("/Login", "_self");
                    });
                }
                else {
                    await console.error(xhr);
                    errorFn(xhr, status, error)
                }
            }
        });
    }

    OpenReport(reportName,query) {
        var _url = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx?/KB3"
        _url = _url + reportName + "&" + query;
        window.open(_url, '_blank');
    }
}
const _xLib = new xLib();