class DataTableLib {
    constructor() {
        this.width = '100%';
        this.paging = false;
        this.scrollCollapse = true;
        this.processing = false;
        this.serverSide = false;
        this.scrollX = true;
        this.scrollY = '350px';
        this.searching = false;
        this.info = false;
        this.ordering = false;
        this.columns = [];
        this.order = [[1, 'asc']];
        this.buttons = [];
        this.layout = {
            topStart: 'buttons',
        };
    }

    InitialDataTable(id, options) {
        if (id == undefined) return console.error("id is undefined");
        if (!id.includes('#')) id = '#' + id;

        let constr = new DataTableLib();
        constr = Object.assign(constr, options);
        //console.log(constr);
        //console.log(constr.scrollCollapse, "scrollCollapse");
        //console.log(this.scrollCollapse, "this.scrollCollapse");

        return $(`${id}`).DataTable({
            width: (constr.width) ?? this.width,
            paging: (constr.paging) ?? this.paging,
            scrollCollapse: (constr.scrollCollapse) ?? this.scrollCollapse,
            processing: (constr.processing) ?? this.processing,
            serverSide: (constr.serverSide) ?? this.serverSide,
            scrollX: (constr.scrollX) ?? this.scrollX,
            scrollY: (constr.scrollY) ?? this.scrollY,
            searching: (constr.searching) ?? this.searching,
            info: (constr.info) ?? this.info,
            ordering: (constr.ordering) ?? this.ordering,
            columns: (constr.columns) ?? this.columns,
            order: (constr.order) ?? this.order,
            buttons: (constr.buttons) ?? this.buttons,
            layout: (constr.layout) ?? this.layout
        });

    }

    ClearAndAddDataDT(id, data) {
        if (id == undefined) return console.error("id is undefined");
        if (!id.includes('#')) id = '#' + id;

        $(`${id}`).DataTable().clear().rows.add(data).draw();

        $("table thead tr th").css("text-align", "center");
        $("table thead tr th").css("vertical-align", "middle");

        $("table tbody tr td").css("text-align", "center");
        $("table tbody tr td").css("vertical-align", "middle");
    }

    GetDataDT(id, closestRow) {
        if (id == undefined) return console.error("id is undefined");
        if (!id.includes('#')) id = '#' + id;

        let table = $(`${id}`).DataTable();
        let data = table.row($(closestRow).closest('tr')).data();

        return data;
    }

    GetSelectedDataDT(id) {
        if (id == undefined) return console.error("id is undefined");
        if (!id.includes('#')) id = '#' + id;

        let table = $(`${id}`).DataTable();
        let data = [];

        $(`${id} tbody`).find('input[type="checkbox"]:checked').each(function () {
            data.push(table.row($(this).closest('tr')).data());
        });

        return data;
    
    }
}

const _xDataTable = new DataTableLib();

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
        try {
            if (jsonResult.hasOwnProperty('data')) {
                for (let each in jsonResult.data) {
                    if (jsonResult.data[each] == null) {
                        jsonResult.data[each] = '';
                    }
                    else {
                        if (typeof jsonResult[each][eachObj] == 'string') jsonResult[each][eachObj] = jsonResult[each][eachObj].trim();
                    }
                }
            }
            else {
                for (let each in jsonResult) {
                    if (jsonResult[each] == null) {
                        jsonResult[each] = '';
                    }
                    else {
                        if (typeof jsonResult[each] == 'string') jsonResult[each] = jsonResult[each].trim();
                    }
                }
            }
            return jsonResult;
        }
        catch (e) {
            console.error(e);
            return jsonResult;
        }
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

    JSONparseMixData(jsonResult) {
        if (jsonResult.hasOwnProperty('data')) {
            //jsonResult.data = JSON.parse(jsonResult.data);
            for (var key in jsonResult.data) {
                try {
                    jsonResult.data[key] = JSON.parse(jsonResult.data[key]);
                    if (typeof jsonResult.data[key] == 'object') jsonResult.data[key] = this.TrimArrayJSON(jsonResult.data[key]);
                    else jsonResult.data[key] = this.TrimJSON(jsonResult.data);
                }
                catch (e) {
                    if (typeof jsonResult.data[key] == 'object') {
                        for (var key in jsonResult.data) {
                            for (var key2 in jsonResult.data[key]) {
                                if (typeof jsonResult.data[key][key2] == 'string') {
                                    jsonResult.data[key][key2] = jsonResult.data[key][key2].trim();
                                }
                            }
                        }
                        return jsonResult;
                    }
                    else {
                        if (typeof jsonResult.data == 'object') {

                            for (var key in jsonResult.data) {
                                if (typeof jsonResult.data[key] == 'string') {
                                    jsonResult.data[key] = jsonResult.data[key].trim();
                                }
                                else if (typeof jsonResult.data[key] == 'array') {
                                    for (var key2 in jsonResult.data[key]) {
                                        if (typeof jsonResult.data[key][key2] == 'string') {
                                            jsonResult.data[key][key2] = jsonResult.data[key][key2].trim();
                                        }
                                    }

                                }
                                return jsonResult;
                            }

                        }

                        jsonResult.data = JSON.parse(jsonResult.data);
                        for(let i = 0; i < jsonResult.data.length; i++) {
                            for (var key in jsonResult.data[i]) {
                                if (typeof jsonResult.data[i][key] == 'string') {
                                    jsonResult.data[i][key] = jsonResult.data[i][key].trim();
                                }
                            }
                        }
                        return jsonResult;
                    }
                    return jsonResult;
                }
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

    SetCookie(name, value) {
        return document.cookie = `${name}=${value}`;
    }

    LoginDateDD() {
        var cookieLoginDate = this.GetCookie('loginDate');
        return moment(cookieLoginDate.substring(0,10)).format("DD/MM/YYYY");
    }

    GetUserName() {
        return $(".pcoded-navigatio-lavel").text().trim();
    }

    ResetSelectPicker(id) {
        if (id == undefined) return console.error("id is undefined");
        if (!id.includes('#')) id = '#' + id;

        $(`${id}`).val('');
        $(`${id}`).selectpicker('refresh');
        $(`${id}`).parent().find("div[class='filter-option-inner-inner']").text("Nothing Selected");
    }


    AJAX_GetNoHeader(url, data, successFn, errorFn) {
        if (window.location.hostname.includes("tpcap")) {
            url = "/kanban" + url;
        }
        return $.ajax({
            type: "GET",
            url: url,
            data: data,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 5400000,
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
                else if (xhr.status == 403) {
                    console.log(xhr);
                    // forbidden
                    await xSplash.hide();
                    await xSwal.error("Forbidden", "You are not allow to access this page.", function () {
                        if (window.location.hostname.includes("tpcap")) {
                            return window.open("/kanban/Home", "_self");
                        }
                        return window.open("/Home", "_self");
                    }
                    );
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
                        if (typeof errorFn == 'function') {
                            return errorFn(xhr, status, error)
                        }
                    }

                }
            }
        });
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
            timeout: 5400000,
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
                else if (xhr.status == 403) {
                    //console.log(xhr);
                    // forbidden
                    await xSplash.hide();
                    await xSwal.error("Forbidden", "You are not allow to access this page.", function () {
                        if (window.location.hostname.includes("tpcap")) {
                            return window.open("/kanban/Home", "_self");
                        }
                        return window.open("/Home", "_self");
                    }
                    );
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
                        if (typeof errorFn == 'function') {
                            return errorFn(xhr, status, error)
                        }
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
            data: typeof data == 'string' ? data : JSON.stringify(data),
            headers: ajexHeader,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 5400000,
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
                else if (xhr.status == 403) {
                    //console.log(xhr);
                    // forbidden
                    await xSplash.hide();
                    await xSwal.error("Forbidden", "You are not allow to access this page.", function () {
                        if (window.location.hostname.includes("tpcap")) {
                            return window.open("/kanban/Home", "_self");
                        }
                        return window.open("/Home", "_self");
                    }
                    );
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
                        if (typeof errorFn == 'function') {
                            return errorFn(xhr, status, error)
                        }
                    }

                    else {
                        xSwal.error("Error", xhr.responseJSON.message)
                    }

                }
            }
        });
    }

    AJAX_PostString(url, data, successFn, errorFn) {
        if (window.location.hostname.includes("tpcap")) {
            url = "/kanban" + url;
        }
        return $.ajax({
            type: "POST",
            url: url,
            data: typeof data == 'string' ? data : JSON.stringify(data),
            headers: ajexHeader,
            contentType: "text/plain; charset=utf-8",
            dataType: "json",
            timeout: 5400000,
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
                else if (xhr.status == 403) {
                    console.log(xhr);
                    // forbidden
                    await xSplash.hide();
                    await xSwal.error("Forbidden", "You are not allow to access this page.", function () {
                        if (window.location.hostname.includes("tpcap")) {
                            return window.open("/kanban/Home", "_self");
                        }
                        return window.open("/Home", "_self");
                    }
                    );
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
                        if (typeof errorFn == 'function') {
                            return errorFn(xhr, status, error)
                        }
                    }

                }
            }
        });
    }

    AJAX_PostFile(url, file, successFn, errorFn) {
        if (window.location.hostname.includes("tpcap")) {
            url = "/kanban" + url;
        }
        return $.ajax({
            type: "POST",
            url: url,
            data: file,
            headers: ajexHeader,
            contentType: false,
            processData: false,
            timeout: 5400000,
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
                else if (xhr.status == 403) {
                    //console.log(xhr);
                    // forbidden
                    await xSplash.hide();
                    await xSwal.error("Forbidden", "You are not allow to access this page.", function () {
                        if (window.location.hostname.includes("tpcap")) {
                            return window.open("/kanban/Home", "_self");
                        }
                        return window.open("/Home", "_self");
                    }
                    );
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
                        if (typeof errorFn == 'function') {
                            return errorFn(xhr, status, error)
                        }
                    }

                    else {
                        xSwal.error("Error", xhr.responseJSON.message)
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

    OpenReportObj(reportName, obj) {

        var query = "";
        for (var key in obj) {
            query += key + "=" + obj[key] + "&";
        }
        if (query.endsWith('&')) query = query.substring(0, query.length - 1);
        if (query.endsWith('&')) query = query.substring(0, query.length - 1);

        var _url = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx?/KB3"
        _url = _url + reportName + "&" + query;
        window.open(_url, '_blank');
    }
}

const _xLib = new xLib();

(($) => {
    $.fn.resetSelectPicker = async function () {
        //console.log(this);
        await $(this).val('');
        await $(this).selectpicker('refresh');
        $(this).parent().find("div[class='filter-option-inner-inner']").text("Nothing Selected");
    };

    $.fn.addListSelectPicker = async function (arrData, objKey) {
        //console.log($(this));
        await $(this).empty();
        await $(this).append(`<option value='' hidden></option >`);
        let thisElement = $(this);
        arrData.forEach(function (each) {
            //console.log(each[objKey]);
            //console.log(thisElement);
            thisElement.append(`<option value='${each[objKey]}' >${each[objKey]}</option>`);
        });
        await $(this).selectpicker('refresh');
    }

    $.fn.initDatepicker = function (date) {
        $(this).datepicker({
            uiLibrary: 'materialdesign',
            format: 'dd/mm/yyyy',
            autoclose: true,
            showRightIcon: false,
            value: date ?? moment().format('DD/MM/YYYY'),
            showOtherMonths: false,
        });
    };

    $.fn.initialDataTable = function (options) {
        if (this.length === 0) return console.error("Element not found");

        let defaultSettings = {
            width: null,
            paging: true,
            scrollCollapse: false,
            processing: false,
            serverSide: false,
            scrollX: false,
            scrollY: null,
            searching: true,
            info: true,
            ordering: true,
            columns: [],
            order: [],
            buttons: [],
            layout: {}
        };

        // Combine default settings with user-provided options
        let settings = $.extend({}, defaultSettings, options);

        // Initialize DataTable for each element in the jQuery object
        return this.each(function () {
            $(this).DataTable({
                width: settings.width,
                paging: settings.paging,
                scrollCollapse: settings.scrollCollapse,
                processing: settings.processing,
                serverSide: settings.serverSide,
                scrollX: settings.scrollX,
                scrollY: settings.scrollY,
                searching: settings.searching,
                info: settings.info,
                ordering: settings.ordering,
                columns: settings.columns,
                order: settings.order,
                buttons: settings.buttons,
                layout: settings.layout
            });
        });
    };

})(jQuery);