class labLog {
    constructor() {
    }

    WriteLog(pMessage = '', pName = 'Default') {
        if (pMessage != null) {
            let _data = {
                "name": pName,
                "message": pMessage
            }
            let _postData = (pMessage != '' ? ajaxPostData(_data) : null);

            return $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                headers: ajexHeader,
                url: '/Log/WriteLog',
                data: _postData,
                success: function (result) {

                },
                error: function (result) {
                    console.error('Ajax.Post: ' + result.responseText);
                }
            });


        }

    }
}
const xLog = new labLog();

