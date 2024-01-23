class labDropDownList {
    constructor() {

    }


    bind(pObject, pData = null, pValue='value', pText='text') {
        pObject = CheckObject(pObject);
        $(pObject).html('');
        $.each(pData, function (key, value) {
            //console.log(value);

            $(pObject).append($('<option>', {
                value: value[pValue],
                text: value[pText]
            }));
        });


        //renderSelectBox();
        var _value = $(pObject).attr('value');

        $(pObject).val(_value);

    }
    Bind(pObject, pData = null, pValue = 'value', pText = 'text') {
        this.bind(pObject, pData, pValue, pText);
    }

}



const xDropDownList = new labDropDownList();