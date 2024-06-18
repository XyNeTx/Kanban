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
        for (let each in jsonResult) {
            if (jsonResult[each] == null) {
                jsonResult[each] = '';
            }
            else {
                jsonResult[each] = jsonResult[each].trim();
            }
        }
        return jsonResult;
    }

    AJAX_Get(url, data, success, error) {
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
}
const _xLib = new xLib();