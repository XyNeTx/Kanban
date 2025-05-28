"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
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
        var _a, _b, _c, _d, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q;
        $(".btn-toolbar[role='toolbar']").addClass("d-none");
        if (id == undefined)
            return console.error("id is undefined");
        if (!id.includes('#'))
            id = '#' + id;
        let constr = new DataTableLib();
        constr = Object.assign(constr, options);
        //console.log(constr);
        //console.log(constr.scrollCollapse, "scrollCollapse");
        //console.log(this.scrollCollapse, "this.scrollCollapse");
        const dataTable = $(`${id}`).DataTable({
            width: (_a = (constr.width)) !== null && _a !== void 0 ? _a : this.width,
            paging: (_b = (constr.paging)) !== null && _b !== void 0 ? _b : this.paging,
            scrollCollapse: (_c = (constr.scrollCollapse)) !== null && _c !== void 0 ? _c : this.scrollCollapse,
            processing: (_d = (constr.processing)) !== null && _d !== void 0 ? _d : this.processing,
            serverSide: (_f = (constr.serverSide)) !== null && _f !== void 0 ? _f : this.serverSide,
            scrollX: (_g = (constr.scrollX)) !== null && _g !== void 0 ? _g : this.scrollX,
            scrollY: (_h = (constr.scrollY)) !== null && _h !== void 0 ? _h : this.scrollY,
            searching: (_j = (constr.searching)) !== null && _j !== void 0 ? _j : this.searching,
            info: (_k = (constr.info)) !== null && _k !== void 0 ? _k : this.info,
            ordering: (_l = (constr.ordering)) !== null && _l !== void 0 ? _l : this.ordering,
            columns: (_m = (constr.columns)) !== null && _m !== void 0 ? _m : this.columns,
            order: (_o = (constr.order)) !== null && _o !== void 0 ? _o : this.order,
            buttons: (_p = (constr.buttons)) !== null && _p !== void 0 ? _p : this.buttons,
            layout: (_q = (constr.layout)) !== null && _q !== void 0 ? _q : this.layout
        });
        //$(`${id} table thead tr th`).css("text-align", "center");
        //$(`${id} table thead tr th`).css("vertical-align", "middle");
        //console.log($(`${id}`).parent().parent().find($(".dataTables_scrollHead .dataTables_scrollHeadInner .dataTable")));
        $(`${id}`).parent().parent().find($("th")).css("vertical-align", "middle");
        $(`${id}`).parent().parent().find($("th")).css("text-align", "center");
        $(`${id} tbody tr td`).css("text-align", "center");
        $(`${id} tbody tr td`).css("vertical-align", "middle");
        return dataTable;
    }
    ClearAndAddDataDT(id, data) {
        if (id == undefined)
            return console.error("id is undefined");
        if (!id.includes('#'))
            id = '#' + id;
        $(`${id}`).DataTable().clear().rows.add(data).draw();
        $(`${id}`).DataTable().columns.adjust().draw();
        $("table thead tr th").css("text-align", "center");
        $("table thead tr th").css("vertical-align", "middle");
        $("table tbody tr td").css("text-align", "center");
        $("table tbody tr td").css("vertical-align", "middle");
    }
    ClearData(id) {
        if (id == undefined)
            return console.error("id is undefined");
        if (!id.includes('#'))
            id = '#' + id;
        $(`${id}`).DataTable().clear().draw();
        $(`${id}`).DataTable().columns.adjust().draw();
        $("table thead tr th").css("text-align", "center");
        $("table thead tr th").css("vertical-align", "middle");
        $("table tbody tr td").css("text-align", "center");
        $("table tbody tr td").css("vertical-align", "middle");
    }
    GetDataDT(id, closestRow) {
        if (id == undefined)
            return console.error("id is undefined");
        if (!id.includes('#'))
            id = '#' + id;
        let table = $(`${id}`).DataTable();
        let data = table.row($(closestRow).closest('tr')).data();
        return data;
    }
    GetSelectedDataDT(id) {
        if (id == undefined)
            return console.error("id is undefined");
        if (!id.includes('#'))
            id = '#' + id;
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
    constructor() {
        //constructor() {
        //    this.ajexHeader = ajexHeader;
        //}
        this.ConsoleTest = () => {
            console.log("Test Import");
        };
        this.ObjSetVal = (objData, isTrigger) => __awaiter(this, void 0, void 0, function* () {
            //console.log(objData);
            for (const e of Object.keys(objData)) {
                //console.log(e);
                let _e = "F" + e.substring(1, e.length);
                //console.log(_e);
                if ($(`#${_e}`).attr("data-datepicker")) {
                    yield $(`#${_e}`).val(moment(objData[e], "YYYYMMDD").format("DD/MM/YYYY"));
                }
                else {
                    yield $(`#${_e}`).val(objData[e]);
                }
            }
            $(".selectpicker").selectpicker("refresh");
        });
    }
    TrimArrayJSON(jsonResult) {
        for (let each in jsonResult) {
            for (let eachObj in jsonResult[each]) {
                if (jsonResult[each][eachObj] == null) {
                    jsonResult[each][eachObj] = '';
                }
                else {
                    if (typeof jsonResult[each][eachObj] == 'string')
                        jsonResult[each][eachObj] = jsonResult[each][eachObj].trim();
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
                        if (typeof jsonResult[each][eachObj] == 'string')
                            jsonResult[each][eachObj] = jsonResult[each][eachObj].trim();
                    }
                }
            }
            else {
                for (let each in jsonResult) {
                    if (jsonResult[each] == null) {
                        jsonResult[each] = '';
                    }
                    else {
                        if (typeof jsonResult[each] == 'string')
                            jsonResult[each] = jsonResult[each].trim();
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
            if (typeof jsonResult.data == 'object')
                jsonResult.data = this.TrimArrayJSON(jsonResult.data);
            else
                jsonResult.data = this.TrimJSON(jsonResult.data);
        }
        return jsonResult;
    }
    JSONparseAndTrimArray(jsonResult) {
        if (jsonResult.hasOwnProperty('data')) {
            for (var key in jsonResult.data) {
                jsonResult.data[key] = JSON.parse(jsonResult.data[key]);
                if (typeof jsonResult.data[key] == 'object')
                    jsonResult.data[key] = this.TrimArrayJSON(jsonResult.data[key]);
                else
                    jsonResult.data[key] = this.TrimJSON(jsonResult.data[key]);
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
                    if (typeof jsonResult.data[key] == 'object')
                        jsonResult.data[key] = this.TrimArrayJSON(jsonResult.data[key]);
                    else
                        jsonResult.data[key] = this.TrimJSON(jsonResult.data);
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
                        for (let i = 0; i < jsonResult.data.length; i++) {
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
                var _appendCookie = cookie.split(';').filter(x => x.indexOf('_xProcess') >= 0) + `,${cName}`;
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
        if (process == undefined)
            return null;
        //console.log(process);
        var arrProcess = process.split('=').pop().split(',');
        //console.log(arrProcess);
        return arrProcess;
    }
    GetCookie(name) {
        var cookie = document.cookie;
        var _cookie = cookie.split(';').find(x => x.includes(name));
        if (_cookie == undefined)
            return null;
        return _cookie.split('=').pop();
    }
    ;
    SetCookie(name, value) {
        return document.cookie = `${name}=${value}`;
    }
    LoginDateDD() {
        var cookieLoginDate = this.GetCookie('loginDate');
        return moment(cookieLoginDate.substring(0, 10)).format("DD/MM/YYYY");
    }
    GetUserName() {
        return $(".pcoded-navigatio-lavel").text().trim();
    }
    ResetSelectPicker(id) {
        if (id == undefined)
            return console.error("id is undefined");
        if (!id.includes('#'))
            id = '#' + id;
        $(`${id}`).val('');
        $(`${id}`).selectpicker('refresh');
        $(`${id}`).parent().find("div[class='filter-option-inner-inner']").text("Nothing Selected");
    }
    AJAX_GetNoHeader(url, data, successFn, errorFn) {
        $(".btn-toolbar[role='toolbar']").addClass("d-none");
        if (window.location.hostname.includes("tpcap")) {
            url = "/kanban" + url;
        }
        else if (window.location.hostname.includes("app07")) {
            url = "/kanban" + url;
        }
        let title = $(document).find("title").text();
        //ajexHeader.title = title;
        let obj = {
            title: title
        };
        //console.log(obj);
        //console.log(url);
        return $.ajax({
            type: "GET",
            url: url,
            data: data,
            headers: obj,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 5400000,
            success: function (xhr, status) {
                return __awaiter(this, void 0, void 0, function* () {
                    //if (xhr.hasOwnProperty('data')) {
                    //    xhr.data = await JSON.parse(xhr.data);
                    //    console.log(xhr.data);
                    //    console.log(typeof xhr.data);
                    //    if (typeof xhr.data == 'object') xhr.data = _xLib.TrimArrayJSON(xhr.data);
                    //    else xhr.data = _xLib.TrimJSON(xhr.data);
                    //}
                    if (typeof successFn != 'function')
                        return;
                    return successFn(xhr, status);
                });
            },
            error: function (xhr, status, error) {
                return __awaiter(this, void 0, void 0, function* () {
                    if (xhr.status == 401) {
                        yield xSplash.hide();
                        yield xSwal.error("Error", "Session expired. Please login again.", function () {
                            if (window.location.hostname.includes("tpcap")) {
                                return window.open("/kanban/Login", "_self");
                            }
                            return window.open("/Login", "_self");
                        });
                    }
                    else if (xhr.status == 403) {
                        console.log(xhr);
                        // forbidden
                        yield xSplash.hide();
                        yield xSwal.error("Forbidden", "You are not allow to access this page.", function () {
                            if (window.location.hostname.includes("tpcap")) {
                                return window.open("/kanban/Home", "_self");
                            }
                            return window.open("/Home", "_self");
                        });
                    }
                    else {
                        yield console.error(xhr);
                        xSplash.hide();
                        // error was return by apicontroller
                        if (xhr.responseJSON.errors != undefined) {
                            if (xhr.responseJSON.errors.message == undefined || xhr.responseJSON.errors.message == null || !xhr.responseJSON.errors.message) {
                                var _error = "";
                                for (var key in xhr.responseJSON.errors) {
                                    _error += xhr.responseJSON.errors[key][0] + "<br>";
                                }
                                return xSwal.ErrorHTML("Error", _error);
                            }
                            return xSwal.error("Error", xhr.responseJSON.message);
                        }
                        // error was return by developer catch it
                        else if (xhr.responseJSON.response) {
                            xSwal.error(xhr.responseJSON.response, xhr.responseJSON.message);
                            if (typeof errorFn == 'function') {
                                return errorFn(xhr, status, error);
                            }
                        }
                    }
                });
            }
        });
    }
    AJAX_Get(url, data, successFn, errorFn) {
        $(".btn-toolbar[role='toolbar']").addClass("d-none");
        if (window.location.hostname.includes("tpcap")) {
            url = "/kanban" + url;
        }
        else if (window.location.hostname.includes("app07")) {
            url = "/kanban" + url;
        }
        let title = $(document).find("title").text();
        title = title.split(" - ")[0];
        title = title.split(" : ")[1];
        ajexHeader.title = title;
        //console.log(ajexHeader);
        return $.ajax({
            type: "GET",
            url: url,
            data: data,
            headers: ajexHeader,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 5400000,
            success: function (xhr, status) {
                return __awaiter(this, void 0, void 0, function* () {
                    //if (xhr.hasOwnProperty('data')) {
                    //    xhr.data = await JSON.parse(xhr.data);
                    //    console.log(xhr.data);
                    //    console.log(typeof xhr.data);
                    //    if (typeof xhr.data == 'object') xhr.data = _xLib.TrimArrayJSON(xhr.data);
                    //    else xhr.data = _xLib.TrimJSON(xhr.data);
                    //}
                    if (typeof successFn != 'function')
                        return;
                    return successFn(xhr, status);
                });
            },
            error: function (xhr, status, error) {
                return __awaiter(this, void 0, void 0, function* () {
                    if (xhr.status == 401) {
                        yield xSplash.hide();
                        yield xSwal.error("Error", "Session expired. Please login again.", function () {
                            if (window.location.hostname.includes("tpcap")) {
                                return window.open("/kanban/Login", "_self");
                            }
                            return window.open("/Login", "_self");
                        });
                    }
                    else if (xhr.status == 403) {
                        //console.log(xhr);
                        // forbidden
                        yield xSplash.hide();
                        yield xSwal.error("Forbidden", "You are not allow to access this page.", function () {
                            if (window.location.hostname.includes("tpcap")) {
                                return window.open("/kanban/Home", "_self");
                            }
                            return window.open("/Home", "_self");
                        });
                    }
                    else {
                        yield console.error(xhr);
                        xSplash.hide();
                        // error was return by apicontroller
                        if (xhr.responseJSON.errors != undefined) {
                            if (xhr.responseJSON.errors.message == undefined || xhr.responseJSON.errors.message == null || !xhr.responseJSON.errors.message) {
                                var _error = "";
                                for (var key in xhr.responseJSON.errors) {
                                    _error += xhr.responseJSON.errors[key][0] + "<br>";
                                }
                                return xSwal.ErrorHTML("Error", _error);
                            }
                            return xSwal.error("Error", xhr.responseJSON.message);
                        }
                        // error was return by developer catch it
                        else if (xhr.responseJSON.response) {
                            //xSwal.error(xhr.responseJSON.response, xhr.responseJSON.message)
                            if (typeof errorFn == 'function') {
                                return errorFn(xhr, status, error);
                            }
                        }
                    }
                });
            }
        });
    }
    AJAX_Post(url, data, successFn, errorFn) {
        $(".btn-toolbar[role='toolbar']").addClass("d-none");
        if (window.location.hostname.includes("tpcap")) {
            url = "/kanban" + url;
        }
        else if (window.location.hostname.includes("app07")) {
            url = "/kanban" + url;
        }
        let title = $(document).find("title").text();
        title = title.split(" - ")[0];
        title = title.split(" : ")[1];
        ajexHeader.title = title;
        //console.log(ajexHeader);
        return $.ajax({
            type: "POST",
            url: url,
            data: typeof data == 'string' ? data : JSON.stringify(data),
            headers: ajexHeader,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            timeout: 5400000,
            success: successFn,
            error: function (xhr, status, error) {
                return __awaiter(this, void 0, void 0, function* () {
                    if (xhr.status == 401) {
                        yield xSplash.hide();
                        yield xSwal.error("Error", "Session expired. Please login again.", function () {
                            if (window.location.hostname.includes("tpcap")) {
                                return window.open("/kanban/Login", "_self");
                            }
                            return window.open("/Login", "_self");
                        });
                    }
                    else if (xhr.status == 403) {
                        //console.log(xhr);
                        // forbidden
                        yield xSplash.hide();
                        yield xSwal.error("Forbidden", "You are not allow to access this page.", function () {
                            if (window.location.hostname.includes("tpcap")) {
                                return window.open("/kanban/Home", "_self");
                            }
                            return window.open("/Home", "_self");
                        });
                    }
                    else {
                        yield console.error(xhr);
                        xSplash.hide();
                        // error was return by apicontroller
                        if (xhr.responseJSON.errors != undefined) {
                            if (xhr.responseJSON.errors.message == undefined || xhr.responseJSON.errors.message == null || !xhr.responseJSON.errors.message) {
                                var _error = "";
                                for (var key in xhr.responseJSON.errors) {
                                    _error += xhr.responseJSON.errors[key][0] + "<br>";
                                }
                                return xSwal.ErrorHTML("Error", _error);
                            }
                            return xSwal.error("Error", xhr.responseJSON.message);
                        }
                        // error was return by developer catch it
                        else if (xhr.responseJSON.response) {
                            //xSwal.error(xhr.responseJSON.response, xhr.responseJSON.message)
                            if (typeof errorFn == 'function') {
                                return errorFn(xhr, status, error);
                            }
                        }
                        else {
                            //xSwal.error("Error", xhr.responseJSON.message)
                        }
                    }
                });
            }
        });
    }
    AJAX_PostString(url, data, successFn, errorFn) {
        $(".btn-toolbar[role='toolbar']").addClass("d-none");
        if (window.location.hostname.includes("tpcap")) {
            url = "/kanban" + url;
        }
        else if (window.location.hostname.includes("app07")) {
            url = "/kanban" + url;
        }
        let title = $(document).find("title").text();
        title = title.split(" - ")[0];
        title = title.split(" : ")[1];
        ajexHeader.title = title;
        //console.log(ajexHeader);
        return $.ajax({
            type: "POST",
            url: url,
            data: typeof data == 'string' ? data : JSON.stringify(data),
            headers: ajexHeader,
            contentType: "text/plain; charset=utf-8",
            dataType: "json",
            timeout: 5400000,
            success: successFn,
            error: function (xhr, status, error) {
                return __awaiter(this, void 0, void 0, function* () {
                    if (xhr.status == 401) {
                        yield xSplash.hide();
                        yield xSwal.error("Error", "Session expired. Please login again.", function () {
                            if (window.location.hostname.includes("tpcap")) {
                                return window.open("/kanban/Login", "_self");
                            }
                            return window.open("/Login", "_self");
                        });
                    }
                    else if (xhr.status == 403) {
                        console.log(xhr);
                        // forbidden
                        yield xSplash.hide();
                        yield xSwal.error("Forbidden", "You are not allow to access this page.", function () {
                            if (window.location.hostname.includes("tpcap")) {
                                return window.open("/kanban/Home", "_self");
                            }
                            return window.open("/Home", "_self");
                        });
                    }
                    else {
                        yield console.error(xhr);
                        xSplash.hide();
                        // error was return by apicontroller
                        if (xhr.responseJSON.errors != undefined) {
                            if (xhr.responseJSON.errors.message == undefined || xhr.responseJSON.errors.message == null || !xhr.responseJSON.errors.message) {
                                var _error = "";
                                for (var key in xhr.responseJSON.errors) {
                                    _error += xhr.responseJSON.errors[key][0] + "<br>";
                                }
                                return xSwal.ErrorHTML("Error", _error);
                            }
                            return xSwal.error("Error", xhr.responseJSON.message);
                        }
                        // error was return by developer catch it
                        else if (xhr.responseJSON.response) {
                            //xSwal.error(xhr.responseJSON.response, xhr.responseJSON.message)
                            if (typeof errorFn == 'function') {
                                return errorFn(xhr, status, error);
                            }
                        }
                    }
                });
            }
        });
    }
    AJAX_PostFile(url, file, successFn, errorFn) {
        $(".btn-toolbar[role='toolbar']").addClass("d-none");
        if (window.location.hostname.includes("tpcap")) {
            url = "/kanban" + url;
        }
        else if (window.location.hostname.includes("app07")) {
            url = "/kanban" + url;
        }
        let title = $(document).find("title").text();
        title = title.split(" - ")[0];
        title = title.split(" : ")[1];
        ajexHeader.title = title;
        //console.log(ajexHeader);
        return $.ajax({
            type: "POST",
            url: url,
            data: file,
            headers: ajexHeader,
            contentType: false,
            processData: false,
            timeout: 5400000,
            success: successFn,
            error: function (xhr, status, error) {
                return __awaiter(this, void 0, void 0, function* () {
                    if (xhr.status == 401) {
                        yield xSplash.hide();
                        yield xSwal.error("Error", "Session expired. Please login again.", function () {
                            if (window.location.hostname.includes("tpcap")) {
                                return window.open("/kanban/Login", "_self");
                            }
                            return window.open("/Login", "_self");
                        });
                    }
                    else if (xhr.status == 403) {
                        //console.log(xhr);
                        // forbidden
                        yield xSplash.hide();
                        yield xSwal.error("Forbidden", "You are not allow to access this page.", function () {
                            if (window.location.hostname.includes("tpcap")) {
                                return window.open("/kanban/Home", "_self");
                            }
                            return window.open("/Home", "_self");
                        });
                    }
                    else {
                        yield console.error(xhr);
                        xSplash.hide();
                        // error was return by apicontroller
                        if (xhr.responseJSON.errors != undefined) {
                            if (xhr.responseJSON.errors.message == undefined || xhr.responseJSON.errors.message == null || !xhr.responseJSON.errors.message) {
                                var _error = "";
                                for (var key in xhr.responseJSON.errors) {
                                    _error += xhr.responseJSON.errors[key][0] + "<br>";
                                }
                                return xSwal.ErrorHTML("Error", _error);
                            }
                            return xSwal.error("Error", xhr.responseJSON.message);
                        }
                        // error was return by developer catch it
                        else if (xhr.responseJSON.response) {
                            xSwal.error(xhr.responseJSON.response, xhr.responseJSON.message);
                            if (typeof errorFn == 'function') {
                                return errorFn(xhr, status, error);
                            }
                        }
                        else {
                            xSwal.error("Error", xhr.responseJSON.message);
                        }
                    }
                });
            }
        });
    }
    OpenReport(reportName, query) {
        var _url = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx?/KB3";
        _url = _url + reportName + "&" + query;
        window.open(_url, '_blank');
    }
    OpenReportObj(reportName, obj) {
        var query = "";
        for (var key in obj) {
            query += key + "=" + obj[key] + "&";
        }
        if (query.endsWith('&'))
            query = query.substring(0, query.length - 1);
        if (query.endsWith('&'))
            query = query.substring(0, query.length - 1);
        var _url = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx?/KB3";
        //const endcodeQuery = encodeURIComponent(query);
        //_url = _url + reportName + "&" + endcodeQuery;
        _url = _url + reportName + "&" + query;
        window.open(_url, '_blank');
    }
}
const _xLib = new xLib();
(($) => {
    $.fn.resetSelectPicker = function () {
        return __awaiter(this, void 0, void 0, function* () {
            $(this).val("");
            //console.log($(this));
            //await $(this).trigger("change");
            $(this).attr("title", "Nothing Selected");
            //$(this).find("option.bs-title-option").remove();
            $(this).selectpicker('refresh');
        });
    };
    $.fn.resetAllSelectPicker = function () {
        return __awaiter(this, void 0, void 0, function* () {
            $(".selectpicker").each(function () {
                $(this).resetSelectPicker();
            });
        });
    };
    $.fn.SelectToInput = function (type, col) {
        return __awaiter(this, void 0, void 0, function* () {
            let select = $(this).parent().find("select");
            let dls = select.attr("data-live-search");
            let ds = select.attr("data-size");
            let dv = select.attr("data-val");
            let dvl = select.attr("data-val-length");
            let dvlm = select.attr("data-val-length-max");
            let dvr = select.attr("data-val-required");
            let id = select.attr("id");
            col = col == "" ? "7" : col;
            select.parent().remove();
            //console.log(`label[for='${id}']`);
            //console.log($(`label[for='${id}']`));
            return $(`label[for='${id}']`).parent().append(`
                <input type="${type}" class="form-control col-${col}" data-live-search="${dls}" data-size="${ds}" 
                data-val="${dv}" data-val-length="${dvl}" data-val-length-max="${dvlm}" 
                data-val-required="${dvr}" id="${id}" name="${id}" />
            `);
        });
    };
    $.fn.InputToSelect = function (col) {
        return __awaiter(this, void 0, void 0, function* () {
            let input = $(this).parent().find("input");
            let dls = input.attr("data-live-search");
            let ds = input.attr("data-size");
            let dv = input.attr("data-val");
            let dvl = input.attr("data-val-length");
            let dvlm = input.attr("data-val-length-max");
            let dvr = input.attr("data-val-required");
            let id = input.attr("id");
            col = col == "" ? "8" : col;
            input.remove();
            //console.log(`label[for='${id}']`);
            //console.log($(`label[for='${id}']`));
            $(`label[for='${id}']`).parent().append(`
                <select class="selectpicker p-0 col-${col}" data-live-search="${dls}" data-size="${ds}" 
                data-val="${dv}" data-val-length="${dvl}" data-val-length-max="${dvlm}" 
                data-val-required="${dvr}" id="${id}" name="${id}" 
                data-live-search="true" data-size="6">
                </select>
            `);
            return $(`#${id}`).selectpicker('refresh');
        });
    };
    $.fn.disableSelectPicker = function () {
        return __awaiter(this, void 0, void 0, function* () {
            var id = $(this).attr("id");
            return $(`button[data-id='${id}']`).addClass("disabled");
        });
    };
    $.fn.enableSelectPicker = function () {
        return __awaiter(this, void 0, void 0, function* () {
            var id = $(this).attr("id");
            return $(`button[data-id='${id}']`).removeClass("disabled");
        });
    };
    $.fn.formToJSON = function () {
        return __awaiter(this, void 0, void 0, function* () {
            let formData = {};
            //console.log($(this));
            yield $(this).serializeArray().forEach(function (item) {
                return __awaiter(this, void 0, void 0, function* () {
                    //console.log(item);
                    if ($(`#${item.name}`).attr("data-datepicker")) {
                        item.value = moment(item.value, "DD/MM/YYYY").format("YYYYMMDD");
                    }
                    formData[item.name] = item.value;
                    return yield formData;
                });
            });
            //console.log(formData);
            return yield formData;
        });
    };
    $.fn.addListSelectPicker = function (arrData, objKey) {
        return __awaiter(this, void 0, void 0, function* () {
            $(".btn-toolbar[role='toolbar']").addClass("d-none");
            //console.log($(this));
            yield $(this).empty();
            yield $(this).append(`<option value=''></option >`);
            let thisElement = $(this);
            arrData.forEach(function (each) {
                //console.log(each[objKey]);
                //console.log(thisElement);
                thisElement.append(`<option value='${each[objKey]}' >${each[objKey]}</option>`);
            });
            return $(this).selectpicker('refresh');
        });
    };
    $.fn.addOptionDataList = function (arrData, objKey) {
        return __awaiter(this, void 0, void 0, function* () {
            var id = $(this).attr("id");
            $(this).parent().find("input[list='" + id + "']").val("");
            yield $(this).empty();
            yield $(this).append(`<option value=''></option >`);
            let thisElement = $(this);
            arrData.forEach(function (each) {
                //console.log(each[objKey]);
                //console.log(thisElement);
                if (each[objKey] == null || each[objKey] == undefined) {
                    thisElement.append(`<option value='${each}'>${each}</option >`);
                }
                else {
                    thisElement.append(`<option value='${each[objKey]}' >${each[objKey]}</option>`);
                }
            });
        });
    };
    $.fn.removeListSelectPicker = function () {
        return __awaiter(this, void 0, void 0, function* () {
            $(".btn-toolbar[role='toolbar']").addClass("d-none");
            //console.log($(this));
            yield $(this).empty();
            yield $(this).append(`<option value=''></option >`);
            return $(this).selectpicker('refresh');
        });
    };
    $.fn.initDatepicker = function (date) {
        return __awaiter(this, void 0, void 0, function* () {
            $(".btn-toolbar[role='toolbar']").addClass("d-none");
            yield $(this).datepicker({
                uiLibrary: 'bootstrap5',
                format: 'dd/mm/yyyy',
                autoclose: true,
                showRightIcon: false,
                value: date !== null && date !== void 0 ? date : moment().format('DD/MM/YYYY'),
                showOtherMonths: false,
            });
            $(this).closest("div[class*='input-group']").removeClass("input-group");
            $(this).append("<span class='fa fa-search'></span>");
        });
    };
    $.fn.initialDataTable = function (options) {
        $(".btn-toolbar[role='toolbar']").addClass("d-none");
        if (this.length === 0)
            return console.error("Element not found");
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
