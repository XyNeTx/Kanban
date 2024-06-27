class xLib {

    TrimArrayJSON(jsonResult) {
        for (let each in jsonResult) {
            for (let eachObj in jsonResult[each]) {
                if (jsonResult[each][eachObj] == null) {
                    jsonResult[each][eachObj] = '';
                }
                else {
                    jsonResult[each][eachObj] = jsonResult[each][eachObj].trim();
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
                jsonResult.data[each] = jsonResult.data[each].trim();
            }
        }
        return jsonResult;
    }

    AJAX_Get(url, data, success, error) {
        if (window.location.hostname.includes("tpcap")) {
            url = "/kanban" + url;
        }
        return $.ajax({
            type: "GET",
            url: url,
            data: data,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: success,
            error: error
        });
    }

    AJAX_Post(url, data, success, error) {
        if (window.location.hostname.includes("tpcap")) {
            url = "/kanban" + url;
        }
        return $.ajax({
            type: "POST",
            url: url,
            data: data,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: success,
            error: error
        });
    }

    OpenReport(reportName,query) {
        var _url = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx?/KB3"
        _url = _url + reportName + "&" + query;
        window.open(_url, '_blank');
    }
}
const _xLib = new xLib();