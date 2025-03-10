
class labAjax {
    constructor() {
        this.ajaxItem = new AjaxItemProperties();
    }



    item(pObject) {
        let _id = '';
        let _name = '';
        let _type = '';
        let _value = '';
        pObject = CheckObject(pObject);


        if ($(pObject).attr('type') != undefined) {
            this.ajaxItem.id = $(pObject).attr('id');
            this.ajaxItem.name = $(pObject).attr('name');
            this.ajaxItem.type = $(pObject).attr('type');
            this.ajaxItem.value = $(pObject).val();
        } else {
            pObject = ReplaceAll(pObject, '#', '');
            $('input[name=' + pObject + ']').each(function (index, obj) {

                if ($('#' + obj.id).prop('type') == 'radio') {
                    if ($('#' + obj.id).prop('checked') == true) {
                        _id = obj.id;
                        _name = pObject;
                        _type = $('#' + obj.id).prop('type');
                        _value = $('#' + obj.id).val();
                    }
                }

            });
            this.ajaxItem.id = _id;
            this.ajaxItem.name = _name;
            this.ajaxItem.type = _type;
            this.ajaxItem.value = _value;
        }
        //console.log(this.ajaxItem);

        return this.ajaxItem;
    }


    on(pObjectName, pFunction = null) {
        pObjectName = CheckObject(pObjectName);
        $(pObjectName).on('input', function (e) {
            if ((pFunction != null) && (typeof (pFunction) == 'function')) {
                pFunction(e);
            }
        });
    }

    onEnter(pObjectName, pFunction = null) {
        pObjectName = CheckObject(pObjectName);
        $(pObjectName).on('keypress', function (e) {
            if (e.keyCode == 13) {
                if ((pFunction != null) && (typeof (pFunction) == 'function')) {
                    pFunction(e);
                    return false;
                } else {
                    return false; // prevent the button click from happening
                }
            }
        });
    }

    onClick(pObjectName, pFunction = null) {
        pObjectName = CheckObject(pObjectName);
        $(pObjectName).on('click', function (e) {
            if ((pFunction != null) && (typeof (pFunction) == 'function')) {
                pFunction(e);
            }
        });
    }

    onCheck(pObjectName, pFunction = null) {
        pObjectName = CheckObject(pObjectName);
        $(pObjectName).on('click', function (e) {
            if ((pFunction != null) && (typeof (pFunction) == 'function')) {
                pFunction(e);
            }
        });
    }
    onChecked(pObjectName, pFunction = null) {
        pObjectName = CheckObject(pObjectName);
        $(pObjectName).on('click', function (e) {
            if ((pFunction != null) && (typeof (pFunction) == 'function')) {
                pFunction(e);
            }
        });
    }

    onChange(pObjectName, pFunction = null) {
        pObjectName = CheckObject(pObjectName);
        $(pObjectName).on('change', function (e) {
            if ((pFunction != null) && (typeof (pFunction) == 'function')) {
                pFunction(e);
            }
        });
    }

    onBlur(pObjectName, pFunction = null) {
        pObjectName = CheckObject(pObjectName);
        $(pObjectName).on('blur', function (e) {
            if ((pFunction != null) && (typeof (pFunction) == 'function')) {
                pFunction(e);
            }
        });
    }

    onKeyPress(pObjectName, pFunction = null) {
        pObjectName = CheckObject(pObjectName);
        $(pObjectName).keyup(function (e) {
            if ((pFunction != null) && (typeof (pFunction) == 'function')) {
                pFunction(e);
            }
        });
    }

    trim(pValue) {
        if (pValue != '' && pValue != null) pValue = pValue.toString().replace(/^\s+|\s+$/gm, '');
        return pValue;
    }
    Trim(pValue) {
        trim(pValue);
    }


    redirect(pURL) {
        //console.log(pURL);
        window.document.location.replace(pURL);
    }
    Redirect(pURL) {
        redirect(pURL);
    }


    Execute(pConfig = null) {
        if (pConfig != null) {

            let _url = (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/EXEC/Execute';
            let _postData = (pConfig.data != undefined ? ajaxPostData(pConfig.data) : null);
            ajexHeader.Records = unescape(encodeURIComponent(ajexHeader.Records));

            //console.log(ajexHeader);
            return $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                headers: ajexHeader,
                url: _url,
                data: _postData,
                success: function (result) {
                    xSplash.hide();
                },
                error: function (result) {
                    console.error('Ajax.Post: ' + result.responseText);
                    xSplash.hide();
                }
            });

        }
    }
    xExecute(pConfig = null) {
        if (pConfig != null) {

            let _url = (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/EXEC/xExecute';
            let _postData = (pConfig.data != undefined ? ajaxPostData(pConfig.data) : null);
            ajexHeader.Records = unescape(encodeURIComponent(ajexHeader.Records));

            return $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                headers: ajexHeader,
                url: _url,
                data: _postData,
                success: function (result) {
                    xSplash.hide();
                },
                error: function (result) {
                    console.error('Ajax.Post: ' + result.responseText);
                    xSplash.hide();
                }
            });

        }
    }

    ExecuteJSON(pConfig = null) {
        if (pConfig != null) {

            let _url = (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/EXEC/ExecuteJSON';
            let _postData = (pConfig.data != undefined ? ajaxPostData(pConfig.data) : null);
            ajexHeader.Records = unescape(encodeURIComponent(ajexHeader.Records));

            console.log(_postData);

            return $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                headers: ajexHeader,
                url: _url,
                data: _postData,
                success: function (result) {
                    xSplash.hide();
                },
                error: function (result) {
                    console.error('Ajax.Post: ' + result.responseText);
                    xSplash.hide();
                }
            });

        }
    }
    xExecuteJSON(pConfig = null) {
        if (pConfig != null) {

            let _url = (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/EXEC/xExecuteJSON';
            let _postData = (pConfig.data != undefined ? ajaxPostData(pConfig.data) : null);
            ajexHeader.Records = unescape(encodeURIComponent(ajexHeader.Records));


            console.log(_postData);

            return $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                headers: ajexHeader,
                url: _url,
                data: _postData,
                success: function (result) {
                    xSplash.hide();
                },
                error: function (result) {
                    console.log(result);
                    console.error('Ajax.Post: ' + (result.responseText != undefined ? result.responseText : result.statusText));
                    xSplash.hide();
                }
            });

        }
    }


    PostAsync(pConfig = null) {
        if (pConfig != null) {
            console.log(pConfig);

            let _url = (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + pConfig.url;
            let _postData = (pConfig.data != undefined ? ajaxPostData(pConfig.data) : null);
            let _type = (pConfig.method != undefined ? pConfig.method : 'POST');

            console.log(_type);
            console.log(_url);
            console.log(_postData);

            return $.ajax({
                type: _type,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                headers: ajexHeader,
                url: _url,
                data: _postData,
                success: function (result) {

                },
                error: function (result) {
                    console.error('Ajax.Post: ' + result.responseText);
                    xSplash.hide();
                }
            });

        }
    }



    Post(pConfig = null) {
        if (pConfig != null) {
            //console.log(pConfig.data);

            let _url = (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + pConfig.url;
            let _postData = (pConfig.data != undefined ? ajaxPostData(pConfig.data) : null);

            //console.log(_postData);

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                headers: ajexHeader,
                url: _url,
                data: _postData,
                success: function (result) {
                    //console.log(result);

                    if (result.response == "OK") {
                        if (typeof (pConfig.then) === 'function') pConfig.then(result);
                        if (typeof (pConfig.callback) === 'function') pConfig.callback(result);
                        xSplash.hide();

                    } else {
                        if (result.status == 401) window.document.location.replace('../Logout');
                        xSwal.error('Error', result.message + ' [Ajax.Post]');
                        xSplash.hide();
                    }

                },
                //then: function (result) {
                //    console.log(result);
                //    console.log('then');

                //},
                error: function (result) {
                    console.error('Ajax.Post: ' + result.responseText);
                    xSplash.hide();
                }
            });

        }
    }
    PostData(pData) {
        try {
            const jsonObject = JSON.parse(pData);

            if (typeof (pData) === 'string') {
                // pData : var _string = `{ "id": "String" }`;
                return JSON.stringify(JSON.stringify(JSON.parse(pData)));

            } else if (typeof (pData) === 'object') {
                // pData : var _json = { "id": "JSON" };
                return JSON.stringify(JSON.stringify(pData));
            }

        } catch (error) {

            if (pData.length > 0) {
                // pData : $('#frmCondition')

                const _formData = $('#' + pData.attr('id')).serializeArray();
                const _jsonData = {};

                $(_formData).each(function (index, obj) {
                    _jsonData[obj.name] = obj.value;
                });
                return JSON.stringify(JSON.stringify(_jsonData));

            } else {
                //console.log(pData);
                var _jObject = JSON.stringify(JSON.stringify(pData));

                if (_jObject === '"{}"') {
                    // pData : var _formData = new FormData();
                    return JSON.stringify(JSON.stringify(Object.fromEntries(pData)));

                } else {
                    // pData :  var _object = {};   _object.id = "Object.id";
                    return _jObject;
                }

            }

        }


    }





    PostFile(pConfig = null) {
        if (pConfig != null) {
            let _url = (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + pConfig.url;
            let _postData = (pConfig.data != undefined ? pConfig.data : null);

            $.ajax({
                type: 'POST',
                enctype: 'multipart/form-data',
                headers: ajexHeader,
                url: _url,
                data: _postData,
                processData: false,
                contentType: false,
                success: function (result) {

                    if (result.response == "OK") {
                        if (typeof (pConfig.then) === 'function') pConfig.then(result);
                        if (typeof (pConfig.callback) === 'function') pConfig.callback(result);

                    } else {
                        console.log(result);
                        xSwal.error('Error', result.message);
                        xSplash.hide();
                    }
                },
                error: function (result) {
                    console.error('Ajax.PostFile: ' + result.responseText);
                    xSplash.hide();
                }
            });

        }
    }



    ToClipboard(pText) {
        try {
            // Create a temporary textarea element
            const _text = document.createElement('textarea');
            _text.value = pText;
            document.body.appendChild(_text);

            // Select the text in the textarea
            _text.select();
            _text.setSelectionRange(0, _text.value.length);

            //// Copy the selected text to the clipboard
            //document.execCommand('copy');
            //navigator.clipboard.writeText(_text.value);

            navigator.clipboard.writeText(_text.value)
                .then(() => {
                    xSwal.toast('Copy to Clipboard.');
                    //console.log('Text copied to clipboard');
                })
                .catch(err => {
                });

            //// Remove the temporary textarea
            //document.body.removeChild(textarea);

        } catch (err) {
            console.clear();
            //document.execCommand("copy");
            //console.error('Unable to copy text to clipboard', err);
            //xSwal.toast('Copy to Clipboard.');
        }
    }









    //sql(pValue) {
    //    $('#divModalSQL').append(pValue + ` \n`);
    //}
    //SQL(pValue) {
    //    sql(pValue);
    //}







}


class AjaxItemProperties {
    constructor() {
        this.id = null;
        this.name = null;
        this.type = null;
        this.value = null;
    }

    properties() {
        return {
            "id": this.id,
            "name": this.name,
            "type": this.type,
            "value": this.value
        }
    }

    attr(pAttribute = null, pValue = null) {
        if (pAttribute == null) return {
            "id": this.id,
            "name": this.name,
            "type": this.type,
            "value": this.value
        }
        //console.log(pAttribute);
        if (pValue != null) {
            if (pAttribute == 'id') this.id = pValue;
            if (pAttribute == 'name') this.name = pValue;
            if (pAttribute == 'type') this.type = pValue;
            if (pAttribute == 'value') this.value = pValue;

            console.log($('#' + this.id));
            $('#' + this.id).attr('name', this.name);
            $('#' + this.id).val(this.value);

        }

        if (pAttribute == 'id') return this.id;
        if (pAttribute == 'name') return this.name;
        if (pAttribute == 'type') return this.type;
        if (pAttribute == 'value') return this.value;
    }

    val(pValue = null) {
        if (pValue != null) {

            this.value = pValue;

            if (this.type == 'radio') {
                $('input[name=' + this.name + ']').each(function (index, obj) {
                    $('#' + obj.id).prop('checked', false);
                    //console.log($(this).id +'=>>'+ $(this).prop('checked'));
                });

                $('#' + this.name + pValue).prop('checked', 'checked');
            } else {
                $('#' + this.id).val(this.value);
            }


        }
        return this.value;
    }


}

const xAjax = new labAjax();


