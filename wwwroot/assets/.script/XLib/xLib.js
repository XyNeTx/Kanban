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

    JSONparseAndTrim(jsonResult) {
        if (jsonResult.hasOwnProperty('data')) {
            jsonResult.data = JSON.parse(jsonResult.data);
            if (typeof jsonResult.data == 'object') jsonResult.data = this.TrimArrayJSON(jsonResult.data);
            else jsonResult.data = this.TrimJSON(jsonResult.data);
        }
        return jsonResult;
    }

    JSONparseAndTrimArray(jsonResult) {
        if (jsonResult.hasOwnProperty('data')) {
            for(var key in jsonResult.data) {
                jsonResult.data[key] = JSON.parse(jsonResult.data[key]);
                if(typeof jsonResult.data[key] == 'object') jsonResult.data[key] = this.TrimArrayJSON(jsonResult.data[key]);
                else jsonResult.data[key] = this.TrimJSON(jsonResult.data[key]);
            }
        }
        return jsonResult;
    }

    SetProcessCookie(cName) {
        let cookie = document.cookie;

        if (cookie.includes('_xProcess')) {
            //console.log(cookie.split(';').find(x => x.indexOf('process') >= 0).includes(`${cName}`));
            if (!(cookie.split(';').find(x => x.indexOf('_xProcess') >= 0).includes(`${cName}`))) {
                //console.log(cookie);
                //console.log(cookie.split(';').find(x => x.indexOf('process') >= 0));
                var _appendCookie = cookie.split(';').filter(x => x.indexOf('_xProcess') >= 0) + `,${cName}`
                // console.log(_appendCookie);
                return document.cookie = _appendCookie;
            }
            return;
        }
        return document.cookie = `_xProcess=${cName}`;
    }

    GetProcessCookie() {
        var cookie = document.cookie;
        var process = cookie.split(';').find(x => x.includes('_xProcess'));
        if(process == undefined) return null;
        //console.log(process);
        var arrProcess = process.split('=').pop().split(',');
        //console.log(arrProcess);
        return arrProcess;
    }

    GetCookie(name) {
        var cookie = document.cookie;
        var _cookie = cookie.split(';').find(x => x.includes(name));
        if (_cookie == undefined) return null;
        return _cookie.split('=').pop();
    };


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
            success: async function (xhr, status) {
                //if (xhr.hasOwnProperty('data')) {
                //    xhr.data = await JSON.parse(xhr.data);
                //    console.log(xhr.data);
                //    console.log(typeof xhr.data);
                //    if (typeof xhr.data == 'object') xhr.data = _xLib.TrimArrayJSON(xhr.data);
                //    else xhr.data = _xLib.TrimJSON(xhr.data);
                //}
                if (typeof successFn != 'function') return;
                return successFn(xhr, status);
            },
            error: async function (xhr, status, error) {
                if (xhr.status == 401) {
                    await xSplash.hide();
                    await xSwal.error("Error", "Session expired. Please login again.", function () {
                        if (window.location.hostname.includes("tpcap")) {
                            return window.open("/kanban/Login", "_self");
                        }
                        return window.open("/Login", "_self");
                    });
                }
                else {
                    await console.error(xhr);
                    xSplash.hide();

                    // error was return by apicontroller
                    if (xhr.responseJSON.errors != undefined) {

                        if (xhr.responseJSON.errors.message == undefined || xhr.responseJSON.errors.message == null || !xhr.responseJSON.errors.message) {
                            var _error = "";
                            for (var key in xhr.responseJSON.errors) {
                                _error += xhr.responseJSON.errors[key][0] + "<br>";
                            }
                            return xSwal.ErrorHTML("Error", _error)
                        }

                        return xSwal.error("Error", xhr.responseJSON.message)
                    }

                    // error was return by developer catch it
                    else if (xhr.responseJSON.response) {
                        xSwal.error(xhr.responseJSON.response, xhr.responseJSON.message)
                        return errorFn(xhr, status, error)
                    }

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
                    await xSplash.hide();
                    await xSwal.error("Error", "Session expired. Please login again.", function () {
                        if (window.location.hostname.includes("tpcap")) {
                            return window.open("/kanban/Login", "_self");
                        }
                        return window.open("/Login", "_self");
                    });
                }
                else {
                    await console.error(xhr);
                    xSplash.hide();

                    // error was return by apicontroller
                    if (xhr.responseJSON.errors != undefined) {

                        if (xhr.responseJSON.errors.message == undefined || xhr.responseJSON.errors.message == null || !xhr.responseJSON.errors.message) {
                            var _error = "";
                            for (var key in xhr.responseJSON.errors) {
                                _error += xhr.responseJSON.errors[key][0] + "<br>";
                            }
                            return xSwal.ErrorHTML("Error", _error)
                        }

                        return xSwal.error("Error", xhr.responseJSON.message)
                    }

                    // error was return by developer catch it
                    else if (xhr.responseJSON.response) {
                        xSwal.error(xhr.responseJSON.response, xhr.responseJSON.message)
                        return errorFn(xhr, status, error)
                    }

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