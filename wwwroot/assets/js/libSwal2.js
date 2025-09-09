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
        Swal.fire({
            icon: 'info',
            title: (pTitle.length > 0 ? pTitle : 'Information'),
            text: (pMessage.length > 0 ? pMessage : '')
        }).then((result) => {
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


    error(pTitle = "", pMessage = "", pCallback = null) {
        Swal.fire({
            icon: 'error',
            title: (pTitle.length > 0 ? pTitle : 'Error'),
            text: (pMessage.length > 0 ? pMessage : '')
        }).then((result) => {
            if (pCallback != null) pCallback(result);
        });
    }
    Error(pTitle = "", pMessage = "", pCallback = null) {
        this.error(pTitle, pMessage, pCallback);
    }


    question(pTitle = "", pMessage = "", pCallback = null) {
        //console.log(pTitle);
        Swal.fire({
            icon: 'question',
            title: (pTitle.length > 0 ? pTitle : i18nLayout.modal.button.swal_save.title),
            text: (pMessage.length > 0 ? pMessage : i18nLayout.modal.button.swal_save.text),
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            cancelButtonText: "Close",
            confirmButtonText: "OK",
        }).then((result) => {
            //console.log(result);
            if (result.isConfirmed) {
                if (pCallback != null) pCallback(result);
            }
        });
    }
    Question(pTitle = "", pMessage = "", pCallback = null) {
        this.question(pTitle, pMessage, pCallback);
    }



    success(pTitle = "", pMessage = "", pCallback = null) {
        Swal.fire({
            icon: 'success',
            title: (pTitle.length > 0 ? pTitle : 'Success'),
            text: (pMessage.length > 0 ? pMessage : ''),
            showCancelButton: true,
            showConfirmButton: false,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            cancelButtonText: "Close",
            confirmButtonText: "OK",
            timer: 5000
        }).then((result) => {
            if (pCallback != null) pCallback(result);
        });
    }
    Success(pTitle = "", pMessage = "", pCallback = null) {
        this.success(pTitle, pMessage, pCallback);
    }


    input(pMessage, pCallback = null, pValue = null) {
        if (pMessage != '') {
            Swal.fire({
                title: pMessage,
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
                if (pCallback != null) pCallback(result);
            });
        }

    }
    Input(pMessage, pCallback = null, pValue = null) {
        this.input(pMessage, pCallback);
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

}
const xSwal = new libSwal();