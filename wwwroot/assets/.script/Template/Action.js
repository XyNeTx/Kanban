class ActionTemplate {
    constructor(pConfig = '') {
        this.Controller = (pConfig.Controller != undefined ? pConfig.Controller : _PAGE_);

        this.ModalName = pConfig.Modal;
        this.FormName = pConfig.Form;
        this.PostData = pConfig.PostData;
    }



    prepare = function (pCallback = '') {
        xSplash.show();
        xSplash.text('PREPARING');

        if (typeof (pCallback) === 'function') pCallback();
    }




    initial = function (pCallback = '') {
        const _Controller = this.Controller;
        const _FormName = this.FormName;

        //console.log((_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + _Controller + '/initial');

        xSplash.show();
        xSplash.text('DATA INITIALIZING');

        var _PostData = this._reCallPostData();
        //console.log(_PostData);

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            headers: ajexHeader,
            url: (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + _Controller + '/initial',
            data: ajaxPostData(_PostData),
            success: function (result) {
                //console.log(result);

                if (result.response == "OK") {
                    if (typeof (pCallback) === 'function') pCallback(result);

                } else {
                    xSwal.error('Error', result.message);
                    xSplash.hide();
                }

            },
            error: function (result) {
                console.error(_Controller + '.Initial: ' + result.responseText);
                xSplash.hide();
            }
        });

    }

    save = function (pCallback = '') {
        const _Controller = this.Controller;
        const _FormName = this.FormName;

        const _frmModal = document.getElementById(_FormName);
        if (!_frmModal.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
            _FormName.classList.add('was-validated');
            return false;
        }

        //console.log($('#' + _FormName).serialize());

        xSwal.questionPack(
            i18nLayout.modal.button.swal_save,
            function (result) {
                if (result.isConfirmed) {
                    xSplash.show();
                    xSplash.text('PROCESSING');

                    $.ajax({
                        type: 'POST',
                        dataType: "json",
                        headers: ajexHeader,
                        url: (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + _Controller + '/save',
                        data: $('#' + _FormName).serialize(),
                        success: function (result) {

                            xSplash.hide();
                            if (typeof (pCallback) === 'function') pCallback(result);
                        },
                        error: function (result) {
                            console.error(_Controller + '.Save: ' + result.responseText);
                            xSplash.hide();
                        }
                    });

                }
            });

    }

    

    execute = function (pCallback = '') {
        const _Controller = this.Controller;
        const _FormName = this.FormName;

        const _frmModal = document.getElementById(_FormName);
        if (!_frmModal.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
            _FormName.classList.add('was-validated');
            return false;
        }

        //console.log($('#' + _FormName).serialize());

        xSwal.questionPack(
            i18nLayout.modal.button.swal_process,
            function (result) {
                if (result.isConfirmed) {
                    xSplash.show();
                    xSplash.text('PROCESSING');

                    $.ajax({
                        type: 'POST',
                        dataType: "json",
                        headers: ajexHeader,
                        url: (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + _Controller + '/execute',
                        data: $('#' + _FormName).serialize(),
                        success: function (result) {

                            xSplash.hide();
                            if (typeof (pCallback) === 'function') pCallback(result);
                        },
                        error: function (result) {
                            console.error(_Controller + '.Execute: ' + result.responseText);
                            xSplash.hide();
                        }
                    });

                }
            });

    }


    execute = function (pCallback = '') {
        const _Controller = this.Controller;
        const _FormName = this.FormName;

        const _frmModal = document.getElementById(_FormName);
        if (!_frmModal.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
            _FormName.classList.add('was-validated');
            return false;
        }

        //console.log($('#' + _FormName).serialize());

        xSwal.questionPack(
            i18nLayout.modal.button.swal_process,
            function (result) {
                if (result.isConfirmed) {
                    xSplash.show();
                    xSplash.text('PROCESSING');

                    $.ajax({
                        type: 'POST',
                        dataType: "json",
                        headers: ajexHeader,
                        url: (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + _Controller + '/execute',
                        data: $('#' + _FormName).serialize(),
                        success: function (result) {

                            xSplash.hide();
                            if (typeof (pCallback) === 'function') pCallback(result);
                        },
                        error: function (result) {
                            console.error(_Controller + '.Execute: ' + result.responseText);
                            xSplash.hide();
                        }
                    });

                }
            });

    }

    delete = function (pCallback = '') {
        const _Controller = this.Controller;
        const _FormName = this.FormName;

        xSwal.questionPack(
            i18nLayout.modal.button.swal_delete,
            function (result) {
                if (result.isConfirmed) {
                    xSplash.show();
                    xSplash.text('DELETING');

                    $.ajax({
                        type: "DELETE",
                        dataType: "json",
                        headers: ajexHeader,
                        url: (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + _Controller + '/delete',
                        data: $('#' + _FormName).serialize(),
                        success: function (result) {

                            xSplash.hide();
                            if (typeof (pCallback) === 'function') pCallback(result);
                        },
                        error: function (result) {
                            console.error(_Controller + '.Delete: ' + result.responseText);
                            xSplash.hide();
                        }
                    });
                }

            });
    }


    _reCallPostData = function () {

        var _PostData = ``;
        $.each(this.PostData, function (key, v) {
            v.value = CheckObject(v.value);
            _PostData += (_PostData == `` ? `` : `, `) + `"` + v.name + `" : "` + ($(v.value).val() != undefined ? $(v.value).val() : ReplaceAll(v.value, '#', '')) + `"`;
        });

        return `{` + _PostData + `}`;
    }

}


xItem.render();