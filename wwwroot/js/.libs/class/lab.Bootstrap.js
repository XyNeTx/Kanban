"use strict";
class labBootStrap {
    constructor() {
    }
    validation(pFormName) {
        var _result = true;
        var form = document.getElementById(pFormName);
        if (!form.checkValidity()) {
            //event.preventDefault();
            //event.stopPropagation();
            _result = false;
        }
        form.classList.add('was-validated');
        return _result;
    }
}
const xBootstrap = new labBootStrap();
