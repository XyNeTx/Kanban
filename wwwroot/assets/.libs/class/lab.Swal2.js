function swalShow(_message = "TYPE|TITLE|TEXT") {
    if (_message != '') {
        const _list = _message.split('|');
        //let word = myArray[1];
        if (_list[0].toUpperCase() == 'ERROR') {
            Swal.fire({
                icon: 'error',
                title: (_list.length > 2 ? (_list[1] != '' ? _list[1] : 'Error!') : 'Error!'),
                text: (_list.length > 2 ? _list[2] : _list[1])
            });
        }
        if (_list[0].toUpperCase() == 'INFO') {
            Swal.fire({
                icon: 'info',
                title: (_list.length > 2 ? (_list[1] != '' ? _list[1] : 'Info?') : 'Info?'),
                text: (_list.length > 2 ? _list[2] : _list[1])
            });
        }
        if (_list[0].toUpperCase() == 'QUESTION') {
            Swal.fire({
                icon: 'question',
                title: (_list.length > 2 ? (_list[1] != '' ? _list[1] : 'Question?') : 'Question?'),
                text: (_list.length > 2 ? _list[2] : _list[1])
            });
        }
        if (_list[0].toUpperCase() == 'SUCCESS') {
            Swal.fire({
                icon: 'success',
                title: (_list.length > 2 ? (_list[1] != '' ? _list[1] : 'Success?') : 'Success?'),
                text: (_list.length > 2 ? _list[2] : _list[1]),
                showConfirmButton: false,
                timer: 5000
            });
        }
    }
}


function swalInput(_message, _callback = null) {
    if (_message != '') {
        Swal.fire({
            title: _message,
            input: 'text',
            inputAttributes: {
                autocapitalize: 'off'
            },
            showCancelButton: true,
            //confirmButtonText: 'Look up',
            showLoaderOnConfirm: true,
            preConfirm: (login) => {
                //console.log(login);
            },
            allowOutsideClick: () => !Swal.isLoading()
        }).then((result) => {
            _callback(result);
        })
    }
}


class libSwal {
    constructor() {
        //console.log("load");
    }

    info(pTitle = "", pMessage = "", pCallback = null) {
        if (TypeOf(pTitle) == 'Object') {
            pCallback = (pTitle.then != undefined ? pTitle.then : null);
            pMessage = (pTitle.message != undefined ? pTitle.message : i18nLayout.modal.swal.info.text);
            pTitle = (pTitle.title != undefined ? pTitle.title : i18nLayout.modal.swal.info.title);
        } else {
            pMessage = (pMessage.length > 0 ? pMessage : i18nLayout.modal.swal.info.text);
            pTitle = (pTitle.length > 0 ? pTitle : i18nLayout.modal.swal.info.title);
        }

        Swal.fire({
            icon: 'info',
            title: pTitle,
            text: pMessage
        }).then((result) => {
            if (pTitle.return != undefined) return pTitle.return;

            if (pCallback != null) pCallback(result);
        });
    }
    Info(pTitle = "", pMessage = "", pCallback = null) {
        this.info(pTitle, pMessage, pCallback);
    }

    information(pTitle = "", pMessage = "", pCallback = null) {
        this.info(pTitle, pMessage, pCallback);
    }
    Information(pTitle = "", pMessage = "", pCallback = null) {
        this.info(pTitle, pMessage, pCallback);
    }

    warning(pTitle = "", pMessage = "", pCallback = null) {
        if (TypeOf(pTitle) == 'Object') {
            pCallback = (pTitle.then != undefined ? pTitle.then : null);
            pMessage = (pTitle.message != undefined ? pTitle.message : i18nLayout.modal.swal.info.text);
            pTitle = (pTitle.title != undefined ? pTitle.title : i18nLayout.modal.swal.info.title);
        } else {
            pMessage = (pMessage.length > 0 ? pMessage : i18nLayout.modal.swal.info.text);
            pTitle = (pTitle.length > 0 ? pTitle : i18nLayout.modal.swal.info.title);
        }

        Swal.fire({
            icon: 'warning',
            title: pTitle,
            text: pMessage
        }).then((result) => {
            if (pTitle.return != undefined) return pTitle.return;

            if (pCallback != null) pCallback(result);
        });
    }
    Warning(pTitle = "", pMessage = "", pCallback = null) {
        this.info(pTitle, pMessage, pCallback);
    }


    error(pTitle = "", pMessage = "", pCallback = null) {
        if (TypeOf(pTitle) == 'Object') {
            pCallback = (pTitle.then != undefined ? pTitle.then : null);
            pMessage = (pTitle.message != undefined ? pTitle.message : i18nLayout.modal.swal.error.text);
            pTitle = (pTitle.title != undefined ? pTitle.title : i18nLayout.modal.swal.error.title);
        } else {
            pMessage = (pMessage.length > 0 ? pMessage : i18nLayout.modal.swal.error.text);
            pTitle = (pTitle.length > 0 ? pTitle : i18nLayout.modal.swal.error.title);
        }

        if ((pMessage.indexOf('<') >= 0 && pMessage.indexOf('>') >= 0) || (pMessage.indexOf('\n') >= 0)) {
            pMessage = ReplaceAll(pMessage, '\n', '<br>');
            Swal.fire({
                icon: 'error',
                title: pTitle,
                html: pMessage
            }).then((result) => {
                if (pTitle.return != undefined) return pTitle.return;

                if (pCallback != null) pCallback(result);
            });
        } else {

            Swal.fire({
                icon: 'error',
                title: pTitle,
                text: pMessage
            }).then((result) => {
                if (pTitle.return != undefined) return pTitle.return;

                if (pCallback != null) pCallback(result);
            });
        }

    }

    Error(pTitle = "", pMessage = "", pCallback = null) {
        this.error(pTitle, pMessage, pCallback);
    }

    ErrorHTML(pTitle = "", pMessage = "", pCallback = null) {
        Swal.fire({
            icon: "error",
            title: pTitle,
            html: pMessage,
            showConfirmButton: true,
            confirmButtonColor: '#FD00A5',
            confirmButtonText: "OK",
        });
    }


    question(pTitle = "", pMessage = "", pCallback = null, pCancel = null) {
        if (TypeOf(pTitle) == 'Object') {
            pCallback = (pTitle.then != undefined ? pTitle.then : null);
            pCancel = (pTitle.cancel != undefined ? pTitle.cancel : null);
            pMessage = (pTitle.message != undefined ? pTitle.message : i18nLayout.modal.swal.question.text);
            pTitle = (pTitle.title != undefined ? pTitle.title : i18nLayout.modal.swal.question.title);
        } else {
            pMessage = (pMessage.length > 0 ? pMessage : i18nLayout.modal.swal.question.text);
            pTitle = (pTitle.length > 0 ? pTitle : i18nLayout.modal.swal.question.title);
        }

        //console.log(pTitle);
        Swal.fire({
            icon: 'question',
            title: pTitle,
            text: pMessage,
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: i18nLayout.modal.swal.question.confirm,
            cancelButtonText: i18nLayout.modal.swal.question.cancel,
        }).then((result) => {
            //console.log(result);
            if (result.isConfirmed) {
                //if (pCallback != null) pCallback(result);
                if (typeof (pCallback) == 'function') pCallback(result);

            } else if (result.isDismissed) {
                //if (pCancel != null) pCancel(result);
                if (typeof (pCancel) == 'function') pCancel(result);

            }
        });
    }
    Question(pTitle = "", pMessage = "", pCallback = null) {
        this.question(pTitle, pMessage, pCallback);
    }

    confirm(pTitle, pMessage) {

        return Swal.fire({
            icon: 'question',
            title: pTitle,
            text: pMessage,
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: i18nLayout.modal.swal.question.confirm,
            cancelButtonText: i18nLayout.modal.swal.question.cancel,
        }).then((result) => {
            console.log(result);
            return result.isConfirmed;
        });
    }

    success(pTitle = "", pMessage = "", pCallback = null) {
        Swal.fire({
            icon: 'success',
            title: (pTitle.length > 0 ? pTitle : 'Success'),
            text: (pMessage.length > 0 ? pMessage : ''),
            showCancelButton: false,
            showConfirmButton: true,
            confirmButtonColor: '#FD00A5',
            cancelButtonColor: '#3085D6',
            cancelButtonText: i18nLayout.modal.button.close,
            confirmButtonText: i18nLayout.modal.button.save,
            timer: 10000
        }).then((result) => {
            if (pCallback != null) pCallback(result);
        });
    }
    Success(pTitle = "", pMessage = "", pCallback = null) {
        this.success(pTitle, pMessage, pCallback);
    }


    input(pMessage, pCallback = null, pValue = null) {
        if (pMessage != '') {

            if (typeof (pValue) == 'function') {
                let _fnc = pValue;
                pValue = pCallback;
                pCallback = _fnc;
            }

            if (pValue == null) pValue = '';

            Swal.fire({
                title: pMessage,
                input: 'text',
                inputValue: pValue,
                inputAttributes: {
                    autocapitalize: 'off'
                },
                showCancelButton: true,
                //confirmButtonText: 'Look up',
                showLoaderOnConfirm: true,
                preConfirm: (login) => {
                    //console.log(login);
                },
                allowOutsideClick: () => !Swal.isLoading()
            }).then((result) => {
                if (typeof (pCallback) == 'function') pCallback(result);
            });
        }

    }
    Input(pMessage, pCallback = null, pValue = null) {
        this.input(pMessage, pCallback, pValue);
    }




    //########### Use for language pack ##############
    infoPack(pLanguagePack = null, pCallback = null) {
        if (pLanguagePack == null) {
            pLanguagePack = i18nLayout.modal.button.swal_save;
        }
        Swal.fire({
            icon: 'info',
            title: pLanguagePack.title,
            text: pLanguagePack.text,
        }).then((result) => {
            if (typeof (pCallback) == 'function') {
                pCallback(result);
            } else {
                return result;
            }
            
        });
    }

    informationPack(pLanguagePack = null, pCallback = null) {
        //console.log(pLanguagePack);
        if (pLanguagePack == null) {
            pLanguagePack = i18nLayout.modal.button.swal_save;
        }
        Swal.fire({
            icon: 'question',
            title: pLanguagePack.title,
            text: pLanguagePack.text,
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            cancelButtonText: pLanguagePack.cancel,
            confirmButtonText: pLanguagePack.save,
        }).then((result) => {
            //console.log(result);
            if (result.isConfirmed) {
                if (typeof (pCallback) == 'function') {
                    pCallback(result);
                } else {
                    return result;
                }
            }
        });
    }


    questionPack(pLanguagePack = null, pCallback = null) {
        //console.log(pLanguagePack);
        if (pLanguagePack == null) {
            pLanguagePack = i18nLayout.modal.button.swal_save;
        }
        Swal.fire({
            icon: 'question',
            title: pLanguagePack.title,
            text: pLanguagePack.text,
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            cancelButtonText: pLanguagePack.cancel,
            confirmButtonText: pLanguagePack.save,
        }).then((result) => {
            //console.log(result);
            if (result.isConfirmed) {
                if (typeof (pCallback) == 'function') {
                    pCallback(result);
                } else {
                    return result;
                }
            }
        });
    }


    errorPack(pLanguagePack = null, pCallback = null) {
        if (pLanguagePack == null) {
            pLanguagePack = i18nLayout.modal.button.swal_save;
        }
        Swal.fire({
            icon: 'error',
            title: pLanguagePack.title,
            text: pLanguagePack.text,
        }).then((result) => {
            if (typeof (pCallback) == 'function') {
                pCallback(result);
            } else {
                return result;
            }
        });
    }

    toast(pMessage = "", pCallback = null) {
        Swal.fire({
            position: "top-end",
            icon: "success",
            title: pMessage,
            showConfirmButton: false,
            timer: 1500
        }).then((result) => {
            if (typeof (pCallback) == 'function') {
                pCallback(result);
            } else {
                return result;
            }
        });
    }

}
const xSwal = new libSwal();